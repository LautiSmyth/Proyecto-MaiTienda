using BE;
using BLL;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

public partial class Carrito : Page
{
    private readonly BLLCarrito _bllCarrito = new BLLCarrito();
    private readonly BLLProducto _bllProducto = new BLLProducto();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarCarrito();
        }
    }

    private void CargarCarrito()
    {
        var usuario = SERVICIOS.SessionManager.GetInstance().Usuario;

        if (usuario != null)
        {
            List<BEProducto> productos = _bllCarrito.ObtenerProductos(usuario.IdUsuario);

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
            _bllCarrito.AgregarProducto(usuario.IdUsuario, idProducto, 1);
        }
        else if (e.CommandName == "Restar")
        {
            _bllCarrito.AgregarProducto(usuario.IdUsuario, idProducto, -1);
        }
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
            new SqlParameter[] {
                new SqlParameter("@IdUsuario", usuario.IdUsuario)
            }
        );

        var master = Master as SiteMaster;
        if (master != null)
        {
            master.ActualizarCarritoContador();
        }

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
            Session["Carrito"] = null;
        }

        var master = Master as SiteMaster;
        if (master != null)
        {
            master.ActualizarCarritoContador();
        }

        Response.Redirect("~/Bienvenida.aspx");
    }

    [WebMethod(EnableSession = true)]
    public static List<BEProducto> ObtenerDetallesCarritoLocal(List<int> productoIds)
    {
        if (productoIds == null || productoIds.Count == 0)
            return new List<BEProducto>();

        var agrupados = productoIds.GroupBy(id => id).ToDictionary(g => g.Key, g => g.Count());

        BLLProducto bllProducto = new BLLProducto();
        List<BEProducto> todosLosProductos = bllProducto.ObtenerProductos();

        List<BEProducto> productosFiltrados = new List<BEProducto>();

        foreach (var entry in agrupados)
        {
            var p = todosLosProductos.FirstOrDefault(prod => prod.IdProducto == entry.Key);
            if (p != null)
            {
                // Clonar objeto y usar Stock para guardar la cantidad
                productosFiltrados.Add(new BEProducto
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    Categoria = p.Categoria,
                    Precio = p.Precio,
                    ImagenUrl = p.ImagenUrl,
                    Descripcion = p.Descripcion,
                    Stock = entry.Value
                });
            }
        }

        return productosFiltrados;
    }
}
