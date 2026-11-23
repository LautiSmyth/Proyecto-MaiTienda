<%@ Page Title="Bitácora del Sistema" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeFile="Bitacora.aspx.cs" Inherits="Bitacora" ResponseEncoding="utf-8" %>

    <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="container" style="margin-top: 50px;">
            <h2>Gestión de Bitácora</h2>
            <hr />
            
            <asp:Panel runat="server" ID="pnlError" Visible="false" CssClass="g-error" role="alert" style="margin-bottom: 20px;">
                <asp:Literal runat="server" ID="litError" />
            </asp:Panel>

            <asp:Panel runat="server" ID="pnlMensajeDesbloqueo" Visible="false" role="status" style="margin-bottom: 20px; padding: 10px 15px; border-radius: 4px; background: #e7f6ec; color: #1e6b3a;">
                <asp:Literal runat="server" ID="litMensajeDesbloqueo" />
            </asp:Panel>

            <asp:Panel runat="server" ID="pnlCuentasBloqueadas" Visible="false" style="margin-bottom: 25px;">
                <h3>Cuentas bloqueadas</h3>
                <p style="color: #666; font-size: 13px;">Se bloquean solas al 3er intento de contraseña incorrecta. Desde acá el WebMaster las puede reactivar.</p>
                <div class="table-responsive">
                    <asp:GridView ID="gvBloqueados" runat="server" CssClass="table table-striped table-bordered table-hover"
                        AutoGenerateColumns="False" GridLines="None" OnRowCommand="gvBloqueados_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="NombreUsuario" HeaderText="Usuario" />
                            <asp:BoundField DataField="Perfil" HeaderText="Perfil" />
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:Button ID="btnDesbloquear" runat="server" Text="Desbloquear" CssClass="g-btn"
                                        CommandName="Desbloquear" CommandArgument='<%# Eval("IdUsuario") %>'
                                        OnClientClick='<%# "return confirm(\"¿Desbloquear a " + Eval("NombreUsuario") + "?\");" %>'
                                        style="padding: 6px 14px !important; margin: 0 !important; width: auto !important;" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </asp:Panel>

            <asp:HiddenField ID="hfFiltersExpanded" runat="server" Value="false" />
            <div style="margin-bottom: 25px;">
                <div class="g-filter-toggle" data-toggle="collapse" data-target="#collapseFilters"
                    style="margin-bottom: -1px; position: relative; z-index: 2;">
                    <span class="glyphicon glyphicon-filter"></span> Filtros <span class="caret"></span>
                </div>

                <div id="collapseFilters" class='collapse g-filter-card <%= hfFiltersExpanded.Value == "true" ? "in" : "" %>' style="margin-bottom: 0;">
                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group" style="margin-bottom: 0;">
                                <label for="ddlPreset" class="g-label">Rango rápido</label>
                                <asp:DropDownList ID="ddlPreset" runat="server" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlPreset_SelectedIndexChanged" CssClass="g-input">
                                    <asp:ListItem Value="todos" Text="Todos los registros" />
                                    <asp:ListItem Value="hoy" Text="Hoy" />
                                    <asp:ListItem Value="ayer" Text="Ayer" />
                                    <asp:ListItem Value="semana" Text="Últimos 7 días" />
                                    <asp:ListItem Value="mes" Text="Último mes" />
                                    <asp:ListItem Value="personalizado" Text="Personalizado..." />
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group" style="margin-bottom: 0;">
                                <label for="txtDesde" class="g-label">Fecha Desde</label>
                                <asp:TextBox ID="txtDesde" runat="server" TextMode="DateTimeLocal" CssClass="g-input" />
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group" style="margin-bottom: 0;">
                                <label for="txtHasta" class="g-label">Fecha Hasta</label>
                                <asp:TextBox ID="txtHasta" runat="server" TextMode="DateTimeLocal" CssClass="g-input" />
                            </div>
                        </div>
                    </div>

                    <div class="row" style="margin-top: 15px;">
                        <div class="col-md-3">
                            <div class="form-group" style="margin-bottom: 0;">
                                <label for="txtUsuario" class="g-label">Usuario</label>
                                <asp:TextBox ID="txtUsuario" runat="server" CssClass="g-input" />
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group" style="margin-bottom: 0;">
                                <label for="ddlPerfil" class="g-label">Perfil</label>
                                <asp:DropDownList ID="ddlPerfil" runat="server" CssClass="g-input">
                                    <asp:ListItem Value="todos" Text="Todos los perfiles" />
                                    <asp:ListItem Value="WebMaster" Text="WebMaster" />
                                    <asp:ListItem Value="Administrador" Text="Administrador" />
                                    <asp:ListItem Value="Cliente" Text="Cliente" />
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group" style="margin-bottom: 0;">
                                <label for="ddlAccion" class="g-label">Acción</label>
                                <asp:DropDownList ID="ddlAccion" runat="server" CssClass="g-input">
                                    <asp:ListItem Value="todos" Text="Todas las acciones" />
                                    <asp:ListItem Value="Inicio de sesion" Text="Inicio de sesión" />
                                    <asp:ListItem Value="Cierre de sesión" Text="Cierre de sesión" />
                                    <asp:ListItem Value="bloqueado por intentos fallidos" Text="Bloqueado por intentos" />
                                    <asp:ListItem Value="Intento fallido" Text="Intento fallido" />
                                    <asp:ListItem Value="Compra registrada" Text="Compra registrada" />
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-3" style="display: flex; align-items: flex-end; gap: 10px; height: 59px;">
                            <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" OnClick="btnFiltrar_Click"
                                CssClass="g-btn" style="margin-top: 0; padding: 11px 0 !important;" />
                            <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" OnClick="btnLimpiar_Click"
                                CssClass="g-btn"
                                style="margin-top: 0; padding: 11px 0 !important; background: #9f7cc0 !important;" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="table-responsive">
                <asp:GridView ID="gvBitacora" runat="server" CssClass="table table-striped table-bordered table-hover"
                    AutoGenerateColumns="False" EmptyDataText="No hay registros en la bitácora." GridLines="None"
                    AllowPaging="True" PageSize="50" OnPageIndexChanging="gvBitacora_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="NombreUsuario" HeaderText="Usuario" />
                        <asp:BoundField DataField="Perfil" HeaderText="Perfil" />
                        <asp:BoundField DataField="Accion" HeaderText="Acción" />
                        <asp:BoundField DataField="Fecha" HeaderText="Fecha y Hora"
                            DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                    </Columns>
                    <PagerStyle CssClass="gv-pager" HorizontalAlign="Center" />
                </asp:GridView>
            </div>
        </div>

        <script type="text/javascript">
            $(document).ready(function () {
                var collapseEl = $('#collapseFilters');
                var hiddenField = $('#<%= hfFiltersExpanded.ClientID %>');

                collapseEl.on('shown.bs.collapse', function () {
                    hiddenField.val("true");
                });

                collapseEl.on('hidden.bs.collapse', function () {
                    hiddenField.val("false");
                });
            });
        </script>
    </asp:Content>