<%@ Page Title="Panel" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Respuesta.aspx.cs" ResponseEncoding="utf-8" Inherits="Respuesta" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

<link href="https://fonts.googleapis.com/css2?family=Orbitron:wght@600;900&family=Exo+2:wght@300;400;500;600&display=swap" rel="stylesheet" />

<style>
    .navbar.navbar-inverse.navbar-fixed-top { display: none !important; }
    body { padding-top: 0 !important; background: #f0ebfa !important; }
    .container.body-content { padding: 0 !important; margin: 0 !important; width: 100% !important; max-width: 100% !important; }
    .container.body-content > hr, .container.body-content > footer { display: none !important; }
    * { box-sizing: border-box; }

    .g-page {
        min-height: 100vh;
        background: #f0ebfa;
        display: flex;
        flex-direction: column;
        position: relative;
        overflow-x: hidden;
    }

    .g-page::before {
        content: '';
        position: fixed;
        inset: 0;
        background-image:
            linear-gradient(rgba(139,92,246,0.05) 1px, transparent 1px),
            linear-gradient(90deg, rgba(139,92,246,0.05) 1px, transparent 1px);
        background-size: 56px 56px;
        pointer-events: none;
        z-index: 0;
    }

    /* ─── Topbar ─── */
    .g-topbar {
        background: #1e0a3c;
        border-bottom: 3px solid #7c3aed;
        height: 58px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 0 28px;
        position: relative;
        z-index: 10;
    }

    .g-topbar-left { display: flex; align-items: center; gap: 20px; }

    .g-logo-bar {
        font-family: 'Orbitron', sans-serif;
        font-size: 16px;
        font-weight: 900;
        color: #ffffff;
        letter-spacing: 2px;
    }

    .g-logo-bar span { color: #c084fc; }

    .g-nav-sep { width: 1px; height: 24px; background: #3b1a6e; }

    .g-nav-tag {
        font-family: 'Exo 2', sans-serif;
        font-size: 10px;
        font-weight: 500;
        color: #9f7cc0;
        letter-spacing: 3px;
        text-transform: uppercase;
    }

    .g-topbar-right { display: flex; align-items: center; gap: 20px; }

    .g-user-block { text-align: right; }

    .g-user-name {
        font-family: 'Exo 2', sans-serif;
        font-size: 14px;
        font-weight: 600;
        color: #ffffff;
        display: block;
        line-height: 1.3;
    }

    .g-user-role {
        font-family: 'Exo 2', sans-serif;
        font-size: 10px;
        color: #c084fc;
        letter-spacing: 2.5px;
        text-transform: uppercase;
        display: block;
    }

    .g-btn-logout {
        background: transparent !important;
        border: 1.5px solid #7c3aed !important;
        border-radius: 0 !important;
        color: #c084fc !important;
        font-family: 'Orbitron', sans-serif !important;
        font-size: 9px !important;
        font-weight: 600 !important;
        letter-spacing: 2px !important;
        text-transform: uppercase !important;
        padding: 7px 16px !important;
        cursor: pointer;
        transition: background 0.2s, color 0.2s;
        white-space: nowrap;
        clip-path: polygon(0 0, calc(100% - 8px) 0, 100% 8px, 100% 100%, 8px 100%, 0 calc(100% - 8px));
    }

    .g-btn-logout:hover, .g-btn-logout:focus {
        background: #7c3aed !important;
        color: #ffffff !important;
    }

    /* ─── Body ─── */
    .g-body {
        flex: 1;
        padding: 40px 28px 56px;
        position: relative;
        z-index: 1;
    }

    /* ─── Welcome ─── */
    .g-welcome {
        background: #ffffff;
        border: 1.5px solid #ddd0f5;
        border-left: 4px solid #7c3aed;
        padding: 24px 28px;
        margin-bottom: 36px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        flex-wrap: wrap;
        gap: 16px;
        box-shadow: 0 2px 16px rgba(109,40,217,0.07);
        clip-path: polygon(0 0, calc(100% - 16px) 0, 100% 16px, 100% 100%, 0 100%);
    }

    .g-welcome-pre {
        font-family: 'Exo 2', sans-serif;
        font-size: 10px;
        font-weight: 600;
        color: #7c3aed;
        letter-spacing: 4px;
        text-transform: uppercase;
        display: block;
        margin-bottom: 6px;
    }

    .g-welcome-title {
        font-family: 'Orbitron', sans-serif;
        font-size: 22px;
        font-weight: 700;
        color: #1e0a3c;
        margin: 0;
        line-height: 1.2;
    }

    .g-welcome-title span { color: #7c3aed; }

    .g-welcome-sub {
        font-family: 'Exo 2', sans-serif;
        font-size: 13px;
        color: #9f7cc0;
        margin: 6px 0 0;
    }

    .g-welcome-badge {
        background: #f5f0ff;
        border: 1.5px solid #ddd0f5;
        padding: 12px 18px;
        text-align: right;
        clip-path: polygon(0 0, calc(100% - 10px) 0, 100% 10px, 100% 100%, 10px 100%, 0 calc(100% - 10px));
    }

    .g-badge-role {
        font-family: 'Orbitron', sans-serif;
        font-size: 11px;
        font-weight: 600;
        color: #4c1d95;
        letter-spacing: 2px;
        display: block;
    }

    .g-badge-status {
        font-family: 'Exo 2', sans-serif;
        font-size: 11px;
        color: #9f7cc0;
        display: flex;
        align-items: center;
        gap: 6px;
        justify-content: flex-end;
        margin-top: 4px;
    }

    .g-dot {
        width: 7px; height: 7px;
        background: #7c3aed;
        border-radius: 50%;
        animation: pulse 2s infinite;
    }

    @keyframes pulse { 0%,100%{opacity:1} 50%{opacity:0.35} }

    /* ─── Modules ─── */
    .g-modules-header {
        display: flex;
        align-items: center;
        gap: 14px;
        margin-bottom: 20px;
    }

    .g-modules-title {
        font-family: 'Exo 2', sans-serif;
        font-size: 10px;
        font-weight: 600;
        color: #9f7cc0;
        letter-spacing: 4px;
        text-transform: uppercase;
        white-space: nowrap;
    }

    .g-modules-line {
        flex: 1;
        height: 1.5px;
        background: linear-gradient(to right, #ddd0f5, transparent);
    }

    .g-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
        gap: 16px;
    }

    .g-card {
        background: #ffffff;
        border: 1.5px solid #ddd0f5;
        padding: 26px 20px 22px;
        position: relative;
        cursor: default;
        transition: border-color 0.2s, box-shadow 0.2s, transform 0.2s;
        box-shadow: 0 2px 8px rgba(109,40,217,0.05);
        clip-path: polygon(0 0, calc(100% - 14px) 0, 100% 14px, 100% 100%, 14px 100%, 0 calc(100% - 14px));
    }

    .g-card::before {
        content: '';
        position: absolute;
        top: 0; left: 0; right: 0;
        height: 3px;
        background: linear-gradient(to right, #7c3aed33, transparent);
        transition: background 0.2s;
    }

    .g-card:hover {
        border-color: #a855f7;
        box-shadow: 0 6px 24px rgba(124,58,237,0.14);
        transform: translateY(-2px);
    }

    .g-card:hover::before {
        background: linear-gradient(to right, #7c3aed, #a855f7, transparent);
    }

    .g-card-icon {
        font-size: 28px;
        display: block;
        margin-bottom: 12px;
    }

    .g-card-title {
        font-family: 'Orbitron', sans-serif;
        font-size: 13px;
        font-weight: 700;
        color: #1e0a3c;
        display: block;
        margin-bottom: 6px;
        letter-spacing: 1px;
    }

    .g-card-desc {
        font-family: 'Exo 2', sans-serif;
        font-size: 13px;
        font-weight: 400;
        color: #7c5ea8;
        display: block;
        line-height: 1.6;
        margin-bottom: 14px;
    }

    .g-card-badge {
        display: inline-block;
        background: #f5f0ff;
        border: 1px solid #ddd0f5;
        font-family: 'Exo 2', sans-serif;
        font-size: 9px;
        font-weight: 600;
        color: #9f7cc0;
        letter-spacing: 2px;
        text-transform: uppercase;
        padding: 3px 10px;
    }

    .g-card-badge-active {
        background: #ede9fe;
        border-color: #a855f7;
        color: #6d28d9;
    }

    /* ─── Footer ─── */
    .g-footer-bar {
        background: #1e0a3c;
        border-top: 2px solid #7c3aed;
        padding: 14px 28px;
        display: flex;
        justify-content: space-between;
        position: relative;
        z-index: 1;
    }

    .g-footer-text {
        font-family: 'Exo 2', sans-serif;
        font-size: 10px;
        color: #9f7cc0;
        letter-spacing: 1.5px;
        text-transform: uppercase;
    }

    @media (max-width: 640px) {
        .g-topbar { padding: 0 14px; }
        .g-nav-tag, .g-nav-sep { display: none; }
        .g-body { padding: 22px 14px 36px; }
        .g-welcome-title { font-size: 17px; }
        .g-grid { grid-template-columns: 1fr 1fr; }
    }
</style>

<div class="g-page">

    <div class="g-topbar">
        <div class="g-topbar-left">
            <span class="g-logo-bar">Mai<span>Tienda</span></span>
            <div class="g-nav-sep"></div>
            <span class="g-nav-tag">Panel de gesti&#243;n</span>
        </div>
        <div class="g-topbar-right">
            <div class="g-user-block">
                <asp:Label runat="server" ID="lblNombreUsuario" CssClass="g-user-name" />
                <asp:Label runat="server" ID="lblPerfil" CssClass="g-user-role" />
            </div>
            <asp:Button runat="server" ID="btnCerrarSesion"
                Text="Cerrar sesi&#243;n"
                CssClass="g-btn-logout"
                OnClick="btnCerrarSesion_Click" />
        </div>
    </div>

    <div class="g-body">

        <div class="g-welcome">
            <div>
                <span class="g-welcome-pre">&#47;&#47; acceso autorizado</span>
                <h2 class="g-welcome-title">
                    Bienvenido, <asp:Label runat="server" ID="lblBienvenida" />
                </h2>
                <p class="g-welcome-sub">Selecion&#225; un m&#243;dulo para comenzar.</p>
            </div>
            <div class="g-welcome-badge">
                <span class="g-badge-role"><asp:Label runat="server" ID="lblPerfilBadge" /></span>
                <span class="g-badge-status"><span class="g-dot"></span>Sesi&#243;n activa</span>
            </div>
        </div>

        <div class="g-modules-header">
            <span class="g-modules-title">M&#243;dulos del sistema</span>
            <div class="g-modules-line"></div>
        </div>

        <div class="g-grid">
             
            <div class="g-card">
                <span class="g-card-icon">&#128202;</span>
                <span class="g-card-title">Bit&#225;cora</span>
                <span class="g-card-desc">Registro de eventos y acciones del sistema</span>
                <span class="g-card-badge g-card-badge-active">Activo</span>
            </div>
        <!--
            <div class="g-card">
                <span class="g-card-icon">&#128290;</span>
                <span class="g-card-title">Componentes</span>
                <span class="g-card-desc">Cat&#225;logo de CPU, GPU, RAM y m&#225;s</span>
                <span class="g-card-badge">Pr&#243;ximamente</span>
            </div>

            <div class="g-card">
                <span class="g-card-icon">&#128101;</span>
                <span class="g-card-title">Clientes</span>
                <span class="g-card-desc">Registro y seguimiento de compradores</span>
                <span class="g-card-badge">Pr&#243;ximamente</span>
            </div>

            <div class="g-card">
                <span class="g-card-icon">&#128203;</span>
                <span class="g-card-title">Pedidos</span>
                <span class="g-card-desc">Control de ventas y estado de env&#237;os</span>
                <span class="g-card-badge">Pr&#243;ximamente</span>
            </div>

            <div class="g-card">
                <span class="g-card-icon">&#129535;</span>
                <span class="g-card-title">Stock</span>
                <span class="g-card-desc">Inventario y alertas de reposici&#243;n</span>
                <span class="g-card-badge">Pr&#243;ximamente</span>
            </div>

            <div class="g-card">
                <span class="g-card-icon">&#128200;</span>
                <span class="g-card-title">Reportes</span>
                <span class="g-card-desc">Estad&#237;sticas y an&#225;lisis de ventas</span>
                <span class="g-card-badge">Pr&#243;ximamente</span>
            </div>
          -->

        </div>
    </div>

    <div class="g-footer-bar">
        <span class="g-footer-text">&#169; <%: DateTime.Now.Year %> MaiTienda &mdash; Sistema de gesti&#243;n</span>
        <span class="g-footer-text">v1.0.0</span>
    </div>

</div>

</asp:Content>
