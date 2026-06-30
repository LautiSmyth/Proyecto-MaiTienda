using BE;
using BE.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class DALUsuario
    {
        private readonly Acceso _acceso = Acceso.GetInstance();

        public BEUsuario BuscarPorNombreUsuario(string nombreUsuario)
        {
            string query = "SELECT * FROM Usuario WHERE NombreUsuario = @NombreUsuario";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@NombreUsuario", nombreUsuario)
            };

            DataTable tabla = _acceso.Leer(query, parameters);

            if (tabla.Rows.Count == 0)
                return null;

            DataRow fila = tabla.Rows[0];
            return new BEUsuario
            {
                IdUsuario = Convert.ToInt32(fila["IdUsuario"]),
                NombreUsuario = fila["NombreUsuario"].ToString(),
                Password = fila["Password"].ToString(),
                Estado = (EstadoUsuario)Convert.ToInt32(fila["Estado"]),
                Perfil = (Perfil)Convert.ToInt32(fila["Perfil"]),
                IntentosFallidos = Convert.ToInt32(fila["IntentosFallidos"]),
                DVH = fila["DVH"] == DBNull.Value ? null : fila["DVH"].ToString()
            };
        }

        public List<BEUsuario> ObtenerTodos()
        {
            List<BEUsuario> lista = new List<BEUsuario>();
            string query = "SELECT * FROM Usuario";
            DataTable tabla = _acceso.Leer(query, null);

            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(new BEUsuario
                {
                    IdUsuario = Convert.ToInt32(fila["IdUsuario"]),
                    NombreUsuario = fila["NombreUsuario"].ToString(),
                    Password = fila["Password"].ToString(),
                    Estado = (EstadoUsuario)Convert.ToInt32(fila["Estado"]),
                    Perfil = (Perfil)Convert.ToInt32(fila["Perfil"]),
                    IntentosFallidos = Convert.ToInt32(fila["IntentosFallidos"]),
                    DVH = fila["DVH"] == DBNull.Value ? null : fila["DVH"].ToString()
                });
            }
            return lista;
        }

        public void Insertar(BEUsuario usuario)
        {
            string queryInsert = "INSERT INTO Usuario (NombreUsuario, Password, Estado, Perfil, IntentosFallidos, DVH) OUTPUT INSERTED.IdUsuario VALUES (@NombreUsuario, @Password, @Estado, @Perfil, @IntentosFallidos, @DVH)";
            SqlParameter[] parametersInsert = new SqlParameter[]
            {
                new SqlParameter("@NombreUsuario", usuario.NombreUsuario),
                new SqlParameter("@Password", usuario.Password),
                new SqlParameter("@Estado", usuario.Estado),
                new SqlParameter("@Perfil", (int)usuario.Perfil),
                new SqlParameter("@IntentosFallidos", usuario.IntentosFallidos),
                new SqlParameter("@DVH", (object)usuario.DVH ?? DBNull.Value)
            };

            DataTable dt = _acceso.Leer(queryInsert, parametersInsert);
            if (dt.Rows.Count > 0)
            {
                usuario.IdUsuario = Convert.ToInt32(dt.Rows[0][0]);
            }
        }

        public void Actualizar(BEUsuario usuario)
        {
            string query = "UPDATE Usuario SET Password = @Password, Estado = @Estado, Perfil = @Perfil, IntentosFallidos = @IntentosFallidos, DVH = @DVH WHERE IdUsuario = @IdUsuario";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Password", usuario.Password),
                new SqlParameter("@Estado", usuario.Estado),
                new SqlParameter("@Perfil", (int)usuario.Perfil),
                new SqlParameter("@IntentosFallidos", usuario.IntentosFallidos),
                new SqlParameter("@DVH", (object)usuario.DVH ?? DBNull.Value),
                new SqlParameter("@IdUsuario", usuario.IdUsuario)
            };
            _acceso.Escribir(query, parameters);
        }

        public bool VerificarConexion()
        {
            return _acceso.VerificarConexion();
        }
    }
}