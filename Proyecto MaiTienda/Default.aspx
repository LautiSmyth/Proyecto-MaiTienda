<%@ Page Title="Iniciar sesion" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" ResponseEncoding="utf-8" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

<link href="https://fonts.googleapis.com/css2?family=Orbitron:wght@600;900&family=Exo+2:wght@300;400;500;600&display=swap" rel="stylesheet" />

<style>
    .navbar.navbar-inverse.navbar-fixed-top { display: none !important; }
    body { padding-top: 0 !important; background: #f0ebfa !important; }
    .container.body-content { padding: 0 !important; margin: 0 !important; width: 100% !important; max-width: 100% !important; }
    .container.body-content > hr, .container.body-content > footer { display: none !important; }
    * { box-sizing: border-box; }

    .g-wrapper {
        min-height: 100vh;
        background: #f0ebfa;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 40px 16px;
        position: relative;
        overflow: hidden;
    }

    .g-wrapper::before {
        content: '';
        position: absolute;
        top: -180px; left: 50%;
        transform: translateX(-50%);
        width: 900px; height: 900px;
        background: radial-gradient(circle, rgba(139,92,246,0.12) 0%, transparent 65%);
        pointer-events: none;
    }

    .g-grid {
        position: absolute;
        inset: 0;
        background-image:
            linear-gradient(rgba(139,92,246,0.06) 1px, transparent 1px),
            linear-gradient(90deg, rgba(139,92,246,0.06) 1px, transparent 1px);
        background-size: 48px 48px;
        pointer-events: none;
    }

    .g-card {
        background: #ffffff;
        border: 1.5px solid #ddd0f5;
        width: 100%;
        max-width: 420px;
        position: relative;
        z-index: 1;
        box-shadow: 0 8px 40px rgba(109,40,217,0.10), 0 2px 8px rgba(109,40,217,0.06);
        clip-path: polygon(0 0, calc(100% - 20px) 0, 100% 20px, 100% 100%, 20px 100%, 0 calc(100% - 20px));
    }

    .g-card::after {
        content: '';
        position: absolute;
        top: 0; left: 0; right: 0;
        height: 3px;
        background: linear-gradient(to right, #7c3aed, #a855f7, #c084fc);
    }

    .g-inner { padding: 42px 38px 36px; }

    .g-brand { margin-bottom: 30px; }

    .g-pretag {
        font-family: 'Exo 2', sans-serif;
        font-size: 10px;
        font-weight: 600;
        color: #7c3aed;
        letter-spacing: 4px;
        text-transform: uppercase;
        display: block;
        margin-bottom: 10px;
    }

    .g-logo {
        font-family: 'Orbitron', sans-serif;
        font-size: 30px;
        font-weight: 900;
        color: #1e0a3c;
        letter-spacing: 2px;
        line-height: 1;
        display: block;
        margin-bottom: 6px;
    }

    .g-logo span { color: #7c3aed; }

    .g-tagline {
        font-family: 'Exo 2', sans-serif;
        font-size: 11px;
        font-weight: 400;
        color: #9f7cc0;
        letter-spacing: 3px;
        text-transform: uppercase;
        display: block;
    }

    .g-divider {
        border: none;
        border-top: 1.5px solid #ede5f8;
        margin: 26px 0;
        position: relative;
    }

    .g-divider::after {
        content: '//';
        position: absolute;
        top: -9px; left: 0;
        background: #ffffff;
        padding-right: 12px;
        font-family: 'Orbitron', sans-serif;
        font-size: 11px;
        color: #a855f7;
        letter-spacing: 2px;
    }

    .g-section-title {
        font-family: 'Exo 2', sans-serif;
        font-size: 10px;
        font-weight: 600;
        color: #9f7cc0;
        letter-spacing: 3px;
        text-transform: uppercase;
        margin-bottom: 22px;
        display: block;
    }

    .g-error {
        background: #fdf2ff;
        border: 1px solid #d8b4fe;
        border-left: 3px solid #7c3aed;
        padding: 10px 14px;
        margin-bottom: 20px;
        font-family: 'Exo 2', sans-serif;
        font-size: 13px;
        color: #5b21b6;
    }

    .g-form-group { margin-bottom: 20px; }

    .g-label {
        font-family: 'Exo 2', sans-serif;
        font-size: 11px;
        font-weight: 600;
        color: #4c1d95;
        letter-spacing: 2px;
        text-transform: uppercase;
        display: block;
        margin-bottom: 8px;
    }

    .g-input {
        width: 100% !important;
        max-width: 100% !important;
        background: #faf7ff !important;
        border: 1.5px solid #ddd0f5 !important;
        border-radius: 0 !important;
        padding: 11px 14px !important;
        font-family: 'Exo 2', sans-serif !important;
        font-size: 15px !important;
        color: #1e0a3c !important;
        transition: border-color 0.2s, box-shadow 0.2s;
        box-shadow: none !important;
        outline: none;
    }

    .g-input:focus {
        border-color: #7c3aed !important;
        background: #fff !important;
        box-shadow: 0 0 0 3px rgba(124,58,237,0.10) !important;
    }

    .g-validator {
        font-family: 'Exo 2', sans-serif;
        font-size: 11px;
        color: #7c3aed;
        display: block;
        margin-top: 5px;
        letter-spacing: 0.5px;
    }

    .g-btn {
        width: 100%;
        background: #7c3aed !important;
        border: none !important;
        border-radius: 0 !important;
        color: #ffffff !important;
        font-family: 'Orbitron', sans-serif !important;
        font-size: 12px !important;
        font-weight: 700 !important;
        letter-spacing: 3px !important;
        text-transform: uppercase !important;
        padding: 14px 0 !important;
        margin-top: 10px;
        cursor: pointer;
        transition: background 0.2s, box-shadow 0.2s;
        clip-path: polygon(0 0, calc(100% - 10px) 0, 100% 10px, 100% 100%, 10px 100%, 0 calc(100% - 10px));
    }

    .g-btn:hover, .g-btn:focus {
        background: #6d28d9 !important;
        box-shadow: 0 4px 20px rgba(109,40,217,0.30) !important;
        color: #fff !important;
    }

    .g-footer {
        margin-top: 28px;
        padding-top: 18px;
        border-top: 1.5px solid #ede5f8;
        display: flex;
        justify-content: space-between;
        align-items: center;
    }

    .g-footer-text {
        font-family: 'Exo 2', sans-serif;
        font-size: 10px;
        color: #c4b0e0;
        letter-spacing: 1px;
        text-transform: uppercase;
    }

    .g-status {
        display: flex;
        align-items: center;
        gap: 6px;
        font-family: 'Exo 2', sans-serif;
        font-size: 10px;
        color: #9f7cc0;
        letter-spacing: 1px;
    }

    .g-status-dot {
        width: 7px; height: 7px;
        background: #7c3aed;
        border-radius: 50%;
        animation: pulse 2s infinite;
    }

    @keyframes pulse { 0%,100%{opacity:1} 50%{opacity:0.3} }

    @media (max-width: 480px) {
        .g-inner { padding: 28px 22px 24px; }
        .g-logo { font-size: 24px; }
    }
</style>

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

            <asp:Button runat="server" ID="btnIngresar" Text="Ingresar"
                CssClass="g-btn" OnClick="btnIngresar_Click" />

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
