using BE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class BLLCotizacion
    {
        private const int HorasVigencia = 24;
        private const int MaximoItemsPorPedido = 50;

        private readonly BLLProducto _bllProducto = new BLLProducto();

        public BECotizacion Cotizar(List<BEItemPedido> items)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException("El pedido debe tener al menos un producto.");

            if (items.Count > MaximoItemsPorPedido)
                throw new ArgumentException("El pedido supera el máximo de " + MaximoItemsPorPedido + " ítems.");

            List<BEItemPedido> agrupados = items
                .Where(i => i != null)
                .GroupBy(i => i.IdProducto)
                .Select(g => new BEItemPedido { IdProducto = g.Key, Cantidad = g.Sum(x => x.Cantidad) })
                .ToList();

            Dictionary<int, BEProducto> catalogo = _bllProducto.ObtenerProductos()
                .ToDictionary(p => p.IdProducto);

            DateTime ahora = DateTime.Now;
            BECotizacion cotizacion = new BECotizacion
            {
                Fecha = ahora,
                VigenteHasta = ahora.AddHours(HorasVigencia)
            };

            foreach (BEItemPedido item in agrupados)
            {
                cotizacion.Lineas.Add(CotizarLinea(item, catalogo));
            }

            cotizacion.Total = cotizacion.Lineas.Sum(l => l.Subtotal);
            cotizacion.CantidadTotalUnidades = cotizacion.Lineas.Sum(l => l.CantidadSolicitada);
            cotizacion.PedidoDisponible = cotizacion.Lineas.All(l => l.Disponible);
            cotizacion.Mensaje = cotizacion.PedidoDisponible
                ? "Pedido disponible. Precios válidos hasta " + cotizacion.VigenteHasta.ToString("dd/MM/yyyy HH:mm") + "."
                : "Hay productos sin stock suficiente o inexistentes. Revisar las observaciones de cada línea.";

            return cotizacion;
        }

        private BELineaCotizacion CotizarLinea(BEItemPedido item, Dictionary<int, BEProducto> catalogo)
        {
            BELineaCotizacion linea = new BELineaCotizacion
            {
                IdProducto = item.IdProducto,
                CantidadSolicitada = item.Cantidad
            };

            BEProducto producto;
            if (!catalogo.TryGetValue(item.IdProducto, out producto))
            {
                linea.Disponible = false;
                linea.Observacion = "Producto inexistente.";
                return linea;
            }

            linea.Nombre = producto.Nombre;
            linea.PrecioUnitario = producto.Precio;
            linea.StockDisponible = producto.Stock;

            if (item.Cantidad <= 0)
            {
                linea.Disponible = false;
                linea.Observacion = "La cantidad debe ser mayor a cero.";
                return linea;
            }

            linea.Subtotal = producto.Precio * item.Cantidad;

            if (producto.Stock < item.Cantidad)
            {
                linea.Disponible = false;
                linea.Observacion = "Stock insuficiente. Disponible: " + producto.Stock + ".";
                return linea;
            }

            linea.Disponible = true;
            linea.Observacion = "OK";
            return linea;
        }
    }
}
