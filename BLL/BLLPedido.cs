using BE;
using DAL;
using SERVICIOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BLL
{
    public class BLLPedido
    {
        private static readonly object _bloqueoCompra = new object();

        private readonly DALPedido _dalPedido = new DALPedido();
        private readonly DALCarrito _dalCarrito = new DALCarrito();
        private readonly BLLProducto _bllProducto = new BLLProducto();
        private readonly BLLEnvio _bllEnvio = new BLLEnvio();
        private readonly BLLFactura _bllFactura = new BLLFactura();
        private readonly GestorIntegridad _gestorIntegridad = new GestorIntegridad();
        private readonly ServicioBitacora _sBitacora = new ServicioBitacora();

        public BEPedido FinalizarCompra(BEUsuario usuario, string nombreCliente, string documentoCliente, string direccionEnvio, double latitud, double longitud)
        {
            if (usuario == null)
                throw new ArgumentException("Iniciá sesión para finalizar la compra.");

            nombreCliente = (nombreCliente ?? string.Empty).Trim();
            direccionEnvio = (direccionEnvio ?? string.Empty).Trim();
            string documento = NormalizarDocumento(documentoCliente);

            if (nombreCliente.Length < 2 || nombreCliente.Length > 100)
                throw new ArgumentException("Ingresá tu nombre y apellido (hasta 100 caracteres).");
            if (documento == null)
                throw new ArgumentException("Ingresá un DNI (7 u 8 dígitos) o un CUIT/CUIL (11 dígitos) válido.");
            if (direccionEnvio.Length < 5 || direccionEnvio.Length > 200)
                throw new ArgumentException("Ingresá la dirección de entrega (entre 5 y 200 caracteres).");

            double distanciaKm = _bllEnvio.CalcularDistanciaKm(latitud, longitud);

            BEPedido pedido = new BEPedido
            {
                IdUsuario = usuario.IdUsuario,
                NombreCliente = nombreCliente,
                DocumentoCliente = documento,
                DireccionEnvio = direccionEnvio,
                DistanciaKm = Math.Round((decimal)distanciaKm, 2),
                Envio = _bllEnvio.CalcularCosto(distanciaKm)
            };

            // Una compra a la vez: el DVV de Producto se recalcula sobre toda la tabla.
            lock (_bloqueoCompra)
            {
                Dictionary<int, int> cantidades = _dalCarrito.ObtenerCantidadesProductos(usuario.IdUsuario);
                if (!cantidades.Any(c => c.Value > 0))
                    throw new ArgumentException("Tu carrito está vacío.");

                // ObtenerProductos ya valida el DVH de cada producto (integridad).
                List<BEProducto> catalogo = _bllProducto.ObtenerProductos();
                List<BEProducto> comprados = new List<BEProducto>();

                foreach (KeyValuePair<int, int> item in cantidades.Where(c => c.Value > 0).OrderBy(c => c.Key))
                {
                    BEProducto producto = catalogo.Find(p => p.IdProducto == item.Key);
                    if (producto == null)
                        throw new ArgumentException("El producto " + item.Key + " ya no está disponible.");
                    if (item.Value > producto.Stock)
                        throw new ArgumentException("Stock insuficiente para " + producto.Nombre + ". Disponible: " + producto.Stock + ".");

                    pedido.Lineas.Add(new BELineaPedido
                    {
                        IdProducto = producto.IdProducto,
                        Nombre = producto.Nombre,
                        PrecioUnitario = producto.Precio,
                        Cantidad = item.Value
                    });

                    producto.Stock -= item.Value;
                    producto.DVH = _gestorIntegridad.CalcularDVH(producto);
                    comprados.Add(producto);
                }

                pedido.Fecha = DateTime.Now;
                pedido.Subtotal = pedido.Lineas.Sum(l => l.Subtotal);
                pedido.Total = pedido.Subtotal + pedido.Envio;
                pedido.Factura = _bllFactura.Emitir(pedido);

                string dvv = _gestorIntegridad.CalcularDVV(catalogo.Select(p => p.DVH).ToList());
                _dalPedido.RegistrarPedido(pedido, comprados, dvv);
            }

            RegistrarEventoCompra($"Compra registrada: pedido {pedido.IdPedido}, factura {BLLFactura.Tipo} {_bllFactura.FormatearNumero(pedido.Factura)}, por ${pedido.Total:N2}", usuario);
            return pedido;
        }

        public List<BEPedido> ObtenerMisCompras(int idUsuario)
        {
            return _dalPedido.ObtenerPorUsuario(idUsuario);
        }

        public BEPedido ObtenerPedido(int idPedido, BEUsuario usuario, bool veTodos)
        {
            BEPedido pedido = _dalPedido.ObtenerPorId(idPedido);
            if (pedido == null)
                return null;
            if (!veTodos && (usuario == null || pedido.IdUsuario != usuario.IdUsuario))
                return null;
            return pedido;
        }

        private static string NormalizarDocumento(string documento)
        {
            string digitos = Regex.Replace(documento ?? string.Empty, @"[\s.\-]", string.Empty);
            return Regex.IsMatch(digitos, "^([0-9]{7,8}|[0-9]{11})$") ? digitos : null;
        }

        private void RegistrarEventoCompra(string accion, BEUsuario usuario)
        {
            try
            {
                _sBitacora.RegistrarEvento(accion, usuario);
            }
            catch (Exception)
            {
            }
        }
    }
}
