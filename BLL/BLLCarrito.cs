using BE;
using DAL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class BLLCarrito
    {
        private readonly DALCarrito _dalCarrito = new DALCarrito();

        public void AgregarProducto(int idUsuario, int idProducto, int cantidad = 1)
        {
            var bllProducto = new BLLProducto();
            var prod = bllProducto.ObtenerProductos().Find(p => p.IdProducto == idProducto);
            if (prod == null) return;

            int stockDisponible = prod.Stock;

            var cantidades = _dalCarrito.ObtenerCantidadesProductos(idUsuario);
            int cantidadActual = 0;
            cantidades.TryGetValue(idProducto, out cantidadActual);

            int nuevaCantidad = cantidadActual + cantidad;
            if (nuevaCantidad > stockDisponible) nuevaCantidad = stockDisponible;
            if (nuevaCantidad < 0) nuevaCantidad = 0;

            _dalCarrito.ActualizarCantidad(idUsuario, idProducto, nuevaCantidad);
        }

        public List<BEProducto> ObtenerProductos(int idUsuario)
        {
            return _dalCarrito.ObtenerProductos(idUsuario);
        }

        public int ObtenerCantidadTotal(int idUsuario)
        {
            return _dalCarrito.ObtenerCantidadTotal(idUsuario);
        }

        public void SincronizarCarrito(int idUsuario, List<int> productoIds)
        {
            _dalCarrito.SincronizarCarrito(idUsuario, productoIds);
        }

        public HashSet<int> ObtenerIdsProductos(int idUsuario)
        {
            return _dalCarrito.ObtenerIdsProductos(idUsuario);
        }

        public Dictionary<int, int> ObtenerCantidadesProductos(int idUsuario)
        {
            return _dalCarrito.ObtenerCantidadesProductos(idUsuario);
        }

        public void ActualizarCantidad(int idUsuario, int idProducto, int nuevaCantidad, int stockDisponible)
        {
            if (nuevaCantidad > stockDisponible) nuevaCantidad = stockDisponible;
            if (nuevaCantidad < 0) nuevaCantidad = 0;
            _dalCarrito.ActualizarCantidad(idUsuario, idProducto, nuevaCantidad);
        }

        public void ImportarCarrito(int idUsuario, List<BEItemPedido> items)
        {
            if (items == null) throw new ArgumentException("El carrito XML no es válido.");
            var catalogo = new BLLProducto().ObtenerProductos();
            var ids = new HashSet<int>();
            foreach (var item in items)
            {
                if (item == null || item.Cantidad <= 0)
                    throw new ArgumentException("Las cantidades deben ser mayores a cero.");
                if (!ids.Add(item.IdProducto))
                    throw new ArgumentException("El XML contiene un producto repetido.");
                var producto = catalogo.Find(p => p.IdProducto == item.IdProducto);
                if (producto == null)
                    throw new ArgumentException("El producto " + item.IdProducto + " no existe.");
                if (item.Cantidad > producto.Stock)
                    throw new ArgumentException("Stock insuficiente para " + producto.Nombre + ".");
            }
            _dalCarrito.ReemplazarCarrito(idUsuario, items);
        }

        public void LimpiarCarrito(int idUsuario)
        {
            _dalCarrito.LimpiarCarrito(idUsuario);
        }
    }
}
