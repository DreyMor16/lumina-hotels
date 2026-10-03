<%@ Page Title="Mi reserva" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MisReservas.aspx.cs" Inherits="WebUIL.MisReservas" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="portal-banner"><div><p class="eyebrow">Antes de viajar</p><h1>Revisa y confirma tu reserva.</h1><p>Comprueba las fechas, servicios y monto de cada habitación antes de completar el pago.</p></div></section>
    <section class="portal-section"><div class="content-shell">
        <div class="page-intro"><div><p class="eyebrow">Resumen de compra</p><h1>Habitaciones seleccionadas.</h1><p>Puedes retirar una habitación mientras la reserva continúe pendiente.</p></div><a class="btn btn-outline" href="HacerReserva.aspx">Agregar habitación</a></div>
        <asp:Panel ID="pnlDescargaPDF" runat="server" CssClass="success-panel" Visible="false">
            <h3 style="margin:0 0 6px">Pago confirmado</h3><p style="margin:0 0 14px">Tu comprobante oficial ya está disponible.</p>
            <asp:LinkButton ID="btnDescargarPDF" runat="server" CssClass="btn" OnClick="btnDescargarPDF_Click">Descargar comprobante PDF</asp:LinkButton>
        </asp:Panel>
        <asp:Label ID="lblMensajeVacio" runat="server" CssClass="empty-state" Visible="false" />
        <div id="reservasCarousel" class="reservation-grid" runat="server">
            <asp:Repeater ID="rptReservas" runat="server" OnItemCommand="rptReservas_ItemCommand">
                <ItemTemplate>
                    <article class="reservation-card">
                        <div class="reservation-image"><div class="reservation-image-content"><strong><%# Eval("NombreHotel") %></strong><span><%# Eval("NombreCategoria") %> · Habitación <%# Eval("NumHabitacion") %></span></div></div>
                        <div class="reservation-body">
                            <div class="reservation-info">
                                <div><span>Cliente</span><strong><%# Eval("CedulaCliente") %></strong></div>
                                <div><span>Noches</span><strong><%# Eval("TotalDias") %></strong></div>
                                <div><span>Llegada</span><strong><%# Convert.ToDateTime(Eval("FechaLlegada")).ToString("dd MMM yyyy") %></strong></div>
                                <div><span>Salida</span><strong><%# Convert.ToDateTime(Eval("FechaSalida")).ToString("dd MMM yyyy") %></strong></div>
                            </div>
                            <div class="amenities">Incluye: <%# Eval("Amenidades") %></div>
                            <div class="reservation-total"><div><span class="field-note">₡ <%# Convert.ToDecimal(Eval("PrecioNoche")).ToString("N2") %> por noche</span><strong>₡ <%# Convert.ToDecimal(Eval("SubTotal")).ToString("N2") %></strong></div>
                                <asp:LinkButton ID="btnQuitar" runat="server" CssClass="btn btn-danger" CommandName="Cancelar" CommandArgument='<%# Eval("IdReservacion") + "|" + Eval("NumHabitacion") + "|" + Eval("IdHotel") + "|" + Eval("SubTotal") + "|" + Eval("FechaLlegada") + "|" + Eval("FechaSalida") %>' OnClientClick="return confirmarCancelacion();">Quitar</asp:LinkButton>
                            </div>
                        </div>
                    </article>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <asp:Panel ID="pnlPaginacionReservas" runat="server" CssClass="card-pager" Visible="false">
            <asp:LinkButton ID="btnReservasAnterior" runat="server" CssClass="pager-button" OnClick="btnReservasAnterior_Click">← Anterior</asp:LinkButton>
            <asp:Label ID="lblPaginaReservas" runat="server" CssClass="pager-status" />
            <asp:LinkButton ID="btnReservasSiguiente" runat="server" CssClass="pager-button" OnClick="btnReservasSiguiente_Click">Siguiente →</asp:LinkButton>
        </asp:Panel>
        <asp:Panel ID="pnlTotal" runat="server" CssClass="payment-summary" Visible="false">
            <div><span class="field-note" style="color:rgba(255,255,255,.65)">Total de la reserva</span><div class="total">₡ <asp:Label ID="lblMontoTotal" runat="server" Text="0.00" /></div></div>
            <div class="payment-actions"><div class="form-group" style="margin:0"><label style="color:rgba(255,255,255,.72)">Método de pago</label><asp:DropDownList ID="ddlMetodoPago" runat="server" CssClass="form-control"><asp:ListItem Text="Tarjeta crédito/débito" Value="Tarjeta" /><asp:ListItem Text="Efectivo" Value="Efectivo" /></asp:DropDownList></div><asp:Button ID="btnConfirmarPago" runat="server" Text="Confirmar pago" CssClass="btn btn-sand" OnClick="btnConfirmarPago_Click" /></div>
        </asp:Panel>
        <div id="carouselIndicators" runat="server"></div>
    </div></section>
    <div id="notice" class="toast-message" role="status"></div>
    <script>
        function confirmarCancelacion() { return window.confirm('¿Deseas quitar esta habitación? El total se actualizará automáticamente.'); }
        function mostrarNotificacion(titulo, mensaje, icono) {
            var box = document.getElementById('notice');
            box.textContent = titulo + ' · ' + mensaje;
            box.className = 'toast-message visible' + (icono === 'error' ? ' error' : '');
            window.setTimeout(function(){ box.className = 'toast-message'; }, 4200);
        }
    </script>
</asp:Content>
