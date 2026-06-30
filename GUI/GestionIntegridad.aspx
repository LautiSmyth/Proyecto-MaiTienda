<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GestionIntegridad.aspx.cs" Inherits="GestionIntegridad" %>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Gestión de Integridad — MaiTienda</title>
    <style>
        *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }
        body {
            font-family: 'Segoe UI', system-ui, sans-serif;
            background: #0d1117;
            color: #c9d1d9;
            min-height: 100vh;
            padding: 2rem;
        }
        header {
            display: flex;
            align-items: center;
            gap: 1rem;
            margin-bottom: 2rem;
            padding-bottom: 1rem;
            border-bottom: 1px solid #21262d;
        }
        header h1 { font-size: 1.4rem; color: #e6edf3; }
        header .badge {
            background: #6e2b2b;
            color: #f85149;
            border: 1px solid #6e2b2b;
            padding: 0.2rem 0.7rem;
            border-radius: 20px;
            font-size: 0.75rem;
            font-weight: 600;
        }
        .section {
            background: #161b22;
            border: 1px solid #21262d;
            border-radius: 10px;
            padding: 1.5rem;
            margin-bottom: 1.5rem;
        }
        .section h2 {
            font-size: 1rem;
            color: #e6edf3;
            margin-bottom: 1rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        .ok-badge { color: #3fb950; }
        .error-badge { color: #f85149; }
        .warn-badge { color: #d29922; }
        .record-table {
            width: 100%;
            border-collapse: collapse;
            font-size: 0.85rem;
            margin-top: 0.8rem;
        }
        .record-table th {
            text-align: left;
            padding: 0.5rem 0.75rem;
            background: #0d1117;
            color: #8b949e;
            font-weight: 500;
            border-bottom: 1px solid #21262d;
        }
        .record-table td {
            padding: 0.5rem 0.75rem;
            border-bottom: 1px solid #21262d;
            color: #c9d1d9;
            font-family: 'Cascadia Code', monospace;
            font-size: 0.8rem;
            word-break: break-all;
        }
        .record-table tr:hover td { background: #1c2128; }
        .tag-tampered {
            display: inline-block;
            background: #3d1f1f;
            color: #f85149;
            border-radius: 4px;
            padding: 0.1rem 0.5rem;
            font-size: 0.75rem;
            font-family: sans-serif;
        }
        .actions-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 1rem;
        }
        .action-card {
            background: #0d1117;
            border: 1px solid #21262d;
            border-radius: 8px;
            padding: 1.25rem;
        }
        .action-card h3 { font-size: 0.95rem; color: #e6edf3; margin-bottom: 0.4rem; }
        .action-card p { font-size: 0.82rem; color: #8b949e; margin-bottom: 1rem; line-height: 1.5; }
        select {
            width: 100%;
            padding: 0.55rem 0.75rem;
            background: #161b22;
            border: 1px solid #30363d;
            border-radius: 6px;
            color: #c9d1d9;
            font-size: 0.85rem;
            margin-bottom: 0.8rem;
            outline: none;
        }
        .btn {
            display: inline-block;
            padding: 0.55rem 1.2rem;
            border: none;
            border-radius: 6px;
            font-size: 0.88rem;
            font-weight: 500;
            cursor: pointer;
            transition: opacity 0.15s;
            text-decoration: none;
        }
        .btn:hover { opacity: 0.85; }
        .btn-success { background: #238636; color: #fff; }
        .btn-danger { background: #da3633; color: #fff; }
        .btn-secondary { background: #21262d; color: #c9d1d9; border: 1px solid #30363d; }
        .alert {
            padding: 0.75rem 1rem;
            border-radius: 6px;
            font-size: 0.88rem;
            margin-bottom: 1rem;
        }
        .alert-success { background: #1a2f1a; border: 1px solid #238636; color: #3fb950; }
        .alert-error { background: #3d1f1f; border: 1px solid #6e2b2b; color: #f85149; }
        .no-backups { font-size: 0.85rem; color: #8b949e; font-style: italic; }
        .dvv-status {
            display: flex;
            gap: 1rem;
            flex-wrap: wrap;
            margin-bottom: 1rem;
        }
        .dvv-chip {
            padding: 0.3rem 0.8rem;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 500;
        }
        .dvv-chip.ok { background: #1a2f1a; color: #3fb950; border: 1px solid #238636; }
        .dvv-chip.fail { background: #3d1f1f; color: #f85149; border: 1px solid #6e2b2b; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <header>
            <h1>🛡 Gestión de Integridad de Datos</h1>
            <span class="badge">ACCESO DE EMERGENCIA</span>
            <asp:HyperLink runat="server" NavigateUrl="~/Login.aspx" CssClass="btn btn-secondary" style="margin-left:auto;font-size:0.82rem;">
                Ir al Login →
            </asp:HyperLink>
        </header>

        <asp:Panel runat="server" ID="pnlResultadoAccion" Visible="false">
            <div id="alertAccion" runat="server" class="alert">
                <asp:Literal runat="server" ID="litResultado" />
            </div>
        </asp:Panel>

        <div class="section">
            <h2>Estado actual de la base de datos</h2>

            <div class="dvv-status">
                <asp:Literal runat="server" ID="litEstadoDVV" />
            </div>

            <asp:Panel runat="server" ID="pnlUsuariosCorruptos" Visible="false">
                <h2 style="margin-bottom:0.5rem;"><span class="error-badge">✗</span> Usuarios con DVH alterado</h2>
                <table class="record-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>NombreUsuario</th>
                            <th>Perfil</th>
                            <th>Estado</th>
                            <th>IntentosFallidos</th>
                            <th>DVH</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater runat="server" ID="repUsuarios">
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("IdUsuario") %></td>
                                    <td><%# Eval("NombreUsuario") %></td>
                                    <td><%# Eval("Perfil") %></td>
                                    <td><%# Eval("Estado") %></td>
                                    <td><%# Eval("IntentosFallidos") %></td>
                                    <td><span class="tag-tampered">ALTERADO</span></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </asp:Panel>

            <asp:Panel runat="server" ID="pnlProductosCorruptos" Visible="false" style="margin-top:1.2rem;">
                <h2 style="margin-bottom:0.5rem;"><span class="error-badge">✗</span> Productos con DVH alterado</h2>
                <table class="record-table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Nombre</th>
                            <th>Categoria</th>
                            <th>Precio</th>
                            <th>Stock</th>
                            <th>Estado</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater runat="server" ID="repProductos">
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("IdProducto") %></td>
                                    <td><%# Eval("Nombre") %></td>
                                    <td><%# Eval("Categoria") %></td>
                                    <td>$<%# string.Format("{0:N2}", Eval("Precio")) %></td>
                                    <td><%# Eval("Stock") %></td>
                                    <td><span class="tag-tampered">ALTERADO</span></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </asp:Panel>

            <asp:Panel runat="server" ID="pnlTodoOk" Visible="false">
                <p class="ok-badge">✓ Todos los registros tienen integridad válida.</p>
            </asp:Panel>
        </div>

        <div class="section">
            <h2>Acciones de recuperación</h2>
            <div class="actions-grid">

                <div class="action-card">
                    <h3>Asumir pérdida y recalcular</h3>
                    <p>Acepta los datos actuales como válidos y recalcula todos los DVH y DVV. Los registros alterados quedan con sus valores actuales.</p>
                    <asp:Button runat="server" ID="btnRecalcular" Text="Recalcular DVH / DVV"
                        CssClass="btn btn-success"
                        OnClick="btnRecalcular_Click"
                        OnClientClick="return confirm('¿Confirma que desea asumir los datos actuales como válidos y recalcular los dígitos verificadores?');" />
                </div>

                <div class="action-card">
                    <h3>Restaurar desde backup</h3>
                    <p>Restaura la base de datos completa desde un archivo de backup. Se perderán todos los cambios posteriores al backup seleccionado.</p>
                    <asp:Panel runat="server" ID="pnlConBackups">
                        <asp:DropDownList runat="server" ID="ddlBackups" />
                        <asp:Button runat="server" ID="btnRestaurar" Text="Restaurar Backup"
                            CssClass="btn btn-danger"
                            OnClick="btnRestaurar_Click"
                            OnClientClick="return confirm('ATENCIÓN: Esta acción restaurará la base de datos completa. Se perderán todos los datos posteriores al backup. ¿Confirma?');" />
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlSinBackups" Visible="false">
                        <p class="no-backups">No se encontraron archivos de backup en la carpeta configurada.</p>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
