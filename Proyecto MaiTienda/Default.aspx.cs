using System;
using System.ServiceModel.Channels;
using System.Web.UI;
using BE;
using BLL;
using SERVICIOS;

public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
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
