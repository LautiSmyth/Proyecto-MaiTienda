using System;

namespace BE
{
    public class BEFactura
    {
        public int IdFactura { get; set; }
        public int Numero { get; set; }
        public int IdPedido { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Neto { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
    }
}
