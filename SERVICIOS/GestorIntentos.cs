using System;
using System.Collections.Generic;
using System.Linq;

namespace SERVICIOS
{
    public class GestorIntentos
    {
        private const int MaximoIntentos = 3;
        private const int MinutosBloqueo = 15;

        private static readonly Dictionary<string, RegistroIntentos> _registros = new Dictionary<string, RegistroIntentos>();
        private static readonly object _lock = new object();

        public bool EstaBloqueado(string clave)
        {
            lock (_lock)
            {
                string key = clave ?? string.Empty;
                RegistroIntentos registro;
                if (!_registros.TryGetValue(key, out registro))
                    return false;

                if (!registro.BloqueadoHasta.HasValue)
                    return false;

                if (registro.BloqueadoHasta.Value > DateTime.Now)
                    return true;

                _registros.Remove(key);
                return false;
            }
        }

        public bool RegistrarFallo(string clave)
        {
            lock (_lock)
            {
                DepurarVencidos();

                string key = clave ?? string.Empty;
                RegistroIntentos registro;
                if (!_registros.TryGetValue(key, out registro))
                {
                    registro = new RegistroIntentos();
                    _registros[key] = registro;
                }

                registro.Fallos++;
                registro.UltimoFallo = DateTime.Now;

                if (registro.Fallos >= MaximoIntentos)
                {
                    registro.BloqueadoHasta = DateTime.Now.AddMinutes(MinutosBloqueo);
                    return true;
                }
                return false;
            }
        }

        public void Reiniciar(string clave)
        {
            lock (_lock)
            {
                _registros.Remove(clave ?? string.Empty);
            }
        }

        private void DepurarVencidos()
        {
            DateTime limite = DateTime.Now.AddMinutes(-MinutosBloqueo);
            List<string> vencidos = _registros
                .Where(r => !r.Value.BloqueadoHasta.HasValue && r.Value.UltimoFallo < limite)
                .Select(r => r.Key)
                .ToList();

            foreach (string key in vencidos)
                _registros.Remove(key);
        }

        private class RegistroIntentos
        {
            public int Fallos { get; set; }
            public DateTime UltimoFallo { get; set; }
            public DateTime? BloqueadoHasta { get; set; }
        }
    }
}
