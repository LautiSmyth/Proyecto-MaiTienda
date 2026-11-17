using BE.Enums;
using System;

namespace BE
{
    public class BEPago
    {
        public int IdPago { get; set; }
        public int IdPedido { get; set; }
        public MedioPago Medio { get; set; }
        public EstadoPago Estado { get; set; }
        public string Referencia { get; set; }
        public DateTime Fecha { get; set; }
        public string Firma { get; set; }

        public bool EstaFirmado
        {
            get { return !string.IsNullOrEmpty(Firma); }
        }
    }
}
