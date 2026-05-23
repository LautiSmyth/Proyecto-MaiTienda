<%@ Page Title="Iniciar sesión" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" ResponseEncoding="utf-8" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link href="https://fonts.googleapis.com/css2?family=Playfair+Display:ital,wght@0,400;0,600;1,400&family=EB+Garamond:wght@400;500&display=swap" rel="stylesheet" />

<style>
    .mai-wrapper {
        min-height: 78vh;
        display: flex;
        align-items: center;
        justify-content: center;
        background: linear-gradient(160deg, #ede5f5 0%, #f5f0fa 40%, #e8ddf2 100%);
        margin: -20px -15px 0 -15px;
        padding: 48px 16px;
        position: relative;
        overflow: hidden;
    }

    .mai-wrapper::before {
        content: '';
        position: absolute;
        top: 0; left: 0; right: 0; bottom: 0;
        background-image:
            radial-gradient(circle at 20% 30%, rgba(180,150,210,0.18) 0%, transparent 50%),
            radial-gradient(circle at 80% 70%, rgba(160,120,195,0.14) 0%, transparent 50%);
        pointer-events: none;
    }

    .mai-card {
        background: #fdf9ff;
        border: 1.5px solid #cfc0e3;
        border-radius: 4px;
        width: 100%;
        max-width: 390px;
        padding: 44px 40px 40px;
        position: relative;
        box-shadow: 0 8px 40px rgba(90,60,130,0.10), 0 2px 8px rgba(90,60,130,0.06);
    }

    .mai-card::before,
    .mai-card::after {
        content: '';
        position: absolute;
        left: 10px; right: 10px;
        height: 1.5px;
        background: linear-gradient(to right, transparent, #b89fd4 20%, #b89fd4 80%, transparent);
    }
    .mai-card::before { top: 8px; }
    .mai-card::after  { bottom: 8px; }

    .mai-brand {
        text-align: center;
        margin-bottom: 28px;
    }

    .mai-ornament {
        font-size: 13px;
        color: #a688c4;
        letter-spacing: 6px;
        display: block;
        margin-bottom: 6px;
        font-family: serif;
    }

    .mai-logo {
        font-family: 'Playfair Display', Georgia, serif;
        font-size: 32px;
        font-weight: 600;
        color: #3d2260;
        letter-spacing: 1px;
        line-height: 1;
        display: block;
        margin-bottom: 4px;
    }

    .mai-tagline {
        font-family: 'EB Garamond', Georgia, serif;
        font-size: 13px;
        color: #9474b8;
        letter-spacing: 2.5px;
        text-transform: uppercase;
        display: block;
    }

    .mai-divider {
        border: none;
        border-top: 1px solid #ddd0ef;
        margin: 20px 0 28px;
        position: relative;
    }

    .mai-divider::after {
        content: '❧';
        position: absolute;
        top: -10px;
        left: 50%;
        transform: translateX(-50%);
        background: #fdf9ff;
        padding: 0 8px;
        color: #b89fd4;
        font-size: 14px;
    }

    .mai-subtitle {
        font-family: 'Playfair Display', Georgia, serif;
        font-size: 16px;
        color: #5b3e82;
        text-align: center;
        margin-bottom: 24px;
        font-style: italic;
    }

    .mai-error {
        background: #f9f0f5;
        border: 1px solid #d4a0b8;
        border-left: 3px solid #b06090;
        border-radius: 3px;
        padding: 10px 14px;
        margin-bottom: 20px;
        font-family: 'EB Garamond', Georgia, serif;
        font-size: 14px;
        color: #7a3055;
    }

    .mai-label {
        font-family: 'EB Garamond', Georgia, serif;
        font-size: 13px;
        color: #7054a0;
        letter-spacing: 1.8px;
        text-transform: uppercase;
        display: block;
        margin-bottom: 6px;
    }

    .mai-input {
        width: 100% !important;
        max-width: 100% !important;
        background: #f8f4fd !important;
        border: 1px solid #cbbfe0 !important;
        border-radius: 3px !important;
        padding: 9px 13px !important;
        font-family: 'EB Garamond', Georgia, serif !important;
        font-size: 15px !important;
        color: #2d1b4e !important;
        transition: border-color 0.2s, background 0.2s;
        box-shadow: none !important;
        outline: none;
    }

    .mai-input:focus {
        border-color: #8b5fb8 !important;
        background: #fff !important;
        box-shadow: 0 0 0 3px rgba(139,95,184,0.10) !important;
    }

    .mai-form-group {
        margin-bottom: 20px;
    }

    .mai-validator {
        font-family: 'EB Garamond', Georgia, serif;
        font-size: 13px;
        color: #b06090;
        display: block;
        margin-top: 4px;
    }

    .mai-btn {
        width: 100%;
        background: #5b3e82 !important;
        border: none !important;
        border-radius: 3px !important;
        color: #f5f0fa !important;
        font-family: 'EB Garamond', Georgia, serif !important;
        font-size: 15px !important;
        letter-spacing: 2px !important;
        text-transform: uppercase !important;
        padding: 11px 0 !important;
        margin-top: 8px;
        cursor: pointer;
        transition: background 0.2s;
    }

    .mai-btn:hover,
    .mai-btn:focus {
        background: #3d2260 !important;
        color: #f5f0fa !important;
    }

    .mai-footer-note {
        text-align: center;
        margin-top: 22px;
        font-family: 'EB Garamond', Georgia, serif;
        font-size: 12px;
        color: #b09fc8;
        letter-spacing: 0.5px;
    }

    @media (max-width: 480px) {
        .mai-card { padding: 36px 24px 32px; }
        .mai-logo { font-size: 26px; }
    }


    .navbar.navbar-inverse.navbar-fixed-top {
        display: none !important;
    }


    body {
        padding-top: 0 !important;
    }

    .container.body-content {
        padding: 0 !important;
        margin: 0 !important;
        width: 100% !important;
        max-width: 100% !important;
    }

    .container.body-content > hr,
    .container.body-content > footer {
        display: none !important;
    }

</style>

<div class="mai-wrapper">
    <div class="mai-card">

        <div class="mai-brand">
            <span class="mai-ornament">&#10022; &nbsp; &#10022; &nbsp; &#10022;</span>
            <span class="mai-logo">MaiTienda</span>
            <span class="mai-tagline">Sistema de gesti&#243;n</span>
        </div>

        <hr class="mai-divider" />

        <p class="mai-subtitle">Iniciar sesi&#243;n</p>

        <asp:Panel runat="server" ID="pnlError" Visible="false" CssClass="mai-error" role="alert">
            <asp:Literal runat="server" ID="litError" />
        </asp:Panel>

        <div class="mai-form-group">
            <asp:Label runat="server" AssociatedControlID="txtUsuario" CssClass="mai-label" Text="Usuario" />
            <asp:TextBox runat="server" ID="txtUsuario" CssClass="mai-input" MaxLength="50" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUsuario"
                CssClass="mai-validator" ErrorMessage="Ingrese el nombre de usuario." Display="Dynamic" />
        </div>

        <div class="mai-form-group">
            <asp:Label runat="server" AssociatedControlID="txtPassword" CssClass="mai-label" Text="Contrase&#241;a" />
            <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="mai-input" MaxLength="100" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword"
                CssClass="mai-validator" ErrorMessage="Ingrese la contrase&#241;a." Display="Dynamic" />
        </div>

        <asp:Button runat="server" ID="btnIngresar" Text="Ingresar"
            CssClass="mai-btn" OnClick="btnIngresar_Click" />

        <p class="mai-footer-note">&#169; <%: DateTime.Now.Year %> MaiTienda</p>

    </div>
</div>

</asp:Content>
