<%@ Page Title="Iniciar sesion" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" ResponseEncoding="utf-8" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">



<div class="g-wrapper">
    <div class="g-grid"></div>
    <div class="g-card">
        <div class="g-inner">

            <div class="g-brand">
                <span class="g-pretag">&#47;&#47; acceso al sistema</span>
                <span class="g-logo">Mai<span>Tienda</span></span>
                <span class="g-tagline">Gesti&#243;n de componentes</span>
            </div>

            <hr class="g-divider" />

            <span class="g-section-title">Autenticaci&#243;n</span>

            <asp:Panel runat="server" ID="pnlError" Visible="false" CssClass="g-error" role="alert">
                <asp:Literal runat="server" ID="litError" />
            </asp:Panel>

            <div class="g-form-group">
                <asp:Label runat="server" AssociatedControlID="txtUsuario" CssClass="g-label" Text="Usuario" />
                <asp:TextBox runat="server" ID="txtUsuario" CssClass="g-input" MaxLength="50" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUsuario"
                    CssClass="g-validator" ErrorMessage="Ingrese el nombre de usuario." Display="Dynamic" />
            </div>

            <div class="g-form-group">
                <asp:Label runat="server" AssociatedControlID="txtPassword" CssClass="g-label" Text="Contrase&#241;a" />
                <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="g-input" MaxLength="100" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword"
                    CssClass="g-validator" ErrorMessage="Ingrese la contrase&#241;a." Display="Dynamic" />
            </div>

            <asp:Button runat="server" ID="btnIngresar" Text="Ingresar" CssClass="g-btn" OnClick="btnIngresar_Click" />

            <div class="g-footer">
                <span class="g-footer-text">&#169; <%: DateTime.Now.Year %> MaiTienda</span>
                <span class="g-status">
                    <asp:Label runat="server" ID="lblConexion" CssClass="g-status" />
                </span>
            </div>

        </div>
    </div>
</div>

</asp:Content>
