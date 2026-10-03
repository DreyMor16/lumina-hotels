<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebUIL.Login" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="Accede a tu espacio personal en Lúmina Hotels." />
    <title>Tu espacio · Lúmina Hotels</title>
    <link href="Assets/Css/luxury.css" rel="stylesheet" />
</head>
<body class="auth-body">
<form id="form1" runat="server" autocomplete="on">
    <main class="login-experience">
        <div class="login-noise" aria-hidden="true"></div>
        <header class="login-header">
            <a class="brand brand-light" href="Default.aspx" aria-label="Volver a Lúmina Hotels">
                <span class="brand-mark" aria-hidden="true">L</span><span>Lúmina Hotels</span>
            </a>
            <a class="login-back" href="Default.aspx"><span>←</span> Explorar hoteles</a>
        </header>

        <section class="login-layout">
            <div class="login-story">
                <p class="login-index">LÚMINA / COSTA RICA</p>
                <h1>Tu viaje empieza<br /><em>antes de llegar.</em></h1>
                <p class="login-lead">Un solo acceso para continuar exactamente donde lo dejaste.</p>

                <div class="login-postcards" aria-hidden="true">
                    <figure class="login-postcard postcard-one"><img src="Assets/Images/suite-vista-mar.jpg" alt="" /></figure>
                    <figure class="login-postcard postcard-two"><img src="Assets/Images/destino-bosque-nuboso.jpg" alt="" /></figure>
                    <div class="postcard-note"><span>04</span><p>destinos<br />extraordinarios</p></div>
                </div>
            </div>

            <section class="login-card" aria-labelledby="loginTitle">
                <div class="login-card-top"><span class="login-card-number">01</span><span class="secure-dot">Acceso seguro</span></div>
                <p class="login-kicker">Bienvenido de nuevo</p>
                <h2 id="loginTitle">Entra a Lúmina.</h2>
                <p class="login-intro">Tus reservas, próximos viajes y herramientas de gestión te esperan.</p>

                <div class="form-group login-field">
                    <label for="txtUserWeb">Nombre de usuario</label>
                    <div class="input-shell">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" aria-hidden="true"><circle cx="12" cy="8" r="3.5"/><path d="M5.5 20c.8-4 3-6 6.5-6s5.7 2 6.5 6"/></svg>
                        <asp:TextBox ID="txtUserWeb" runat="server" CssClass="form-control" placeholder="Escribe tu usuario" autocomplete="username" />
                    </div>
                </div>
                <div class="form-group login-field">
                    <label for="txtPassWeb">Contraseña</label>
                    <div class="input-shell">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" aria-hidden="true"><rect x="5" y="10" width="14" height="10" rx="2"/><path d="M8 10V7a4 4 0 0 1 8 0v3M12 14v2"/></svg>
                        <asp:TextBox ID="txtPassWeb" runat="server" CssClass="form-control" TextMode="Password" placeholder="Escribe tu contraseña" autocomplete="current-password" />
                    </div>
                </div>
                <label class="password-toggle"><input type="checkbox" onclick="document.getElementById('<%= txtPassWeb.ClientID %>').type=this.checked?'text':'password'" /><span></span> Mostrar contraseña</label>

                <asp:Button ID="btnEntrarWeb" runat="server" Text="Continuar a mi espacio" OnClick="btnEntrarWeb_Click" CssClass="btn login-submit" />
                <asp:Label ID="lblError" runat="server" CssClass="error-message login-error" role="alert" />

                <div class="login-divider"><span>¿Primera vez en Lúmina?</span></div>
                <a class="signup-link" href="RegistroCliente.aspx">Crear una cuenta <span>↗</span></a>
                <p class="login-legal">Al continuar aceptas nuestros términos de servicio y política de privacidad.</p>
            </section>
        </section>
    </main>
</form>
</body>
</html>
