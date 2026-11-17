namespace BE
{
    public class BELineaCotizacion
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int CantidadSolicitada { get; set; }
        public int StockDisponible { get; set; }
        public decimal Subtotal { get; set; }
        public bool Disponible { get; set; }
        public string Observacion { get; set; }
    }
}
