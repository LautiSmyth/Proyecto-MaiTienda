using System;
using System.Web.UI;
using BLL;
using SERVICIOS;

public partial class Respuesta : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string nombreUsuario = Session["nombreUsuario"] as string;
        string perfil        = Session["perfil"] as string;

        if (string.IsNullOrEmpty(nombreUsuario))
        {
            Response.Redirect("~/Default.aspx");
            return;
        }

        if (!IsPostBack)
        {
            lblNombreUsuario.Text = nombreUsuario;
            lblPerfil.Text        = perfil;
            lblBienvenida.Text    = nombreUsuario;
            lblPerfilBadge.Text   = perfil;
        }
    }

    protected void btnCerrarSesion_Click(object sender, EventArgs e)
    {
        BLLUsuario _bllUsuario = new BLLUsuario();
        _bllUsuario.CerrarSesion();
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/Default.aspx");
    }
}
