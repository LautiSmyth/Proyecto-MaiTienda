using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class DALBitacora
    {
        private readonly Acceso _acceso = Acceso.GetInstance();

        // El DVH se guarda null al insertar: todavía no se conoce el IdBitacora que asigna la base.
        // ServicioBitacora lo calcula apenas vuelve el Id y lo guarda con ActualizarDVH.
        public void RegistrarEvento(BEBitacora bitacora)
        {
            string query = @"
                INSERT INTO Bitacora (IdUsuario, NombreUsuario, Perfil, Accion, Fecha, DVH)
                VALUES (@IdUsuario, @NombreUsuario, @Perfil, @Accion, @Fecha, @DVH);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@IdUsuario", bitacora.IdUsuario),
                new SqlParameter("@NombreUsuario", bitacora.NombreUsuario),
                new SqlParameter("@Perfil", bitacora.Perfil),
                new SqlParameter("@Accion", bitacora.Accion),
                new SqlParameter("@Fecha", bitacora.Fecha),
                new SqlParameter("@DVH", (object)bitacora.DVH ?? DBNull.Value)
            };
            DataTable resultado = _acceso.Leer(query, parameters);
            bitacora.IdBitacora = Convert.ToInt32(resultado.Rows[0][0]);
        }

        public List<BEBitacora> LeerBitacora()
        {
            List<BEBitacora> bitacoras = new List<BEBitacora>();
            string query = "SELECT IdBitacora, IdUsuario, NombreUsuario, Perfil, Accion, Fecha, DVH FROM Bitacora ORDER BY Fecha DESC";
            DataTable tabla = _acceso.Leer(query, null);

            foreach (DataRow row in tabla.Rows)
            {
                bitacoras.Add(Mapear(row));
            }

            return bitacoras;
        }

        // Todas las filas en orden de inserción, para recalcular o verificar el DVH de cada una y el DVV de la tabla.
        public List<BEBitacora> ObtenerTodasOrdenadas()
        {
            List<BEBitacora> bitacoras = new List<BEBitacora>();
            string query = "SELECT IdBitacora, IdUsuario, NombreUsuario, Perfil, Accion, Fecha, DVH FROM Bitacora ORDER BY IdBitacora ASC";
            DataTable tabla = _acceso.Leer(query, null);

            foreach (DataRow row in tabla.Rows)
            {
                bitacoras.Add(Mapear(row));
            }

            return bitacoras;
        }

        public void ActualizarDVH(int idBitacora, string dvh)
        {
            string query = "UPDATE Bitacora SET DVH = @DVH WHERE IdBitacora = @IdBitacora";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@DVH", (object)dvh ?? DBNull.Value),
                new SqlParameter("@IdBitacora", idBitacora)
            };
            _acceso.Escribir(query, parameters);
        }

        public List<string> ObtenerTodosDVH()
        {
            List<string> dvhs = new List<string>();
            string query = "SELECT DVH FROM Bitacora WHERE DVH IS NOT NULL ORDER BY IdBitacora ASC";
            DataTable tabla = _acceso.Leer(query, null);
            foreach (DataRow row in tabla.Rows)
            {
                dvhs.Add(row["DVH"].ToString());
            }
            return dvhs;
        }

        private static BEBitacora Mapear(DataRow row)
        {
            return new BEBitacora
            {
                IdBitacora = Convert.ToInt32(row["IdBitacora"]),
                IdUsuario = Convert.ToInt32(row["IdUsuario"]),
                NombreUsuario = row["NombreUsuario"].ToString(),
                Perfil = row["Perfil"].ToString(),
                Accion = row["Accion"].ToString(),
                Fecha = Convert.ToDateTime(row["Fecha"]),
                DVH = row["DVH"] == DBNull.Value ? null : row["DVH"].ToString()
            };
        }
    }
}
