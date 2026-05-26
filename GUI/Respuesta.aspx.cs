using System;
using System.Web.UI;

public partial class Respuesta : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string nombreUsuario = Session["nombreUsuario"] as string;
        string perfil = Session["perfil"] as string;

        if (string.IsNullOrEmpty(nombreUsuario))
        {
            Response.Redirect("~/Default.aspx");
            return;
        }

        if (!IsPostBack)
        {
            lblUsuario.Text = nombreUsuario;
            lblRol.Text = perfil;
        }
    }
}