using BE;
using DAL;
using System.Collections.Generic;

namespace BLL
{
    public class BLLCarrito
    {
        private readonly DALCarrito _dalCarrito = new DALCarrito();

        public void AgregarProducto(int idUsuario, int idProducto, int cantidad = 1)
        {
            _dalCarrito.AgregarProducto(idUsuario, idProducto, cantidad);
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

        public void LimpiarCarrito(int idUsuario)
        {
            _dalCarrito.LimpiarCarrito(idUsuario);
        }
    }
}
