using BE;
using DAL;
using System.Collections.Generic;

namespace BLL
{
    public class BLLProducto
    {
        private DALProducto _dalProducto = new DALProducto();

        public List<BEProducto> ObtenerProductos(
            string categoria = null, 
            string busqueda = null, 
            string orden = null, 
            decimal? precioMin = null, 
            decimal? precioMax = null, 
            string marca = null)
        {
            return _dalProducto.ObtenerProductos(categoria, busqueda, orden, precioMin, precioMax, marca);
        }

        public List<string> ObtenerCategorias()
        {
            return _dalProducto.ObtenerCategorias();
        }
    }
}
