<%@ Page Title="Mis compras" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="MisCompras.aspx.cs" Inherits="MisCompras" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div style="background: #f0ebfa; min-height: 100vh; padding: 40px 0; position: relative;">
        <div class="g-grid"></div>

        <div class="container" style="position: relative; z-index: 2; max-width: 1100px; margin: 0 auto;">

            <div style="text-align: center; margin-bottom: 30px;">
                <span class="g-pretag">Historial</span>
                <h1 class="store-title" style="font-size: 36px; margin: 5px 0;">Mis <span>Compras</span></h1>
                <p class="store-subtitle" style="font-size: 11px; letter-spacing: 2px;">Los pedidos que confirmaste con tu cuenta</p>
                <div class="g-divider" style="margin: 15px auto 0; width: 60px;"></div>
            </div>

            <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="g-card" style="max-width: none; padding: 40px; text-align: center;">
                <h3 style="font-family: 'Orbitron', sans-serif; font-size: 18px; font-weight: 900; color: #7c3aed; margin-bottom: 5px;">Todavía no hiciste compras</h3>
                <p style="font-family: 'Exo 2', sans-serif; color: #9f7cc0; font-size: 13px; margin-bottom: 20px;">Cuando finalices una compra desde el carrito, el pedido queda guardado acá.</p>
                <a href="Default.aspx" class="g-btn" style="text-decoration: none; display: inline-block; padding: 10px 25px;">Ir a la Tienda</a>
            </asp:Panel>

            <asp:GridView ID="gvCompras" runat="server" AutoGenerateColumns="false" CssClass="table table-hover"
                GridLines="None" Width="100%" HeaderStyle-Wrap="false">
                <Columns>
                    <asp:BoundField DataField="IdPedido" HeaderText="Pedido" ItemStyle-Wrap="false" DataFormatString="#{0}" />
                    <asp:BoundField DataField="Fecha" HeaderText="Fecha" ItemStyle-Wrap="false" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:TemplateField HeaderText="Productos">
                        <ItemTemplate><%# DescribirLineas((BE.BEPedido)Container.DataItem) %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Subtotal" HeaderText="Subtotal" ItemStyle-Wrap="false" DataFormatString="${0:N2}" />
                    <asp:BoundField DataField="Envio" HeaderText="Envío" ItemStyle-Wrap="false" DataFormatString="${0:N2}" />
                    <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Wrap="false" DataFormatString="${0:N2}" />
                    <asp:TemplateField HeaderText="Pago">
                        <ItemTemplate><%# DescribirPago((BE.BEPedido)Container.DataItem) %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Factura" ItemStyle-Wrap="false">
                        <ItemTemplate><%# DescribirFactura((BE.BEPedido)Container.DataItem) %></ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

        </div>
    </div>
</asp:Content>
