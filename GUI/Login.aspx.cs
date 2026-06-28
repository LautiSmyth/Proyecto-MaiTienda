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
        try
        {
            new BLLUsuario().ValidarCredenciales(txtUsuario.Text.Trim(), txtPassword.Text);

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
