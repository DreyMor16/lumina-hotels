<%@ Page Title="Próximas estancias" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReservaActual.aspx.cs" Inherits="WebUIL.ReservaActual" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="portal-banner" style="background-image:linear-gradient(90deg,rgba(8,39,54,.8),rgba(8,39,54,.08)),url('Assets/Images/destino-bosque-nuboso.jpg')"><div><p class="eyebrow">Todo listo para llegar</p><h1>Tus próximas estancias.</h1><p>Consulta fechas, habitación, servicios incluidos y descarga tu comprobante.</p></div></section>
    <section class="portal-section"><div class="content-shell">
        <div class="page-intro"><div><p class="eyebrow">Reservas confirmadas</p><h1>Viajes en agenda.</h1><p>Filtra tus próximas estancias por hotel, categoría o fecha.</p></div><a class="btn btn-outline" href="HistorialReservas.aspx">Ver historial</a></div>
        <div class="surface"><div class="filter-bar">
            <div class="form-group"><label>Hotel</label><asp:DropDownList ID="ddlFiltroHotel" runat="server" CssClass="form-control" /></div>
            <div class="form-group"><label>Categoría</label><asp:DropDownList ID="ddlFiltroCategoria" runat="server" CssClass="form-control" /></div>
            <div class="form-group"><label>Fecha de llegada</label><asp:TextBox ID="txtFiltroFecha" runat="server" TextMode="Date" CssClass="form-control" /></div>
            <asp:Button ID="btnFiltrar" runat="server" Text="Aplicar filtros" CssClass="btn" OnClick="btnFiltrar_Click" />
            <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-outline" OnClick="btnLimpiar_Click" />
        </div></div>
        <asp:Label ID="lblMensajeVacio" runat="server" CssClass="empty-state" Visible="false" />
        <div id="divCarousel" class="reservation-grid" runat="server">
            <asp:Repeater ID="rptReservasActuales" runat="server" OnItemCommand="rptReservasActuales_ItemCommand">
                <ItemTemplate>
                    <article class="reservation-card">
                        <div class="reservation-image" style="background-image:url('Assets/Images/destino-bosque-nuboso.jpg')"><div class="reservation-image-content"><strong><%# Eval("NombreHotel") %></strong><span>Confirmada · <%# Eval("NombreCategoria") %></span></div></div>
                        <div class="reservation-body">
                            <div class="reservation-info">
                                <div><span>Reserva</span><strong>#<%# Eval("IdReservacion") %></strong></div><div><span>Habitación</span><strong>#<%# Eval("NumHabitacion") %></strong></div>
                                <div><span>Check-in</span><strong><%# Convert.ToDateTime(Eval("FechaLlegada")).ToString("dd MMM yyyy") %></strong></div><div><span>Check-out</span><strong><%# Convert.ToDateTime(Eval("FechaSalida")).ToString("dd MMM yyyy") %></strong></div>
                            </div>
                            <div class="amenities">Incluye: <%# Eval("Amenidades") %></div>
                            <div class="reservation-total"><asp:LinkButton ID="btnImprimir" runat="server" CommandName="Imprimir" CommandArgument='<%# Eval("IdReservacion") %>' CssClass="btn btn-outline">Descargar comprobante</asp:LinkButton><div style="text-align:right"><span class="field-note">Total pagado</span><strong>₡ <%# Convert.ToDecimal(Eval("SubTotal")).ToString("N2") %></strong></div></div>
                        </div>
                    </article>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <asp:Panel ID="pnlPaginacionActuales" runat="server" CssClass="card-pager" Visible="false">
            <asp:LinkButton ID="btnActualesAnterior" runat="server" CssClass="pager-button" OnClick="btnActualesAnterior_Click">← Anterior</asp:LinkButton>
            <asp:Label ID="lblPaginaActuales" runat="server" CssClass="pager-status" />
            <asp:LinkButton ID="btnActualesSiguiente" runat="server" CssClass="pager-button" OnClick="btnActualesSiguiente_Click">Siguiente →</asp:LinkButton>
        </asp:Panel>
    </div></section>
</asp:Content>
