<%@ Page Title="Reservaciones" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="AdminReservas.aspx.cs" Inherits="WebUIL.AdminReservas" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Content2" ContentPlaceHolderID="AdminContent" runat="server">
<div class="control-page">
    <section class="control-subhero">
        <div><span class="control-eyebrow">RECEPCIÓN DIGITAL</span><h1>Reservaciones<br />con atención personal.</h1><p>Busca disponibilidad, acompaña al huésped y gestiona cada llegada desde una sola mesa.</p></div>
        <div class="control-subhero-card"><span class="control-card-icon sand">✦</span><small>FLUJO DE RECEPCIÓN</small><strong>Buscar · Reservar · Recibir</strong><p>Resultados organizados en páginas de 10 para mantener cada consulta clara.</p></div>
    </section>

    <section class="control-workspace control-reservations-workspace">
        <div class="control-workspace-heading">
            <div><span class="control-section-kicker">OPERACIÓN DE ESTANCIAS</span><h2>Una llegada sin fricción.</h2><p>Dos procesos definidos: crear la reservación y atender el día de llegada o salida.</p></div>
            <a class="btn btn-outline control-back-button" href="AdminDashboard.aspx">← Volver al resumen</a>
        </div>

        <section class="control-module" id="nueva-reserva">
            <header class="control-module-head"><span class="control-module-number">01</span><div><span>DISPONIBILIDAD</span><h2>Nueva reservación</h2><p>Confirma la identidad del cliente y encuentra la habitación adecuada.</p></div><span class="control-module-status"><i></i> Flujo asistido</span></header>
            <div class="control-module-layout reservation-layout">
                <div class="control-form-card">
                    <span class="control-form-label">DATOS DE BÚSQUEDA</span>
                    <div class="form-group"><label>Cédula del cliente</label><asp:TextBox ID="txtCedulaReserva" runat="server" CssClass="form-control" placeholder="Identificación registrada" /></div>
                    <div class="form-group"><label>Hotel</label><asp:DropDownList ID="ddlHotelReserva" runat="server" CssClass="form-control" /></div>
                    <div class="control-date-grid"><div class="form-group"><label>Llegada</label><asp:TextBox ID="txtLlegadaReserva" runat="server" CssClass="form-control" TextMode="Date" /></div><div class="form-group"><label>Salida</label><asp:TextBox ID="txtSalidaReserva" runat="server" CssClass="form-control" TextMode="Date" /></div></div>
                    <div class="form-group"><label>Categoría</label><asp:DropDownList ID="ddlCategoriaReserva" runat="server" CssClass="form-control" /></div>
                    <asp:Button ID="btnBuscarDisponibilidad" runat="server" Text="Buscar habitaciones" CssClass="btn control-primary" OnClick="btnBuscarDisponibilidad_Click" />
                </div>
                <div class="control-table-card">
                    <div class="control-table-heading"><div><strong>Habitaciones disponibles</strong><span>Resultados según fechas y preferencias · 10 por página</span></div><span class="control-table-badge">Disponibles</span></div>
                    <div class="control-table-scroll"><asp:GridView ID="gvDisponibles" runat="server" AutoGenerateColumns="False" CssClass="grid-view control-table" GridLines="None" DataKeyNames="NumHabitacion,IdHotel,Precio" AllowPaging="True" PageSize="10" OnPageIndexChanging="gvDisponibles_PageIndexChanging" OnRowCommand="gvDisponibles_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="NumHabitacion" HeaderText="Habitación" />
                            <asp:BoundField DataField="DireccionHotel" HeaderText="Hotel" />
                            <asp:BoundField DataField="NombreCategoria" HeaderText="Categoría" />
                            <asp:BoundField DataField="Caracteristicas" HeaderText="Servicios" />
                            <asp:BoundField DataField="Precio" HeaderText="Precio / noche" DataFormatString="₡ {0:N2}" />
                            <asp:TemplateField HeaderText="Acción"><ItemTemplate><asp:Button runat="server" Text="Reservar" CommandName="Reservar" CommandArgument="<%# ((System.Web.UI.WebControls.GridViewRow)Container).RowIndex %>" CssClass="btn control-table-action" /></ItemTemplate></asp:TemplateField>
                        </Columns>
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="«" LastPageText="»" /><PagerStyle CssClass="grid-pager" />
                    </asp:GridView></div>
                </div>
            </div>
        </section>

        <section class="control-module" id="llegadas">
            <header class="control-module-head"><span class="control-module-number">02</span><div><span>RECEPCIÓN</span><h2>Check-in y check-out</h2><p>Consulta las habitaciones asociadas a las llegadas y salidas de hoy.</p></div><span class="control-module-status quiet">Operación diaria</span></header>
            <div class="control-module-layout checkin-layout">
                <div class="control-form-card">
                    <span class="control-form-label">BUSCAR ESTANCIA</span>
                    <div class="form-group"><label>Cédula del cliente</label><asp:TextBox ID="txtCedulaEstancia" runat="server" CssClass="form-control" placeholder="Identificación del huésped" /></div>
                    <asp:Button ID="btnBuscarEstancia" runat="server" Text="Consultar llegada" CssClass="btn control-primary" OnClick="btnBuscarEstancia_Click" />
                    <p class="control-form-note">La consulta muestra únicamente movimientos correspondientes a la fecha actual.</p>
                </div>
                <div class="control-table-card">
                    <div class="control-table-heading"><div><strong>Movimientos de hoy</strong><span>Entradas y salidas programadas · 10 por página</span></div><span class="control-table-badge gold">Recepción</span></div>
                    <div class="control-table-scroll"><asp:GridView ID="gvEstancias" runat="server" AutoGenerateColumns="False" CssClass="grid-view control-table" GridLines="None" DataKeyNames="IdReservacion,NumHabitacion,IdHotel" AllowPaging="True" PageSize="10" OnPageIndexChanging="gvEstancias_PageIndexChanging" OnRowCommand="gvEstancias_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="IdReservacion" HeaderText="Reserva" />
                            <asp:BoundField DataField="NumHabitacion" HeaderText="Habitación" />
                            <asp:BoundField DataField="NombreHotel" HeaderText="Hotel" />
                            <asp:BoundField DataField="FechaLlegada" HeaderText="Llegada" DataFormatString="{0:dd MMM yyyy}" />
                            <asp:BoundField DataField="FechaSalida" HeaderText="Salida" DataFormatString="{0:dd MMM yyyy}" />
                            <asp:CheckBoxField DataField="CheckIn" HeaderText="Check-in" />
                            <asp:CheckBoxField DataField="CheckOut" HeaderText="Check-out" />
                            <asp:TemplateField HeaderText="Operación"><ItemTemplate><div class="control-row-actions"><asp:Button runat="server" Text="Check-in" CommandName="CheckIn" CommandArgument="<%# ((System.Web.UI.WebControls.GridViewRow)Container).RowIndex %>" CssClass="btn control-table-action" /><asp:Button runat="server" Text="Check-out" CommandName="CheckOut" CommandArgument="<%# ((System.Web.UI.WebControls.GridViewRow)Container).RowIndex %>" CssClass="btn btn-outline control-table-action" /></div></ItemTemplate></asp:TemplateField>
                        </Columns>
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="«" LastPageText="»" /><PagerStyle CssClass="grid-pager" />
                    </asp:GridView></div>
                </div>
            </div>
        </section>
    </section>
    <asp:Label ID="lblMensaje" runat="server" CssClass="message control-message" />
</div>
</asp:Content>
