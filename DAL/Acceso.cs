using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public sealed class Acceso
    {
        private static volatile Acceso _instance;
        private static readonly object _lock = new object();

        private readonly string _cadenaConexion;

        private Acceso()
        {
            try
            {
                ConnectionStringSettings entrada = ConfigurationManager.ConnectionStrings["DefaultConnection"];
                if (entrada != null)
                    _cadenaConexion = entrada.ConnectionString;
            }
            catch (Exception)
            {
                _cadenaConexion = null;
            }
        }

        public static Acceso GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new Acceso();
                }
            }
            return _instance;
        }

        public string ObtenerNombreBaseDatos()
        {
            if (string.IsNullOrEmpty(_cadenaConexion))
                return "desconocida";

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_cadenaConexion);
            return builder.InitialCatalog;
        }

        public bool VerificarConexion()
        {
            if (string.IsNullOrEmpty(_cadenaConexion))
                return false;

            try
            {
                using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
                {
                    conexion.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Acceso] Error de conexion: {ex.Message}");
                return false;
            }
        }

        public DataTable Leer(string consulta, SqlParameter[] parametros)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
                using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                {
                    if (parametros != null)
                        cmd.Parameters.AddRange(parametros);

                    conexion.Open();
                    DataTable tabla = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        adapter.Fill(tabla);
                    return tabla;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al leer de la base de datos: {ex.Message}", ex);
            }
        }

        public int Escribir(string consulta, SqlParameter[] parametros)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
                using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                {
                    if (parametros != null)
                        cmd.Parameters.AddRange(parametros);

                    conexion.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al escribir en la base de datos: {ex.Message}", ex);
            }
        }
    }
}