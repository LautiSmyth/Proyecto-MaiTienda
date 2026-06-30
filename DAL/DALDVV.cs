using System;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class DALDVV
    {
        private readonly Acceso _acceso = Acceso.GetInstance();

        public string ObtenerDVV(string tabla)
        {
            string query = "SELECT DVV FROM DVV WHERE Tabla = @Tabla";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Tabla", tabla)
            };
            DataTable dt = _acceso.Leer(query, parameters);
            if (dt.Rows.Count == 0)
                return null;
            return dt.Rows[0]["DVV"].ToString();
        }

        public void GuardarDVV(string tabla, string dvv)
        {
            string query = @"
                MERGE INTO DVV AS target
                USING (SELECT @Tabla AS Tabla) AS source
                ON (target.Tabla = source.Tabla)
                WHEN MATCHED THEN
                    UPDATE SET DVV = @DVV
                WHEN NOT MATCHED THEN
                    INSERT (Tabla, DVV) VALUES (source.Tabla, @DVV);";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Tabla", tabla),
                new SqlParameter("@DVV", dvv)
            };
            _acceso.Escribir(query, parameters);
        }
    }
}
