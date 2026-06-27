namespace BE
{
    public class BEProducto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public decimal Precio { get; set; }
        public string ImagenUrl { get; set; }
        public string Descripcion { get; set; }
        public int Stock { get; set; }
    }
}
