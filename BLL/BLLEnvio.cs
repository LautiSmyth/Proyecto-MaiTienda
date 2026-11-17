using System;

namespace BLL
{
    public class BLLEnvio
    {
        private const double LatitudNegocio = -34.7621;
        private const double LongitudNegocio = -58.3940;
        private const double RadioTierraKm = 6371;
        private const double KmEnvioGratis = 10;
        private const double CostoBase = 5000;
        private const double MultiploCosto = 500;

        public double CalcularDistanciaKm(double latitud, double longitud)
        {
            if (double.IsNaN(latitud) || double.IsNaN(longitud) ||
                latitud < -90 || latitud > 90 || longitud < -180 || longitud > 180)
            {
                throw new ArgumentException("La ubicación indicada no es válida.");
            }

            double dLat = ARadianes(latitud - LatitudNegocio);
            double dLon = ARadianes(longitud - LongitudNegocio);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ARadianes(LatitudNegocio)) * Math.Cos(ARadianes(latitud)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            a = Math.Min(1, a);
            return RadioTierraKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        public decimal CalcularCosto(double distanciaKm)
        {
            if (distanciaKm <= KmEnvioGratis)
                return 0m;

            double costo = CostoBase * Math.Pow(distanciaKm / KmEnvioGratis, 0.6);
            return (decimal)(Math.Round(costo / MultiploCosto, MidpointRounding.AwayFromZero) * MultiploCosto);
        }

        private static double ARadianes(double grados)
        {
            return grados * Math.PI / 180;
        }
    }
}
