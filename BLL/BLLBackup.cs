using DAL;
using SERVICIOS;
using System.Configuration;

namespace BLL
{
    public class BLLBackup
    {
        private readonly DALBackup _dalBackup = new DALBackup();
        private readonly ServicioBitacora _sBitacora = new ServicioBitacora();

        private string ObtenerCarpeta()
        {
            return ConfigurationManager.AppSettings["BackupFolder"] ?? @"C:\Backups\MaiTienda";
        }

        public string GenerarBackupManual()
        {
            string ruta = _dalBackup.EjecutarBackup(ObtenerCarpeta());
            _sBitacora.RegistrarEvento($"Backup manual generado: {ruta}");
            return ruta;
        }

        public string GenerarBackupAutomatico()
        {
            string ruta = _dalBackup.EjecutarBackup(ObtenerCarpeta());
            _sBitacora.RegistrarEventoSistema($"Backup automatico generado: {ruta}");
            return ruta;
        }

        public void EjecutarRestore(string rutaArchivo)
        {
            _dalBackup.EjecutarRestore(rutaArchivo);
            _sBitacora.RegistrarEventoSistema($"Restore ejecutado desde: {rutaArchivo}");
        }

        public string ObtenerCarpetaBackup()
        {
            return ObtenerCarpeta();
        }
    }
}
