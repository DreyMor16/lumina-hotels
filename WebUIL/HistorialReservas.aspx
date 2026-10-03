<%@ Page Title="Historial de viajes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="HistorialReservas.aspx.cs" Inherits="WebUIL.HistorialReservas" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="portal-banner" style="background-image:linear-gradient(90deg,rgba(8,39,54,.8),rgba(8,39,54,.08)),url('Assets/Images/hotel-ciudad-colonial.jpg')"><div><p class="eyebrow">Memorias de viaje</p><h1>Tu historial de estancias.</h1><p>Revive tus destinos anteriores y conserva los comprobantes de cada visita.</p></div></section>
    <section class="portal-section"><div class="content-shell">
        <div class="page-intro"><div><p class="eyebrow">Estancias completadas</p><h1>Viajes que ya son historia.</h1><p>Consulta por hotel, mes o año.</p></div><a class="btn btn-outline" href="ReservaActual.aspx">Próximas estancias</a></div>
        <div class="surface"><div class="filter-bar">
            <div class="form-group"><label>Hotel</label><asp:DropDownList ID="ddlFiltroHotel" runat="server" CssClass="form-control" /></div>
            <div class="form-group"><label>Mes</label><asp:DropDownList ID="ddlFiltroMes" runat="server" CssClass="form-control"><asp:ListItem Text="Todos los meses" Value="0" /><asp:ListItem Text="Enero" Value="1" /><asp:ListItem Text="Febrero" Value="2" /><asp:ListItem Text="Marzo" Value="3" /><asp:ListItem Text="Abril" Value="4" /><asp:ListItem Text="Mayo" Value="5" /><asp:ListItem Text="Junio" Value="6" /><asp:ListItem Text="Julio" Value="7" /><asp:ListItem Text="Agosto" Value="8" /><asp:ListItem Text="Septiembre" Value="9" /><asp:ListItem Text="Octubre" Value="10" /><asp:ListItem Text="Noviembre" Value="11" /><asp:ListItem Text="Diciembre" Value="12" /></asp:DropDownList></div>
            <div class="form-group"><label>Año</label><asp:DropDownList ID="ddlFiltroAnio" runat="server" CssClass="form-control" /></div>
            <asp:Button ID="btnFiltrar" runat="server" Text="Aplicar filtros" CssClass="btn" OnClick="btnFiltrar_Click" /><asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-outline" OnClick="btnLimpiar_Click" />
        </div></div>
        <asp:Label ID="lblMensajeVacio" runat="server" CssClass="empty-state" Visible="false" />
        <div id="divCarousel" class="reservation-grid" runat="server">
            <asp:Repeater ID="rptHistorial" runat="server" OnItemCommand="rptHistorial_ItemCommand">
                <ItemTemplate>
                    <article class="reservation-card">
                        <div class="reservation-image" style="background-image:url('Assets/Images/hotel-ciudad-colonial.jpg')"><div class="reservation-image-content"><strong><%# Eval("NombreHotel") %></strong><span>Estancia completada · <%# Eval("NombreCategoria") %></span></div></div>
                        <div class="reservation-body">
                            <div class="reservation-info"><div><span>Reserva</span><strong>#<%# Eval("IdReservacion") %></strong></div><div><span>Habitación</span><strong>#<%# Eval("NumHabitacion") %></strong></div><div><span>Entrada</span><strong><%# Convert.ToDateTime(Eval("FechaLlegada")).ToString("dd MMM yyyy") %></strong></div><div><span>Salida</span><strong><%# Convert.ToDateTime(Eval("FechaSalida")).ToString("dd MMM yyyy") %></strong></div></div>
                            <div class="reservation-total"><asp:LinkButton ID="btnImprimir" runat="server" CommandName="PDF" CommandArgument='<%# Eval("IdReservacion") %>' CssClass="btn btn-outline">Descargar comprobante</asp:LinkButton><div style="text-align:right"><span class="field-note">Total neto</span><strong>₡ <%# Convert.ToDecimal(Eval("SubTotal")).ToString("N2") %></strong></div></div>
                        </div>
                    </article>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <asp:Panel ID="pnlPaginacionHistorial" runat="server" CssClass="card-pager" Visible="false">
            <asp:LinkButton ID="btnHistorialAnterior" runat="server" CssClass="pager-button" OnClick="btnHistorialAnterior_Click">← Anterior</asp:LinkButton>
            <asp:Label ID="lblPaginaHistorial" runat="server" CssClass="pager-status" />
            <asp:LinkButton ID="btnHistorialSiguiente" runat="server" CssClass="pager-button" OnClick="btnHistorialSiguiente_Click">Siguiente →</asp:LinkButton>
        </asp:Panel>
    </div></section>
</asp:Content>
