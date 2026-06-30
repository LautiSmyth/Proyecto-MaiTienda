using BE;
using DAL;
using SERVICIOS;
using System;
using System.Collections.Generic;
using System.Data;

namespace BLL
{
    public class BLLIntegridad
    {
        private readonly DALUsuario _dalUsuario = new DALUsuario();
        private readonly DALProducto _dalProducto = new DALProducto();
        private readonly DALDVV _dalDVV = new DALDVV();
        private readonly GestorIntegridad _gestorIntegridad = new GestorIntegridad();

        public List<BEUsuario> VerificarIntegridadUsuarios(out bool dvvValido)
        {
            dvvValido = true;
            List<BEUsuario> usuariosCorruptos = new List<BEUsuario>();
            List<BEUsuario> todosLosUsuarios = _dalUsuario.ObtenerTodos();

            List<string> dvhs = new List<string>();
            foreach (var usuario in todosLosUsuarios)
            {
                string dvCalculado = _gestorIntegridad.CalcularDVH(usuario);
                dvhs.Add(dvCalculado);
                if (usuario.DVH != dvCalculado)
                {
                    usuariosCorruptos.Add(usuario);
                }
            }

            string dvvCalculado = _gestorIntegridad.CalcularDVV(dvhs);
            string dvvRegistrado = _dalDVV.ObtenerDVV("Usuario");
            if (dvvCalculado != dvvRegistrado)
            {
                dvvValido = false;
            }

            return usuariosCorruptos;
        }

        public List<BEProducto> VerificarIntegridadProductos(out bool dvvValido)
        {
            dvvValido = true;
            List<BEProducto> productosCorruptos = new List<BEProducto>();
            List<BEProducto> todosLosProductos = _dalProducto.ObtenerProductos();

            List<string> dvhs = new List<string>();
            foreach (var producto in todosLosProductos)
            {
                string dvCalculado = _gestorIntegridad.CalcularDVH(producto);
                dvhs.Add(dvCalculado);
                if (producto.DVH != dvCalculado)
                {
                    productosCorruptos.Add(producto);
                }
            }

            string dvvCalculado = _gestorIntegridad.CalcularDVV(dvhs);
            string dvvRegistrado = _dalDVV.ObtenerDVV("Producto");
            if (dvvCalculado != dvvRegistrado)
            {
                dvvValido = false;
            }

            return productosCorruptos;
        }

        private void VerificarYAsegurarEsquema()
        {
            Acceso acceso = Acceso.GetInstance();
            bool recienCreado = false;

            // Check and add DVH column in Usuario
            string checkUsuario = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Usuario]') AND name = 'DVH'";
            DataTable dtUsr = acceso.Leer(checkUsuario, null);
            bool tieneDVHUsuario = dtUsr.Rows.Count > 0 && Convert.ToInt32(dtUsr.Rows[0][0]) > 0;

            if (!tieneDVHUsuario)
            {
                // If there's an old DV column from previous attempts, let's drop it or keep it.
                // Just to be safe, drop the old 'DV' column if it exists.
                string checkDVOld = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Usuario]') AND name = 'DV'";
                DataTable dtOld = acceso.Leer(checkDVOld, null);
                if (dtOld.Rows.Count > 0 && Convert.ToInt32(dtOld.Rows[0][0]) > 0)
                {
                    acceso.Escribir("ALTER TABLE [dbo].[Usuario] DROP COLUMN [DV]", null);
                }
                
                acceso.Escribir("ALTER TABLE [dbo].[Usuario] ADD [DVH] NVARCHAR(255) NULL", null);
                recienCreado = true;
            }

            // Check and add DVH column in Producto
            string checkProducto = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Producto]') AND name = 'DVH'";
            DataTable dtProd = acceso.Leer(checkProducto, null);
            bool tieneDVHProducto = dtProd.Rows.Count > 0 && Convert.ToInt32(dtProd.Rows[0][0]) > 0;

            if (!tieneDVHProducto)
            {
                // Drop 'DV' if exists
                string checkDVOld = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Producto]') AND name = 'DV'";
                DataTable dtOld = acceso.Leer(checkDVOld, null);
                if (dtOld.Rows.Count > 0 && Convert.ToInt32(dtOld.Rows[0][0]) > 0)
                {
                    acceso.Escribir("ALTER TABLE [dbo].[Producto] DROP COLUMN [DV]", null);
                }

                acceso.Escribir("ALTER TABLE [dbo].[Producto] ADD [DVH] NVARCHAR(255) NULL", null);
                recienCreado = true;
            }

            // Check and create DVV table
            string checkDVV = "SELECT COUNT(*) FROM sys.tables WHERE name = 'DVV'";
            DataTable dtDvv = acceso.Leer(checkDVV, null);
            bool tieneDVV = dtDvv.Rows.Count > 0 && Convert.ToInt32(dtDvv.Rows[0][0]) > 0;

            if (!tieneDVV)
            {
                string createTableDVV = @"
                    CREATE TABLE [dbo].[DVV](
                        [Tabla] [nvarchar](50) NOT NULL,
                        [DVV] [nvarchar](255) NOT NULL,
                        CONSTRAINT [PK_DVV] PRIMARY KEY CLUSTERED ([Tabla] ASC)
                    )";
                acceso.Escribir(createTableDVV, null);
                recienCreado = true;
            }

            if (recienCreado)
            {
                RecalcularDVUsuarios();
                RecalcularDVProductos();
            }
        }

        public void ValidarIntegridadGlobal()
        {
            VerificarYAsegurarEsquema();

            bool dvvUsuariosValido;
            var usuariosCorruptos = VerificarIntegridadUsuarios(out dvvUsuariosValido);

            bool dvvProductosValido;
            var productosCorruptos = VerificarIntegridadProductos(out dvvProductosValido);

            if (usuariosCorruptos.Count > 0 || !dvvUsuariosValido || productosCorruptos.Count > 0 || !dvvProductosValido)
            {
                string msg = "Error de integridad de datos detectado:";
                if (usuariosCorruptos.Count > 0) msg += " Tabla Usuario tiene DVH corruptos.";
                if (!dvvUsuariosValido) msg += " Tabla Usuario tiene DVV inválido.";
                if (productosCorruptos.Count > 0) msg += " Tabla Producto tiene DVH corruptos.";
                if (!dvvProductosValido) msg += " Tabla Producto tiene DVV inválido.";

                throw new InvalidOperationException(msg);
            }
        }

        public void RecalcularDVUsuarios()
        {
            List<BEUsuario> todosLosUsuarios = _dalUsuario.ObtenerTodos();
            List<string> dvhs = new List<string>();
            foreach (var usuario in todosLosUsuarios)
            {
                usuario.DVH = _gestorIntegridad.CalcularDVH(usuario);
                dvhs.Add(usuario.DVH);
                _dalUsuario.Actualizar(usuario);
            }

            string dvv = _gestorIntegridad.CalcularDVV(dvhs);
            _dalDVV.GuardarDVV("Usuario", dvv);
        }

        public void RecalcularDVProductos()
        {
            List<BEProducto> todosLosProductos = _dalProducto.ObtenerProductos();
            List<string> dvhs = new List<string>();
            foreach (var producto in todosLosProductos)
            {
                producto.DVH = _gestorIntegridad.CalcularDVH(producto);
                dvhs.Add(producto.DVH);
                _dalProducto.Actualizar(producto);
            }

            string dvv = _gestorIntegridad.CalcularDVV(dvhs);
            _dalDVV.GuardarDVV("Producto", dvv);
        }
    }
}
