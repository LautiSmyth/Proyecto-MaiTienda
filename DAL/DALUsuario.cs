using BE;
using BE.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if (tabla.Rows.Count > 0)
            {
                DataRow fila = tabla.Rows[0];
                return new BEUsuario
                {
                    IdUsuario = Convert.ToInt32(fila["IdUsuario"]),
                    NombreUsuario = fila["NombreUsuario"].ToString(),
                    Password = fila["Password"].ToString(),
                    Estado = (EstadoUsuario)Convert.ToInt32(fila["Estado"]),
                    Perfil = (Perfil)Convert.ToInt32(fila["Perfil"])
                };
            }
            return null;
        }

        public void Insertar(BEUsuario usuario)
        {
            string query = "INSERT INTO Usuario (NombreUsuario, Password, Estado, Perfil) VALUES (@NombreUsuario, @Password, @Estado, @Perfil)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@NombreUsuario", usuario.NombreUsuario),
                new SqlParameter("@Password", usuario.Password),
                new SqlParameter("@Estado", usuario.Estado),
                new SqlParameter("@Perfil", (int)usuario.Perfil)
            };
            _acceso.Escribir(query, parameters);
        }
    }
}
