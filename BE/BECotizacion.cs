using System;
using System.Collections.Generic;

namespace BE
{
    public class BECotizacion
    {
        public DateTime Fecha { get; set; }
        public DateTime VigenteHasta { get; set; }
        public List<BELineaCotizacion> Lineas { get; set; }
        public int CantidadTotalUnidades { get; set; }
        public decimal Total { get; set; }
        public bool PedidoDisponible { get; set; }
        public string Mensaje { get; set; }

        public BECotizacion()
        {
            Lineas = new List<BELineaCotizacion>();
        }
    }
}
