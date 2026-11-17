namespace BE
{
    public class BELineaPedido
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal
        {
            get { return PrecioUnitario * Cantidad; }
        }
    }
}
