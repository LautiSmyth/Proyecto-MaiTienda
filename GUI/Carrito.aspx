<%@ Page Title="Mi Carrito" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Carrito.aspx.cs" Inherits="Carrito" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div style="background: #f0ebfa; min-height: 100vh; padding: 40px 0; position: relative;">
        <div class="g-grid"></div>

        <div class="container" style="position: relative; z-index: 2; max-width: 1200px; margin: 0 auto;">

            <div style="text-align: center; margin-bottom: 40px;">
                <span class="g-pretag">Resumen de Selección</span>
                <h1 class="store-title" style="font-size: 36px; margin: 5px 0;">Mi <span>Carrito</span></h1>
                <p class="store-subtitle" style="font-size: 11px; letter-spacing: 2px;">Revisa tus componentes antes de confirmar la compra</p>
                <div class="g-divider" style="margin: 15px auto 0; width: 60px;"></div>
            </div>

            <div class="row" style="display: flex; flex-wrap: wrap; gap: 30px;">

                <div style="flex: 1 1 700px; display: flex; flex-direction: column; gap: 20px;">

                    <div id="pnlSugerenciaLogin" class="g-card" style="display: none; background: linear-gradient(135deg, #faf7ff, #f3ebfa); border-color: #ddd0f5; padding: 15px 20px; align-items: center; gap: 15px;">
                        <span style="font-size: 20px;">💡</span>
                        <div style="flex: 1;">
                            <h4 style="font-family: 'Exo 2', sans-serif; font-size: 13px; font-weight: 600; color: #4c1d95; margin: 0;">Inicia sesión para guardar tu carrito</h4>
                            <p style="font-family: 'Exo 2', sans-serif; font-size: 11px; color: #7c5ea8; margin: 2px 0 0 0;">Guarda tus componentes permanentemente en tu cuenta y accede a envíos gratis.</p>
                        </div>
                        <a href="Login.aspx" class="g-btn" style="text-decoration: none; padding: 6px 15px; font-size: 10px; margin: 0; width: auto !important;">Iniciar Sesión</a>
                    </div>

                    <asp:PlaceHolder ID="phCarritoServidor" runat="server" Visible="false">
                        <asp:Repeater ID="repCarrito" runat="server" OnItemCommand="repCarrito_ItemCommand">
                            <ItemTemplate>
                                <div class="g-card" style="display: flex; align-items: center; padding: 15px 20px; gap: 20px; flex-wrap: wrap;">
                                    <img src='<%# ResolveUrl(Eval("ImagenUrl").ToString()) %>' alt='<%# Eval("Nombre") %>'
                                         style="width: 80px; height: 80px; object-fit: cover; border-radius: 4px; border: 1px solid #ede5f8;"
                                         onerror="this.onerror=null;this.src='https://placehold.co/80x80/faf7ff/7c3aed?text=Hardware';" />
                                    <div style="flex: 1; min-width: 200px;">
                                        <span style="font-family: 'Exo 2', sans-serif; font-size: 10px; font-weight: 600; color: #9f7cc0; text-transform: uppercase;"><%# Eval("Categoria") %></span>
                                        <h3 style="font-family: 'Exo 2', sans-serif; font-size: 16px; font-weight: 600; color: #1e0a3c; margin: 2px 0 0 0;"><%# Eval("Nombre") %></h3>
                                    </div>
                                    <div style="display: flex; align-items: center; gap: 5px;">
                                        <asp:LinkButton ID="btnRestar" runat="server" CommandName="Restar" CommandArgument='<%# Eval("IdProducto") %>'
                                             style="width: 28px; height: 28px; display: flex; align-items: center; justify-content: center; background: #faf7ff; border: 1px solid #ddd0f5; color: #7c3aed; font-weight: bold; text-decoration: none; cursor: pointer;">-</asp:LinkButton>
                                        <span style="font-family: 'Orbitron', sans-serif; font-weight: 900; width: 30px; text-align: center; color: #1e0a3c;"><%# Eval("Stock") %></span>
                                        <asp:LinkButton ID="btnSumar" runat="server" CommandName="Sumar" CommandArgument='<%# Eval("IdProducto") %>'
                                             style="width: 28px; height: 28px; display: flex; align-items: center; justify-content: center; background: #faf7ff; border: 1px solid #ddd0f5; color: #7c3aed; font-weight: bold; text-decoration: none; cursor: pointer;">+</asp:LinkButton>
                                    </div>
                                    <div style="text-align: right; min-width: 120px;">
                                        <div style="font-family: 'Exo 2', sans-serif; font-size: 11px; color: #9f7cc0;">Precio Unitario: $<%# Eval("Precio", "{0:N2}") %></div>
                                        <div style="font-family: 'Orbitron', sans-serif; font-size: 18px; font-weight: 900; color: #7c3aed;">
                                            $<%# Convert.ToDecimal(Eval("Precio")) * Convert.ToInt32(Eval("Stock")) %>
                                        </div>
                                    </div>
                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("IdProducto") %>'
                                         style="color: #ef4444; background: none; border: none; cursor: pointer; padding: 5px; font-size: 16px; text-decoration: none;">✕</asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:PlaceHolder>

                    <div id="carritoClienteContainer" style="display: none; flex-direction: column; gap: 20px;"></div>

                    <div id="divCarritoVacio" runat="server" class="g-card" style="padding: 40px; text-align: center;">
                        <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="#9f7cc0" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" style="margin-bottom: 15px;">
                            <circle cx="9" cy="21" r="1"></circle><circle cx="20" cy="21" r="1"></circle>
                            <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
                        </svg>
                        <h3 style="font-family: 'Orbitron', sans-serif; font-size: 18px; font-weight: 900; color: #7c3aed; margin-bottom: 5px;">Tu carrito está vacío</h3>
                        <p style="font-family: 'Exo 2', sans-serif; color: #9f7cc0; font-size: 13px; margin-bottom: 20px;">Parece que aún no has agregado ningún componente de hardware a tu selección.</p>
                        <a href="Default.aspx" class="g-btn" style="text-decoration: none; display: inline-block; padding: 10px 25px;">Volver a la Tienda</a>
                    </div>
                </div>

                <div style="flex: 1 1 350px; max-width: 450px;">
                    <div class="g-card" style="padding: 25px; position: sticky; top: 100px;">
                        <h3 style="font-family: 'Orbitron', sans-serif; font-size: 18px; font-weight: 900; color: #1e0a3c; margin-bottom: 20px; border-bottom: 1.5px solid #ede5f8; padding-bottom: 10px;">Resumen</h3>

                        <div style="display: flex; justify-content: space-between; margin-bottom: 12px; font-family: 'Exo 2', sans-serif; font-size: 14px;">
                            <span style="color: #7c5ea8;">Subtotal</span>
                            <span id="lblSubtotal" runat="server" style="font-weight: 600; color: #1e0a3c;">$0,00</span>
                        </div>

                        <div style="display: flex; justify-content: space-between; margin-bottom: 4px; font-family: 'Exo 2', sans-serif; font-size: 14px;">
                            <span style="color: #7c5ea8;">Envío</span>
                            <span id="lblEnvio" runat="server" style="color: #9f7cc0; font-weight: 600;">Calculando...</span>
                        </div>

                        <div id="divDistanciaInfo" style="display: none; text-align: right; margin-bottom: 4px;">
                            <span style="font-family: 'Exo 2', sans-serif; font-size: 11px; color: #9f7cc0;">📍 <span id="spanDistancia"></span></span>
                        </div>

                        <div id="divModificarUbicacion" style="display: none; text-align: right; margin-bottom: 8px;">
                            <a href="javascript:void(0)" onclick="mostrarInputManual()"
                               style="font-family: 'Exo 2', sans-serif; font-size: 11px; color: #7c3aed; text-decoration: underline; cursor: pointer;">✏️ Modificar ubicación</a>
                        </div>

                        <div id="divUbicacionManual" style="display: none; margin-bottom: 12px;">
                            <label style="font-family: 'Exo 2', sans-serif; font-size: 11px; font-weight: 600; color: #7c5ea8; display: block; margin-bottom: 5px;">📍 Ingresá tu dirección:</label>
                            <div style="display: flex; gap: 6px;">
                                <input type="text" id="txtDireccion" placeholder="Ej: Av. Corrientes 1234, CABA"
                                       style="flex: 1; font-family: 'Exo 2', sans-serif; font-size: 11px; padding: 6px 10px; border: 1px solid #ddd0f5; border-radius: 3px; color: #1e0a3c; background: #faf7ff; outline: none;" />
                                <button type="button" id="btnCalcularEnvio" onclick="geocodificarDireccion()"
                                        style="font-family: 'Exo 2', sans-serif; font-size: 10px; font-weight: 600; padding: 6px 12px; background: #7c3aed; color: #fff; border: none; border-radius: 3px; cursor: pointer; white-space: nowrap; letter-spacing: 0.5px;">
                                    Calcular
                                </button>
                            </div>
                            <span id="lblGeocodingError" style="font-family: 'Exo 2', sans-serif; font-size: 10px; color: #ef4444; display: none; margin-top: 4px;">⚠️ No se encontró la dirección. Intentá con más detalle.</span>
                        </div>

                        <div style="margin: 20px 0; border-top: 1.5px dashed #ede5f8; padding-top: 15px; display: flex; justify-content: space-between; align-items: center;">
                            <span style="font-family: 'Orbitron', sans-serif; font-weight: 900; color: #1e0a3c; font-size: 16px;">Total</span>
                            <span id="lblTotal" runat="server" style="font-family: 'Orbitron', sans-serif; font-weight: 900; color: #7c3aed; font-size: 24px;">$0,00</span>
                        </div>

                        <div id="pnlSignature" style="margin-bottom: 20px;">
                            <label style="font-family: 'Exo 2', sans-serif; font-size: 11px; font-weight: 600; color: #7c5ea8; display: block; margin-bottom: 8px;">Firme aquí para autorizar el despacho de hardware:</label>
                            <div style="border: 1px solid #ddd0f5; background: #faf7ff; border-radius: 4px; position: relative;">
                                <canvas id="signature-canvas" style="display: block; width: 100%; height: 110px; cursor: crosshair; touch-action: none; background: #faf7ff;"></canvas>
                                <button type="button" id="btnClearSignature" style="position: absolute; bottom: 5px; right: 5px; background: rgba(239, 68, 68, 0.1); border: 1px solid rgba(239, 68, 68, 0.3); color: #ef4444; font-family: 'Exo 2', sans-serif; font-size: 10px; padding: 2px 8px; cursor: pointer; border-radius: 3px;">Borrar</button>
                            </div>
                            <span id="lblSignatureError" style="font-family: 'Exo 2', sans-serif; font-size: 10px; color: #ef4444; display: none; margin-top: 5px;">⚠️ Se requiere firma de conformidad para proceder.</span>
                        </div>

                        <asp:Button ID="btnCheckout" runat="server" Text="Finalizar Compra" CssClass="g-btn" OnClick="btnCheckout_Click" OnClientClick="return validarFirmaDigital();"
                            style="width: 100% !important; padding: 14px 0 !important; font-size: 12px !important; margin-top: 10px !important;" />

                        <a href="Default.aspx" style="display: block; text-align: center; margin-top: 15px; font-family: 'Exo 2', sans-serif; font-size: 12px; color: #7c3aed; text-decoration: none; font-weight: 600;">← Seguir Comprando</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        var STORE_LAT = -34.7621;
        var STORE_LNG = -58.3940;
        var FREE_SHIPPING_KM = 10;
        var costoEnvio = 0;

        function haversineKm(lat1, lon1, lat2, lon2) {
            var R = 6371;
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                    Math.cos(lat1 * Math.PI / 180) * Math.cos(lat2 * Math.PI / 180) *
                    Math.sin(dLon / 2) * Math.sin(dLon / 2);
            return R * 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        }

        function calcularCostoEnvio(distanciaKm) {
            if (distanciaKm <= FREE_SHIPPING_KM) return 0;
            return Math.round(5000 * Math.pow(distanciaKm / FREE_SHIPPING_KM, 0.6) / 500) * 500;
        }

        function parsearPrecio(texto) {
            var s = (texto || '').replace(/\$/g, '').trim();
            return s.lastIndexOf(',') > s.lastIndexOf('.')
                ? parseFloat(s.replace(/\./g, '').replace(',', '.'))
                : parseFloat(s.replace(/,/g, '')) || 0;
        }

        function el(id) { return document.getElementById(id); }

        function actualizarEnvioEnUI(distanciaKm, costo) {
            var envioEl = el('<%= lblEnvio.ClientID %>');
            var totalEl = el('<%= lblTotal.ClientID %>');
            var subtotalEl = el('<%= lblSubtotal.ClientID %>');

            if (el('lblGeocodingError')) el('lblGeocodingError').style.display = 'none';

            if (envioEl) {
                envioEl.innerText = costo === 0 ? 'Bonificado' : '$' + costo.toLocaleString('es-AR', { minimumFractionDigits: 2 });
                envioEl.style.color = costo === 0 ? '#10b981' : '#ef4444';
            }

            if (totalEl)
                totalEl.innerText = '$' + ((subtotalEl ? parsearPrecio(subtotalEl.innerText) : 0) + costo).toLocaleString('es-AR', { minimumFractionDigits: 2 });

            if (el('spanDistancia')) el('spanDistancia').innerText = distanciaKm.toFixed(1) + ' km del negocio';
            if (el('divDistanciaInfo'))    el('divDistanciaInfo').style.display    = 'block';
            if (el('divModificarUbicacion')) el('divModificarUbicacion').style.display = 'block';
            if (el('divUbicacionManual'))  el('divUbicacionManual').style.display  = 'none';
        }

        function mostrarInputManual() {
            costoEnvio = 0;
            if (el('divDistanciaInfo'))    el('divDistanciaInfo').style.display    = 'none';
            if (el('divModificarUbicacion')) el('divModificarUbicacion').style.display = 'none';
            if (el('divUbicacionManual'))  el('divUbicacionManual').style.display  = 'block';
            var input = el('txtDireccion');
            if (input) {
                input.focus();
                var envioEl = el('<%= lblEnvio.ClientID %>');
                if (envioEl) { envioEl.innerText = 'Ingresá tu dirección'; envioEl.style.color = '#9f7cc0'; }
            }
        }

        function iniciarGeolocalizacion() {
            var envioEl = el('<%= lblEnvio.ClientID %>');
            if (envioEl) { envioEl.innerText = 'Calculando...'; envioEl.style.color = '#9f7cc0'; }

            if (!navigator.geolocation) { mostrarInputManual(); return; }

            navigator.geolocation.getCurrentPosition(
                function (pos) {
                    var distancia = haversineKm(pos.coords.latitude, pos.coords.longitude, STORE_LAT, STORE_LNG);
                    costoEnvio = calcularCostoEnvio(distancia);
                    actualizarEnvioEnUI(distancia, costoEnvio);
                },
                function () { mostrarInputManual(); },
                { timeout: 10000, maximumAge: 300000 }
            );
        }

        window.mostrarInputManual = mostrarInputManual;

        window.geocodificarDireccion = function () {
            var input   = el('txtDireccion');
            var btnCalc = el('btnCalcularEnvio');
            var lblErr  = el('lblGeocodingError');

            if (!input || !input.value.trim()) return;
            if (lblErr) lblErr.style.display = 'none';
            if (btnCalc) { btnCalc.disabled = true; btnCalc.innerText = '...'; }

            fetch('https://nominatim.openstreetmap.org/search?q=' + encodeURIComponent(input.value.trim()) + '&format=json&limit=1&countrycodes=ar',
                  { headers: { 'Accept-Language': 'es' } })
                .then(function (r) { return r.json(); })
                .then(function (data) {
                    if (btnCalc) { btnCalc.disabled = false; btnCalc.innerText = 'Calcular'; }
                    if (!data || data.length === 0) { if (lblErr) lblErr.style.display = 'block'; return; }
                    var distancia = haversineKm(parseFloat(data[0].lat), parseFloat(data[0].lon), STORE_LAT, STORE_LNG);
                    costoEnvio = calcularCostoEnvio(distancia);
                    actualizarEnvioEnUI(distancia, costoEnvio);
                })
                .catch(function () {
                    if (btnCalc) { btnCalc.disabled = false; btnCalc.innerText = 'Calcular'; }
                    if (lblErr) lblErr.style.display = 'block';
                });
        };

        document.addEventListener('DOMContentLoaded', function () {
            iniciarGeolocalizacion();

            var inputDir = el('txtDireccion');
            if (inputDir) inputDir.addEventListener('keydown', function (e) {
                if (e.key === 'Enter') { e.preventDefault(); geocodificarDireccion(); }
            });

            if (!usuarioLogueado) {
                var sugerencia = el('pnlSugerenciaLogin');
                if (sugerencia) sugerencia.style.display = 'flex';
                cargarCarritoCliente();
            }

            function cargarCarritoCliente() {
                var carrito     = JSON.parse(localStorage.getItem('carrito')) || [];
                var container   = el('carritoClienteContainer');
                var emptyDiv    = el('<%= divCarritoVacio.ClientID %>');
                var checkoutBtn = el('<%= btnCheckout.ClientID %>');

                if (carrito.length === 0) {
                    container.style.display = 'none';
                    if (emptyDiv)    emptyDiv.style.display = 'block';
                    if (checkoutBtn) checkoutBtn.disabled   = true;
                    actualizarResumenPrecios(0);
                    return;
                }

                if (emptyDiv)    emptyDiv.style.display    = 'none';
                if (checkoutBtn) checkoutBtn.disabled      = false;
                container.style.display = 'flex';

                fetch('<%= ResolveUrl("~/Carrito.aspx/ObtenerDetallesCarritoLocal") %>', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    body: JSON.stringify({ productoIds: carrito })
                })
                .then(function (r) { if (!r.ok) throw new Error('HTTP ' + r.status); return r.json(); })
                .then(function (data) {
                    if (!data.d || data.d.length === 0) {
                        container.innerHTML = '<div style="background:#fee2e2;border:1px solid #fca5a5;padding:15px;color:#991b1b;font-family:\'Exo 2\',sans-serif;font-size:12px;">Error al obtener productos del servidor.</div>';
                        return;
                    }
                    renderizarCarritoCliente(data.d);
                })
                .catch(function (err) {
                    container.innerHTML = '<div style="background:#fee2e2;border:1px solid #fca5a5;padding:15px;color:#991b1b;font-family:\'Exo 2\',sans-serif;font-size:12px;">Error: ' + err.message + '</div>';
                });
            }

            function renderizarCarritoCliente(productos) {
                var container = el('carritoClienteContainer');
                container.innerHTML = '';
                var subtotal = 0;

                productos.forEach(function (p) {
                    var itemSubtotal = p.Precio * p.Cantidad;
                    subtotal += itemSubtotal;
                    container.innerHTML += `
                        <div class="g-card" style="display:flex;align-items:center;padding:15px 20px;gap:20px;flex-wrap:wrap;">
                            <img src="${p.ImagenUrl.replace('~/', '')}" alt="${p.Nombre}"
                                 style="width:80px;height:80px;object-fit:cover;border-radius:4px;border:1px solid #ede5f8;"
                                 onerror="this.onerror=null;this.src='https://placehold.co/80x80/faf7ff/7c3aed?text=Hardware';" />
                            <div style="flex:1;min-width:200px;">
                                <span style="font-family:'Exo 2',sans-serif;font-size:10px;font-weight:600;color:#9f7cc0;text-transform:uppercase;">${p.Categoria}</span>
                                <h3 style="font-family:'Exo 2',sans-serif;font-size:16px;font-weight:600;color:#1e0a3c;margin:2px 0 0 0;">${p.Nombre}</h3>
                            </div>
                            <div style="display:flex;align-items:center;gap:5px;">
                                <button type="button" onclick="modificarCantidadLocal(${p.IdProducto},-1)"
                                     style="width:28px;height:28px;display:flex;align-items:center;justify-content:center;background:#faf7ff;border:1px solid #ddd0f5;color:#7c3aed;font-weight:bold;cursor:pointer;">-</button>
                                <span style="font-family:'Orbitron',sans-serif;font-weight:900;width:30px;text-align:center;color:#1e0a3c;">${p.Cantidad}</span>
                                <button type="button" onclick="modificarCantidadLocal(${p.IdProducto},1,${p.Stock})"
                                     style="width:28px;height:28px;display:flex;align-items:center;justify-content:center;background:#faf7ff;border:1px solid #ddd0f5;color:#7c3aed;font-weight:bold;cursor:pointer;">+</button>
                            </div>
                            <div style="text-align:right;min-width:120px;">
                                <div style="font-family:'Exo 2',sans-serif;font-size:11px;color:#9f7cc0;">Unitario: $${p.Precio.toLocaleString('es-AR',{minimumFractionDigits:2})}</div>
                                <div style="font-family:'Orbitron',sans-serif;font-size:18px;font-weight:900;color:#7c3aed;">$${itemSubtotal.toLocaleString('es-AR',{minimumFractionDigits:2})}</div>
                            </div>
                            <button type="button" onclick="eliminarProductoLocal(${p.IdProducto})"
                                 style="color:#ef4444;background:none;border:none;cursor:pointer;padding:5px;font-size:16px;">✕</button>
                        </div>`;
                });

                actualizarResumenPrecios(subtotal);
            }

            function guardarCarritoCookie(carrito) {
                var json = JSON.stringify(carrito);
                var encoded = encodeURIComponent(json);
                document.cookie = "carrito_anon=" + encoded + "; path=/; max-age=31536000";
            }

            function actualizarContadorNavbar(carrito) {
                var lbl = document.querySelector('[id*="lblCarritoContador"]');
                if (lbl) lbl.innerText = carrito.length;
            }

            window.modificarCantidadLocal = function (id, cambio, stockMax) {
                var carrito = JSON.parse(localStorage.getItem('carrito')) || [];
                if (cambio === 1) {
                    var currentQty = carrito.filter(function(x) { return x === id; }).length;
                    if (currentQty >= stockMax) {
                        alert("No hay suficiente stock disponible para este producto.");
                        return;
                    }
                    carrito.push(id);
                }
                else { var idx = carrito.indexOf(id); if (idx > -1) carrito.splice(idx, 1); }
                localStorage.setItem('carrito', JSON.stringify(carrito));
                guardarCarritoCookie(carrito);
                cargarCarritoCliente();
                actualizarContadorNavbar(carrito);
            };

            window.eliminarProductoLocal = function (id) {
                var carrito = (JSON.parse(localStorage.getItem('carrito')) || []).filter(function (x) { return x !== id; });
                localStorage.setItem('carrito', JSON.stringify(carrito));
                guardarCarritoCookie(carrito);
                cargarCarritoCliente();
                actualizarContadorNavbar(carrito);
            };

            function actualizarResumenPrecios(subtotal) {
                var subtotalSpan = el('<%= lblSubtotal.ClientID %>');
                var totalSpan    = el('<%= lblTotal.ClientID %>');
                if (subtotalSpan) subtotalSpan.innerText = '$' + subtotal.toLocaleString('es-AR', { minimumFractionDigits: 2 });
                if (totalSpan)    totalSpan.innerText    = '$' + (subtotal + costoEnvio).toLocaleString('es-AR', { minimumFractionDigits: 2 });
            }

            var sigCanvas   = el('signature-canvas');
            var btnClearSig = el('btnClearSignature');
            var lblSigError = el('lblSignatureError');
            var hasSigned   = false;

            if (sigCanvas) {
                var sigCtx  = sigCanvas.getContext('2d');
                var drawing = false;

                function initCtx() {
                    sigCtx.strokeStyle = '#7c3aed';
                    sigCtx.lineWidth   = 3;
                    sigCtx.lineCap     = 'round';
                    sigCtx.lineJoin    = 'round';
                }

                function resizeSigCanvas() {
                    var tmp = document.createElement('canvas');
                    tmp.width = sigCanvas.width; tmp.height = sigCanvas.height;
                    tmp.getContext('2d').drawImage(sigCanvas, 0, 0);
                    sigCanvas.width = sigCanvas.offsetWidth; sigCanvas.height = sigCanvas.offsetHeight;
                    sigCtx.drawImage(tmp, 0, 0); initCtx();
                }

                window.addEventListener('resize', resizeSigCanvas);
                sigCanvas.width = sigCanvas.offsetWidth; sigCanvas.height = sigCanvas.offsetHeight;
                initCtx();

                function getPos(c, e)  { var r = c.getBoundingClientRect(); return { x: e.clientX - r.left, y: e.clientY - r.top }; }
                function getTPos(c, t) { var r = c.getBoundingClientRect(); return { x: t.clientX - r.left, y: t.clientY - r.top }; }

                sigCanvas.addEventListener('mousedown', function (e) { drawing = true; sigCtx.beginPath(); var p = getPos(sigCanvas, e); sigCtx.moveTo(p.x, p.y); });
                sigCanvas.addEventListener('mousemove', function (e) {
                    if (!drawing) return;
                    var p = getPos(sigCanvas, e); sigCtx.lineTo(p.x, p.y); sigCtx.stroke();
                    hasSigned = true; if (lblSigError) lblSigError.style.display = 'none';
                });
                window.addEventListener('mouseup', function () { drawing = false; });

                sigCanvas.addEventListener('touchstart', function (e) { drawing = true; sigCtx.beginPath(); var p = getTPos(sigCanvas, e.touches[0]); sigCtx.moveTo(p.x, p.y); e.preventDefault(); }, { passive: false });
                sigCanvas.addEventListener('touchmove', function (e) {
                    if (!drawing) return;
                    var p = getTPos(sigCanvas, e.touches[0]); sigCtx.lineTo(p.x, p.y); sigCtx.stroke();
                    hasSigned = true; if (lblSigError) lblSigError.style.display = 'none'; e.preventDefault();
                }, { passive: false });
                sigCanvas.addEventListener('touchend', function () { drawing = false; });

                if (btnClearSig) btnClearSig.addEventListener('click', function () {
                    sigCtx.clearRect(0, 0, sigCanvas.width, sigCanvas.height);
                    hasSigned = false; initCtx();
                    if (lblSigError) lblSigError.style.display = 'none';
                });
            }

            window.validarFirmaDigital = function () {
                if (!hasSigned) {
                    if (lblSigError) lblSigError.style.display = 'block';
                    sigCanvas.parentElement.style.borderColor = '#ef4444';
                    setTimeout(function () { sigCanvas.parentElement.style.borderColor = '#ddd0f5'; }, 1000);
                    return false;
                }
                if (!usuarioLogueado) localStorage.removeItem('carrito');
                return true;
            };
        });
    </script>
</asp:Content>
