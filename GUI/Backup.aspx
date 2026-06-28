<%@ Page Title="Backup del Sistema" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeFile="Backup.aspx.cs" Inherits="Backup" ResponseEncoding="utf-8" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container" style="margin-top: 50px; margin-bottom: 50px;">

        <h2>Backup de Base de Datos</h2>
        <hr />

        <%-- Mensajes de resultado --%>
        <asp:Panel runat="server" ID="pnlExito" Visible="false" CssClass="alert alert-success" role="alert" style="margin-bottom: 20px;">
            <strong>&#10003; Backup generado correctamente</strong><br />
            <asp:Literal runat="server" ID="litRuta" />
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlError" Visible="false" CssClass="g-error" role="alert" style="margin-bottom: 20px;">
            <asp:Literal runat="server" ID="litError" />
        </asp:Panel>

        <%-- Panel principal --%>
        <div class="g-card" style="padding: 30px; margin-bottom: 30px; max-width: 100%;">
            <h4 style="margin-top: 0;">Backup Manual</h4>
            <p>Genera un archivo <code>.bak</code> de la base de datos <strong>MaiTiendaDB</strong> en la carpeta configurada del servidor.</p>
            <p style="color: #666; font-size: 13px;">
                Carpeta destino: <strong><asp:Literal runat="server" ID="litCarpeta" /></strong>
            </p>
            <asp:Button ID="btnGenerarBackup" runat="server" Text="Generar Backup Ahora"
                CssClass="btn btn-primary"
                OnClick="btnGenerarBackup_Click"
                OnClientClick="return confirm('¿Confirmar generación de backup?');"
                style="margin-top: 10px;" />
        </div>

        <%-- Próximo backup automático --%>
        <div class="g-card" style="padding: 30px; margin-bottom: 30px; max-width: 100%;">
            <h4 style="margin-top: 0;">Backup Automático</h4>
            <p>El sistema genera un backup automáticamente cada 24 horas.</p>
            <p style="color: #666; font-size: 13px;">
                Próxima ejecución programada: <strong><asp:Literal runat="server" ID="litProximoBackup" /></strong>
            </p>
        </div>

        <%-- Listado de backups existentes --%>
        <div class="g-card" style="padding: 30px; max-width: 100%;">
            <h4 style="margin-top: 0;">Archivos de Backup Existentes</h4>
            <asp:Panel runat="server" ID="pnlSinArchivos" Visible="false">
                <p style="color: #999;">No se encontraron archivos de backup en la carpeta configurada.</p>
            </asp:Panel>
            <div style="overflow-x: auto; width: 100%;">
                <asp:GridView ID="gvBackups" runat="server" AutoGenerateColumns="false"
                    CssClass="table table-striped table-hover"
                    GridLines="None"
                    Width="100%"
                    EmptyDataText="No hay backups disponibles.">
                    <Columns>
                        <asp:BoundField DataField="Nombre" HeaderText="Archivo"
                            ItemStyle-Wrap="true" />
                        <asp:BoundField DataField="Fecha" HeaderText="Fecha"
                            DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"
                            ItemStyle-Wrap="false"
                            ItemStyle-Width="160px" />
                        <asp:BoundField DataField="Tamaño" HeaderText="Tamaño"
                            ItemStyle-Wrap="false"
                            ItemStyle-Width="100px"
                            ItemStyle-HorizontalAlign="Right"
                            HeaderStyle-HorizontalAlign="Right" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

    </div>
</asp:Content>
