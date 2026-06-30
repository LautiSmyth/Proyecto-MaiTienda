<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs"
    ResponseEncoding="utf-8" Inherits="_Default" %>

    <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


        <div style="background: #f0ebfa; min-height: 100vh; position: relative;">
            <div class="g-grid"></div>

            <!-- Cabecera de la tienda (Hero Panel) -->
            <div class="store-hero-container">
                <div class="g-card store-hero-card">
                    <span class="g-pretag">Estación de Componentes</span>
                    <h1 class="store-title">Mai<span>Tienda</span></h1>
                    <p class="store-subtitle">Hardware Vanguardista & Alto Rendimiento</p>
                    <div class="g-divider"></div>
                    <p class="store-hero-desc">
                        Explora nuestra selección exclusiva de procesadores, tarjetas gráficas de última generación,
                        memorias ultra veloces y almacenamiento de estado sólido para llevar tu setup al siguiente nivel
                        de rendimiento.
                    </p>
                </div>
            </div>

            <!-- Contenedor del Layout (Sidebar + Catálogo) -->
            <div class="store-layout-container">
                <!-- Botón para abrir filtros en móviles/tablets -->
                <button type="button" class="g-btn-toggle-filters" onclick="toggleSidebar(true)">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                        <line x1="4" y1="21" x2="4" y2="14"></line><line x1="4" y1="10" x2="4" y2="3"></line>
                        <line x1="12" y1="21" x2="12" y2="12"></line><line x1="12" y1="8" x2="12" y2="3"></line>
                        <line x1="20" y1="21" x2="20" y2="16"></line><line x1="20" y1="12" x2="20" y2="3"></line>
                        <line x1="1" y1="14" x2="7" y2="14"></line><line x1="9" y1="8" x2="15" y2="8"></line>
                        <line x1="17" y1="16" x2="23" y2="16"></line>
                    </svg>
                    <span>Filtros</span>
                </button>

                <!-- Fondo oscuro para desenfoque cuando se abre el sidebar -->
                <div id="sidebarOverlay" class="sidebar-overlay" onclick="toggleSidebar(false)"></div>

                <!-- Columna Izquierda: Buscador y Filtros -->
                <div class="store-sidebar">
                    <!-- Cabecera del Sidebar para móviles -->
                    <div class="sidebar-mobile-header">
                        <span>Filtros</span>
                        <button type="button" class="btn-close-sidebar" onclick="toggleSidebar(false)">✕</button>
                    </div>

                    <div class="g-filter-card" style="padding: 20px; margin-bottom: 0;">
                        <span class="g-section-title" style="margin-bottom: 12px;">Búsqueda</span>
                        <div class="search-box-container">
                            <asp:TextBox ID="txtBuscar" runat="server" CssClass="g-input"
                                placeholder="Buscar componentes..." OnTextChanged="txtBuscar_TextChanged"
                                AutoPostBack="true"></asp:TextBox>
                            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="g-btn g-btn-search"
                                OnClick="btnBuscar_Click" />
                        </div>


                        <!-- Sección de Categorías Colapsable -->
                        <div class="category-dropdown-container" style="margin-top: 15px;">
                            <div class="category-dropdown-header" onclick="toggleCategoryCollapse()" style="display: flex; justify-content: space-between; align-items: center; cursor: pointer; padding: 10px 0; border-bottom: 1.5px solid #ddd0f5; margin-bottom: 10px;">
                                <span class="g-section-title" style="margin: 0;">Categorías</span>
                                <span id="categoryArrow" style="transition: transform 0.3s ease; transform: rotate(-90deg); color: #7c3aed; font-weight: bold; font-family: 'Orbitron', sans-serif; font-size: 11px;">▼</span>
                            </div>
                            
                            <div id="categoryCollapseContent" style="max-height: 0px; overflow-y: auto; transition: max-height 0.4s ease, opacity 0.3s ease; opacity: 0; display: flex; flex-direction: column; gap: 8px; padding-right: 5px;">
                                <asp:Panel ID="pnlFiltros" runat="server" CssClass="filter-container">
                                    <asp:LinkButton ID="btnCatTodos" runat="server" Text="Todos" CommandArgument="Todos"
                                        OnClick="Categoria_Click" CssClass="g-btn-filter active" style="width: 100%; text-align: left; text-decoration: none; display: block;" />
                                    
                                    <asp:Repeater ID="repCategorias" runat="server" OnItemCommand="repCategorias_ItemCommand">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnCatDinamico" runat="server" 
                                                Text='<%# Container.DataItem %>' 
                                                CommandName="FiltrarCategoria"
                                                CommandArgument='<%# Container.DataItem %>'
                                                CssClass='<%# Session["CategoriaSeleccionada"] as string == Container.DataItem.ToString() ? "g-btn-filter active" : "g-btn-filter" %>'
                                                style="width: 100%; text-align: left; text-decoration: none; display: block; margin-top: 5px;" />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </asp:Panel>
                            </div>
                        </div>


                        <span class="g-section-title" style="margin-top: 15px; margin-bottom: 12px;">Ordenar por</span>
                        <asp:DropDownList ID="ddlOrden" runat="server" CssClass="g-input" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlOrden_SelectedIndexChanged"
                            style="width: 100%; margin-bottom: 15px; border-radius: 0;">
                            <asp:ListItem Value="None">Sin Ordenar</asp:ListItem>
                            <asp:ListItem Value="PrecioAsc">Precio: Menor a Mayor</asp:ListItem>
                            <asp:ListItem Value="PrecioDesc">Precio: Mayor a Menor</asp:ListItem>
                        </asp:DropDownList>

                        <span class="g-section-title" style="margin-top: 15px; margin-bottom: 12px;">Rango de
                            Precios</span>
                        <div style="display: flex; gap: 8px; margin-bottom: 10px;">
                            <asp:TextBox ID="txtPrecioMin" runat="server" CssClass="g-input" placeholder="Min"
                                type="number" min="0"></asp:TextBox>
                            <asp:TextBox ID="txtPrecioMax" runat="server" CssClass="g-input" placeholder="Max"
                                type="number" min="0"></asp:TextBox>
                        </div>
                        <asp:Button ID="btnAplicarPrecio" runat="server" Text="Filtrar Precio" CssClass="g-btn"
                            OnClick="btnAplicarPrecio_Click"
                            style="padding: 8px 0 !important; font-size: 10px !important; margin-top: 0 !important; margin-bottom: 15px;" />

                        <span class="g-section-title" style="margin-top: 15px; margin-bottom: 12px;">Filtrar por
                            Marca</span>
                        <asp:DropDownList ID="ddlMarca" runat="server" CssClass="g-input" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlMarca_SelectedIndexChanged"
                            style="width: 100%; margin-bottom: 20px; border-radius: 0;">
                            <asp:ListItem Value="Todas">Todas las Marcas</asp:ListItem>
                            <asp:ListItem Value="AMD">AMD</asp:ListItem>
                            <asp:ListItem Value="Intel">Intel</asp:ListItem>
                            <asp:ListItem Value="NVIDIA">NVIDIA</asp:ListItem>
                            <asp:ListItem Value="ASUS">ASUS</asp:ListItem>
                            <asp:ListItem Value="Corsair">Corsair</asp:ListItem>
                            <asp:ListItem Value="Samsung">Samsung</asp:ListItem>
                            <asp:ListItem Value="Kingston">Kingston</asp:ListItem>
                            <asp:ListItem Value="MSI">MSI</asp:ListItem>
                            <asp:ListItem Value="Gigabyte">Gigabyte</asp:ListItem>
                            <asp:ListItem Value="Western Digital">Western Digital</asp:ListItem>
                        </asp:DropDownList>
                        
                        <asp:Button ID="btnLimpiarFiltros" runat="server" Text="Limpiar Filtros" CssClass="g-btn" OnClick="btnLimpiarFiltros_Click" style="background: #ef4444 !important; margin-top: 10px !important; font-size: 11px !important; padding: 10px 0 !important;" />
                    </div>
                </div>

                <!-- Columna Derecha: Catálogo de productos -->
                <div class="store-content">
                <asp:UpdatePanel ID="upCatalogo" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <!-- Paginador Superior -->
                    <asp:Panel ID="pnlPaginadorSup" runat="server" CssClass="store-pager"
                        style="margin-top: 0; margin-bottom: 25px;">
                        <asp:LinkButton ID="btnAntSup" runat="server" OnClick="btnAnterior_Click" CssClass="g-btn-page">
                            < Anterior</asp:LinkButton>
                                <asp:Repeater ID="repPaginasSup" runat="server" OnItemCommand="repPaginas_ItemCommand">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnPaginaSup" runat="server"
                                            CommandArgument='<%# Container.DataItem %>'
                                            CssClass='<%# Convert.ToInt32(Container.DataItem) == PaginaActual ? "g-btn-page active" : "g-btn-page" %>'>
                                            <%# Convert.ToInt32(Container.DataItem) + 1 %>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <asp:LinkButton ID="btnSigSup" runat="server" OnClick="btnSiguiente_Click"
                                    CssClass="g-btn-page">Siguiente ></asp:LinkButton>
                    </asp:Panel>

                    <div class="products-grid" style="margin: 0; padding: 0;">
                        <asp:Label ID="lblSinResultados" runat="server" Visible="false" CssClass="no-results">
                            <div class="no-results-title">Sin Componentes</div>
                            <div class="no-results-text">No se encontraron componentes que coincidan con la búsqueda.
                            </div>
                        </asp:Label>

                        <asp:Repeater ID="repProductos" runat="server">
                            <ItemTemplate>
                                <div class='<%# "g-card product-card" + (IsEnCarrito(Eval("IdProducto")) ? " en-carrito" : "") %>'>
                                    <div class="product-image-area">
                                        <span class="product-badge"><%# Eval("Categoria") %></span>
                                        <a href="Carrito.aspx" class='<%# IsEnCarrito(Eval("IdProducto")) ? "en-carrito-ribbon" : "en-carrito-ribbon" %>' style='<%# IsEnCarrito(Eval("IdProducto")) ? "pointer-events:auto;text-decoration:none;" : "pointer-events:none;" %>'><span>&#10003; En carrito</span></a>
                                        <img class="product-img" src='<%# ResolveUrl(Eval("ImagenUrl").ToString()) %>'
                                            alt='<%# Eval("Nombre") %>'
                                            onerror="this.onerror=null;this.src='https://placehold.co/300x160/faf7ff/7c3aed?text=Hardware';" />
                                    </div>
                                    <div class="product-info">
                                        <span class="product-category"><%# Eval("Stock") %> unidades en stock</span>
                                        <h3 class="product-name"><%# Eval("Nombre") %></h3>
                                        <p class="product-desc"><%# Eval("Descripcion") %></p>
                                        <div class="product-footer">
                                            <span class="product-price"><%# Eval("Precio", "{0:N2}") %></span>
                                            <%-- No en carrito --%>
                                            <asp:Panel runat="server" Visible='<%# !IsEnCarrito(Eval("IdProducto")) %>'>
                                                <asp:LinkButton ID="btnAgregar" runat="server" CommandName="Agregar"
                                                    CommandArgument='<%# string.Format("{0}|{1}", Eval("IdProducto"), Eval("Stock")) %>'
                                                    data-id='<%# Eval("IdProducto") %>'
                                                    OnCommand="AgregarAlCarrito_Click"
                                                    Enabled='<%# (int)Eval("Stock") > 0 %>'
                                                    CssClass='<%# (int)Eval("Stock") > 0 ? "g-btn g-btn-buy" : "g-btn g-btn-buy btn-sin-stock" %>'>
                                                    <span class="btn-plus">+</span> <span class="btn-label">Agregar</span>
                                                </asp:LinkButton>
                                            </asp:Panel>
                                            <%-- En carrito: controles qty (logueado y anónimo) --%>
                                            <asp:Panel runat="server" Visible='<%# IsEnCarrito(Eval("IdProducto")) %>' CssClass="qty-ctrl">
                                                <asp:LinkButton ID="btnMenos" runat="server" CommandName="Decrementar"
                                                    CommandArgument='<%# string.Format("{0}|{1}", Eval("IdProducto"), Eval("Stock")) %>'
                                                    OnCommand="CambiarCantidad_Click" CssClass="qty-btn">&#8722;</asp:LinkButton>
                                                <span class="qty-val"><%# GetCantidadEnCarrito(Eval("IdProducto")) %></span>
                                                <asp:LinkButton ID="btnMas" runat="server" CommandName="Incrementar"
                                                    CommandArgument='<%# string.Format("{0}|{1}", Eval("IdProducto"), Eval("Stock")) %>'
                                                    OnCommand="CambiarCantidad_Click"
                                                    Enabled='<%# GetCantidadEnCarrito(Eval("IdProducto")) < (int)Eval("Stock") %>'
                                                    CssClass="qty-btn">+</asp:LinkButton>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <!-- Paginador Inferior -->
                    <asp:Panel ID="pnlPaginadorInf" runat="server" CssClass="store-pager"
                        style="margin-top: 30px; margin-bottom: 0;">
                        <asp:LinkButton ID="btnAntInf" runat="server" OnClick="btnAnterior_Click" CssClass="g-btn-page">
                            < Anterior</asp:LinkButton>
                                <asp:Repeater ID="repPaginasInf" runat="server" OnItemCommand="repPaginas_ItemCommand">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnPaginaInf" runat="server"
                                            CommandArgument='<%# Container.DataItem %>'
                                            CssClass='<%# Convert.ToInt32(Container.DataItem) == PaginaActual ? "g-btn-page active" : "g-btn-page" %>'>
                                            <%# Convert.ToInt32(Container.DataItem) + 1 %>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <asp:LinkButton ID="btnSigInf" runat="server" OnClick="btnSiguiente_Click"
                                    CssClass="g-btn-page">Siguiente ></asp:LinkButton>
                    </asp:Panel>
                </ContentTemplate>
                </asp:UpdatePanel>
                </div>
            </div>
        </div>

        <script type="text/javascript">
            function toggleSidebar(open) {
                var sidebar = document.querySelector('.store-sidebar');
                var overlay = document.getElementById('sidebarOverlay');
                if (sidebar && overlay) {
                    if (open) {
                        sidebar.classList.add('open');
                        overlay.classList.add('open');
                        document.body.style.overflow = 'hidden';
                    } else {
                        sidebar.classList.remove('open');
                        overlay.classList.remove('open');
                        document.body.style.overflow = '';
                    }
                }
            }


            function toggleCategoryCollapse() {
                var content = document.getElementById('categoryCollapseContent');
                var arrow = document.getElementById('categoryArrow');
                if (content && arrow) {
                    if (content.style.maxHeight === '0px' || content.style.maxHeight === 0) {
                        content.style.maxHeight = '400px';
                        content.style.opacity = '1';
                        arrow.style.transform = 'rotate(0deg)';
                    } else {
                        content.style.maxHeight = '0px';
                        content.style.opacity = '0';
                        arrow.style.transform = 'rotate(-90deg)';
                    }
                }
            }

        </script>
    </asp:Content>