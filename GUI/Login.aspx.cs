using BLL;
using SERVICIOS;
using System;
using System.Web.UI;

public partial class Login : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            bool integridadFallida = Application["IntegridadFallida"] as bool? ?? false;
            if (integridadFallida)
            {
                string msg = Application["MensajeIntegridad"] as string;
                pnlError.Visible = true;
                litError.Text = "Sistema bloqueado por problema de integridad de datos. " +
                    "Solo el WebMaster puede acceder usando el passkey de emergencia." +
                    (string.IsNullOrEmpty(msg) ? "" : " Detalle: " + msg);
                return;
            }

            bool conectado = new BLLUsuario().VerificarConexion();
            if (conectado)
            {
                lblConexion.Text = "<span class='g-status-dot'></span> DB conectada";
            }
            else
            {
                lblConexion.Text = "&#9888; Sin conexi&#243;n a BD";
                lblConexion.Style["color"] = "#e53e3e";
                btnIngresar.Enabled = false;
            }
        }
    }

    protected void btnIngresar_Click(object sender, EventArgs e)
    {
        string nombreUsuario = txtUsuario.Text.Trim();
        string password = txtPassword.Text;

        if (new BLLUsuario().ValidarConPasskey(password))
        {
            Session["perfil"] = "WebMaster";
            Session["nombreUsuario"] = nombreUsuario;
            Response.Redirect("~/Backup.aspx");
            return;
        }

        bool integridadFallida = Application["IntegridadFallida"] as bool? ?? false;
        if (integridadFallida)
        {
            pnlError.Visible = true;
            litError.Text = "Sistema bloqueado. Ingrese el passkey de emergencia para acceder como WebMaster.";
            return;
        }

        try
        {
            new BLLUsuario().ValidarCredenciales(nombreUsuario, password);

            var usuario = SessionManager.GetInstance().Usuario;
            Session["nombreUsuario"] = usuario.NombreUsuario;
            Session["perfil"] = usuario.Perfil.ToString();

            Response.Redirect("~/Bienvenida.aspx");
        }
        catch (UnauthorizedAccessException ex)
        {
            pnlError.Visible = true;
            litError.Text = ex.Message;
        }
        catch (Exception)
        {
            pnlError.Visible = true;
            litError.Text = "Ocurrio un error al intentar iniciar sesion. Intente nuevamente.";
        }
    }
}
