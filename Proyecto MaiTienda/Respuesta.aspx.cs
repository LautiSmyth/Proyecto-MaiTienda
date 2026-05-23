using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Respuesta : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string nombreUsuario = Session["nombreUsuario"] as string;
        string perfil = Session["perfil"] as string;

        lblRespuesta.Text = $"Bienvenido {perfil} '{nombreUsuario}'";
    }
}