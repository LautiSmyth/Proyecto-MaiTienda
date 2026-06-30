using BLL;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.UI;

public partial class Backup : Page
{
    private readonly BLLBackup _bllBackup = new BLLBackup();

    protected void Page_Load(object sender, EventArgs e)
    {
        string perfil = Session["perfil"] as string;
        if (string.IsNullOrEmpty(perfil))
        {
            Response.Redirect("~/Login.aspx");
            return;
        }
        if (perfil != "WebMaster")
        {
            Response.Redirect("~/Default.aspx");
            return;
        }

        if (!IsPostBack)
        {
            string carpeta = ConfigurationManager.AppSettings["BackupFolder"] ?? @"C:\Backups\MaiTienda";
            litCarpeta.Text = carpeta;
            CargarProximoBackup();
            CargarListaBackups(carpeta);
            
            btnVerificarIntegridad_Click(null, null);
        }
    }

    protected void btnVerificarIntegridad_Click(object sender, EventArgs e)
    {
        pnlIntegridadExito.Visible = false;
        pnlIntegridadError.Visible = false;
        lblDetalleErrorIntegridad.Text = "";

        try
        {
            var bllIntegridad = new BLL.BLLIntegridad();
            bool dvvUsuariosValido;
            var usuariosCorruptos = bllIntegridad.VerificarIntegridadUsuarios(out dvvUsuariosValido);
            bool dvvProductosValido;
            var productosCorruptos = bllIntegridad.VerificarIntegridadProductos(out dvvProductosValido);

            if (usuariosCorruptos.Count == 0 && dvvUsuariosValido && productosCorruptos.Count == 0 && dvvProductosValido)
            {
                pnlIntegridadExito.Visible = true;
            }
            else
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                if (usuariosCorruptos.Count > 0)
                {
                    sb.AppendLine("Tabla de Usuarios: DVHs inválidos (modificación de filas):");
                    foreach (var u in usuariosCorruptos)
                    {
                        sb.AppendLine($"- ID Usuario: {u.IdUsuario}, Nombre: {u.NombreUsuario}");
                    }
                }
                if (!dvvUsuariosValido)
                {
                    if (sb.Length > 0) sb.AppendLine();
                    sb.AppendLine("Tabla de Usuarios: DVV inválido (filas agregadas/eliminadas/modificadas).");
                }
                if (productosCorruptos.Count > 0)
                {
                    if (sb.Length > 0) sb.AppendLine();
                    sb.AppendLine("Tabla de Productos: DVHs inválidos (modificación de filas):");
                    foreach (var p in productosCorruptos)
                    {
                        sb.AppendLine($"- ID Producto: {p.IdProducto}, Nombre: {p.Nombre}");
                    }
                }
                if (!dvvProductosValido)
                {
                    if (sb.Length > 0) sb.AppendLine();
                    sb.AppendLine("Tabla de Productos: DVV inválido (filas agregadas/eliminadas/modificadas).");
                }
                lblDetalleErrorIntegridad.Text = sb.ToString();
                pnlIntegridadError.Visible = true;
            }
        }
        catch (Exception ex)
        {
            lblDetalleErrorIntegridad.Text = $"Error al verificar la integridad: {ex.Message}";
            pnlIntegridadError.Visible = true;
        }
    }

    protected void btnRecalcularDVs_Click(object sender, EventArgs e)
    {
        pnlIntegridadExito.Visible = false;
        pnlIntegridadError.Visible = false;

        try
        {
            var bllIntegridad = new BLL.BLLIntegridad();
            bllIntegridad.RecalcularDVUsuarios();
            bllIntegridad.RecalcularDVProductos();

            Application["IntegridadFallida"] = false;
            Application["MensajeIntegridad"] = null;

            btnVerificarIntegridad_Click(sender, e);
        }
        catch (Exception ex)
        {
            lblDetalleErrorIntegridad.Text = $"Error al recalcular los DVs: {ex.Message}";
            pnlIntegridadError.Visible = true;
        }
    }

    protected void btnGenerarBackup_Click(object sender, EventArgs e)
    {
        pnlExito.Visible = false;
        pnlError.Visible = false;

        try
        {
            string ruta = _bllBackup.GenerarBackupManual();
            litRuta.Text = ruta;
            pnlExito.Visible = true;

            string carpeta = ConfigurationManager.AppSettings["BackupFolder"] ?? @"C:\Backups\MaiTienda";
            CargarListaBackups(carpeta);
        }
        catch (Exception ex)
        {
            litError.Text = $"Error al generar el backup: {ex.Message}";
            pnlError.Visible = true;
        }
    }

    private void CargarProximoBackup()
    {
        DateTime? proximo = Application["BackupProximaEjecucion"] as DateTime?;
        litProximoBackup.Text = proximo.HasValue
            ? proximo.Value.ToString("dd/MM/yyyy HH:mm:ss")
            : "No programado (la app debe reiniciarse)";
    }

    private void CargarListaBackups(string carpeta)
    {
        if (!Directory.Exists(carpeta))
        {
            pnlSinArchivos.Visible = true;
            gvBackups.Visible = false;
            return;
        }

        var archivos = Directory.GetFiles(carpeta, "*.bak")
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.LastWriteTime)
            .Select(f => new
            {
                Nombre = f.Name,
                Fecha = f.LastWriteTime,
                Tamaño = FormatearTamaño(f.Length)
            })
            .ToList();

        if (archivos.Count == 0)
        {
            pnlSinArchivos.Visible = true;
            gvBackups.Visible = false;
        }
        else
        {
            pnlSinArchivos.Visible = false;
            gvBackups.Visible = true;
            gvBackups.DataSource = archivos;
            gvBackups.DataBind();
        }
    }

    private string FormatearTamaño(long bytes)
    {
        if (bytes >= 1073741824) return $"{bytes / 1073741824.0:F2} GB";
        if (bytes >= 1048576) return $"{bytes / 1048576.0:F2} MB";
        if (bytes >= 1024) return $"{bytes / 1024.0:F2} KB";
        return $"{bytes} B";
    }
}
