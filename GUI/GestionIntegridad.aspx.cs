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
        if (!EstadoIntegridad.EsWebMasterConPasskey(Session))
        {
            Response.Redirect("~/Login.aspx");
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
        bool dvvUsuariosOk, dvvProductosOk, dvvBitacoraOk, dvvPedidosOk, dvvFacturasOk;
        List<BE.BEUsuario> usuariosCorruptos = bll.VerificarIntegridadUsuarios(out dvvUsuariosOk);
        List<BE.BEProducto> productosCorruptos = bll.VerificarIntegridadProductos(out dvvProductosOk);
        List<BE.BEBitacora> eventosCorruptos = bll.VerificarIntegridadBitacora(out dvvBitacoraOk);
        List<BE.BEPedido> pedidosCorruptos = bll.VerificarIntegridadPedidos(out dvvPedidosOk);
        List<BE.BEFactura> facturasCorruptas = bll.VerificarIntegridadFacturas(out dvvFacturasOk);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(dvvUsuariosOk
            ? "<span class='dvv-chip ok'>DVV Usuarios ✓</span>"
            : "<span class='dvv-chip fail'>DVV Usuarios ✗</span>");
        sb.Append(dvvProductosOk
            ? "<span class='dvv-chip ok'>DVV Productos ✓</span>"
            : "<span class='dvv-chip fail'>DVV Productos ✗</span>");
        sb.Append(dvvBitacoraOk && eventosCorruptos.Count == 0
            ? "<span class='dvv-chip ok'>DVV Bitácora ✓</span>"
            : "<span class='dvv-chip fail'>DVV Bitácora ✗</span>");
        sb.Append(dvvPedidosOk && pedidosCorruptos.Count == 0
            ? "<span class='dvv-chip ok'>DVV Pedidos ✓</span>"
            : "<span class='dvv-chip fail'>DVV Pedidos ✗</span>");
        sb.Append(dvvFacturasOk && facturasCorruptas.Count == 0
            ? "<span class='dvv-chip ok'>DVV Facturas ✓</span>"
            : "<span class='dvv-chip fail'>DVV Facturas ✗</span>");
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
                   && productosCorruptos.Count == 0 && dvvProductosOk
                   && eventosCorruptos.Count == 0 && dvvBitacoraOk
                   && pedidosCorruptos.Count == 0 && dvvPedidosOk
                   && facturasCorruptas.Count == 0 && dvvFacturasOk;
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
            bll.RecalcularDVBitacora();
            bll.RecalcularDVPedidosYFacturas();

            EstadoIntegridad.Verificar();
            if (EstadoIntegridad.HayAlteraciones)
                MostrarResultado(false, "Se recalcularon los dígitos verificadores, pero siguen alteradas: " + string.Join(", ", EstadoIntegridad.TablasAlteradas()) + ".");
            else
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

            EstadoIntegridad.Verificar();
            if (EstadoIntegridad.HayAlteraciones)
                MostrarResultado(false, $"Se restauró '{Path.GetFileName(rutaBackup)}', pero ese backup también tiene datos alterados en: " +
                    string.Join(", ", EstadoIntegridad.TablasAlteradas()) + ". Pruebe con otro backup o recalcule los dígitos verificadores.");
            else
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
