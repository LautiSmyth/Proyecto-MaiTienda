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
        ServicioBitacora _sBitacora = new ServicioBitacora();

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
                if (!esValido)
                {
                    if(usuario.IntentosFallidos >= 3)
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
                
                SessionManager _session = SessionManager.GetInstance();
                _session.Login(usuario);
                usuario.IntentosFallidos = 0;
                _dalUsuario.Actualizar(usuario);
                _sBitacora.RegistrarEvento("Inicio de sesion");

            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("usuario o contraseña incorrectos");
            }
        }

        public void ActualizarEstado(BEUsuario usuario, EstadoUsuario nuevoEstado)
        {
            usuario.Estado = nuevoEstado;
            _dalUsuario.Insertar(usuario);
        }

        public void CerrarSesion()
        {
            _sBitacora.RegistrarEvento("Cierre de sesión");
            SessionManager.GetInstance().Logout();
        }
    }
}
