using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SiteMaster : MasterPage
{
    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;

    protected void Page_Init(object sender, EventArgs e)
    {
        var requestCookie = Request.Cookies[AntiXsrfTokenKey];
        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out Guid requestCookieGuidValue))
        {
            _antiXsrfTokenValue = requestCookie.Value;
            Page.ViewStateUserKey = _antiXsrfTokenValue;
        }
        else
        {
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
            Page.ViewStateUserKey = _antiXsrfTokenValue;

            var responseCookie = new HttpCookie(AntiXsrfTokenKey)
            {
                HttpOnly = true,
                Value = _antiXsrfTokenValue
            };
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                responseCookie.Secure = true;

            Response.Cookies.Set(responseCookie);
        }

        Page.PreLoad += master_Page_PreLoad;
    }

    protected void master_Page_PreLoad(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
            ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? string.Empty;
        }
        else
        {
            if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? string.Empty))
            {
                throw new InvalidOperationException("Error de validacion del token Anti-XSRF.");
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string perfilActual = Session["perfil"] as string;
        bool esWebMaster = perfilActual == "WebMaster";

        liBitacora.Visible = esWebMaster;
        liBackup.Visible = esWebMaster;
        liLogin.Visible = string.IsNullOrEmpty(perfilActual);
        liLogout.Visible = !string.IsNullOrEmpty(perfilActual);

        if (Session["LimpiarCarritoLocal"] as bool? == true)
        {
            Session.Remove("LimpiarCarritoLocal");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "clearLocalCart",
                "localStorage.removeItem('carrito');", true);
        }

        ActualizarCarritoContador();
    }

    protected void btnNavLogout_Click(object sender, EventArgs e)
    {
        new BLLUsuario().CerrarSesion();
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/Default.aspx");
    }

    public void ActualizarCarritoContador()
    {
        var usuario = SERVICIOS.SessionManager.GetInstance().Usuario;
        int cantidad;

        if (usuario != null)
        {
            cantidad = new BLLCarrito().ObtenerCantidadTotal(usuario.IdUsuario);
        }
        else
        {
            var carrito = Session["CarritoAnon"] as Dictionary<int, int>;
            cantidad = carrito?.Values.Sum() ?? 0;
        }

        lblCarritoContador.InnerText = cantidad.ToString();

        // Update counter in the DOM during async (UpdatePanel) postbacks
        ScriptManager.RegisterStartupScript(this, this.GetType(), "updateCartCounter",
            $"(function(){{ var lbl = document.querySelector('[id*=\"lblCarritoContador\"]'); if(lbl) lbl.innerText = '{cantidad}'; }})();", true);
    }
}
