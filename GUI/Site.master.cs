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
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "clearLocalCart",
                "localStorage.removeItem('carrito');", true);
            LimpiarCarritoCookie(Context);
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        ActualizarCarritoContador();
    }

    protected void btnNavLogout_Click(object sender, EventArgs e)
    {
        new BLLUsuario().CerrarSesion();
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/Default.aspx");
    }

    public static Dictionary<int, int> ObtenerCarritoDesdeCookie(HttpContext context)
    {
        var carrito = new Dictionary<int, int>();
        var cookie = context.Request.Cookies["carrito_anon"];
        if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
        {
            try
            {
                string decodedValue = context.Server.UrlDecode(cookie.Value);
                var jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var ids = jsSerializer.Deserialize<List<int>>(decodedValue);
                if (ids != null)
                {
                    foreach (var id in ids)
                    {
                        if (carrito.ContainsKey(id))
                            carrito[id]++;
                        else
                            carrito[id] = 1;
                    }
                }
            }
            catch { }
        }
        return carrito;
    }

    public static void GuardarCarritoEnCookie(HttpContext context, Dictionary<int, int> carrito)
    {
        var ids = new List<int>();
        if (carrito != null)
        {
            foreach (var entry in carrito)
            {
                for (int i = 0; i < entry.Value; i++)
                {
                    ids.Add(entry.Key);
                }
            }
        }

        var jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        string json = jsSerializer.Serialize(ids);
        string encodedJson = context.Server.UrlEncode(json);

        var cookie = new HttpCookie("carrito_anon", encodedJson)
        {
            Path = "/",
            Expires = DateTime.Now.AddYears(1)
        };
        context.Response.Cookies.Set(cookie);
    }

    public static void LimpiarCarritoCookie(HttpContext context)
    {
        var cookie = new HttpCookie("carrito_anon", "")
        {
            Path = "/",
            Expires = DateTime.Now.AddDays(-1)
        };
        context.Response.Cookies.Set(cookie);
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
            if (carrito == null)
            {
                carrito = ObtenerCarritoDesdeCookie(Context);
                Session["CarritoAnon"] = carrito;
            }
            cantidad = carrito.Values.Sum();

            var ids = new List<int>();
            foreach (var entry in carrito)
            {
                for (int i = 0; i < entry.Value; i++)
                {
                    ids.Add(entry.Key);
                }
            }
            var jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = jsSerializer.Serialize(ids);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "syncLocalCart",
                $"localStorage.setItem('carrito', '{json}');", true);
        }

        lblCarritoContador.InnerText = cantidad.ToString();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "updateCartCounter",
            $"(function(){{ var lbl = document.querySelector('[id*=\"lblCarritoContador\"]'); if(lbl) lbl.innerText = '{cantidad}'; }})();", true);
    }
}
