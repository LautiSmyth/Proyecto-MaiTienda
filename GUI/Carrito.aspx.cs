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
            // Logueado: Renderizar desde el Servidor
            List<BEProducto> productos = _bllCarrito.ObtenerProductos(usuario.IdUsuario);
            
            phCarritoServidor.Visible = true;
            divCarritoVacio.Style["display"] = productos.Any() ? "none" : "block";
            btnCheckout.Enabled = productos.Any();

            repCarrito.DataSource = productos;
            repCarrito.DataBind();

            // Calcular Precios
            decimal subtotal = productos.Sum(p => p.Precio * p.Stock); // Usamos Stock temporalmente para cantidad
            lblSubtotal.InnerText = $"${subtotal:N2}";
            lblTotal.InnerText = $"${subtotal:N2}";
        }
        else
        {
            // Deslogueado: Se renderizará en el cliente por JavaScript llamando al WebMethod
            phCarritoServidor.Visible = false;
            divCarritoVacio.Style["display"] = "none"; // Mantener en el DOM pero oculto inicialmente
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
            // Para restar mandamos cantidad negativa
            _bllCarrito.AgregarProducto(usuario.IdUsuario, idProducto, -1);
        }
        else if (e.CommandName == "Eliminar")
        {
            // Eliminar producto por completo
            // Podemos hacer una lógica que ponga la cantidad en cero o una llamada directa de borrado
            _bllCarrito.AgregarProducto(usuario.IdUsuario, idProducto, -9999); // Nuestra DAL con merge restará y si da <=0, podemos borrarlo o dejarlo en 0.
            // Para simplificar, agreguemos un método en DALCarrito que elimine el producto específico del carrito del usuario.
            // O bien, usemos AgregarProducto con un valor muy negativo.
            // Modifiquemos la DAL o usemos un query directo para borrar. Para hacerlo limpio, usemos BLLCarrito.LimpiarCarritoItem que crearemos si hace falta, o hagamos un Delete directo.
            // De hecho, en DALCarrito creamos LimpiarCarrito. Podemos modificar DALCarrito para agregar eliminar un producto específico, o simplemente hacerlo aquí.
            // Hagamos un comando simple a través de Acceso para borrar la fila:
            DAL.Acceso.GetInstance().Escribir(
                "DELETE FROM ItemCarrito WHERE IdUsuario = @IdUsuario AND IdProducto = @IdProducto",
                new SqlParameter[] {
                    new SqlParameter("@IdUsuario", usuario.IdUsuario),
                    new SqlParameter("@IdProducto", idProducto)
                }
            );
        }

        // Después de restar, borrar los que queden con cantidad <= 0
        DAL.Acceso.GetInstance().Escribir(
            "DELETE FROM ItemCarrito WHERE IdUsuario = @IdUsuario AND Cantidad <= 0",
            new SqlParameter[] {
                new SqlParameter("@IdUsuario", usuario.IdUsuario)
            }
        );

        // Actualizar Navbar y recargar
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
            // Logueado: Limpiar carrito en BD
            _bllCarrito.LimpiarCarrito(usuario.IdUsuario);
        }
        else
        {
            // Deslogueado: Se limpiará en el cliente antes de redirigir
            // Usamos un script del lado del cliente para limpiar localStorage
            ClientScript.RegisterStartupScript(this.GetType(), "clearCart", "localStorage.removeItem('carrito');", true);
            Session["Carrito"] = null;
        }

        // Actualizar Navbar
        var master = Master as SiteMaster;
        if (master != null)
        {
            master.ActualizarCarritoContador();
        }

        // Redirigir a Respuesta.aspx que avisa compra exitosa
        Response.Redirect("~/Respuesta.aspx");
    }

    [WebMethod(EnableSession = true)]
    public static List<BEProducto> ObtenerDetallesCarritoLocal(List<int> productoIds)
    {
        if (productoIds == null || productoIds.Count == 0)
            return new List<BEProducto>();

        // Agrupar por ID y contar las ocurrencias para saber la cantidad
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
                    Stock = entry.Value // Cantidad agregada en el cliente
                });
            }
        }

        return productosFiltrados;
    }
}
