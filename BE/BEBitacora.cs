using System;

namespace BE
{
    public class BEBitacora
    {
        public int IdBitacora { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Perfil { get; set; }
        public string Accion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
