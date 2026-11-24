using System;

namespace BLL
{
    public class IntegridadException : InvalidOperationException
    {
        public string Tabla { get; private set; }

        public IntegridadException(string tabla, string mensaje) : base(mensaje)
        {
            Tabla = tabla;
        }
    }
}
