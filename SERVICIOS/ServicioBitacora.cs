using BE;
using System;
using System.Collections.Generic;

namespace SERVICIOS
{
    public class ServicioBitacora
    {
        private readonly DAL.DALBitacora _dalBitacora = new DAL.DALBitacora();

        public void RegistrarEvento(string accion)
        {
            BEUsuario usuario = SessionManager.GetInstance().Usuario;
            if (usuario == null)
            {
                RegistrarEventoSistema(accion);
                return;
            }
            _dalBitacora.RegistrarEvento(new BEBitacora
            {
                IdUsuario = usuario.IdUsuario,
                NombreUsuario = usuario.NombreUsuario,
                Perfil = usuario.Perfil.ToString(),
                Accion = accion,
                Fecha = DateTime.Now
            });
        }

        public void RegistrarEvento(string accion, BEUsuario usuario)
        {
            _dalBitacora.RegistrarEvento(new BEBitacora
            {
                IdUsuario = usuario.IdUsuario,
                NombreUsuario = usuario.NombreUsuario,
                Perfil = usuario.Perfil.ToString(),
                Accion = accion,
                Fecha = DateTime.Now
            });
        }

        public void RegistrarEventoSistema(string accion)
        {
            _dalBitacora.RegistrarEvento(new BEBitacora
            {
                IdUsuario = 0,
                NombreUsuario = "Sistema",
                Perfil = "Sistema",
                Accion = accion,
                Fecha = DateTime.Now
            });
        }

        public List<BEBitacora> ListarBitacora()
        {
            return _dalBitacora.LeerBitacora();
        }
    }
}
