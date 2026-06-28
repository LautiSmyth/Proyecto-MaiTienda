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

        /// <summary>
        /// Backup manual disparado por el WebMaster desde la UI.
        /// Registra el evento en bitácora con el usuario logueado.
        /// </summary>
        public string GenerarBackupManual()
        {
            string ruta = _dalBackup.EjecutarBackup(ObtenerCarpeta());
            _sBitacora.RegistrarEvento($"Backup manual generado: {ruta}");
            return ruta;
        }

        /// <summary>
        /// Backup automático disparado por el timer. No requiere sesión de usuario.
        /// Registra el evento como "Sistema" en bitácora.
        /// </summary>
        public string GenerarBackupAutomatico()
        {
            string ruta = _dalBackup.EjecutarBackup(ObtenerCarpeta());
            _sBitacora.RegistrarEventoSistema($"Backup automático generado: {ruta}");
            return ruta;
        }
    }
}
