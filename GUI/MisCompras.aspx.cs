using BE;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

public partial class MisCompras : Page
{
    private readonly BLLFactura _bllFactura = new BLLFactura();

    protected void Page_Load(object sender, EventArgs e)
    {
        string nombreUsuario = Session["nombreUsuario"] as string;
        if (Session["perfil"] == null || string.IsNullOrEmpty(nombreUsuario))
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        if (!IsPostBack)
            CargarCompras(nombreUsuario);
    }

    private void CargarCompras(string nombreUsuario)
    {
        BEUsuario usuario = new BLLUsuario().BuscarPorNombreUsuario(nombreUsuario);
        List<BEPedido> compras = usuario == null
            ? new List<BEPedido>()
            : new BLLPedido().ObtenerMisCompras(usuario.IdUsuario);

        pnlVacio.Visible = compras.Count == 0;
        gvCompras.Visible = compras.Count > 0;
        gvCompras.DataSource = compras;
        gvCompras.DataBind();
    }

    protected string DescribirLineas(BEPedido pedido)
    {
        return string.Join("<br />", pedido.Lineas.Select(l => Server.HtmlEncode(l.Nombre) + " × " + l.Cantidad));
    }

    protected string DescribirFactura(BEPedido pedido)
    {
        if (pedido.Factura == null)
            return "—";

        return "<a href=\"" + ResolveUrl("~/Factura.aspx?pedido=" + pedido.IdPedido) + "\">" +
            BLLFactura.Tipo + " " + _bllFactura.FormatearNumero(pedido.Factura) + "</a>";
    }
}
