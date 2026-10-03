<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegistroCliente.aspx.cs" Inherits="WebUIL.RegistroCliente" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Crear cuenta · Lúmina Hotels</title>
    <link href="Assets/Css/luxury.css" rel="stylesheet" />
</head>
<body class="signup-body">
<form id="form1" runat="server">
    <main class="signup-experience">
        <header class="signup-header">
            <a class="brand" href="Default.aspx"><span class="brand-mark">L</span><span>Lúmina Hotels</span></a>
            <p>¿Ya tienes una cuenta? <a href="Login.aspx">Ingresar</a></p>
        </header>
        <div class="signup-layout">
            <aside class="signup-story">
                <div class="signup-story-image"><img src="Assets/Images/destino-bosque-nuboso.jpg" alt="Villa privada entre el bosque nuboso" /></div>
                <div class="signup-story-copy"><span class="story-number">01 — 04</span><p class="eyebrow">Tu colección personal</p><h1>Viajes que se sienten tuyos.</h1><p>Guarda tus datos una vez y dedica el resto del tiempo a imaginar el destino.</p></div>
                <div class="signup-stamp">PURA<br />VIDA</div>
            </aside>
            <section class="signup-card">
                <p class="login-kicker">Comienza tu historia</p>
                <h2>Crea tu cuenta.</h2>
                <p class="signup-intro">Completa tus datos para descubrir disponibilidad y administrar tus estancias.</p>
                <div class="form-grid signup-form-grid">
                    <div class="form-group"><label>Cédula</label><asp:TextBox ID="txtCedula" runat="server" CssClass="form-control" MaxLength="50" placeholder="Identificación" /></div>
                    <div class="form-group"><label>Nombre completo</label><asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" MaxLength="50" placeholder="Nombre y apellidos" /></div>
                    <div class="form-group"><label>Usuario</label><asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" MaxLength="50" placeholder="Elige un usuario" /></div>
                    <div class="form-group"><label>Contraseña</label><asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" MaxLength="50" placeholder="Contraseña segura" /></div>
                    <div class="form-group full"><label>Teléfono</label><div class="inline-field"><asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" MaxLength="8" placeholder="88888888" /><asp:Button ID="btnAgregarTel" runat="server" Text="Agregar" OnClick="btnAgregarTel_Click" CssClass="btn btn-soft" /></div></div>
                    <div class="form-group full"><label>Teléfonos agregados</label><asp:ListBox ID="LstTelefonos" runat="server" CssClass="form-control phone-list" Height="78px" /><asp:Button ID="btnEliminarTel" runat="server" Text="Quitar seleccionado" OnClick="btnEliminarTel_Click" CssClass="text-action" /></div>
                </div>
                <asp:Button ID="btnGuardar" runat="server" Text="Crear mi cuenta" OnClick="btnGuardar_Click" CssClass="btn login-submit" />
                <asp:Label ID="lblMensaje" runat="server" CssClass="error-message login-error" />
                <p class="login-legal">Tus datos se utilizan únicamente para gestionar tus reservas.</p>
            </section>
        </div>
    </main>
</form>
</body>
</html>
