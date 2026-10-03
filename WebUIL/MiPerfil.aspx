<%@ Page Title="Mi perfil" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MiPerfil.aspx.cs" Inherits="WebUIL.MiPerfil" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="portal-section"><div class="content-shell">
        <div class="page-intro"><div><p class="eyebrow">Cuenta personal</p><h1>Mi perfil.</h1><p>Mantén actualizados tus datos de contacto y seguridad.</p></div><a class="btn btn-outline" href="PerfilCliente.aspx">Volver al inicio</a></div>
        <div class="profile-layout">
            <aside class="profile-aside"><div class="profile-avatar">◎</div><h2>Tu espacio, siempre listo.</h2><p>Una cuenta actualizada nos ayuda a darte una llegada más ágil y una comunicación más cercana.</p></aside>
            <div class="surface">
                <asp:Panel ID="pnlVista" runat="server">
                    <div class="panel-head"><div><h2>Datos personales</h2><p>Información vinculada a tus reservas.</p></div></div>
                    <div class="profile-data">
                        <div class="profile-data-item"><span>Cédula</span><asp:Label ID="lblCedulaVista" runat="server" /></div>
                        <div class="profile-data-item"><span>Nombre completo</span><asp:Label ID="lblNombreVista" runat="server" /></div>
                        <div class="profile-data-item"><span>Nombre de usuario</span><asp:Label ID="lblUsuarioVista" runat="server" /></div>
                    </div>
                    <div style="display:flex;flex-wrap:wrap;gap:10px"><asp:Button ID="btnIrModificar" runat="server" Text="Editar información" CssClass="btn" OnClick="btnIrModificar_Click" /><asp:Button ID="btnIrPassword" runat="server" Text="Cambiar contraseña" CssClass="btn btn-outline" OnClick="btnIrPassword_Click" /></div>
                </asp:Panel>
                <asp:Panel ID="pnlModificarDatos" runat="server" Visible="false">
                    <div class="panel-head"><div><h2>Editar información</h2><p>Revisa cuidadosamente los datos antes de guardar.</p></div></div>
                    <div class="form-grid">
                        <div class="form-group"><label>Cédula</label><asp:TextBox ID="txtCedula" runat="server" CssClass="form-control" ReadOnly="true" /></div>
                        <div class="form-group"><label>Nombre completo</label><asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" /></div>
                        <div class="form-group full"><label>Usuario</label><asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" /></div>
                        <div class="form-group full"><label>Agregar teléfono</label><div style="display:grid;grid-template-columns:1fr auto;gap:10px"><asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" placeholder="88888888" /><asp:Button ID="btnAgregarTel" runat="server" Text="Agregar" OnClick="btnAgregarTel_Click" CssClass="btn btn-soft" /></div></div>
                        <div class="form-group full"><label>Teléfonos asociados</label><asp:ListBox ID="LstTelefonos" runat="server" CssClass="form-control" Height="100px" /><asp:Button ID="btnEliminarTel" runat="server" Text="Quitar seleccionado" OnClick="btnEliminarTel_Click" CssClass="btn btn-outline" style="margin-top:9px" /></div>
                    </div>
                    <div style="display:flex;flex-wrap:wrap;gap:10px"><asp:Button ID="btnGuardar" runat="server" Text="Guardar cambios" CssClass="btn" OnClick="btnGuardarDatos_Click" /><asp:LinkButton ID="btnVolver1" runat="server" Text="Cancelar" OnClick="btnVolver_Click" CssClass="btn btn-outline" /></div>
                </asp:Panel>
                <asp:Panel ID="pnlModificarPass" runat="server" Visible="false">
                    <div class="panel-head"><div><h2>Seguridad y acceso</h2><p>Utiliza una contraseña que no uses en otros sitios.</p></div></div>
                    <div class="form-group"><label>Contraseña actual</label><asp:TextBox ID="txtPasswordActual" runat="server" CssClass="form-control" TextMode="Password" /></div>
                    <div class="form-group"><label>Nueva contraseña</label><asp:TextBox ID="txtPasswordNueva" runat="server" CssClass="form-control" TextMode="Password" /></div>
                    <div style="display:flex;flex-wrap:wrap;gap:10px"><asp:Button ID="btnGuardarPass" runat="server" Text="Actualizar contraseña" CssClass="btn" OnClick="btnGuardarPass_Click" /><asp:LinkButton ID="btnVolver2" runat="server" Text="Cancelar" OnClick="btnVolver_Click" CssClass="btn btn-outline" /></div>
                </asp:Panel>
                <asp:Label ID="lblMensaje" runat="server" CssClass="message" />
            </div>
        </div>
    </div></section>
</asp:Content>
