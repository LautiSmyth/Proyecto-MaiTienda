using BLL;
using System;
using System.Web;
using System.Web.Services;
using System.Web.UI;

public partial class Mantenimiento : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        bool integridadFallida = Application["IntegridadFallida"] as bool? ?? false;
        if (!integridadFallida)
            Response.Redirect("~/Default.aspx");
    }

    [WebMethod(EnableSession = true)]
    public static bool VerificarPasskey(string passkey)
    {
        bool valido = new BLLUsuario().ValidarConPasskey(passkey);
        if (valido)
        {
            HttpContext.Current.Session["PasskeyVerificado"] = true;
            HttpContext.Current.Session["perfil"] = "WebMaster";
            HttpContext.Current.Session["nombreUsuario"] = "WebMaster";
        }
        return valido;
    }
}
