<%@ Page Title="Factura" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Factura.aspx.cs" Inherits="Factura" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        @media print {
            .navbar, .no-imprimir, .g-grid { display: none !important; }
            body { padding-top: 0 !important; background: #fff !important; }
            .factura-fondo { background: #fff !important; padding: 0 !important; min-height: 0 !important; }
            .factura-hoja { border: 1px solid #000 !important; box-shadow: none !important; }
        }
    </style>

    <div class="factura-fondo" style="background: #f0ebfa; min-height: 100vh; padding: 40px 0; position: relative;">
        <div class="g-grid"></div>

        <div class="container" style="position: relative; z-index: 2; max-width: 900px; margin: 0 auto;">

            <asp:Panel ID="pnlNoEncontrada" runat="server" Visible="false" CssClass="g-card no-imprimir" style="max-width: none; padding: 40px; text-align: center;">
                <h3 style="font-family: 'Orbitron', sans-serif; font-size: 18px; font-weight: 900; color: #7c3aed; margin-bottom: 5px;">No encontramos ese comprobante</h3>
                <p style="font-family: 'Exo 2', sans-serif; color: #9f7cc0; font-size: 13px; margin-bottom: 20px;">Puede que no exista o que pertenezca a otra cuenta.</p>
                <a href="MisCompras.aspx" class="g-btn" style="text-decoration: none; display: inline-block; padding: 10px 25px;">Ir a Mis compras</a>
            </asp:Panel>

            <asp:Panel ID="pnlMantenimiento" runat="server" Visible="false" CssClass="g-card no-imprimir" style="max-width: none; padding: 40px; text-align: center;">
                <h3 style="font-family: 'Orbitron', sans-serif; font-size: 18px; font-weight: 900; color: #7c3aed; margin-bottom: 5px;">Servidor en mantenimiento</h3>
                <p style="font-family: 'Exo 2', sans-serif; color: #9f7cc0; font-size: 13px; margin-bottom: 0;">Las facturas no están disponibles en este momento. Intente nuevamente más tarde.</p>
            </asp:Panel>

            <asp:Panel ID="pnlFactura" runat="server" Visible="false">

                <asp:Label ID="lblNueva" runat="server" Visible="false" role="status" CssClass="no-imprimir"
                    style="display: block; font-family: 'Exo 2', sans-serif; font-size: 13px; padding: 12px 15px; border-radius: 4px; margin-bottom: 20px; background: #e7f6ec; color: #1e6b3a;"
                    Text="¡Compra registrada! Esta es tu factura: podés imprimirla o volver a verla en Mis compras." />

                <div class="g-card factura-hoja" style="max-width: none; padding: 30px; background: #fff; font-family: 'Exo 2', sans-serif; color: #1e0a3c;">

                    <table style="width: 100%; border-collapse: collapse; margin-bottom: 20px;">
                        <tr>
                            <td style="width: 45%; vertical-align: top; padding-right: 15px;">
                                <div style="font-family: 'Orbitron', sans-serif; font-size: 22px; font-weight: 900; color: #7c3aed;"><asp:Literal ID="litEmisor" runat="server" /></div>
                                <div style="font-size: 12px; margin-top: 6px;"><asp:Literal ID="litEmisorDomicilio" runat="server" /></div>
                                <div style="font-size: 12px;">CUIT: <asp:Literal ID="litEmisorCuit" runat="server" /></div>
                                <div style="font-size: 12px;"><asp:Literal ID="litEmisorCondicionIva" runat="server" /></div>
                            </td>
                            <td style="width: 10%; text-align: center; vertical-align: top;">
                                <div style="border: 2px solid #1e0a3c; padding: 6px 0;">
                                    <div style="font-size: 32px; font-weight: 900; line-height: 1;"><asp:Literal ID="litTipo" runat="server" /></div>
                                    <div style="font-size: 9px;">COD. <asp:Literal ID="litCodigo" runat="server" /></div>
                                </div>
                            </td>
                            <td style="width: 45%; vertical-align: top; text-align: right; padding-left: 15px;">
                                <div style="font-size: 20px; font-weight: 700;">FACTURA</div>
                                <div style="font-size: 14px; font-weight: 600;">Nº <asp:Literal ID="litNumero" runat="server" /></div>
                                <div style="font-size: 12px; margin-top: 6px;">Fecha de emisión: <asp:Literal ID="litFecha" runat="server" /></div>
                                <div style="font-size: 12px;">Pedido Nº <asp:Literal ID="litPedido" runat="server" /></div>
                            </td>
                        </tr>
                    </table>

                    <div style="border-top: 1.5px solid #1e0a3c; border-bottom: 1.5px solid #1e0a3c; padding: 10px 0; margin-bottom: 20px; font-size: 13px;">
                        <div><strong>Cliente:</strong> <asp:Literal ID="litCliente" runat="server" /></div>
                        <div><strong>DNI / CUIT:</strong> <asp:Literal ID="litDocumento" runat="server" /></div>
                        <div><strong>Domicilio de entrega:</strong> <asp:Literal ID="litDireccion" runat="server" /></div>
                        <div><strong>Condición frente al IVA:</strong> <asp:Literal ID="litCondicionCliente" runat="server" /></div>
                    </div>

                    <table style="width: 100%; border-collapse: collapse; font-size: 13px;">
                        <thead>
                            <tr style="border-bottom: 1.5px solid #1e0a3c;">
                                <th style="text-align: left; padding: 8px;">Código</th>
                                <th style="text-align: left; padding: 8px;">Descripción</th>
                                <th style="text-align: right; padding: 8px;">Cant.</th>
                                <th style="text-align: right; padding: 8px;">Precio unit.</th>
                                <th style="text-align: right; padding: 8px;">Subtotal</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="repLineas" runat="server">
                                <ItemTemplate>
                                    <tr style="border-bottom: 1px solid #ede5f8;">
                                        <td style="padding: 8px;"><%# Eval("IdProducto") %></td>
                                        <td style="padding: 8px;"><%# Server.HtmlEncode(Eval("Nombre").ToString()) %></td>
                                        <td style="padding: 8px; text-align: right;"><%# Eval("Cantidad") %></td>
                                        <td style="padding: 8px; text-align: right;">$<%# Eval("PrecioUnitario", "{0:N2}") %></td>
                                        <td style="padding: 8px; text-align: right;">$<%# Eval("Subtotal", "{0:N2}") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr style="border-bottom: 1px solid #ede5f8;">
                                <td style="padding: 8px;">—</td>
                                <td style="padding: 8px;"><asp:Literal ID="litEnvioDetalle" runat="server" /></td>
                                <td style="padding: 8px; text-align: right;">1</td>
                                <td style="padding: 8px; text-align: right;">$<asp:Literal ID="litEnvioPrecio" runat="server" /></td>
                                <td style="padding: 8px; text-align: right;">$<asp:Literal ID="litEnvioSubtotal" runat="server" /></td>
                            </tr>
                        </tbody>
                    </table>

                    <table style="margin-left: auto; margin-top: 20px; font-size: 13px; border-collapse: collapse; min-width: 300px;">
                        <tr>
                            <td style="padding: 4px 10px;">Importe neto gravado</td>
                            <td style="padding: 4px 10px; text-align: right;">$<asp:Literal ID="litNeto" runat="server" /></td>
                        </tr>
                        <tr>
                            <td style="padding: 4px 10px;">IVA <asp:Literal ID="litAlicuota" runat="server" /></td>
                            <td style="padding: 4px 10px; text-align: right;">$<asp:Literal ID="litIva" runat="server" /></td>
                        </tr>
                        <tr style="border-top: 1.5px solid #1e0a3c; font-size: 16px; font-weight: 700;">
                            <td style="padding: 8px 10px;">TOTAL</td>
                            <td style="padding: 8px 10px; text-align: right;">$<asp:Literal ID="litTotal" runat="server" /></td>
                        </tr>
                    </table>

                    <asp:Panel ID="pnlPago" runat="server" Visible="false" style="margin-top: 20px; border-top: 1.5px solid #1e0a3c; padding-top: 10px; font-size: 13px;">
                        <div><strong>Forma de pago:</strong> <asp:Literal ID="litMedioPago" runat="server" /></div>
                        <div><strong>Estado del pago:</strong> <asp:Literal ID="litEstadoPago" runat="server" /></div>
                        <asp:Panel ID="pnlReferenciaPago" runat="server" Visible="false">
                            <strong>Referencia de pago:</strong> <asp:Literal ID="litReferenciaPago" runat="server" />
                        </asp:Panel>
                        <asp:Panel ID="pnlFirmaPago" runat="server" Visible="false" style="margin-top: 8px;">
                            <div><strong>Firma del cliente (autoriza el pago):</strong></div>
                            <asp:Literal ID="litFirmaPago" runat="server" />
                            <div style="font-size: 11px; color: #7c5ea8; margin-top: 4px;">Pago simulado: no se realizó ningún cobro real.</div>
                        </asp:Panel>
                    </asp:Panel>

                    <p style="font-size: 11px; color: #7c5ea8; margin: 20px 0 0 0;">Los precios del detalle incluyen IVA; el neto y el IVA se discriminan sobre el total.</p>
                </div>

                <div class="no-imprimir" style="text-align: center; margin-top: 20px;">
                    <button type="button" class="g-btn" onclick="window.print();" style="width: auto !important; padding: 10px 25px !important; margin-right: 10px;">Imprimir factura</button>
                    <a href="MisCompras.aspx" style="font-family: 'Exo 2', sans-serif; font-size: 12px; color: #7c3aed; text-decoration: none; font-weight: 600;">← Mis compras</a>
                </div>

            </asp:Panel>

        </div>
    </div>
</asp:Content>
