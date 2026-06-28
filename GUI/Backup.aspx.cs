using BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.UI;

public partial class Backup : Page
{
    private readonly BLLBackup _bllBackup = new BLLBackup();

    protected void Page_Load(object sender, EventArgs e)
    {
        // Solo WebMaster puede acceder
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

            // Refrescar lista
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
        // Leer la próxima ejecución guardada en Application
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
