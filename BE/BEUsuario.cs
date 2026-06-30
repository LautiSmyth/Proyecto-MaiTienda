namespace BE
{
    public class BEUsuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public Enums.EstadoUsuario Estado { get; set; }
        public Enums.Perfil Perfil { get; set; }
        public int IntentosFallidos { get; set; }
        public string DVH { get; set; }
    }
}