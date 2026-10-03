<%@ Page Title="Reservar" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="HacerReserva.aspx.cs" Inherits="WebUIL.HacerReserva" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .results-title { display:block;margin:30px 0 12px;font:500 1.65rem Georgia,serif; }
        .toast { position:fixed;z-index:100;right:22px;bottom:22px;display:none;max-width:360px;padding:18px 20px;color:#fff;background:#17364a;border-radius:16px;box-shadow:0 20px 55px rgba(0,0,0,.22); }
        .toast.visible { display:block;animation:toast-in .25s ease; } .toast.error { background:#9b4242; }
        .toast strong { display:block;margin-bottom:4px; } .toast span { opacity:.82;font-size:.82rem; }
        @keyframes toast-in { from { transform:translateY(10px);opacity:0 } to { transform:none;opacity:1 } }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="portal-banner booking-banner" style="background-image:linear-gradient(90deg,rgba(8,39,54,.78),rgba(8,39,54,.1)),url('Assets/Images/suite-vista-mar.jpg')"><div><p class="eyebrow">Encuentra tu lugar</p><h1>Una habitación para cada historia.</h1><p>Selecciona tus fechas y preferencias. Te mostraremos únicamente las habitaciones realmente disponibles.</p></div></section>
    <section class="portal-section"><div class="content-shell">
        <div class="page-intro"><div><p class="eyebrow">Disponibilidad en tiempo real</p><h1>Planifica tu estancia.</h1><p>Compara ubicaciones, categorías, servicios y precio por noche.</p></div><a class="btn btn-outline" href="PerfilCliente.aspx">Volver al inicio</a></div>
        <div class="surface">
            <div class="search-surface">
                <div class="form-group"><label>Fecha de llegada</label><asp:TextBox ID="txtLlegada" runat="server" CssClass="form-control" TextMode="Date" /></div>
                <div class="form-group"><label>Fecha de salida</label><asp:TextBox ID="txtSalida" runat="server" CssClass="form-control" TextMode="Date" /></div>
                <div class="form-group"><label>Hotel</label><asp:DropDownList ID="ddlHotel" runat="server" CssClass="form-control" /></div>
                <div class="form-group"><label>Categoría</label><asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-control" /></div>
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar habitaciones" OnClick="btnBuscar_Click" CssClass="btn" />
            </div>
            <asp:Label ID="lblTituloGrid" runat="server" CssClass="results-title" />
            <asp:GridView ID="gvHabitaciones" runat="server" AutoGenerateColumns="False" CssClass="grid-view" GridLines="None"
                DataKeyNames="NumHabitacion,IdHotel,Precio" AllowPaging="True" PageSize="10"
                OnPageIndexChanging="gvHabitaciones_PageIndexChanging" OnRowCommand="gvHabitaciones_RowCommand">
                <Columns>
                    <asp:BoundField DataField="NumHabitacion" HeaderText="Habitación" />
                    <asp:BoundField DataField="DireccionHotel" HeaderText="Hotel / ubicación" />
                    <asp:BoundField DataField="NombreCategoria" HeaderText="Categoría" />
                    <asp:BoundField DataField="Caracteristicas" HeaderText="Servicios incluidos" NullDisplayText="Servicios básicos" />
                    <asp:BoundField DataField="Precio" HeaderText="Precio por noche" DataFormatString="₡ {0:N2}" HtmlEncode="false" />
                    <asp:TemplateField HeaderText="Acción"><ItemTemplate><asp:Button runat="server" Text="Elegir habitación" CommandName="Reservar" CommandArgument="<%# ((System.Web.UI.WebControls.GridViewRow)Container).RowIndex %>" CssClass="btn" /></ItemTemplate></asp:TemplateField>
                </Columns>
                <PagerSettings Mode="NumericFirstLast" FirstPageText="«" LastPageText="»" /><PagerStyle HorizontalAlign="Center" CssClass="grid-pager" />
                <EmptyDataTemplate><div style="padding:35px;text-align:center;color:#728690">No encontramos habitaciones para estos criterios.</div></EmptyDataTemplate>
            </asp:GridView>
            <asp:Label ID="lblMensaje" runat="server" CssClass="message" />
        </div>
    </div></section>
    <div id="toast" class="toast" role="status"><strong id="toastTitle"></strong><span id="toastText"></span></div>
    <script>
        function showToast(title, text, error) {
            var toast = document.getElementById('toast');
            document.getElementById('toastTitle').textContent = title;
            document.getElementById('toastText').textContent = text;
            toast.className = 'toast visible' + (error ? ' error' : '');
            window.setTimeout(function(){ toast.className = 'toast'; }, 4200);
        }
        function mostrarExito(titulo, texto) { showToast(titulo, texto, false); }
        function mostrarError(titulo, texto) { showToast(titulo, texto, true); }
    </script>
</asp:Content>
