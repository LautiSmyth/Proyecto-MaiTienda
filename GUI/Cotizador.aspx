<%@ Page Title="Cotizador" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Cotizador.aspx.cs" Inherits="Cotizador" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div style="background: #f0ebfa; min-height: 100vh; padding: 40px 0; position: relative;">
        <div class="g-grid"></div>

        <div class="container" style="position: relative; z-index: 2; max-width: 1100px; margin: 0 auto;">

            <div style="text-align: center; margin-bottom: 30px;">
                <span class="g-pretag">Web Service WSCatalogo</span>
                <h1 class="store-title" style="font-size: 36px; margin: 5px 0;">Cotizador de <span>Pedidos</span></h1>
                <p class="store-subtitle" style="font-size: 11px; letter-spacing: 2px;">Precios y stock consultados en tiempo real al Web Service</p>
                <div class="g-divider" style="margin: 15px auto 0; width: 60px;"></div>
            </div>

            <div style="display: flex; flex-wrap: wrap; gap: 20px; margin-bottom: 20px;">

            <!-- Opción 1: cotizar el carrito del usuario logueado -->
            <div class="g-card" style="flex: 1 1 380px; max-width: none; padding: 24px;">
                <h3 style="font-family: 'Exo 2', sans-serif; font-size: 16px; color: #1e0a3c; margin-top: 0;">Cotizar mi carrito</h3>
                <p style="font-family: 'Exo 2', sans-serif; font-size: 12px; color: #7c5ea8;">Trae los productos de tu carrito para su cotizacion.</p>
                <asp:Button ID="btnCotizarCarrito" runat="server" Text="Cotizar carrito" CssClass="g-btn" OnClick="btnCotizarCarrito_Click"
                    style="width: 100% !important; padding: 12px 0 !important; margin-top: 10px !important;" />
            </div>

            <!-- Opción 2: cotizar un producto puntual -->
            <div class="g-card" style="flex: 1 1 380px; max-width: none; padding: 24px;">
                <h3 style="font-family: 'Exo 2', sans-serif; font-size: 16px; color: #1e0a3c; margin-top: 0;">Cotizar un producto</h3>
                <p style="font-family: 'Exo 2', sans-serif; font-size: 12px; color: #7c5ea8;">Usa el método CotizarProducto del Web Service.</p>
                <div style="display: flex; gap: 10px; align-items: center;">
                    <asp:DropDownList ID="ddlProducto" runat="server" CssClass="form-control" style="flex: 1; min-width: 0;" />
                    <asp:TextBox ID="txtCantidad" runat="server" TextMode="Number" Text="1" min="1" CssClass="form-control" style="width: 80px;" />
                </div>
                <asp:Button ID="btnCotizarProducto" runat="server" Text="Cotizar" CssClass="g-btn" OnClick="btnCotizarProducto_Click"
                    style="width: 100% !important; padding: 12px 0 !important; margin-top: 12px !important;" />
            </div>

            </div>

            <asp:Label ID="lblMensaje" runat="server" Visible="false"
                style="display: block; font-family: 'Exo 2', sans-serif; font-size: 13px; padding: 12px 15px; border-radius: 4px; margin-bottom: 20px;" />

            <!-- Resultado -->
            <asp:Panel ID="pnlResultado" runat="server" Visible="false" CssClass="g-card" style="max-width: none; padding: 24px;">
                <h3 style="font-family: 'Exo 2', sans-serif; font-size: 16px; color: #1e0a3c; margin-top: 0;">Resultado de la cotización</h3>

                <asp:GridView ID="gvLineas" runat="server" AutoGenerateColumns="false" CssClass="table table-hover"
                    GridLines="None" Width="100%" HeaderStyle-Wrap="false" OnRowDataBound="gvLineas_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="IdProducto" HeaderText="ID" />
                        <asp:BoundField DataField="Nombre" HeaderText="Producto" />
                        <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio unit." ItemStyle-Wrap="false" DataFormatString="${0:N2}" />
                        <asp:BoundField DataField="CantidadSolicitada" HeaderText="Cant." ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="StockDisponible" HeaderText="Stock" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="Subtotal" HeaderText="Subtotal" ItemStyle-Wrap="false" DataFormatString="${0:N2}" />
                        <asp:BoundField DataField="Observacion" HeaderText="Estado" ItemStyle-Wrap="false" />
                    </Columns>
                </asp:GridView>

                <div style="display: flex; justify-content: space-between; flex-wrap: wrap; gap: 10px; font-family: 'Exo 2', sans-serif; margin-top: 10px;">
                    <div style="font-size: 12px; color: #7c5ea8;">
                        Fecha: <asp:Literal ID="litFecha" runat="server" /><br />
                        Válida hasta: <asp:Literal ID="litVigencia" runat="server" /><br />
                        Unidades: <asp:Literal ID="litUnidades" runat="server" />
                    </div>
                    <div style="font-size: 22px; font-weight: 700; color: #1e0a3c;">
                        Total: <asp:Literal ID="litTotal" runat="server" />
                    </div>
                </div>
            </asp:Panel>

        </div>
    </div>
</asp:Content>
