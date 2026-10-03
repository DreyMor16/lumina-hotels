<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server"><meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" /><meta name="description" content="Descubre hoteles boutique y experiencias únicas en Costa Rica." /><title>Lúmina Hotels · Estancias extraordinarias</title><link href="Assets/Css/luxury.css" rel="stylesheet" /></head>
<body class="public-page">
<div class="page-frame">
    <div class="utility-bar"><span>Reservas directas · Atención personalizada</span><div><span>CRC · Español</span><a href="Login.aspx">Acceso privado</a></div></div>
    <header class="site-header">
        <a class="brand" href="Default.aspx" aria-label="Inicio Lúmina Hotels"><span class="brand-mark" aria-hidden="true"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8"><path d="M4 17c4-1 5-6 8-10 2 4 3 8 8 10"/><path d="M6 17h12M9 20h6"/></svg></span><span>Lúmina Hotels</span></a>
        <button class="menu-toggle" type="button" data-menu-toggle aria-expanded="false" aria-label="Abrir navegación">☰</button>
        <nav class="site-nav" data-site-menu aria-label="Navegación principal"><a href="#destinos">Destinos</a><a href="#hoteles">Hoteles</a><a href="#experiencias">Experiencias</a><a href="#historias">Historias</a><a class="nav-account" href="Login.aspx"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" aria-hidden="true"><circle cx="12" cy="8" r="3.5"/><path d="M5.5 20c.8-4 3-6 6.5-6s5.7 2 6.5 6"/></svg>Ingresar</a></nav>
    </header>
    <main>
        <section class="hero"><div class="hero-copy"><p class="eyebrow">Costa Rica, naturalmente extraordinaria</p><h1>Descubre tu próximo paraíso.</h1><p>Una colección de estancias con diseño, naturaleza y hospitalidad honesta para viajar a tu propio ritmo.</p><div class="hero-rating"><span>★★★★★</span><div><strong>4.9 de 5</strong><small>Experiencias memorables, atención cercana</small></div></div></div></section>
        <form class="booking-panel" method="get" action="Login.aspx">
            <div class="booking-field"><label for="destination">Destino</label><select id="destination" name="destino"><option>Cualquier destino</option><option>Pacífico Central</option><option>Bosque Nuboso</option><option>Valle Central</option><option>Zona Volcánica</option></select></div>
            <div class="booking-field"><label for="arrival">Llegada</label><input id="arrival" name="llegada" type="date" data-arrival /></div>
            <div class="booking-field"><label for="departure">Salida</label><input id="departure" name="salida" type="date" data-departure /></div>
            <div class="booking-field"><label for="guests">Huéspedes</label><select id="guests" name="huespedes"><option>2 huéspedes</option><option>1 huésped</option><option>3 huéspedes</option><option>4 huéspedes</option></select></div>
            <button class="btn btn-sand" type="submit">Buscar estadías</button>
        </form>
        <section class="service-strip" aria-label="Beneficios de reservar directamente">
            <div class="service-item"><span class="service-icon" aria-hidden="true"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7"><path d="M3 12l5 5L21 4"/><path d="M21 12a9 9 0 1 1-5-8"/></svg></span><div><strong>Mejor tarifa directa</strong><small>Sin cargos ocultos ni intermediarios</small></div></div>
            <div class="service-item"><span class="service-icon" aria-hidden="true"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7"><path d="M7 10V7a5 5 0 0 1 10 0v3"/><rect x="4" y="10" width="16" height="10" rx="2"/><path d="M12 14v2"/></svg></span><div><strong>Reserva protegida</strong><small>Tu información permanece segura</small></div></div>
            <div class="service-item"><span class="service-icon" aria-hidden="true"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7"><path d="M4 13a8 8 0 0 1 16 0"/><path d="M4 13v5h3v-5H4Zm13 0v5h3v-5h-3Z"/><path d="M17 19c-1 1-2.5 1.5-5 1.5"/></svg></span><div><strong>Acompañamiento local</strong><small>Atención antes, durante y después</small></div></div>
        </section>
        <section class="section-compact" id="destinos"><div class="container">
            <div class="section-heading"><div><p class="eyebrow">Elige tu paisaje</p><h2 class="editorial-title">Un país, muchas formas de sentirlo.</h2></div><a class="section-link" href="Login.aspx">Explorar disponibilidad →</a></div>
            <div class="destination-strip">
                <article class="destination-card"><img src="Assets/Images/destino-bosque-nuboso.jpg" alt="Villa en bosque nuboso" loading="lazy" /><div class="destination-info"><strong>Bosque Nuboso</strong><span>Refugios entre montañas</span></div></article>
                <article class="destination-card"><img src="Assets/Images/villas-atardecer.jpg" alt="Villas frente al mar al atardecer" loading="lazy" /><div class="destination-info"><strong>Pacífico Azul</strong><span>Villas junto al océano</span></div></article>
                <article class="destination-card"><img src="Assets/Images/experiencia-volcan.jpg" alt="Retiro de bienestar con vista al volcán" loading="lazy" /><div class="destination-info"><strong>Tierras Volcánicas</strong><span>Bienestar y aventura</span></div></article>
                <article class="destination-card"><img src="Assets/Images/hotel-ciudad-colonial.jpg" alt="Hotel boutique de arquitectura colonial" loading="lazy" /><div class="destination-info"><strong>Valle Cultural</strong><span>Diseño, historia y sabor</span></div></article>
            </div>
        </div></section>
        <section class="section section-tint" id="hoteles"><div class="container">
            <div class="section-heading"><div><p class="eyebrow">Selección Lúmina</p><h2 class="editorial-title">Hoteles destacados.</h2></div><p>Cada propiedad combina carácter local, servicio cercano y espacios pensados para recordar.</p></div>
            <div class="hotel-grid">
                <article class="hotel-card"><div class="hotel-image"><img src="Assets/Images/hero-costa-rica.jpg" alt="Lúmina Península Resort" loading="lazy" /><button class="favorite" type="button" aria-label="Agregar a favoritos">♡</button></div><div class="hotel-body"><div class="hotel-meta"><span>Pacífico Central</span><span class="stars">★★★★★</span></div><h3>Lúmina Península Resort</h3><p class="field-note">Piscina infinita · Playa privada · Desayuno</p><div class="hotel-price-row"><span class="price">₡84.900 <small>/ noche</small></span><a class="btn btn-outline" href="Login.aspx">Reservar</a></div></div></article>
                <article class="hotel-card"><div class="hotel-image"><img src="Assets/Images/destino-bosque-nuboso.jpg" alt="Nébula Forest Lodge" loading="lazy" /><button class="favorite" type="button" aria-label="Agregar a favoritos">♡</button></div><div class="hotel-body"><div class="hotel-meta"><span>Monteverde</span><span class="stars">★★★★★</span></div><h3>Nébula Forest Lodge</h3><p class="field-note">Piscina privada · Senderos · Vista panorámica</p><div class="hotel-price-row"><span class="price">₡69.500 <small>/ noche</small></span><a class="btn btn-outline" href="Login.aspx">Reservar</a></div></div></article>
                <article class="hotel-card"><div class="hotel-image"><img src="Assets/Images/hotel-ciudad-colonial.jpg" alt="Casa Ámbar hotel boutique" loading="lazy" /><button class="favorite" type="button" aria-label="Agregar a favoritos">♡</button></div><div class="hotel-body"><div class="hotel-meta"><span>Valle Central</span><span class="stars">★★★★</span></div><h3>Casa Ámbar Boutique</h3><p class="field-note">Patio histórico · Gastronomía · Arte local</p><div class="hotel-price-row"><span class="price">₡52.000 <small>/ noche</small></span><a class="btn btn-outline" href="Login.aspx">Reservar</a></div></div></article>
            </div>
        </div></section>
        <section class="section" id="experiencias"><div class="container">
            <div class="section-heading"><div><p class="eyebrow">Más que una habitación</p><h2 class="editorial-title">Experiencias con sentido.</h2></div><a class="section-link" href="Login.aspx">Ver todas →</a></div>
            <div class="experience-grid"><article class="experience-card"><img src="Assets/Images/experiencia-volcan.jpg" alt="Spa mineral frente a un volcán" /><div class="experience-overlay"><h3>Rituales de tierra y agua</h3><p>Sesiones de bienestar entre jardines tropicales, piedra volcánica y aguas minerales.</p></div></article><div class="experience-stack"><article class="experience-card small"><img src="Assets/Images/suite-vista-mar.jpg" alt="Suite con vista al mar" /><div class="experience-overlay"><h3>Despertar frente al mar</h3><p>Suites abiertas a la luz del Pacífico.</p></div></article><article class="experience-card small"><img src="Assets/Images/villas-atardecer.jpg" alt="Atardecer en villas de playa" /><div class="experience-overlay"><h3>Atardeceres privados</h3><p>La costa, sin prisa y a tu manera.</p></div></article></div></div>
        </div></section>
        <section class="section section-tint" id="historias"><div class="container">
            <div class="section-heading"><div><p class="eyebrow">La voz de nuestros huéspedes</p><h2 class="editorial-title">Historias que regresan.</h2></div></div>
            <div class="testimonial-grid"><article class="quote-card"><p>“Todo se sintió cercano y especial. La vista era increíble, pero el cuidado del equipo fue lo que convirtió el viaje en un recuerdo.”</p><div class="guest"><span class="guest-avatar">AM</span><div><strong>Andrea M.</strong><small>San José, Costa Rica</small></div></div></article><article class="quote-card"><p>“Reservar fue muy sencillo y la habitación era exactamente como la imaginábamos: luminosa, tranquila y rodeada de naturaleza.”</p><div class="guest"><span class="guest-avatar">CR</span><div><strong>Carlos R.</strong><small>Heredia, Costa Rica</small></div></div></article><article class="quote-card"><p>“La mezcla perfecta entre diseño y autenticidad. Volveremos por el spa volcánico y por esas mañanas sin reloj.”</p><div class="guest"><span class="guest-avatar">LV</span><div><strong>Laura V.</strong><small>Alajuela, Costa Rica</small></div></div></article></div>
        </div></section>
    </main>
    <footer class="site-footer"><div><a class="brand" href="Default.aspx" style="color:white"><span class="brand-mark">L</span><span>Lúmina Hotels</span></a><p>Una colección costarricense de hoteles boutique donde cada estancia comienza con un paisaje.</p></div><div><h4>Explora</h4><a href="#destinos">Destinos</a><a href="#hoteles">Hoteles</a><a href="#experiencias">Experiencias</a></div><div><h4>Tu estancia</h4><a href="Login.aspx">Ingresar</a><a href="RegistroCliente.aspx">Crear cuenta</a><a href="Login.aspx">Mis reservas</a></div><div><h4>Información</h4><a href="#historias">Nuestra historia</a><a href="mailto:reservas@lumina.example">Contacto</a><a href="Login.aspx">Acceso privado</a></div></footer>
    <div class="footer-bottom"><span>© 2026 Lúmina Hotels · Costa Rica</span><div><span>Privacidad</span><span>Términos</span><span>Reserva responsable</span></div></div>
</div>
<script src="Assets/Js/site.js"></script>
</body>
</html>
