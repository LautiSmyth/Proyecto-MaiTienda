using BE;
using DAL;
using SERVICIOS;
using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace BLL
{
    public class BLLBackup
    {
        private const int MaximoArchivosPorDefecto = 7;

        private readonly DALBackup _dalBackup = new DALBackup();
        private readonly ServicioBitacora _sBitacora = new ServicioBitacora();

        private string ObtenerCarpeta()
        {
            return ConfigurationManager.AppSettings["BackupFolder"] ?? @"C:\Backups\MaiTienda";
        }

        private int ObtenerMaximoArchivos()
        {
            int valor;
            return int.TryParse(ConfigurationManager.AppSettings["BackupMaximoArchivos"], out valor) && valor > 0
                ? valor
                : MaximoArchivosPorDefecto;
        }

        public string GenerarBackupManual(BEUsuario usuario)
        {
            string carpeta = ObtenerCarpeta();
            string ruta = _dalBackup.EjecutarBackup(carpeta);
            AplicarRetencion(carpeta);

            if (usuario != null)
            {
                _sBitacora.RegistrarEvento($"Backup manual generado: {ruta}", usuario);
            }
            else
            {
                _sBitacora.RegistrarEventoSistema($"Backup manual generado: {ruta}");
            }
            return ruta;
        }

        public string GenerarBackupAutomatico()
        {
            string carpeta = ObtenerCarpeta();
            string ruta = _dalBackup.EjecutarBackup(carpeta);
            AplicarRetencion(carpeta);

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

        public int ObtenerMaximoArchivosBackup()
        {
            return ObtenerMaximoArchivos();
        }

        private void AplicarRetencion(string carpeta)
        {
            try
            {
                int maximo = ObtenerMaximoArchivos();
                var archivos = Directory.GetFiles(carpeta, "*.bak")
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.LastWriteTime)
                    .Skip(maximo);

                foreach (FileInfo archivo in archivos)
                {
                    try
                    {
                        archivo.Delete();
                    }
                    catch (Exception ex)
                    {
                        _sBitacora.RegistrarEventoSistema($"No se pudo borrar el backup antiguo '{archivo.Name}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _sBitacora.RegistrarEventoSistema($"Error al aplicar la retención de backups: {ex.Message}");
            }
        }
    }
}
