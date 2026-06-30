using BLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;

public partial class GestionIntegridad : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["PasskeyVerificado"] as bool? != true)
        {
            Response.Redirect("~/Mantenimiento.aspx");
            return;
        }

        if (!IsPostBack)
        {
            CargarEstadoIntegridad();
            CargarListaBackups();
        }
    }

    private void CargarEstadoIntegridad()
    {
        var bll = new BLLIntegridad();
        bool dvvUsuariosOk, dvvProductosOk;
        List<BE.BEUsuario> usuariosCorruptos = bll.VerificarIntegridadUsuarios(out dvvUsuariosOk);
        List<BE.BEProducto> productosCorruptos = bll.VerificarIntegridadProductos(out dvvProductosOk);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(dvvUsuariosOk
            ? "<span class='dvv-chip ok'>DVV Usuarios ✓</span>"
            : "<span class='dvv-chip fail'>DVV Usuarios ✗</span>");
        sb.Append(dvvProductosOk
            ? "<span class='dvv-chip ok'>DVV Productos ✓</span>"
            : "<span class='dvv-chip fail'>DVV Productos ✗</span>");
        litEstadoDVV.Text = sb.ToString();

        if (usuariosCorruptos.Count > 0)
        {
            pnlUsuariosCorruptos.Visible = true;
            repUsuarios.DataSource = usuariosCorruptos;
            repUsuarios.DataBind();
        }

        if (productosCorruptos.Count > 0)
        {
            pnlProductosCorruptos.Visible = true;
            repProductos.DataSource = productosCorruptos;
            repProductos.DataBind();
        }

        bool todoOk = usuariosCorruptos.Count == 0 && dvvUsuariosOk
                   && productosCorruptos.Count == 0 && dvvProductosOk;
        pnlTodoOk.Visible = todoOk;
    }

    private void CargarListaBackups()
    {
        string carpeta = new BLLBackup().ObtenerCarpetaBackup();
        if (!Directory.Exists(carpeta))
        {
            pnlConBackups.Visible = false;
            pnlSinBackups.Visible = true;
            return;
        }

        var archivos = Directory.GetFiles(carpeta, "*.bak")
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.LastWriteTime)
            .ToList();

        if (archivos.Count == 0)
        {
            pnlConBackups.Visible = false;
            pnlSinBackups.Visible = true;
            return;
        }

        ddlBackups.Items.Clear();
        foreach (var archivo in archivos)
        {
            ddlBackups.Items.Add(new System.Web.UI.WebControls.ListItem(
                $"{archivo.Name}  ({archivo.LastWriteTime:dd/MM/yyyy HH:mm})",
                archivo.FullName));
        }
    }

    protected void btnRecalcular_Click(object sender, EventArgs e)
    {
        try
        {
            var bll = new BLLIntegridad();
            bll.RecalcularDVUsuarios();
            bll.RecalcularDVProductos();

            Application["IntegridadFallida"] = false;
            Application["MensajeIntegridad"] = null;

            MostrarResultado(true, "DVH y DVV recalculados correctamente. El sistema ha sido desbloqueado.");
            CargarEstadoIntegridad();
        }
        catch (Exception ex)
        {
            MostrarResultado(false, $"Error al recalcular: {ex.Message}");
        }
    }

    protected void btnRestaurar_Click(object sender, EventArgs e)
    {
        string rutaBackup = ddlBackups.SelectedValue;
        if (string.IsNullOrEmpty(rutaBackup) || !File.Exists(rutaBackup))
        {
            MostrarResultado(false, "Archivo de backup no encontrado.");
            return;
        }

        try
        {
            new BLLBackup().EjecutarRestore(rutaBackup);

            new BLLIntegridad().ValidarIntegridadGlobal();
            Application["IntegridadFallida"] = false;
            Application["MensajeIntegridad"] = null;

            MostrarResultado(true, $"Base de datos restaurada correctamente desde '{Path.GetFileName(rutaBackup)}'. El sistema ha sido desbloqueado.");
            CargarEstadoIntegridad();
            CargarListaBackups();
        }
        catch (Exception ex)
        {
            MostrarResultado(false, $"Error al restaurar: {ex.Message}");
        }
    }

    private void MostrarResultado(bool exito, string mensaje)
    {
        pnlResultadoAccion.Visible = true;
        alertAccion.Attributes["class"] = exito ? "alert alert-success" : "alert alert-error";
        litResultado.Text = mensaje;
    }
}
