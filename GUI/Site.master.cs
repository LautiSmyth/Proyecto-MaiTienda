using BLL;
using System;
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
        Guid requestCookieGuidValue;
        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
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
            {
                responseCookie.Secure = true;
            }
            Response.Cookies.Set(responseCookie);
        }

        Page.PreLoad += master_Page_PreLoad;
    }

    protected void master_Page_PreLoad(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
            ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
        }
        else
        {
            if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
            {
                throw new InvalidOperationException("Error de validación del token Anti-XSRF.");
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string perfilActual = Session["perfil"] as string;

        if (perfilActual == "WebMaster")
        {
            liBitacora.Visible = true;
            liBackup.Visible = true;
        }
        else
        {
            liBitacora.Visible = false;
            liBackup.Visible = false;
        }

        if (!string.IsNullOrEmpty(perfilActual))
        {
            liLogin.Visible = false;
            liLogout.Visible = true;
        }
        else
        {
            liLogin.Visible = true;
            liLogout.Visible = false;
        }

        // Registrar variable global de login en el cliente para todas las páginas
        var usuario = SERVICIOS.SessionManager.GetInstance().Usuario;
        string script = $"var usuarioLogueado = {(usuario != null ? "true" : "false")};";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "sessionInfoGlobal", script, true);

        ActualizarCarritoContador();
    }

    protected void btnNavLogout_Click(object sender, EventArgs e)
    {
        BLLUsuario _bllUsuario = new BLLUsuario();
        _bllUsuario.CerrarSesion();
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/Default.aspx");
    }

    protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
    {
        Context.GetOwinContext().Authentication.SignOut();
    }

    public void ActualizarCarritoContador()
    {
        var usuario = SERVICIOS.SessionManager.GetInstance().Usuario;
        int cantidad = 0;

        if (usuario != null)
        {
            BLL.BLLCarrito bllCarrito = new BLL.BLLCarrito();
            cantidad = bllCarrito.ObtenerCantidadTotal(usuario.IdUsuario);
        }
        else
        {
            var carrito = Session["Carrito"] as System.Collections.Generic.List<int>;
            cantidad = carrito != null ? carrito.Count : 0;
     