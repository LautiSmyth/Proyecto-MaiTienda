using BE;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Web.Services.Protocols;

[WebService(Namespace = "http://maitienda.com/ws/catalogo", Description = "MaiTienda - Consulta de catálogo, stock y cotización de pedidos.")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class WSCatalogo : WebService
{
    private readonly BLLProducto _bllProducto = new BLLProducto();
    private readonly BLLCotizacion _bllCotizacion = new BLLCotizacion();

    [WebMethod(Description = "Devuelve las categorías de productos disponibles.")]
    public List<string> ObtenerCategorias()
    {
        return Ejecutar(() => _bllProducto.ObtenerCategorias());
    }

    [WebMethod(Description = "Devuelve los productos del catálogo. Dejar 'categoria' vacío para traer todos.")]
    public List<ProductoWS> ObtenerCatalogo(string categoria)
    {
        return Ejecutar(() => _bllProducto.ObtenerProductos(categoria)
            .Select(ProductoWS.DesdeEntidad)
            .ToList());
    }

    [WebMethod(Description = "Devuelve un producto con su precio y stock actual. Devuelve vacío si no existe.")]
    public ProductoWS ConsultarProducto(int idProducto)
    {
        return Ejecutar(() =>
        {
            BEProducto producto = _bllProducto.ObtenerProductos().Find(p => p.IdProducto == idProducto);
            return producto == null ? null : ProductoWS.DesdeEntidad(producto);
        });
    }

    [WebMethod(Description = "Cotiza un único producto. Versión simple de CotizarPedido, se puede probar desde el navegador.")]
    public BECotizacion CotizarProducto(int idProducto, int cantidad)
    {
        List<BEItemPedido> items = new List<BEItemPedido>
        {
            new BEItemPedido { IdProducto = idProducto, Cantidad = cantidad }
        };
        return CotizarPedido(items);
    }

    [WebMethod(Description = "Cotiza un pedido con varios productos: valida stock, calcula subtotales y total.")]
    public BECotizacion CotizarPedido(List<BEItemPedido> items)
    {
        return Ejecutar(() =>
        {
            // El WS no escribe en la bitacora: lo puede llamar cualquier sistema externo sin sesion
            // y cada llamada anonima llenaria la tabla. Registra el cliente que conoce al usuario (Cotizador.aspx).
            return _bllCotizacion.Cotizar(items);
        });
    }

    private static T Ejecutar<T>(Func<T> accion)
    {
        try
        {
            return accion();
        }
        catch (ArgumentException ex)
        {
            throw new SoapException(ex.Message, SoapException.ClientFaultCode);
        }
        catch (InvalidOperationException ex)
        {
            throw new SoapException(ex.Message, SoapException.ServerFaultCode);
        }
    }
}

public class ProductoWS
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; }
    public string Categoria { get; set; }
    public decimal Precio { get; set; }
    public string Descripcion { get; set; }
    public int Stock { get; set; }
    public bool HayStock { get; set; }

    public static ProductoWS DesdeEntidad(BEProducto p)
    {
        return new ProductoWS
        {
            IdProducto = p.IdProducto,
            Nombre = p.Nombre,
            Categoria = p.Categoria,
            Precio = p.Precio,
            Descripcion = p.Descripcion,
            Stock = p.Stock,
            HayStock = p.Stock > 0
        };
    }
}
