using BE;
using BLL;
using SERVICIOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BLLUsuario _bllUsuario = new BLLUsuario();
            bool conectado = _bllUsuario.VerificarConexion();

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
            BLLUsuario _bllUsuario = new BLLUsuario();
            _bllUsuario.ValidarCredenciales(txtUsuario.Text.Trim(), txtPassword.Text);


            BEUsuario _usuario = SessionManager.GetInstance().Usuario;
            Session["nombreUsuario"] = _usuario.NombreUsuario;
            Session["perfil"] = _usuario.Perfil.ToString();

            Response.Redirect("Respuesta.aspx");
        }
        catch (UnauthorizedAccessException ex)
        {
            pnlError.Visible = true;
            litError.Text = ex.Message;
        }
        catch (Exception)
        {
            pnlError.Visible = true;
            litError.Text = "Ocurrió un error al intentar iniciar sesión. Intente nuevamente.";
        }
    }
}