<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PerfilCliente.aspx.cs" Inherits="WebUIL.PerfilCliente" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="client-hero"><div class="client-hero-content">
        <p class="eyebrow">Bienvenido, <%= Server.HtmlEncode(Convert.ToString(Session["Username"])) %></p>
        <h1>La escapada que imaginaste está más cerca.</h1>
        <p>Busca disponibilidad en nuestra colección de hoteles y organiza cada detalle de tu estancia desde un mismo lugar.</p>
        <div class="client-actions"><a class="btn btn-sand" href="HacerReserva.aspx">Encontrar habitación</a><a class="btn btn-outline" href="ReservaActual.aspx" style="color:white;border-color:rgba(255,255,255,.45)">Ver mis estancias</a></div>
    </div></section>
    <section class="portal-section" style="padding-top:0"><div class="content-shell">
        <div class="quick-grid">
            <a class="quick-card" href="HacerReserva.aspx"><span class="quick-icon">⌕</span><div><strong>Nueva reserva</strong><span>Encuentra tu próxima habitación</span></div></a>
            <a class="quick-card" href="MisReservas.aspx"><span class="quick-icon">₡</span><div><strong>Reserva pendiente</strong><span>Revisa y confirma tu pago</span></div></a>
            <a class="quick-card" href="ReservaActual.aspx"><span class="quick-icon">⌂</span><div><strong>Próximas estancias</strong><span>Todo listo para tu llegada</span></div></a>
            <a class="quick-card" href="MiPerfil.aspx"><span class="quick-icon">◎</span><div><strong>Mi perfil</strong><span>Actualiza tus datos y acceso</span></div></a>
        </div>
    </div></section>
    <section class="portal-section" style="padding-top:20px"><div class="content-shell">
        <div class="section-heading"><div><p class="eyebrow">Inspiración para tu viaje</p><h2 class="editorial-title">Tres maneras de desconectar.</h2></div></div>
        <div class="hotel-grid">
            <article class="hotel-card"><div class="hotel-image"><img src="Assets/Images/suite-vista-mar.jpg" alt="Suite con vista al mar" /></div><div class="hotel-body"><span class="field-note">Frente al Pacífico</span><h3>Suite Horizonte</h3><p class="field-note">Luz natural, terraza privada y el océano como despertador.</p></div></article>
            <article class="hotel-card"><div class="hotel-image"><img src="Assets/Images/destino-bosque-nuboso.jpg" alt="Villa en el bosque" /></div><div class="hotel-body"><span class="field-note">Bosque Nuboso</span><h3>Villa Canopy</h3><p class="field-note">Un refugio íntimo sobre el verde, con piscina privada.</p></div></article>
            <article class="hotel-card"><div class="hotel-image"><img src="Assets/Images/experiencia-volcan.jpg" alt="Spa junto al volcán" /></div><div class="hotel-body"><span class="field-note">Zona Volcánica</span><h3>Retiro Mineral</h3><p class="field-note">Aguas termales y rituales de bienestar al atardecer.</p></div></article>
        </div>
    </div></section>
</asp:Content>
