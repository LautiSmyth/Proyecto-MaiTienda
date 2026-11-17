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
        private readonly GestorIntentos _gestorIntentos = new GestorIntentos();

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

        public void ValidarCredenciales(string nombreUsuario, string password, string origen = null)
        {
            try
            {
                BEUsuario usuario = _dalUsuario.BuscarPorNombreUsuario(nombreUsuario);
                if (usuario == null)
                {
                    RegistrarEventoSistemaSeguro($"Intento de inicio de sesion con usuario inexistente '{Recortar(nombreUsuario, 50)}'{DescribirOrigen(origen)}");
                    throw new UnauthorizedAccessException();
                }

                if (usuario.Estado != EstadoUsuario.Activo)
                {
                    string estadoCuenta = usuario.Estado == EstadoUsuario.Bloqueado ? "bloqueada" : "inactiva";
                    RegistrarEventoUsuarioSeguro($"Intento de inicio de sesion con cuenta {estadoCuenta}{DescribirOrigen(origen)}", usuario);
                    throw new UnauthorizedAccessException();
                }

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

        public ResultadoPasskey ValidarConPasskey(string passkey, string origen)
        {
            if (_gestorIntentos.EstaBloqueado(origen))
            {
                RegistrarEventoPasskey($"Intento fallido de passkey desde {origen}: origen bloqueado temporalmente");
                return ResultadoPasskey.Bloqueado;
            }

            string passkeyConfigurado = ConfigurationManager.AppSettings["WebMasterPasskey"];
            bool valido = !string.IsNullOrEmpty(passkeyConfigurado) && Encriptador.CompararSeguro(passkey, passkeyConfigurado);

            if (valido)
            {
                _gestorIntentos.Reiniciar(origen);
                RegistrarEventoPasskey($"Inicio de sesion con passkey desde {origen}");
                return ResultadoPasskey.Valido;
            }

            if (_gestorIntentos.RegistrarFallo(origen))
            {
                RegistrarEventoPasskey($"Passkey: bloqueado por intentos fallidos desde {origen}");
                return ResultadoPasskey.Bloqueado;
            }

            RegistrarEventoPasskey($"Intento fallido de passkey desde {origen}");
            return ResultadoPasskey.Incorrecto;
        }

        // El mensaje al usuario es siempre el mismo ("usuario o contraseña incorrectos"),
        // asi no se revela si la cuenta existe o esta bloqueada; el detalle queda solo en la bitacora.
        private void RegistrarEventoSistemaSeguro(string accion)
        {
            try
            {
                _sBitacora.RegistrarEventoSistema(Recortar(accion, 255));
            }
            catch (Exception)
            {
            }
        }

        private void RegistrarEventoUsuarioSeguro(string accion, BEUsuario usuario)
        {
            try
            {
                _sBitacora.RegistrarEvento(Recortar(accion, 255), usuario);
            }
            catch (Exception)
            {
            }
        }

        private static string DescribirOrigen(string origen)
        {
            return string.IsNullOrEmpty(origen) ? string.Empty : " desde " + origen;
        }

        private static string Recortar(string texto, int largoMaximo)
        {
            texto = texto ?? string.Empty;
            return texto.Length <= largoMaximo ? texto : texto.Substring(0, largoMaximo);
        }

        private void RegistrarEventoPasskey(string accion)
        {
            try
            {
                _sBitacora.RegistrarEventoSistema(accion);
            }
            catch (Exception)
            {
            }
        }

        public bool VerificarConexion()
        {
            return _dalUsuario.VerificarConexion();
        }
    }
}
