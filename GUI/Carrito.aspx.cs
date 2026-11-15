using BE;
using BLL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.IO;
using System.Xml;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Carrito : Page
{
    private readonly BLLCarrito _bllCarrito = new BLLCarrito();

    protected void Page_Load(object sender, EventArgs e)
    {
        Form.Enctype = "multipart/form-data";
        if (!IsPostBack)
            CargarCarrito();
    }

    private BEUsuario ObtenerUsuarioSesion()
    {
        string nombre = Session["nombreUsuario"] as string;
        return Session["perfil"] == null || string.IsNullOrEmpty(nombre)
            ? null : new BLLUsuario().BuscarPorNombreUsuario(nombre);
    }

    protected void btnExportarXml_Click(object sender, EventArgs e)
    {
        var usuario = ObtenerUsuarioSesion();
        if (usuario == null) { lblXml.Text = "Iniciá sesión para exportar tu carrito."; return; }
        var items = _bllCarrito.ObtenerProductos(usuario.IdUsuario)
            .Select(p => new BEItemPedido { IdProducto = p.IdProducto, Cantidad = p.Stock }).ToList();
        byte[] archivo = CarritoXml.GuardarCarrito(items);
        Response.Clear();
        Response.ContentType = "application/xml";
        Response.AddHeader("Content-Disposition", "attachment; filename=carrito.xml");
        Response.BinaryWrite(archivo);
        Response.End();
    }

    protected void btnImportarXml_Click(object sender, EventArgs e)
    {
        var usuario = ObtenerUsuarioSesion();
        if (usuario == null) { lblXml.Text = "Iniciá sesión para importar tu carrito."; return; }
        if (!archivoCarritoXml.HasFile || archivoCarritoXml.PostedFile.ContentLength > 65536 ||
            !string.Equals(Path.GetExtension(archivoCarritoXml.FileName), ".xml", StringComparison.OrdinalIgnoreCase))
        {
            lblXml.Text = "Seleccioná un archivo .xml de hasta 64 KB.";
            return;
        }
        try
        {
            using (var archivo = archivoCarritoXml.PostedFile.InputStream)
                _bllCarrito.ImportarCarrito(usuario.IdUsuario, CarritoXml.LeerCarrito(archivo));
        }
        catch (XmlException) { lblXml.Text = "El XML no tiene el formato de un carrito válido."; return; }
        catch (ArgumentException ex) { lblXml.Text = Server.HtmlEncode(ex.Message); return; }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError(ex.ToString());
            lblXml.Text = "No se pudo importar el archivo. Tu carrito no fue reemplazado.";
            return;
        }
        CargarCarrito();
        (Master as SiteMaster)?.ActualizarCarritoContador();
        lblXml.Text = "Carrito importado correctamente.";
    }

    private void CargarCarrito()
    {
        var usuario = ObtenerUsuarioSesion();

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
        var usuario = ObtenerUsuarioSesion();
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
        var usuario = ObtenerUsuarioSesion();

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
