<%@ Page Title="Iniciar Sesión" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeFile="Login.aspx.cs" Inherits="Login" ResponseEncoding="utf-8" %>

    <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="g-wrapper">
            <div class="g-grid"></div>
            <div class="g-card">
                <div class="g-inner">

                    <div class="g-brand">
                        <span class="g-pretag">// acceso al sistema</span>
                        <span class="g-logo">Mai<span>Tienda</span></span>
                        <span class="g-tagline">Gestión de componentes</span>
                    </div>

                    <hr class="g-divider" />

                    <span class="g-section-title">Autenticación</span>

                    <asp:Panel runat="server" ID="pnlError" Visible="false" CssClass="g-error" role="alert">
                        <asp:Literal runat="server" ID="litError" />
                    </asp:Panel>

                    <div class="g-form-group">
                        <asp:Label runat="server" AssociatedControlID="txtUsuario" CssClass="g-label" Text="Usuario" />
                        <asp:TextBox runat="server" ID="txtUsuario" CssClass="g-input" MaxLength="50" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUsuario" CssClass="g-validator"
                            ErrorMessage="Ingrese el nombre de usuario." Display="Dynamic" />
                    </div>

                    <div class="g-form-group">
                        <asp:Label runat="server" AssociatedControlID="txtPassword" CssClass="g-label"
                            Text="Contraseña" />
                        <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="g-input"
                            MaxLength="100" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword"
                            CssClass="g-validator" ErrorMessage="Ingrese la contraseña." Display="Dynamic" />
                    </div>

                    <asp:HiddenField ID="hfCarritoLocal" runat="server" />

                    <asp:Button runat="server" ID="btnIngresar" Text="Ingresar" CssClass="g-btn"
                        OnClick="btnIngresar_Click" />

                    <div class="g-footer">
                        <span class="g-footer-text">&#169; <%: DateTime.Now.Year %> MaiTienda</span>
                        <span class="g-status">
                            <asp:Label runat="server" ID="lblConexion" CssClass="g-status" />
                        </span>
                    </div>

                </div>
            </div>
        </div>

        <script type="text/javascript">
            document.getElementById('<%= btnIngresar.ClientID %>').addEventListener('click', function () {
                var carrito = localStorage.getItem('carrito') || '[]';
                document.getElementById('<%= hfCarritoLocal.ClientID %>').value = carrito;
            });
        </script>
    </asp:Content>
