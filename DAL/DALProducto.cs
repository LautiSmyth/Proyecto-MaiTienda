using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class DALProducto
    {
        private Acceso _acceso = Acceso.GetInstance();

        public List<BEProducto> ObtenerProductos(
            string categoria = null, 
            string busqueda = null, 
            string orden = null, 
            decimal? precioMin = null, 
            decimal? precioMax = null, 
            string marca = null)
        {
            List<BEProducto> lista = new List<BEProducto>();
            string consulta = "SELECT IdProducto, Nombre, Categoria, Precio, ImagenUrl, Descripcion, Stock FROM Producto WHERE 1=1";
            List<SqlParameter> parametros = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(categoria) && categoria != "Todos")
            {
                consulta += " AND Categoria = @Categoria";
                parametros.Add(new SqlParameter("@Categoria", categoria));
            }

            if (!string.IsNullOrEmpty(busqueda))
            {
                consulta += " AND (Nombre LIKE @Busqueda OR Descripcion LIKE @Busqueda)";
                parametros.Add(new SqlParameter("@Busqueda", $"%{busqueda}%"));
            }

            if (precioMin.HasValue)
            {
                consulta += " AND Precio >= @PrecioMin";
                parametros.Add(new SqlParameter("@PrecioMin", precioMin.Value));
            }

            if (precioMax.HasValue)
            {
                consulta += " AND Precio <= @PrecioMax";
                parametros.Add(new SqlParameter("@PrecioMax", precioMax.Value));
            }

            if (!string.IsNullOrEmpty(marca) && marca != "Todas")
            {
                consulta += " AND Nombre LIKE @Marca";
                parametros.Add(new SqlParameter("@Marca", $"%{marca}%"));
            }

            if (!string.IsNullOrEmpty(orden))
            {
                if (orden == "PrecioAsc")
                {
                    consulta += " ORDER BY Precio ASC";
                }
                else if (orden == "PrecioDesc")
                {
                    consulta += " ORDER BY Precio DESC";
                }
            }

            DataTable dt = _acceso.Leer(consulta, parametros.ToArray());

            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new BEProducto
                {
                    IdProducto = Convert.ToInt32(row["IdProducto"]),
                    Nombre = row["Nombre"].ToString(),
                    Categoria = row["Categoria"].ToString(),
                    Precio = Convert.ToDecimal(row["Precio"]),
                    ImagenUrl = row["ImagenUrl"].ToString(),
                    Descripcion = row["Descripcion"].ToString(),
                    Stock = Convert.ToInt32(row["Stock"])
                });
            }

            return lista;
        }
        public List<string> ObtenerCategorias()
        {
            List<string> lista = new List<string>();
            string consulta = "SELECT DISTINCT Categoria FROM Producto WHERE Categoria IS NOT NULL AND Categoria <> '' ORDER BY Categoria ASC";
            DataTable dt = _acceso.Leer(consulta, new SqlParameter[0]);
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(row["Categoria"].ToString());
            }
            return lista;
        }
    }
}
