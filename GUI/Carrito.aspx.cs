using BE;
using BLL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Carrito : Page
{
    private readonly BLLCarrito _bllCarrito = new BLLCarrito();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            CargarCarrito();
    }

    private void CargarCarrito()
    {
        var usuario = SERVICIOS.SessionManager.GetInstance().Usuario;

        if (usuario != null)
        {
            var productos = _bllCarrito.ObtenerProductos(usuario.IdUsuario);

            phCarritoServidor.Visible = true;
            divCarritoVacio.Style["display"] = productos.Any() ? "none" : "block";
            btnCheckout.Enabled = productos.Any();

            repCarrito.DataSource = productos;
            repCarrito.DataBind();

            decimal subtotal = productos.Sum(p => p.Precio * p.Stock);
            lblSubtotal.InnerText = $"${subtotal:N2}";
            lblTotal.InnerText = $"${subtotal:N2}";
        }
        else
        {
            phCarritoServidor.Visible = false;
            divCarritoVacio.Style["display"] = "none";
        }
    }

    protected void repCarrito_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        var usuario = SERVICIOS.SessionManager.GetInstance().Usuario;
        if (usuario == null) return;

        int idProducto = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "Sumar")
        {
            var prodReal = new BLLProducto().ObtenerProductos().FirstOrDefault(p => p.IdProducto == idProducto);
            int stockDisponible = prodReal != null ? prodReal.Stock : 0;

            var itemsEnCarrito = _bllCarrito.ObtenerProductos(usuario.IdUsuario);
            var itemActual = itemsEnCarrito.FirstOrDefault(p => p.IdProducto == idProducto);
            int cantidadActual = itemActual != null ? itemActual.Stock : 0;

            if (cantidadActual < stockDisponible)
            {
                _bllCarrito.AgregarProducto(usuario.IdUsuario, idProducto, 1);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "noStockAlert",
                    "alert('No hay suficiente stock disponible para este producto.');", true);
            }
        }
        else if (e.CommandName == "Restar")
            _bllCarrito.AgregarProducto(usuario.IdUsuario, idProducto, -1);
        else if (e.CommandName == "Eliminar")
        {
            _bllCarrito.AgregarProducto(usuario.IdUsuario, idProducto, -9999);
            DAL.Acceso.GetInstance().Escribir(
                "DELETE FROM ItemCarrito WHERE IdUsuario = @IdUsuario AND IdProducto = @IdProducto",
                new SqlParameter[] {
                    new SqlParameter("@IdUsuario", usuario.IdUsuario),
                    new SqlParameter("@IdProducto", idProducto)
                }
            );
        }

        DAL.Acceso.GetInstance().Escribir(
            "DELETE FROM ItemCarrito WHERE IdUsuario = @IdUsuario AND Cantidad <= 0",
            new SqlParameter[] { new SqlParameter("@IdUsuario", usuario.IdUsuario) }
        );

        (Master as SiteMaster)?.ActualizarCarritoContador();
        CargarCarrito();
    }

    protected void btnCheckout_Click(object sender, EventArgs e)
    {
        var usuario = SERVICIOS.SessionManager.GetInstance().Usuario;

        if (usuario != null)
        {
            _bllCarrito.LimpiarCarrito(usuario.IdUsuario);
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "clearCart", "localStorage.removeItem('carrito');", true);
            Session["CarritoAnon"] = null;
            SiteMaster.LimpiarCarritoCookie(Context);
        }

        (Master as SiteMaster)?.ActualizarCarritoContador();
        Response.Redirect("~/Bienvenida.aspx");
    }

    [WebMethod(EnableSession = true)]
    public static object ObtenerDetallesCarritoLocal(List<int> productoIds)
    {
        if (productoIds == null || productoIds.Count == 0)
            return new List<object>();

        var agrupados = productoIds.GroupBy(id => id).ToDictionary(g => g.Key, g => g.Count());
        var todos = new BLLProducto().ObtenerProductos();

        return agrupados
             .Select(entry =>
             {
                 var prod = todos.FirstOrDefault(p => p.IdProducto == entry.Key);
                 if (prod == null) return null;
                 return new
                 {
                     IdProducto  = prod.IdProducto,
                     Nombre      = prod.Nombre,
                     Categoria   = prod.Categoria,
                     Precio      = prod.Precio,
                     ImagenUrl   = prod.ImagenUrl,
                     Descripcion = prod.Descripcion,
                     Stock       = prod.Stock,
                     Cantidad    = entry.Value
                 };
             })
             .Where(x => x != null)
             .ToList();
    }
}
