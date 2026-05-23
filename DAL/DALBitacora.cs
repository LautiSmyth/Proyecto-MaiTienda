using BE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DALBitacora
    {
        private readonly Acceso _acceso = Acceso.GetInstance();

        public void RegistrarEvento(BEBitacora bitacora)
        {
            string query = "INSERT INTO Bitacora (IdUsuario, NombreUsuario, Perfil, Accion, Fecha) VALUES (@IdUsuario, @NombreUsuario, @Perfil, @Accion, @Fecha)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@IdUsuario", bitacora.IdUsuario),
                new SqlParameter("@NombreUsuario", bitacora.NombreUsuario),
                new SqlParameter("@Perfil", bitacora.Perfil),
                new SqlParameter("@Accion", bitacora.Accion),
                new SqlParameter("@Fecha", bitacora.Fecha)
            };
            _acceso.Escribir(query, parameters);
        }
    }
}
