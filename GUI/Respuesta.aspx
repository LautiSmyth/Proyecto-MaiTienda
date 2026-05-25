<%@ Page Title="Panel" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Respuesta.aspx.cs" ResponseEncoding="utf-8" Inherits="Respuesta" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

<div class="g-wrapper">
    <div class="g-grid"></div>
    <div class="g-card">
        <div class="g-inner">

            <div class="g-brand">
                <span class="g-pretag">// acceso autorizado</span>
                <span class="g-logo">Mai<span>Tienda</span></span>
            </div>

            <hr class="g-divider" />

            <div class="g-success-alert">
                <span class="g-success-icon">&#10003;</span>
                <h3 class="g-success-title">&iexcl;Sesi&#243;n Iniciada!</h3>
                <p class="g-success-message">
                    Se logeo el <asp:Label runat="server" ID="lblRol" CssClass="g-highlight" /> <asp:Label runat="server" ID="lblUsuario" CssClass="g-highlight-user" />
                </p>
            </div>

            <asp:Button runat="server" ID="btnCerrarSesion"
                Text="Cerrar sesi&#243;n"
                CssClass="g-btn"
                OnClick="btnCerrarSesion_Click" />

        </div>
    </div>
</div>

</asp:Content>
