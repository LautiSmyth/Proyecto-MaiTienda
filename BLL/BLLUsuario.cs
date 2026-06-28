using BE;
using BE.Enums;
using SERVICIOS;
using System;

namespace BLL
{
    public class BLLUsuario
    {
        private readonly DAL.DALUsuario _dalUsuario = new DAL.DALUsuario();
        private readonly ServicioBitacora _sBitacora = new ServicioBitacora();

        public BEUsuario BuscarPorNombreUsuario(string nombreUsuario)
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
                Perfil = perfil
            };
            _dalUsuario.Insertar(usuario);
        }

        public void ValidarCredenciales(string nombreUsuario, string password)
        {
            try
            {
                BEUsuario usuario = _dalUsuario.BuscarPorNombreUsuario(nombreUsuario);
                if (usuario == null || usuario.Estado != EstadoUsuario.Activo)
                    throw new UnauthorizedAccessException();

                bool esValido = Encriptador.Verificar(password, usuario.Password);
                if (!esValido)
                {
                    if (usuario.IntentosFallidos >= 2)
                    {
                        usuario.Estado = EstadoUsuario.Bloqueado;
                        _dalUsuario.Actualizar(usuario);
                        _sBitacora.RegistrarEvento("bloqueado por intentos fallidos", usuario);
                    }
                    else
                    {
                        usuario.IntentosFallidos++;
                        _dalUsuario.Actualizar(usuario);
                        _sBitacora.RegistrarEvento($"Intento fallido {usuario.IntentosFallidos}", usuario);
                    }
                    throw new UnauthorizedAccessException();
                }

                SessionManager.GetInstance().Login(usuario);
                usuario.IntentosFallidos = 0;
                _dalUsuario.Actualizar(usuario);
                _sBitacora.RegistrarEvento("Inicio de sesion");
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("usuario o contraseña incorrectos");
            }
        }

        public void CerrarSesion()
        {
            _sBitacora.RegistrarEvento("Cierre de sesion");
            SessionManager.GetInstance().Logout();
        }

        public bool VerificarConexion()
        {
            return _dalUsuario.VerificarConexion();
        }
    }
}
