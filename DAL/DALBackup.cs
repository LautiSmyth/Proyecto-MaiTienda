using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace DAL
{
    public class DALBackup
    {
        private readonly string _cadenaConexion;

        public DALBackup()
        {
            ConnectionStringSettings entrada = ConfigurationManager.ConnectionStrings["DefaultConnection"];
            _cadenaConexion = entrada?.ConnectionString;
        }

        public string EjecutarBackup(string carpetaDestino)
        {
            if (string.IsNullOrEmpty(_cadenaConexion))
                throw new InvalidOperationException("No se encontró la cadena de conexión.");

            Directory.CreateDirectory(carpetaDestino);

            string nombreArchivo = $"MaiTiendaDB_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            string rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

            string query = $@"BACKUP DATABASE [MaiTiendaDB] TO DISK = N'{rutaCompleta}'
                              WITH NOFORMAT, NOINIT,
                              NAME = N'MaiTiendaDB-Full Backup',
                              SKIP, NOREWIND, NOUNLOAD, STATS = 10";

            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.CommandTimeout = 600;
                conexion.Open();
                cmd.ExecuteNonQuery();
            }

            return rutaCompleta;
        }

        public void EjecutarRestore(string rutaArchivo)
        {
            if (string.IsNullOrEmpty(_cadenaConexion))
                throw new InvalidOperationException("No se encontró la cadena de conexión.");

            var builder = new SqlConnectionStringBuilder(_cadenaConexion);
            string dbName = builder.InitialCatalog;
            builder.InitialCatalog = "master";
            string masterConnection = builder.ConnectionString;

            SqlConnection.ClearAllPools();

            using (SqlConnection conexion = new SqlConnection(masterConnection))
            {
                conexion.Open();

                using (SqlCommand cmd = new SqlCommand(
                    $"ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", conexion))
                    cmd.ExecuteNonQuery();

                using (SqlCommand cmd = new SqlCommand(
                    $@"RESTORE DATABASE [{dbName}] FROM DISK = N'{rutaArchivo}'
                       WITH FILE = 1, NOUNLOAD, REPLACE, RECOVERY, STATS = 10", conexion))
                {
                    cmd.CommandTimeout = 600;
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(
                    $"ALTER DATABASE [{dbName}] SET MULTI_USER", conexion))
                    cmd.ExecuteNonQuery();
            }
        }
    }
}
