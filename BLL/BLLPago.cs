using BE;
using BE.Enums;
using System;
using System.Text.RegularExpressions;

namespace BLL
{
    public class BLLPago
    {
        private const int LargoMaximoFirma = 400000;
        private const int BytesMinimosFirma = 100;
        private const string PrefijoDataUrl = "data:image/png;base64,";
        private static readonly byte[] CabeceraPng = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        public bool RequiereFirma(MedioPago medio)
        {
            return medio != MedioPago.Efectivo;
        }

        // Valida la firma del recuadro (imagen PNG en Base64). Con efectivo no se pide firma.
        public string ValidarFirma(MedioPago medio, string firma)
        {
            if (!RequiereFirma(medio))
                return null;

            string base64 = (firma ?? string.Empty).Trim();
            if (base64.StartsWith(PrefijoDataUrl, StringComparison.OrdinalIgnoreCase))
                base64 = base64.Substring(PrefijoDataUrl.Length);

            if (base64.Length == 0)
                throw new ArgumentException("Firmá en el recuadro para autorizar el pago.");
            if (base64.Length > LargoMaximoFirma || !Regex.IsMatch(base64, "^[A-Za-z0-9+/]+={0,2}$"))
                throw new ArgumentException("La firma no es válida. Volvé a firmar en el recuadro.");

            byte[] imagen;
            try
            {
                imagen = Convert.FromBase64String(base64);
            }
            catch (FormatException)
            {
                throw new ArgumentException("La firma no es válida. Volvé a firmar en el recuadro.");
            }

            if (imagen.Length < BytesMinimosFirma || !EsPng(imagen))
                throw new ArgumentException("La firma no es válida. Volvé a firmar en el recuadro.");

            return base64;
        }

        // Pasarela simulada: con un medio electrónico firmado el pago se aprueba al instante.
        public BEPago Preparar(BEPedido pedido, MedioPago medio, string firmaValidada)
        {
            BEPago pago = new BEPago
            {
                Medio = medio,
                Fecha = pedido.Fecha
            };

            if (!RequiereFirma(medio))
            {
                pago.Estado = EstadoPago.Pendiente;
                return pago;
            }

            pago.Firma = firmaValidada;
            pago.Estado = EstadoPago.Aprobado;
            pago.Referencia = "SIM-" + Guid.NewGuid().ToString("N").Substring(0, 10).ToUpperInvariant();
            return pago;
        }

        public string Describir(MedioPago medio)
        {
            switch (medio)
            {
                case MedioPago.Efectivo: return "Efectivo (se abona al recibir)";
                case MedioPago.TarjetaDebito: return "Tarjeta de débito";
                case MedioPago.TarjetaCredito: return "Tarjeta de crédito";
                case MedioPago.MercadoPago: return "Mercado Pago (simulado)";
                default: return medio.ToString();
            }
        }

        public string Describir(EstadoPago estado)
        {
            return estado == EstadoPago.Aprobado ? "Aprobado" : "Pendiente de cobro";
        }

        public bool EsBase64Png(string base64)
        {
            try
            {
                return !string.IsNullOrEmpty(base64) && Regex.IsMatch(base64, "^[A-Za-z0-9+/]+={0,2}$") && EsPng(Convert.FromBase64String(base64));
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static bool EsPng(byte[] imagen)
        {
            if (imagen.Length < CabeceraPng.Length)
                return false;
            for (int i = 0; i < CabeceraPng.Length; i++)
            {
                if (imagen[i] != CabeceraPng[i])
                    return false;
            }
            return true;
        }
    }
}
