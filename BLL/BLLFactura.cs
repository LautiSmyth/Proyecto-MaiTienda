using BE;
using System;

namespace BLL
{
    public class BLLFactura
    {
        public const string Tipo = "B";
        public const string CodigoComprobante = "006";
        public const decimal AlicuotaIva = 0.21m;
        public const string EmisorRazonSocial = "MaiTienda";
        public const string EmisorCuit = "30-00000000-0";
        public const string EmisorDomicilio = "Oficinas centrales UAI Lomas, Lomas de Zamora, Buenos Aires";
        public const string EmisorCondicionIva = "IVA Responsable Inscripto";
        public const string ClienteCondicionIva = "Consumidor Final";

        private const int PuntoDeVenta = 1;

        // Los precios del catálogo ya incluyen IVA: se discrimina el 21% contenido en el total del pedido.
        public BEFactura Emitir(BEPedido pedido)
        {
            decimal neto = Math.Round(pedido.Total / (1 + AlicuotaIva), 2, MidpointRounding.AwayFromZero);
            return new BEFactura
            {
                Fecha = pedido.Fecha,
                Neto = neto,
                IVA = pedido.Total - neto,
                Total = pedido.Total
            };
        }

        public string FormatearNumero(BEFactura factura)
        {
            return PuntoDeVenta.ToString("D4") + "-" + factura.Numero.ToString("D8");
        }
    }
}
