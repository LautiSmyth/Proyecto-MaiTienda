using BE;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SERVICIOS
{
    public class GestorIntegridad
    {
        public string CalcularDVH(BEUsuario usuario)
        {
            if (usuario == null) return string.Empty;
            string data = $"{usuario.IdUsuario}{usuario.NombreUsuario}{usuario.Password}{(int)usuario.Estado}{(int)usuario.Perfil}{usuario.IntentosFallidos}";
            return Encriptador.HashIntegridad(data);
        }

        public string CalcularDVH(BEProducto producto)
        {
            if (producto == null) return string.Empty;
            string precioStr = producto.Precio.ToString("F2", CultureInfo.InvariantCulture);
            string data = $"{producto.IdProducto}{producto.Nombre}{producto.Categoria}{precioStr}{producto.ImagenUrl}{producto.Descripcion}{producto.Stock}";
            return Encriptador.HashIntegridad(data);
        }

        public string CalcularDVV(List<string> listaHashesDVH)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string hash in listaHashesDVH)
            {
                sb.Append(hash);
            }
            return Encriptador.HashIntegridad(sb.ToString());
        }
    }
}
