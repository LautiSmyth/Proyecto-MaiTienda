using BE;
using BE.Enums;
using SERVICIOS;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace BLL
{
    public class BLLUsuario
    {
        private readonly DAL.DALUsuario _dalUsuario = new DAL.DALUsuario();
        private readonly ServicioBitacora _sBitacora = new ServicioBitacora();

        private readonly GestorIntegridad _gestorIntegridad = new GestorIntegridad();

        public BEUsuario BuscarPorNombreUsuario(string nombreUsuario)
        {
            BEUsuario usuario = _dalUsuario.BuscarPorNombreUsuario(nombreUsuario);
            if (usuario != null)
            {
                string dvCalculado = _gestorIntegridad.CalcularDVH(usuario);
                if (usuario.DVH != dvCalculado)
                {
                    throw new InvalidOperationException("Error de integridad de datos: El registro del usuario ha sido alterado externamente.");
                }
            }
            return usuario;
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
            Actualizar(usuario);
        }

        public void Actualizar(BEUsuario usuario)
        {
            usuario.DVH = _gestorIntegridad.CalcularDVH(usuario);
            _dalUsuario.Actualizar(usuario);
            RecalcularDVV();
        }

        private void RecalcularDVV()
        {
            var usuarios = _dalUsuario.ObtenerTodos();
            var dvhs = new List<string>();
            foreach (var u in usuarios)
            {
                dvhs.Add(u.DVH ?? _gestorIntegridad.CalcularDVH(u));
            }
            string dvv = _gestorIntegridad.CalcularDVV(dvhs);
            new DAL.DALDVV().GuardarDVV("Usuario", dvv);
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
                        Actualizar(usuario);
                        _sBitacora.RegistrarEvento("bloqueado por intentos fallidos", usuario);
                    }
                    else
                    {
                        usuario.IntentosFallidos++;
                        Actualizar(usuario);
                        _sBitacora.RegistrarEvento($"Intento fallido {usuario.IntentosFallidos}", usuario);
                    }
                    throw new UnauthorizedAccessException();
                }

                SessionManager.GetInstance().Login(usuario);
                usuario.IntentosFallidos = 0;
                Actualizar(usuario);
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

        public bool ValidarConPasskey(string passkey)
        {
            string passkeyConfigurado = ConfigurationManager.AppSettings["WebMasterPasskey"];
            return !string.IsNullOrEmpty(passkeyConfigurado) && passkey == passkeyConfigurado;
        }

        public bool VerificarConexion()
        {
            return _dalUsuario.VerificarConexion();
        }
    }
}
