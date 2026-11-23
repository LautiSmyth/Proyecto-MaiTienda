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
        private readonly DALBitacora _dalBitacora = new DALBitacora();
        private readonly DALPedido _dalPedido = new DALPedido();
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

        public List<BEBitacora> VerificarIntegridadBitacora(out bool dvvValido)
        {
            dvvValido = true;
            List<BEBitacora> eventosCorruptos = new List<BEBitacora>();
            List<BEBitacora> todosLosEventos = _dalBitacora.ObtenerTodasOrdenadas();

            List<string> dvhs = new List<string>();
            foreach (var evento in todosLosEventos)
            {
                string dvCalculado = _gestorIntegridad.CalcularDVH(evento);
                dvhs.Add(dvCalculado);
                if (evento.DVH != dvCalculado)
                {
                    eventosCorruptos.Add(evento);
                }
            }

            string dvvCalculado = _gestorIntegridad.CalcularDVV(dvhs);
            string dvvRegistrado = _dalDVV.ObtenerDVV("Bitacora");
            if (dvvCalculado != dvvRegistrado)
            {
                dvvValido = false;
            }

            return eventosCorruptos;
        }

        public List<BEPedido> VerificarIntegridadPedidos(out bool dvvValido)
        {
            dvvValido = true;
            List<BEPedido> pedidosCorruptos = new List<BEPedido>();
            List<BEPedido> todosLosPedidos = _dalPedido.ObtenerTodosParaIntegridad();

            List<string> dvhs = new List<string>();
            foreach (var pedido in todosLosPedidos)
            {
                string dvCalculado = _gestorIntegridad.CalcularDVH(pedido);
                dvhs.Add(dvCalculado);
                if (pedido.DVH != dvCalculado)
                {
                    pedidosCorruptos.Add(pedido);
                }
            }

            string dvvCalculado = _gestorIntegridad.CalcularDVV(dvhs);
            string dvvRegistrado = _dalDVV.ObtenerDVV("Pedido");
            if (dvvCalculado != dvvRegistrado)
            {
                dvvValido = false;
            }

            return pedidosCorruptos;
        }

        public List<BEFactura> VerificarIntegridadFacturas(out bool dvvValido)
        {
            dvvValido = true;
            List<BEFactura> facturasCorruptas = new List<BEFactura>();
            List<BEFactura> todasLasFacturas = _dalPedido.ObtenerTodasFacturasParaIntegridad();

            List<string> dvhs = new List<string>();
            foreach (var factura in todasLasFacturas)
            {
                string dvCalculado = _gestorIntegridad.CalcularDVH(factura);
                dvhs.Add(dvCalculado);
                if (factura.DVH != dvCalculado)
                {
                    facturasCorruptas.Add(factura);
                }
            }

            string dvvCalculado = _gestorIntegridad.CalcularDVV(dvhs);
            string dvvRegistrado = _dalDVV.ObtenerDVV("Factura");
            if (dvvCalculado != dvvRegistrado)
            {
                dvvValido = false;
            }

            return facturasCorruptas;
        }

        private void VerificarYAsegurarEsquema()
        {
            Acceso acceso = Acceso.GetInstance();
            bool recienCreado = false;

            string checkUsuario = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Usuario]') AND name = 'DVH'";
            DataTable dtUsr = acceso.Leer(checkUsuario, null);
            bool tieneDVHUsuario = dtUsr.Rows.Count > 0 && Convert.ToInt32(dtUsr.Rows[0][0]) > 0;

            if (!tieneDVHUsuario)
            {
                string checkDVOld = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Usuario]') AND name = 'DV'";
                DataTable dtOld = acceso.Leer(checkDVOld, null);
                if (dtOld.Rows.Count > 0 && Convert.ToInt32(dtOld.Rows[0][0]) > 0)
                {
                    acceso.Escribir("ALTER TABLE [dbo].[Usuario] DROP COLUMN [DV]", null);
                }
                
                acceso.Escribir("ALTER TABLE [dbo].[Usuario] ADD [DVH] NVARCHAR(255) NULL", null);
                recienCreado = true;
            }

            string checkProducto = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Producto]') AND name = 'DVH'";
            DataTable dtProd = acceso.Leer(checkProducto, null);
            bool tieneDVHProducto = dtProd.Rows.Count > 0 && Convert.ToInt32(dtProd.Rows[0][0]) > 0;

            if (!tieneDVHProducto)
            {
                string checkDVOld = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Producto]') AND name = 'DV'";
                DataTable dtOld = acceso.Leer(checkDVOld, null);
                if (dtOld.Rows.Count > 0 && Convert.ToInt32(dtOld.Rows[0][0]) > 0)
                {
                    acceso.Escribir("ALTER TABLE [dbo].[Producto] DROP COLUMN [DV]", null);
                }

                acceso.Escribir("ALTER TABLE [dbo].[Producto] ADD [DVH] NVARCHAR(255) NULL", null);
                recienCreado = true;
            }

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

            string checkBitacora = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Bitacora]') AND name = 'DVH'";
            DataTable dtBit = acceso.Leer(checkBitacora, null);
            bool tieneDVHBitacora = dtBit.Rows.Count > 0 && Convert.ToInt32(dtBit.Rows[0][0]) > 0;
            bool bitacoraRecienAgregada = false;

            if (!tieneDVHBitacora)
            {
                acceso.Escribir("ALTER TABLE [dbo].[Bitacora] ADD [DVH] NVARCHAR(255) NULL", null);
                bitacoraRecienAgregada = true;
            }
            else
            {
                // La columna puede existir pero con filas sembradas por db.sql que nunca tuvieron DVH
                // (a diferencia de Usuario y Producto, la bitácora sembrada no trae hashes precalculados).
                DataTable dtNulos = acceso.Leer("SELECT COUNT(*) FROM [dbo].[Bitacora] WHERE [DVH] IS NULL", null);
                if (dtNulos.Rows.Count > 0 && Convert.ToInt32(dtNulos.Rows[0][0]) > 0)
                {
                    bitacoraRecienAgregada = true;
                }
            }

            try
            {
                new DALPedido().AsegurarEsquema();
            }
            catch (Exception ex)
            {
                // Si no se pueden crear las tablas de compras, el catálogo sigue funcionando: solo fallan las compras.
                System.Diagnostics.Trace.TraceError(ex.ToString());
            }

            // Si Pedido/Factura ya existían de antes (sin la columna DVH), AsegurarEsquema no las vuelve a crear.
            bool pedidoFacturaRecienAgregada = false;
            try
            {
                string checkPedido = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Pedido]') AND name = 'DVH'";
                DataTable dtPed = acceso.Leer(checkPedido, null);
                if (dtPed.Rows.Count > 0 && Convert.ToInt32(dtPed.Rows[0][0]) == 0)
                {
                    acceso.Escribir("ALTER TABLE [dbo].[Pedido] ADD [DVH] NVARCHAR(255) NULL", null);
                    pedidoFacturaRecienAgregada = true;
                }

                string checkFactura = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Factura]') AND name = 'DVH'";
                DataTable dtFac = acceso.Leer(checkFactura, null);
                if (dtFac.Rows.Count > 0 && Convert.ToInt32(dtFac.Rows[0][0]) == 0)
                {
                    acceso.Escribir("ALTER TABLE [dbo].[Factura] ADD [DVH] NVARCHAR(255) NULL", null);
                    pedidoFacturaRecienAgregada = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
            }

            if (recienCreado)
            {
                RecalcularDVUsuarios();
                RecalcularDVProductos();
            }

            if (bitacoraRecienAgregada)
            {
                RecalcularDVBitacora();
            }

            if (pedidoFacturaRecienAgregada)
            {
                RecalcularDVPedidosYFacturas();
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

        public void RecalcularDVBitacora()
        {
            List<BEBitacora> todosLosEventos = _dalBitacora.ObtenerTodasOrdenadas();
            List<string> dvhs = new List<string>();
            foreach (var evento in todosLosEventos)
            {
                evento.DVH = _gestorIntegridad.CalcularDVH(evento);
                dvhs.Add(evento.DVH);
                _dalBitacora.ActualizarDVH(evento.IdBitacora, evento.DVH);
            }

            string dvv = _gestorIntegridad.CalcularDVV(dvhs);
            _dalDVV.GuardarDVV("Bitacora", dvv);
        }

        public void RecalcularDVPedidosYFacturas()
        {
            List<BEPedido> todosLosPedidos = _dalPedido.ObtenerTodosParaIntegridad();
            List<string> dvhsPedidos = new List<string>();
            foreach (var pedido in todosLosPedidos)
            {
                pedido.DVH = _gestorIntegridad.CalcularDVH(pedido);
                dvhsPedidos.Add(pedido.DVH);
                _dalPedido.ActualizarDVHPedido(pedido.IdPedido, pedido.DVH);
            }
            _dalDVV.GuardarDVV("Pedido", _gestorIntegridad.CalcularDVV(dvhsPedidos));

            List<BEFactura> todasLasFacturas = _dalPedido.ObtenerTodasFacturasParaIntegridad();
            List<string> dvhsFacturas = new List<string>();
            foreach (var factura in todasLasFacturas)
            {
                factura.DVH = _gestorIntegridad.CalcularDVH(factura);
                dvhsFacturas.Add(factura.DVH);
                _dalPedido.ActualizarDVHFactura(factura.IdFactura, factura.DVH);
            }
            _dalDVV.GuardarDVV("Factura", _gestorIntegridad.CalcularDVV(dvhsFacturas));
        }
    }
}
