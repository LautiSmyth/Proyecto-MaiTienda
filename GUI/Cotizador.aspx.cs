using BE;
using BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Services.Protocols;
using System.Web.UI;
using System.Web.UI.WebControls;

using WsCliente = WSCatalogoRef.WSCatalogo;
using WsItemPedido = WSCatalogoRef.BEItemPedido;
using WsCotizacion = WSCatalogoRef.BECotizacion;
using WsLinea = WSCatalogoRef.BELineaCotizacion;

public partial class Cotizador : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (EstadoIntegridad.TablaAlterada(BLLIntegridad.TablaProducto))
        {
            btnCotizarCarrito.Enabled = false;
            btnCotizarProducto.Enabled = false;
            pnlResultado.Visible = false;
            MostrarMensaje(EstadoIntegridad.MensajeMantenimiento, false);
            return;
        }

        if (!IsPostBack)
            CargarProductos();
    }

    private void CargarProductos()
    {
        try
        {
            using (WsCliente ws = CrearClienteWS())
            {
                var productos = ws.ObtenerCatalogo("");
                ddlProducto.DataSource = productos.Select(p => new
                {
                    p.IdProducto,
                    Texto = p.Nombre + " ($" + p.Precio.ToString("N2") + ")"
                });
                ddlProducto.DataValueField = "IdProducto";
                ddlProducto.DataTextField = "Texto";
                ddlProducto.DataBind();
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje("No se pudo conectar con el Web Service: " + ex.Message, false);
        }
    }

    protected void btnCotizarCarrito_Click(object sender, EventArgs e)
    {
        BEUsuario usuario = ObtenerUsuarioLogueado();
        if (usuario == null)
        {
            MostrarMensaje("Tenés que iniciar sesión para cotizar tu carrito.", false);
            return;
        }

        List<BEProducto> carrito = new BLLCarrito().ObtenerProductos(usuario.IdUsuario);
        if (carrito.Count == 0)
        {
            MostrarMensaje("Tu carrito está vacío.", false);
            return;
        }

        WsItemPedido[] items = carrito
            .Select(p => new WsItemPedido { IdProducto = p.IdProducto, Cantidad = p.Stock })
            .ToArray();

        Cotizar(ws => ws.CotizarPedido(items));
    }

    protected void btnCotizarProducto_Click(object sender, EventArgs e)
    {
        int idProducto;
        int cantidad;
        if (!int.TryParse(ddlProducto.SelectedValue, out idProducto) || !int.TryParse(txtCantidad.Text, out cantidad))
        {
            MostrarMensaje("Seleccioná un producto e ingresá una cantidad válida.", false);
            return;
        }

        Cotizar(ws => ws.CotizarProducto(idProducto, cantidad));
    }

    private void Cotizar(Func<WsCliente, WsCotizacion> llamada)
    {
        try
        {
            WsCotizacion cotizacion;
            using (WsCliente ws = CrearClienteWS())
            {
                cotizacion = llamada(ws);
            }
            MostrarCotizacion(cotizacion);
            RegistrarCotizacionEnBitacora(cotizacion);
        }
        catch (SoapException ex)
        {
            pnlResultado.Visible = false;
            MostrarMensaje("El Web Service rechazó la cotización: " + ex.Message, false);
        }
        catch (Exception ex)
        {
            pnlResultado.Visible = false;
            MostrarMensaje("Error al comunicarse con el Web Service: " + ex.Message, false);
        }
    }

    private WsCliente CrearClienteWS()
    {
        WsCliente ws = new WsCliente();
        string urlConfigurada = ConfigurationManager.AppSettings["WSCatalogoRef.WSCatalogo"];
        ws.Url = string.IsNullOrWhiteSpace(urlConfigurada)
            ? new Uri(Request.Url, ResolveUrl("~/WSCatalogo.asmx")).ToString()
            : urlConfigurada.Trim();
        ws.Timeout = 15000;
        return ws;
    }

    private BEUsuario ObtenerUsuarioLogueado()
    {
        string nombreUsuario = Session["nombreUsuario"] as string;
        if (string.IsNullOrEmpty(nombreUsuario))
            return null;
        return new BLLUsuario().BuscarPorNombreUsuario(nombreUsuario);
    }

    private void RegistrarCotizacionEnBitacora(WsCotizacion cotizacion)
    {
        try
        {
            BEUsuario usuario = ObtenerUsuarioLogueado();
            if (usuario == null)
                return;

            new SERVICIOS.ServicioBitacora().RegistrarEvento(
                "Cotizacion via WS: " + cotizacion.Lineas.Length + " producto(s), total $" + cotizacion.Total.ToString("N2"),
                usuario);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError(ex.ToString());
        }
    }

    private void MostrarCotizacion(WsCotizacion cotizacion)
    {
        gvLineas.DataSource = cotizacion.Lineas;
        gvLineas.DataBind();

        litFecha.Text = cotizacion.Fecha.ToString("dd/MM/yyyy HH:mm");
        litVigencia.Text = cotizacion.VigenteHasta.ToString("dd/MM/yyyy HH:mm");
        litUnidades.Text = cotizacion.CantidadTotalUnidades.ToString();
        litTotal.Text = "$" + cotizacion.Total.ToString("N2");

        pnlResultado.Visible = true;
        MostrarMensaje(cotizacion.Mensaje, cotizacion.PedidoDisponible);
    }

    protected void gvLineas_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow) return;

        WsLinea linea = (WsLinea)e.Row.DataItem;
        e.Row.Cells[6].ForeColor = linea.Disponible
            ? System.Drawing.Color.SeaGreen
            : System.Drawing.Color.Firebrick;
    }

    private void MostrarMensaje(string texto, bool exito)
    {
        lblMensaje.Text = texto;
        lblMensaje.Visible = true;
        lblMensaje.Style["background"] = exito ? "#e7f6ec" : "#fdecea";
        lblMensaje.Style["color"] = exito ? "#1e6b3a" : "#8a1c1c";
    }
}
