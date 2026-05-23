using BE;
using BE.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SERVICIOS;
using System.CodeDom;

namespace BLL
{
    public class BLLUsuario
    {
        private readonly DAL.DALUsuario _dalUsuario = new DAL.DALUsuario();

        public BE.BEUsuario BuscarPorNombreUsuario(string nombreUsuario)
        {
            return _dalUsuario.BuscarPorNombreUsuario(nombreUsuario);
        }

        public void Insertar(string nombreUsuario, string password, Perfil perfil)
        {
            BEUsuario usuario = new BEUsuario
            {
                NombreUsuario = nombreUsuario,
                Password = Encriptador.Hash(password),
                Estado = EstadoUsuario.Activo,
                Perfil = (Perfil)perfil
            };
            _dalUsuario.Insertar(usuario);
        }

        public void ValidarCredenciales(string nombreUsuario, string password)
        {
            try
            {
                BEUsuario usuario = _dalUsuario.BuscarPorNombreUsuario(nombreUsuario);
                if (usuario == null || usuario.Estado != EstadoUsuario.Activo) throw new UnauthorizedAccessException();

                bool esValido = Encriptador.Verificar(password, usuario.Password);
                if (!esValido) throw new UnauthorizedAccessException();    
                
                SessionManager _session = SessionManager.GetInstance();
                _session.Login(usuario);
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("usuario o contraseña incorrectos");
            }
        }

    }
}
