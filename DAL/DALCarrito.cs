using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class DALCarrito
    {
        private readonly Acceso _acceso = Acceso.GetInstance();

        public void AgregarProducto(int idUsuario, int idProducto, int cantidad = 1)
        {
            string query = @"
                MERGE INTO ItemCarrito AS target
                USING (SELECT @IdUsuario AS IdUsuario, @IdProducto AS IdProducto) AS source
                ON (target.IdUsuario = source.IdUsuario AND target.IdProducto = source.IdProducto)
                WHEN MATCHED THEN
                    UPDATE SET target.Cantidad = target.Cantidad + @Cantidad
                WHEN NOT MATCHED THEN
                    INSERT (IdUsuario, IdProducto, Cantidad) VALUES (source.IdUsuario, source.IdProducto, @Cantidad);";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@IdUsuario", idUsuario),
                new SqlParameter("@IdProducto", idProducto),
                new SqlParameter("@Cantidad", cantidad)
            };

            _acceso.Escribir(query, parameters);
        }

        public List<BEProducto> ObtenerProductos(int idUsuario)
        {
            List<BEProducto> lista = new List<BEProducto>();
            string query = @"
                SELECT p.IdProducto, p.Nombre, p.Categoria, p.Precio, p.ImagenUrl, p.Descripcion, ic.Cantidad AS Stock
                FROM ItemCarrito ic
                INNER JOIN Producto p ON ic.IdProducto = p.IdProducto
                WHERE ic.IdUsuario = @IdUsuario";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@IdUsuario", idUsuario)
            };

            DataTable dt = _acceso.Leer(query, parameters);

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

        public int ObtenerCantidadTotal(int idUsuario)
        {
            string query = "SELECT ISNULL(SUM(Cantidad), 0) FROM ItemCarrito WHERE IdUsuario = @IdUsuario";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@IdUsuario", idUsuario)
            };

            DataTable dt = _acceso.Leer(query, parameters);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            return 0;
        }

        public void SincronizarCarrito(int idUsuario, List<int> productoIds)
        {
            if (productoIds == null || productoIds.Count == 0) return;

            foreach (int idProducto in productoIds)
            {
                AgregarProducto(idUsuario, idProducto, 1);
            }
        }

        public HashSet<int> ObtenerIdsProductos(int idUsuario)
        {
            HashSet<int> ids = new HashSet<int>();
            string query = "SELECT IdProducto FROM ItemCarrito WHERE IdUsuario = @IdUsuario";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@IdUsuario", idUsuario)
            };

            DataTable dt = _acceso.Leer(query, parameters);
            foreach (DataRow row in dt.Rows)
                ids.Add(Convert.ToInt32(row["IdProducto"]));

            return ids;
        }

        public void LimpiarCarrito(int idUsuario)
        {
            string query = "DELETE FROM ItemCarrito WHERE IdUsuario = @IdUsuario";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@IdUsuario", idUsuario)
            };

            _acceso.Escribir(query, parameters);
        }
    }
}
