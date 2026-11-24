using BE;
using System;
using System.Collections.Generic;

namespace SERVICIOS
{
    public class ServicioBitacora
    {
        private readonly DAL.DALBitacora _dalBitacora = new DAL.DALBitacora();
        private readonly DAL.DALDVV _dalDVV = new DAL.DALDVV();
        private readonly GestorIntegridad _gestorIntegridad = new GestorIntegridad();

        public void RegistrarEvento(string accion, BEUsuario usuario)
        {
            Registrar(new BEBitacora
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
            Registrar(new BEBitacora
            {
                IdUsuario = 0,
                NombreUsuario = "Sistema",
                Perfil = "Sistema",
                Accion = accion,
                Fecha = DateTime.Now
            });
        }

        private void Registrar(BEBitacora bitacora)
        {
            bitacora.Fecha = GestorIntegridad.FechaAlmacenable(bitacora.Fecha);
            _dalBitacora.RegistrarEvento(bitacora);
            bitacora.DVH = _gestorIntegridad.CalcularDVH(bitacora);
            _dalBitacora.ActualizarDVH(bitacora.IdBitacora, bitacora.DVH);
            RecalcularDVV();
        }

        private void RecalcularDVV()
        {
            try
            {
                List<string> dvhs = _dalBitacora.ObtenerTodosDVH();
                string dvv = _gestorIntegridad.CalcularDVV(dvhs);
                _dalDVV.GuardarDVV("Bitacora", dvv);
            }
            catch (Exception)
            {
            }
        }

        public List<BEBitacora> ListarBitacora()
        {
            return _dalBitacora.LeerBitacora();
        }
    }
}
