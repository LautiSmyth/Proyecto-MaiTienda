using BE;
using BLL;
using System;
using System.Web.UI;

public partial class Factura : Page
{
    private readonly BLLFactura _bllFactura = new BLLFactura();

    protected void Page_Load(object sender, EventArgs e)
    {
        string nombreUsuario = Session["nombreUsuario"] as string;
        string perfil = Session["perfil"] as string;
        if (string.IsNullOrEmpty(perfil) || string.IsNullOrEmpty(nombreUsuario))
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        if (IsPostBack)
            return;

        BEUsuario usuario = new BLLUsuario().BuscarPorNombreUsuario(nombreUsuario);
        bool veTodos = perfil == "WebMaster" || perfil == "Administrador";

        int idPedido;
        BEPedido pedido = int.TryParse(Request.QueryString["pedido"], out idPedido)
            ? new BLLPedido().ObtenerPedido(idPedido, usuario, veTodos)
            : null;

        if (pedido == null || pedido.Factura == null)
        {
            pnlNoEncontrada.Visible = true;
            return;
        }

        MostrarFactura(pedido);
    }

    private void MostrarFactura(BEPedido pedido)
    {
        BEFactura factura = pedido.Factura;

        litEmisor.Text = Server.HtmlEncode(BLLFactura.EmisorRazonSocial);
        litEmisorDomicilio.Text = Server.HtmlEncode(BLLFactura.EmisorDomicilio);
        litEmisorCuit.Text = Server.HtmlEncode(BLLFactura.EmisorCuit);
        litEmisorCondicionIva.Text = Server.HtmlEncode(BLLFactura.EmisorCondicionIva);
        litTipo.Text = BLLFactura.Tipo;
        litCodigo.Text = BLLFactura.CodigoComprobante;
        litNumero.Text = _bllFactura.FormatearNumero(factura);
        litFecha.Text = factura.Fecha.ToString("dd/MM/yyyy");
        litPedido.Text = pedido.IdPedido.ToString();

        litCliente.Text = Server.HtmlEncode(pedido.NombreCliente);
        litDocumento.Text = Server.HtmlEncode(pedido.DocumentoCliente);
        litDireccion.Text = Server.HtmlEncode(pedido.DireccionEnvio);
        litCondicionCliente.Text = Server.HtmlEncode(BLLFactura.ClienteCondicionIva);

        repLineas.DataSource = pedido.Lineas;
        repLineas.DataBind();
        litEnvioDetalle.Text = pedido.Envio == 0
            ? "Envío (bonificado, a " + pedido.DistanciaKm.ToString("N1") + " km del negocio)"
            : "Envío (a " + pedido.DistanciaKm.ToString("N1") + " km del negocio)";
        litEnvioPrecio.Text = pedido.Envio.ToString("N2");
        litEnvioSubtotal.Text = pedido.Envio.ToString("N2");

        litNeto.Text = factura.Neto.ToString("N2");
        litAlicuota.Text = (BLLFactura.AlicuotaIva * 100).ToString("0") + "%";
        litIva.Text = factura.IVA.ToString("N2");
        litTotal.Text = factura.Total.ToString("N2");

        lblNueva.Visible = Request.QueryString["nueva"] == "1";
        pnlFactura.Visible = true;
    }
}
