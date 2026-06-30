<%@ Page Title="Mi Carrito" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Carrito.aspx.cs" Inherits="Carrito" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div style="background: #f0ebfa; min-height: 100vh; padding: 40px 0; position: relative;">
        <div class="g-grid"></div>

        <div class="container" style="position: relative; z-index: 2; max-width: 1200px; margin: 0 auto;">
            
            <!-- Encabezado de la Página -->
            <div style="text-align: center; margin-bottom: 40px;">
                <span class="g-pretag">Resumen de Selección</span>
                <h1 class="store-title" style="font-size: 36px; margin: 5px 0;">Mi <span>Carrito</span></h1>
                <p class="store-subtitle" style="font-size: 11px; letter-spacing: 2px;">Revisa tus componentes antes de confirmar la compra</p>
                <div class="g-divider" style="margin: 15px auto 0; width: 60px;"></div>
            </div>

            <div class="row" style="display: flex; flex-wrap: wrap; gap: 30px;">
                
                <!-- Columna Izquierda: Detalle de Productos -->
                <div style="flex: 1 1 700px; display: flex; flex-direction: column; gap: 20px;">
                    
                    <!-- Mensaje para no logueados -->
                    <div id="pnlSugerenciaLogin" class="g-card" style="display: none; background: linear-gradient(135deg, #faf7ff, #f3ebfa); border-color: #ddd0f5; padding: 15px 20px; align-items: center; gap: 15px;">
                        <span style="font-size: 20px;">💡</span>
                        <div style="flex: 1;">
                            <h4 style="font-family: 'Exo 2', sans-serif; font-size: 13px; font-weight: 600; color: #4c1d95; margin: 0;">Inicia sesión para guardar tu carrito</h4>
                            <p style="font-family: 'Exo 2', sans-serif; font-size: 11px; color: #7c5ea8; margin: 2px 0 0 0;">Guarda tus componentes permanentemente en tu cuenta y accede a envíos gratis.</p>
                        </div>
                        <a href="Login.aspx" class="g-btn" style="text-decoration: none; padding: 6px 15px; font-size: 10px; margin: 0; width: auto !important;">Iniciar Sesión</a>
                    </div>

                    <!-- Panel para usuarios logueados (Server-Side) -->
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

                                    <!-- Selector de Cantidad -->
                                    <div style="display: flex; align-items: center; gap: 5px;">
                                        <asp:LinkButton ID="btnRestar" runat="server" CommandName="Restar" CommandArgument='<%# Eval("IdProducto") %>' 
                                             style="width: 28px; height: 28px; display: flex; align-items: center; justify-content: center; background: #faf7ff; border: 1px solid #ddd0f5; color: #7c3aed; font-weight: bold; text-decoration: none; cursor: pointer;">-</asp:LinkButton>
                                        <span style="font-family: 'Orbitron', sans-serif; font-weight: 900; width: 30px; text-align: center; color: #1e0a3c;"><%# Eval("Stock") %></span>
                                        <asp:LinkButton ID="btnSumar" runat="server" CommandName="Sumar" CommandArgument='<%# Eval("IdProducto") %>'
                                             style="width: 28px; height: 28px; display: flex; align-items: center; justify-content: center; background: #faf7ff; border: 1px solid #ddd0f5; color: #7c3aed; font-weight: bold; text-decoration: none; cursor: pointer;">+</asp:LinkButton>
                                    </div>

                                    <!-- Precio -->
                                    <div style="text-align: right; min-width: 120px;">
                                        <div style="font-family: 'Exo 2', sans-serif; font-size: 11px; color: #9f7cc0;">Precio Unitario: $<%# Eval("Precio", "{0:N2}") %></div>
                                        <div style="font-family: 'Orbitron', sans-serif; font-size: 18px; font-weight: 900; color: #7c3aed;">
                                            $<%# Convert.ToDecimal(Eval("Precio")) * Convert.ToInt32(Eval("Stock")) %>
                                        </div>
                                    </div>

                                    <!-- Quitar item -->
                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("IdProducto") %>' 
                                         style="color: #ef4444; background: none; border: none; cursor: pointer; padding: 5px; font-size: 16px; text-decoration: none;">✕</asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:PlaceHolder>

                    <!-- Panel para usuarios deslogueados (Client-Side HTML Container) -->
                    <div id="carritoClienteContainer" style="display: none; flex-direction: column; gap: 20px;">
                        <!-- Se poblará por JS leyendo local storage -->
                    </div>

                    <!-- Carrito Vacío -->
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

                <!-- Columna Derecha: Resumen de Pago -->
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

                        <!-- Info de distancia -->
                        <div id="divDistanciaInfo" style="display: none; text-align: right; margin-bottom: 12px;">
                            <span style="font-family: 'Exo 2', sans-serif; font-size: 11px; color: #9f7cc0;">📍 <span id="spanDistancia"></span></span>
                        </div>
                        <div id="divGeoError" style="display: none; text-align: right; margin-bottom: 12px;">
                            <span style="font-family: 'Exo 2', sans-serif; font-size: 11px; color: #ef4444;">⚠️ Ubicación no disponible</span>
                        </div>

                        <div style="margin: 20px 0; border-top: 1.5px dashed #ede5f8; padding-top: 15px; display: flex; justify-content: space-between; align-items: center;">
                            <span style="font-family: 'Orbitron', sans-serif; font-weight: 900; color: #1e0a3c; font-size: 16px;">Total</span>
                            <span id="lblTotal" runat="server" style="font-family: 'Orbitron', sans-serif; font-weight: 900; color: #7c3aed; font-size: 24px;">$0,00</span>
                        </div>

                        <!-- Panel de Firma Digital -->
                        <div id="pnlSignature" style="margin-bottom: 20px;">
                            <label style="font-family: 'Exo 2', sans-serif; font-size: 11px; font-weight: 600; color: #7c5ea8; display: block; margin-bottom: 8px;">Firme aquí para autorizar el despacho de hardware:</label>
                            <div style="border: 1px solid #ddd0f5; background: #faf7ff; border-radius: 4px; position: relative;">
                                <canvas id="signature-canvas" style="display: block; width: 100%; height: 110px; cursor: crosshair; touch-action: none; background: #faf7ff;"></canvas>
                                <button type="button" id="btnClearSignature" style="position: absolute; bottom: 5px; right: 5px; background: rgba(239, 68, 68, 0.1); border: 1px solid rgba(239, 68, 68, 0.3); color: #ef4444; font-family: 'Exo 2', sans-serif; font-size: 10px; padding: 2px 8px; cursor: pointer; border-radius: 3px;">Borrar</button>
                            </div>
                            <span id="lblSignatureError" style="font-family: 'Exo 2', sans-serif; font-size: 10px; color: #ef4444; display: none; margin-top: 5px;">⚠️ Se requiere firma de conformidad para proceder.</span>
                        </div>

                        <!-- Botón de Proceder -->
                        <asp:Button ID="btnCheckout" runat="server" Text="Finalizar Compra" CssClass="g-btn" OnClick="btnCheckout_Click" OnClientClick="return validarFirmaDigital();"
                            style="width: 100% !important; padding: 14px 0 !important; font-size: 12px !important; margin-top: 10px !important;" />
                        
                        <a href="Default.aspx" style="display: block; text-align: center; margin-top: 15px; font-family: 'Exo 2', sans-serif; font-size: 12px; color: #7c3aed; text-decoration: none; font-weight: 600;">← Seguir Comprando</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        // =============================================
        // GEOLOCALIZACIÓN Y CÁLCULO DE ENVÍO
        // =============================================
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
            // Progresivo: $5000 base a 10 km, crece con potencia 0.6
            var costo = 5000 * Math.pow(distanciaKm / FREE_SHIPPING_KM, 0.6);
            // Redondear a los $500 más cercanos
            return Math.round(costo / 500) * 500;
        }

        function parsearPrecio(texto) {
            var s = (texto || '').replace(/\$/g, '').trim();
            // Detectar formato: si la última coma aparece después del último punto -> es-AR
            var ultimoPunto = s.lastIndexOf('.');
            var ultimaComa = s.lastIndexOf(',');
            if (ultimaComa > ultimoPunto) {
                s = s.replace(/\./g, '').replace(',', '.');
            } else {
                s = s.replace(/,/g, '');
            }
            return parseFloat(s) || 0;
        }

        function actualizarEnvioEnUI(distanciaKm, costo) {
            var envioEl = document.getElementById('<%= lblEnvio.ClientID %>');
            var totalEl = document.getElementById('<%= lblTotal.ClientID %>');
            var subtotalEl = document.getElementById('<%= lblSubtotal.ClientID %>');
            var divDistancia = document.getElementById('divDistanciaInfo');
            var spanDistancia = document.getElementById('spanDistancia');
            var divGeoError = document.getElementById('divGeoError');

            if (divGeoError) divGeoError.style.display = 'none';

            if (envioEl) {
                if (costo === 0) {
                    envioEl.innerText = 'Bonificado';
                    envioEl.style.color = '#10b981';
                } else {
                    envioEl.innerText = '$' + costo.toLocaleString('es-AR', { minimumFractionDigits: 2 });
                    envioEl.style.color = '#ef4444';
                }
            }

            var subtotal = subtotalEl ? parsearPrecio(subtotalEl.innerText) : 0;
            var total = subtotal + costo;
            if (totalEl) {
                totalEl.innerText = '$' + total.toLocaleString('es-AR', { minimumFractionDigits: 2 });
            }

            if (spanDistancia && divDistancia) {
                spanDistancia.innerText = distanciaKm.toFixed(1) + ' km del negocio';
                divDistancia.style.display = 'block';
            }
        }

        function mostrarErrorGeo() {
            var envioEl = document.getElementById('<%= lblEnvio.ClientID %>');
            var divGeoError = document.getElementById('divGeoError');
            var divDistancia = document.getElementById('divDistanciaInfo');

            // Sin ubicación: cobrar envío base por precaución
            costoEnvio = 0;
            if (envioEl) {
                envioEl.innerText = 'No disponible';
                envioEl.style.color = '#9f7cc0';
            }
            if (divGeoError) divGeoError.style.display = 'block';
            if (divDistancia) divDistancia.style.display = 'none';
        }

        function iniciarGeolocalizacion() {
            var envioEl = document.getElementById('<%= lblEnvio.ClientID %>');
            if (envioEl) {
                envioEl.innerText = 'Calculando...';
                envioEl.style.color = '#9f7cc0';
            }

            if (!navigator.geolocation) {
                mostrarErrorGeo();
                return;
            }

            navigator.geolocation.getCurrentPosition(
                function (pos) {
                    var distancia = haversineKm(pos.coords.latitude, pos.coords.longitude, STORE_LAT, STORE_LNG);
                    costoEnvio = calcularCostoEnvio(distancia);
                    actualizarEnvioEnUI(distancia, costoEnvio);
                },
                function () {
                    mostrarErrorGeo();
                },
                { timeout: 10000, maximumAge: 300000 }
            );
        }

        // =============================================
        // LÓGICA DEL CARRITO
        // =============================================
        document.addEventListener('DOMContentLoaded', function () {
            var logueado = typeof usuarioLogueado !== 'undefined' ? usuarioLogueado : false;

            // Iniciar geolocalización siempre (para ambos tipos de usuario)
            iniciarGeolocalizacion();

            if (!logueado) {
                // Mostrar panel de login recomendado
                var sugerencia = document.getElementById('pnlSugerenciaLogin');
                if (sugerencia) sugerencia.style.display = 'flex';
                cargarCarritoCliente();
            }

            function cargarCarritoCliente() {
                var carrito = JSON.parse(localStorage.getItem('carrito')) || [];
                var container = document.getElementById('carritoClienteContainer');
                var emptyDiv = document.getElementById('<%= divCarritoVacio.ClientID %>');
                var checkoutBtn = document.getElementById('<%= btnCheckout.ClientID %>');

                if (carrito.length === 0) {
                    container.style.display = 'none';
                    if (emptyDiv) emptyDiv.style.display = 'block';
                    if (checkoutBtn) checkoutBtn.disabled = true;
                    actualizarResumenPrecios(0);
                    return;
                }

                if (emptyDiv) emptyDiv.style.display = 'none';
                container.style.display = 'flex';
                if (checkoutBtn) checkoutBtn.disabled = false;

                console.log("Enviando IDs al WebMethod:", carrito);
                fetch('<%= ResolveUrl("~/Carrito.aspx/ObtenerDetallesCarritoLocal") %>', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    body: JSON.stringify({ productoIds: carrito })
                })
                .then(response => {
                    console.log("Estado de la respuesta HTTP:", response.status);
                    if (!response.ok) throw new Error("HTTP error " + response.status);
                    return response.json();
                })
                .then(data => {
                    console.log("Respuesta JSON recibida:", data);
                    var productos = data.d;
                    if (!productos || productos.length === 0) {
                        container.innerHTML = `<div style="background:#fee2e2; border:1px solid #fca5a5; padding:15px; color:#991b1b; font-family:'Exo 2', sans-serif; font-size:12px;">
                            <strong>Depuración Servidor:</strong> El backend devolvió un array vacío de productos para los IDs locales: ${JSON.stringify(carrito)}.
                        </div>`;
                        return;
                    }
                    renderizarCarritoCliente(productos);
                })
                .catch(err => {
                    console.error("Error en la petición de detalles de carrito local:", err);
                    container.innerHTML = `<div style="background:#fee2e2; border:1px solid #fca5a5; padding:15px; color:#991b1b; font-family:'Exo 2', sans-serif; font-size:12px;">
                            <strong>Depuración JS Error:</strong> Falló la llamada Fetch. Detalle: ${err.message}
                        </div>`;
                });
            }

            function renderizarCarritoCliente(productos) {
                var container = document.getElementById('carritoClienteContainer');
                container.innerHTML = '';
                var subtotal = 0;

                productos.forEach(function (p) {
                    var itemSubtotal = p.Precio * p.Stock;
                    subtotal += itemSubtotal;

                    var html = `
                        <div class="g-card" style="display: flex; align-items: center; padding: 15px 20px; gap: 20px; flex-wrap: wrap;">
                            <img src="${p.ImagenUrl.replace('~/', '')}" alt="${p.Nombre}" 
                                 style="width: 80px; height: 80px; object-fit: cover; border-radius: 4px; border: 1px solid #ede5f8;"
                                 onerror="this.onerror=null;this.src='https://placehold.co/80x80/faf7ff/7c3aed?text=Hardware';" />
                            
                            <div style="flex: 1; min-width: 200px;">
                                <span style="font-family: 'Exo 2', sans-serif; font-size: 10px; font-weight: 600; color: #9f7cc0; text-transform: uppercase;">${p.Categoria}</span>
                                <h3 style="font-family: 'Exo 2', sans-serif; font-size: 16px; font-weight: 600; color: #1e0a3c; margin: 2px 0 0 0;">${p.Nombre}</h3>
                            </div>

                            <div style="display: flex; align-items: center; gap: 5px;">
                                <button type="button" onclick="modificarCantidadLocal(${p.IdProducto}, -1)" 
                                     style="width: 28px; height: 28px; display: flex; align-items: center; justify-content: center; background: #faf7ff; border: 1px solid #ddd0f5; color: #7c3aed; font-weight: bold; cursor: pointer;">-</button>
                                <span style="font-family: 'Orbitron', sans-serif; font-weight: 900; width: 30px; text-align: center; color: #1e0a3c;">${p.Stock}</span>
                                <button type="button" onclick="modificarCantidadLocal(${p.IdProducto}, 1)"
                                     style="width: 28px; height: 28px; display: flex; align-items: center; justify-content: center; background: #faf7ff; border: 1px solid #ddd0f5; color: #7c3aed; font-weight: bold; cursor: pointer;">+</button>
                            </div>

                            <div style="text-align: right; min-width: 120px;">
                                <div style="font-family: 'Exo 2', sans-serif; font-size: 11px; color: #9f7cc0;">Unitario: $${p.Precio.toLocaleString('es-AR', {minimumFractionDigits: 2})}</div>
                                <div style="font-family: 'Orbitron', sans-serif; font-size: 18px; font-weight: 900; color: #7c3aed;">
                                    $${itemSubtotal.toLocaleString('es-AR', {minimumFractionDigits: 2})}
                                </div>
                            </div>

                            <button type="button" onclick="eliminarProductoLocal(${p.IdProducto})" 
                                 style="color: #ef4444; background: none; border: none; cursor: pointer; padding: 5px; font-size: 16px;">✕</button>
                        </div>
                    `;
                    container.innerHTML += html;
                });

                actualizarResumenPrecios(subtotal);
            }

            window.modificarCantidadLocal = function(id, cambio) {
                var carrito = JSON.parse(localStorage.getItem('carrito')) || [];
                if (cambio === 1) {
                    carrito.push(id);
                } else if (cambio === -1) {
                    var index = carrito.indexOf(id);
                    if (index > -1) carrito.splice(index, 1);
                }
                localStorage.setItem('carrito', JSON.stringify(carrito));
                cargarCarritoCliente();
                var lblContador = document.getElementById('lblCarritoContador') || document.querySelector('[id*="lblCarritoContador"]');
                if (lblContador) lblContador.innerText = carrito.length.toString();
            };

            window.eliminarProductoLocal = function(id) {
                var carrito = JSON.parse(localStorage.getItem('carrito')) || [];
                carrito = carrito.filter(x => x !== id);
                localStorage.setItem('carrito', JSON.stringify(carrito));
                cargarCarritoCliente();
                var lblContador = document.getElementById('lblCarritoContador') || document.querySelector('[id*="lblCarritoContador"]');
                if (lblContador) lblContador.innerText = carrito.length.toString();
            };

            function actualizarResumenPrecios(subtotal) {
                var subtotalSpan = document.getElementById('<%= lblSubtotal.ClientID %>');
                var totalSpan = document.getElementById('<%= lblTotal.ClientID %>');
                if (subtotalSpan) subtotalSpan.innerText = '$' + subtotal.toLocaleString('es-AR', {minimumFractionDigits: 2});
                var total = subtotal + costoEnvio;
                if (totalSpan) totalSpan.innerText = '$' + total.toLocaleString('es-AR', {minimumFractionDigits: 2});
            }

            // --- Lógica del Canvas de Firma Digital ---
            var sigCanvas = document.getElementById('signature-canvas');
            var btnClearSig = document.getElementById('btnClearSignature');
            var lblSigError = document.getElementById('lblSignatureError');
            var hasSigned = false;

            if (sigCanvas) {
                var sigCtx = sigCanvas.getContext('2d');
                var drawing = false;

                function resizeSigCanvas() {
                    var tempCanvas = document.createElement('canvas');
                    tempCanvas.width = sigCanvas.width;
                    tempCanvas.height = sigCanvas.height;
                    var tempCtx = tempCanvas.getContext('2d');
                    tempCtx.drawImage(sigCanvas, 0, 0);
                    sigCanvas.width = sigCanvas.offsetWidth;
                    sigCanvas.height = sigCanvas.offsetHeight;
                    sigCtx.drawImage(tempCanvas, 0, 0);
                    sigCtx.strokeStyle = '#7c3aed';
                    sigCtx.lineWidth = 3;
                    sigCtx.lineCap = 'round';
                    sigCtx.lineJoin = 'round';
                }
                window.addEventListener('resize', resizeSigCanvas);
                sigCanvas.width = sigCanvas.offsetWidth;
                sigCanvas.height = sigCanvas.offsetHeight;
                sigCtx.strokeStyle = '#7c3aed';
                sigCtx.lineWidth = 3;
                sigCtx.lineCap = 'round';
                sigCtx.lineJoin = 'round';

                sigCanvas.addEventListener('mousedown', function (e) {
                    drawing = true;
                    sigCtx.beginPath();
                    var pos = getMousePos(sigCanvas, e);
                    sigCtx.moveTo(pos.x, pos.y);
                });
                sigCanvas.addEventListener('mousemove', function (e) {
                    if (!drawing) return;
                    var pos = getMousePos(sigCanvas, e);
                    sigCtx.lineTo(pos.x, pos.y);
                    sigCtx.stroke();
                    hasSigned = true;
                    if (lblSigError) lblSigError.style.display = 'none';
                });
                window.addEventListener('mouseup', function () { drawing = false; });

                sigCanvas.addEventListener('touchstart', function (e) {
                    drawing = true;
                    sigCtx.beginPath();
                    var pos = getTouchPos(sigCanvas, e.touches[0]);
                    sigCtx.moveTo(pos.x, pos.y);
                    e.preventDefault();
                }, { passive: false });
                sigCanvas.addEventListener('touchmove', function (e) {
                    if (!drawing) return;
                    var pos = getTouchPos(sigCanvas, e.touches[0]);
                    sigCtx.lineTo(pos.x, pos.y);
                    sigCtx.stroke();
                    hasSigned = true;
                    if (lblSigError) lblSigError.style.display = 'none';
                    e.preventDefault();
                }, { passive: false });
                sigCanvas.addEventListener('touchend', function () { drawing = false; });

                function getMousePos(canvasEl, evt) {
                    var rect = canvasEl.getBoundingClientRect();
                    return { x: evt.clientX - rect.left, y: evt.clientY - rect.top };
                }
                function getTouchPos(canvasEl, touchEl) {
                    var rect = canvasEl.getBoundingClientRect();
                    return { x: touchEl.clientX - rect.left, y: touchEl.clientY - rect.top };
                }

                if (btnClearSig) {
                    btnClearSig.addEventListener('click', function () {
                        sigCtx.clearRect(0, 0, sigCanvas.width, sigCanvas.height);
                        hasSigned = false;
                        sigCtx.strokeStyle = '#7c3aed';
                        sigCtx.lineWidth = 3;
                        sigCtx.lineCap = 'round';
                        sigCtx.lineJoin = 'round';
                        if (lblSigError) lblSigError.style.display = 'none';
                    });
                }
            }

            window.validarFirmaDigital = function () {
                var sigError = document.getElementById('lblSignatureError');
                if (!hasSigned) {
                    if (sigError) sigError.style.display = 'block';
                    sigCanvas.parentElement.style.borderColor = '#ef4444';
                    setTimeout(function () { sigCanvas.parentElement.style.borderColor = '#ddd0f5'; }, 1000);
                    return false;
                }
                var logueado = typeof usuarioLogueado !== 'undefined' ? usuarioLogueado : false;
                if (!logueado) localStorage.removeItem('carrito');
                return true;
            };
        });
    </script>
</asp:Content>
