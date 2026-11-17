using System;
using System.Collections.Generic;

namespace BE
{
    public class BEPedido
    {
        public int IdPedido { get; set; }
        public int IdUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public string NombreCliente { get; set; }
        public string DocumentoCliente { get; set; }
        public string DireccionEnvio { get; set; }
        public decimal DistanciaKm { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Envio { get; set; }
        public decimal Total { get; set; }
        public List<BELineaPedido> Lineas { get; set; }
        public BEFactura Factura { get; set; }

        public BEPedido()
        {
            Lineas = new List<BELineaPedido>();
        }
    }
}
