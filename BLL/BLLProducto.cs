using BE;
using DAL;
using System.Collections.Generic;

namespace BLL
{
    public class BLLProducto
    {
        private DALProducto _dalProducto = new DALProducto();

        private readonly SERVICIOS.GestorIntegridad _gestorIntegridad = new SERVICIOS.GestorIntegridad();

        public List<BEProducto> ObtenerProductos(
            string categoria = null, 
            string busqueda = null, 
            string orden = null, 
            decimal? precioMin = null, 
            decimal? precioMax = null, 
            string marca = null)
        {
            List<BEProducto> productos = _dalProducto.ObtenerProductos(categoria, busqueda, orden, precioMin, precioMax, marca);
            foreach (var producto in productos)
            {
                string dvCalculado = _gestorIntegridad.CalcularDVH(producto);
                if (producto.DVH != dvCalculado)
                {
                    throw new System.InvalidOperationException("Error de integridad de datos: Un registro de producto ha sido alterado externamente.");
                }
            }
            return productos;
        }

        public List<string> ObtenerCategorias()
        {
            return _dalProducto.ObtenerCategorias();
        }
    }
}
