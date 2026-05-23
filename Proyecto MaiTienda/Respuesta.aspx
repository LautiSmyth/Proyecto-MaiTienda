<%@ Page Title="Menu" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Respuesta.aspx.cs" ResponseEncoding="utf-8" Inherits="Respuesta" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link href="https://fonts.googleapis.com/css2?family=Playfair+Display:ital,wght@0,400;0,600;1,400&family=EB+Garamond:wght@400;500&display=swap" rel="stylesheet" />

<style>
    .men-page {
        background: linear-gradient(160deg, #ede5f5 0%, #f5f0fa 50%, #e8ddf2 100%);
        min-height: 85vh;
        margin: -20px -15px 0 -15px;
        padding: 0 0 48px 0;
    }

    /* ── Barra superior ── */
    .men-topbar {
        background: #3d2260;
        padding: 0 32px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        height: 52px;
        border-bottom: 2px solid #b89fd4;
    }

    .men-topbar-left {
        display: flex;
        align-items: center;
        gap: 18px;
    }

    .men-system-name {
        font-family: 'Playfair Display', Georgia, serif;
        font-size: 18px;
        font-weight: 600;
        color: #f0e8ff;
        letter-spacing: 1px;
        text-decoration: none;
    }

    .men-separator {
        width: 1px;
        height: 22px;
        background: #7b56a8;
    }

    .men-ornament-bar {
        font-size: 11px;
        color: #b89fd4;
        letter-spacing: 4px;
    }

    .men-topbar-right {
        display: flex;
        align-items: center;
        gap: 16px;
    }

    .men-user-info {
        text-align: right;
    }

    .men-user-nombre {
        font-family: 'Playfair Display', Georgia, serif;
        font-size: 14px;
        font-weight: 600;
        color: #f0e8ff;
        display: block;
        line-height: 1.2;
    }

    .men-user-perfil {
        font-family: 'EB Garamond', Georgia, serif;
        font-size: 11px;
        color: #b89fd4;
        letter-spacing: 1.5px;
        text-transform: uppercase;
        display: block;
    }

    .men-btn-logout {
        background: transparent !important;
        border: 1px solid #7b56a8 !important;
        border-radius: 3px !important;
        color: #d4b8f0 !important;
        font-family: 'EB Garamond', Georgia, serif !important;
        font-size: 12px !important;
        letter-spacing: 1.5px !important;
        text-transform: uppercase !important;
        padding: 5px 14px !important;
        cursor: pointer;
        transition: background 0.2s, border-color 0.2s;
        white-space: nowrap;
    }

    .men-btn-logout:hover,
    .men-btn-logout:focus {
        background: rgba(184,159,212,0.15) !important;
        border-color: #b89fd4 !important;
        color: #f0e8ff !important;
    }

    /* ── Contenido principal ── */
    .men-body {
        padding: 40px 32px;
    }

    .men-welcome-section {
        margin-bottom: 36px;
        text-align: center;
    }

    .men-welcome-ornament {
        font-size: 12px;
        color: #a688c4;
        letter-spacing: 6px;
        display: block;
        margin-bottom: 8px;
    }

    .men-welcome-title {
        font-family: 'Playfair Display', Georgia, serif;
        font-size: 26px;
        color: #3d2260;
        margin: 0 0 6px 0;
    }

    .men-welcome-sub {
        font-family: 'EB Garamond', Georgia, serif;
        font-size: 15px;
        color: #7054a0;
        margin: 0;
    }

    .men-divider {
        border: none;
        border-top: 1px solid #cfc0e3;
        margin: 0 auto 36px;
        max-width: 480px;
        position: relative;
    }

    .men-divider::after {
        content: '&#10022;';
        position: absolute;
        top: -10px;
        left: 50%;
        transform: translateX(-50%);
        background: transparent;
        padding: 0 10px;
        color: #b89fd4;
        font-size: 13px;
    }

    /* ── Grid de m&#243;dulos ── */
    .men-modules-label {
        font-family: 'EB Garamond', Georgia, serif;
        font-size: 11px;
        color: #9474b8;
        letter-spacing: 3px;
        text-transform: uppercase;
        text-align: center;
        display: block;
        margin-bottom: 24px;
    }

    .men-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
        gap: 20px;
        max-width: 860px;
        margin: 0 auto;
    }

    .men-card {
        background: #fdf9ff;
        border: 1.5px solid #cfc0e3;
        border-radius: 4px;
        padding: 28px 22px 24px;
        position: relative;
        text-align: center;
        cursor: default;
        transition: box-shadow 0.2s, border-color 0.2s;
    }

    .men-card::before {
        content: '';
        position: absolute;
        top: 6px; left: 8px; right: 8px;
        height: 1px;
        background: linear-gradient(to right, transparent, #d4c0e8 30%, #d4c0e8 70%, transparent);
    }

    .men-card:hover {
        border-color: #9474b8;
        box-shadow: 0 4px 20px rgba(90,60,130,0.10);
    }

    .men-card-icon {
        font-size: 28px;
        display: block;
        margin-bottom: 12px;
        line-height: 1;
    }

    .men-card-title {
        font-family: 'Playfair Display', Georgia, serif;
        font-size: 15px;
        font-weight: 600;
        color: #3d2260;
        display: block;
        margin-bottom: 6px;
    }

    .men-card-desc {
        font-family: 'EB Garamond', Georgia, serif;
        font-size: 13px;
        color: #9474b8;
        display: block;
        line-height: 1.5;
    }

    .men-card-badge {
        display: inline-block;
        margin-top: 12px;
        background: #ede5f5;
        border: 1px solid #cfc0e3;
        border-radius: 20px;
        font-family: 'EB Garamond', Georgia, serif;
        font-size: 10px;
        color: #7054a0;
        letter-spacing: 1.5px;
        text-transform: uppercase;
        padding: 3px 12px;
    }

    @media (max-width: 600px) {
        .men-topbar { padding: 0 16px; height: auto; min-height: 52px; flex-wrap: wrap; gap: 8px; padding-top: 8px; padding-bottom: 8px; }
        .men-body { padding: 28px 16px; }
        .men-grid { grid-template-columns: 1fr 1fr; }
        .men-ornament-bar { display: none; }
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

<div class="men-page">

    <%-- Barra superior con nombre de usuario y cerrar sesion --%>
    <div class="men-topbar">
        <div class="men-topbar-left">
            <span class="men-system-name">MaiTienda</span>
            <div class="men-separator"></div>
            <span class="men-ornament-bar">&#10022; &nbsp; &#10022;</span>
        </div>
        <div class="men-topbar-right">
            <div class="men-user-info">
                <asp:Label runat="server" ID="lblNombreUsuario" CssClass="men-user-nombre" />
                <asp:Label runat="server" ID="lblPerfil" CssClass="men-user-perfil" />
            </div>
            <asp:Button runat="server" ID="btnCerrarSesion"
                Text="Cerrar sesi&#243;n"
                CssClass="men-btn-logout"
                OnClick="btnCerrarSesion_Click" />
        </div>
    </div>

    <%-- Cuerpo --%>
    <div class="men-body">

        <div class="men-welcome-section">
            <span class="men-welcome-ornament">&#10022; &nbsp; &#10022; &nbsp; &#10022;</span>
            <h2 class="men-welcome-title">
                Bienvenid&#243;, <asp:Label runat="server" ID="lblBienvenida" />
            </h2>
            <p class="men-welcome-sub">&#191;Qu&#233; desea gestionar hoy?</p>
        </div>

        <hr class="men-divider" />

        <span class="men-modules-label">M&#243;dulos del sistema</span>

        <div class="men-grid">

            <div class="men-card">
                <span class="men-card-icon">&#128200;</span>
                <span class="men-card-title">Bitacora</span>
                <span class="men-card-desc">Registros de acciones</span>
                <span class="men-card-badge">Pr&#243;ximamente</span>
            </div>

        </div>

    </div>
</div>

</asp:Content>
