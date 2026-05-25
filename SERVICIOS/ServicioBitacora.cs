using BE;
using SERVICIOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERVICIOS
{
    public class ServicioBitacora
    {
        private readonly DAL.DALBitacora _dalBitacora = new DAL.DALBitacora();

        public void RegistrarEvento(string accion)
        {
            BEUsuario _usuario = SessionManager.GetInstance().Usuario;
            BEBitacora _bitacora = new BEBitacora
            {
                IdUsuario = _usuario.IdUsuario,
                NombreUsuario = _usuario.NombreUsuario,
                Perfil = _usuario.Perfil.ToString(),
                Accion = accion,
                Fecha = DateTime.Now
            };
            _dalBitacora.RegistrarEvento(_bitacora);
        }
        public void RegistrarEvento(string accion, BEUsuario _usuario)
        {
            BEBitacora _bitacora = new BEBitacora
            {
                IdUsuario = _usuario.IdUsuario,
                NombreUsuario = _usuario.NombreUsuario,
                Perfil = _usuario.Perfil.ToString(),
                Accion = accion,
                Fecha = DateTime.Now
            };
            _dalBitacora.RegistrarEvento(_bitacora);
        }

        public List<BEBitacora> ListarBitacora()
        {
            return _dalBitacora.LeerBitacora();
        }
    }
}
