using BE;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

public partial class MisCompras : Page
{
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

        int idPedido;
        if (int.TryParse(Request.QueryString["pedido"], out idPedido) && compras.Any(c => c.IdPedido == idPedido))
        {
            lblMensaje.Text = "¡Compra registrada! Tu pedido Nº " + idPedido + " quedó guardado.";
            lblMensaje.Visible = true;
        }
    }

    protected string DescribirLineas(BEPedido pedido)
    {
        return string.Join("<br />", pedido.Lineas.Select(l => Server.HtmlEncode(l.Nombre) + " × " + l.Cantidad));
    }
}
