<%@ Page Title="Panel de gestión" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="WebUIL.AdminDashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Content2" ContentPlaceHolderID="AdminContent" runat="server">
<div class="control-page">
    <section class="control-overview-hero" aria-labelledby="controlHeroTitle">
        <div class="control-hero-heading">
            <span class="control-eyebrow">CONSERJERÍA Y GESTIÓN CREATIVA</span>
            <h1 id="controlHeroTitle">Navegador de<br />estancias Lúmina</h1>
            <p>Una vista serena de cada propiedad, habitación y detalle que sostiene la experiencia del huésped.</p>
        </div>
        <span class="control-live"><i></i> Operación en línea</span>
        <aside class="control-float-card control-stats-board" aria-label="Resumen de inventario">
            <div class="control-card-heading"><div><span>COLECCIÓN ACTIVA</span><h2>Inventario vivo</h2></div><span class="control-card-icon">⌁</span></div>
            <div class="control-stat-grid">
                <div class="control-stat"><span>Hoteles</span><asp:Label ID="lblHoteles" runat="server" Text="0" /><small>propiedades</small></div>
                <div class="control-stat"><span>Habitaciones</span><asp:Label ID="lblHabitaciones" runat="server" Text="0" /><small>espacios</small></div>
                <div class="control-stat"><span>Categorías</span><asp:Label ID="lblCategorias" runat="server" Text="0" /><small>experiencias</small></div>
                <div class="control-stat"><span>Mobiliario</span><asp:Label ID="lblMuebles" runat="server" Text="0" /><small>referencias</small></div>
            </div>
        </aside>
        <aside class="control-float-card control-action-board" aria-label="Acciones rápidas">
            <div class="control-card-heading"><div><span>ACCESOS DIRECTOS</span><h2>Mesa de hoy</h2></div><span class="control-card-icon sand">✦</span></div>
            <div class="control-action-list">
                <a href="AdminReservas.aspx"><span class="control-action-icon">＋</span><span><strong>Nueva reservación</strong><small>Asistir a un huésped</small></span><i>→</i></a>
                <a href="#habitaciones"><span class="control-action-icon">⌂</span><span><strong>Gestionar habitación</strong><small>Editar espacios y atributos</small></span><i>→</i></a>
                <a href="#inventario-habitaciones"><span class="control-action-icon">◇</span><span><strong>Asignar mobiliario</strong><small>Equipar o trasladar piezas</small></span><i>→</i></a>
            </div>
        </aside>
        <div class="control-hero-signature"><span>LÚMINA</span><small>COSTA RICA · PURA VIDA</small></div>
    </section>

    <section class="control-workspace">
        <div class="control-workspace-heading">
            <div><span class="control-section-kicker">MESA DE GESTIÓN</span><h2>Todo en su lugar.</h2><p>Crea, actualiza y elimina registros desde módulos separados y fáciles de revisar.</p></div>
            <nav class="control-anchor-nav" aria-label="Secciones de gestión">
                <a href="#hoteles">Hoteles</a><a href="#categorias">Categorías</a><a href="#habitaciones">Habitaciones</a><a href="#mobiliario">Catálogo</a><a href="#inventario-habitaciones">Asignaciones</a>
            </nav>
        </div>

        <section class="control-module" id="hoteles">
            <header class="control-module-head"><span class="control-module-number">01</span><div><span>PROPIEDADES</span><h2>Directorio de hoteles</h2><p>Registra, corrige o retira las propiedades de la colección Lúmina.</p></div><span class="control-module-status"><i></i> CRUD completo</span></header>
            <div class="control-module-layout">
                <div class="control-form-card">
                    <asp:Label ID="lblModoHotel" runat="server" CssClass="control-form-label" Text="NUEVA PROPIEDAD" />
                    <div class="form-group"><label>Código del hotel</label><asp:TextBox ID="txtHotelId" runat="server" CssClass="form-control" TextMode="Number" placeholder="Ej. 06" /></div>
                    <div class="form-group"><label>Teléfono</label><asp:TextBox ID="txtHotelTelefono" runat="server" CssClass="form-control" MaxLength="12" placeholder="22223333" /></div>
                    <div class="form-group"><label>Dirección / nombre comercial</label><asp:TextBox ID="txtHotelDireccion" runat="server" CssClass="form-control" placeholder="Lúmina Pacífico · Puntarenas" /></div>
                    <div class="control-form-actions"><asp:Button ID="btnGuardarHotel" runat="server" Text="Agregar propiedad" CssClass="btn control-primary" OnClick="btnGuardarHotel_Click" /><asp:Button ID="btnCancelarHotel" runat="server" Text="Cancelar edición" CssClass="btn control-cancel-edit" Visible="false" CausesValidation="false" OnClick="btnCancelarHotel_Click" /></div>
                </div>
                <div class="control-table-card">
                    <div class="control-table-heading"><div><strong>Propiedades registradas</strong><span>Selecciona Editar para cargar los datos en el formulario · 10 por página</span></div><span class="control-table-badge">Colección</span></div>
                    <div class="control-table-scroll"><asp:GridView ID="gvHoteles" runat="server" AutoGenerateColumns="False" CssClass="grid-view control-table" GridLines="None" DataKeyNames="IdHotel,Direccion,Telefono" AllowPaging="True" PageSize="10" OnPageIndexChanging="gvHoteles_PageIndexChanging" OnRowCommand="gvHoteles_RowCommand"><Columns><asp:BoundField DataField="IdHotel" HeaderText="Código" /><asp:BoundField DataField="Direccion" HeaderText="Propiedad / dirección" /><asp:BoundField DataField="Telefono" HeaderText="Teléfono" /><asp:TemplateField HeaderText="Acciones"><ItemTemplate><div class="control-row-actions"><asp:Button runat="server" Text="Editar" CommandName="EditarRegistro" CommandArgument="<%# ((System.Web.UI.WebControls.GridViewRow)Container).RowIndex %>" CssClass="control-row-link" CausesValidation="false" /><asp:Button runat="server" Text="Eliminar" CommandName="EliminarRegistro" CommandArgument="<%# ((System.Web.UI.WebControls.GridViewRow)Container).RowIndex %>" CssClass="control-row-link danger" CausesValidation="false" OnClientClick="return confirm('¿Eliminar este hotel? Solo será posible si no tiene habitaciones asociadas.');" /></div></ItemTemplate></asp:TemplateField></Columns><PagerSettings Mode="NumericFirstLast" FirstPageText="«" LastPageText="»" /><PagerStyle CssClass="grid-pager" /></asp:GridView></div>
                </div>
            </div>
        </section>

        <section class="control-module" id="categorias">
            <header class="control-module-head"><span class="control-module-number">02</span><div><span>CATEGORÍAS Y TARIFAS</span><h2>Niveles de estancia</h2><p>Define la experiencia de cada habitación y mantén sus tarifas actualizadas.</p></div><span class="control-module-status quiet">CRUD completo</span></header>
            <div class="control-module-layout">
                <div class="control-form-card">
                    <asp:Label ID="lblModoCategoria" runat="server" CssClass="control-form-label" Text="NUEVA CATEGORÍA" />
                    <div class="form-group"><label>Código</label><asp:TextBox ID="txtCategoriaId" runat="server" CssClass="form-control" TextMode="Number" /></div>
                    <div class="form-group"><label>Nombre de categoría</label><asp:TextBox ID="txtCategoriaNombre" runat="server" CssClass="form-control" placeholder="Suite Horizonte" /></div>
                    <div class="form-group"><label>Precio por noche</label><asp:TextBox ID="txtCategoriaPrecio" runat="server" CssClass="form-control" TextMode="Number" /></div>
                    <div class="control-form-actions"><asp:Button ID="btnGuardarCategoria" runat="server" Text="Guardar categoría" CssClass="btn control-primary" OnClick="btnGuardarCategoria_Click" /><asp:Button ID="btnCancelarCategoria" runat="server" Text="Cancelar edición" CssClass="btn control-cancel-edit" Visible="false" CausesValidation="false" OnClick="btnCancelarCategoria_Click" /></div>
                </div>
                <div class="control-table-card">
                    <div class="control-table-heading"><div><strong>Categorías disponibles</strong><span>Edita nombre y tarifa desde la misma ficha · 10 por página</span></div><span class="control-table-badge gold">Tarifas</span></div>
                    <div class="control-table-scroll"><asp:GridView ID="gvCategorias" runat="server" AutoGenerateColumns="False" CssClass="grid-view control-table" GridLines="None" DataKeyNames="IdCategoria,NombreCategoria,PrecioActual" AllowPaging="True" PageSize="10" OnPageIndexChanging="gvCategorias_PageIndexChanging" OnRowCommand="gvCategorias_RowCommand"><Columns><asp:BoundField DataField="IdCategoria" HeaderText="ID" /><asp:BoundField DataField="NombreCategoria" HeaderText="Categoría" /><asp:BoundField DataField="PrecioActual" HeaderText="Precio" DataFormatString="₡ {0:N2}" /><asp:TemplateField HeaderText="Acciones"><ItemTemplate><div class="control-row-actions"><asp:Button runat="server" Text="Editar" CommandName="EditarRegistro" CommandArgument="<%# ((System.Web.UI.WebControls.GridViewRow)Container).RowIndex %>" CssClass="control-row-link" CausesValidation="false" /><asp:Button runat="server" Text="Eliminar" CommandName="EliminarRegistro" CommandArgument="<%# ((System.Web.UI.WebControls.GridViewRow)Container).RowIndex %>" CssClass="control-row-link danger" CausesValidation="false" OnClientClick="return confirm('¿Eliminar esta categoría? Solo será posible si no está asignada a habitaciones.');" /></div></ItemTemplate></asp:TemplateField></Columns><PagerSettings Mode="NumericFirstLast" FirstPageText="«" LastPageText="»" /><PagerStyle CssClass="grid-pager" /></asp:GridView></div>
                </div>
            </div>
        </section>

        <section class="control-module" id="habitaciones">
            <header class="control-module-head"><span class="control-module-number">03</span><div><span>HABITACIONES</span><h2>Mapa de espacios</h2><p>Administra categoría y características sin perder la relación con cada propiedad.</p></div><span class="control-module-status quiet">Espacios y atributos</span></header>
            <div class="control-module-layout">
                <div class="control-form-card">
                    <asp:Label ID="lblModoHabitacion" runat="server" CssClass="control-form-label" Text="NUEVA HABITACIÓN" />
                    <div class="form-group"><label>Propiedad</label><asp:DropDownList ID="ddlHotelHabitacion" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlHotelHabitacion_SelectedIndexChanged" /></div>
                    <div class="form-group"><label>Número de habitación</label><asp:TextBox ID="txtHabitacionNumero" runat="server" CssClass="form-control" TextMode="Number" /></div>
                    <div class="form-group"><label>Categoría</label><asp:DropDownList ID="ddlCategoriaHabitacion" runat="server" CssClass="form-control" /></div>
                    <div class="form-group"><label>Características</label><asp:CheckBoxList ID="cblCaracteristicasHabitacion" runat="server" CssClass="control-check-list" RepeatLayout="UnorderedList" /></div>
                    <div class="control-form-actions"><asp:Button ID="btnGuardarHabitacion" runat="server" Text="Agregar habitación" CssClass="btn control-primary" OnClick="btnGuardarHabitacion_Click" /><asp:Button ID="btnCancelarHabitacion" runat="server" Text="Cancelar edición" CssClass="btn control-cancel-edit" Visible="false" CausesValidation="false" OnClick="btnCancelarHabitacion_Click" /></div>
                </div>
                <div class="control-table-card">
                    <div class="control-table-heading"><div><strong>Habitaciones de la propiedad</strong><span>Selecciona un hotel para consultar y editar · 10 por página</span></div><span class="control-table-badge">Espacios</span></div>
                    <div class="control-table-scroll"><asp:GridView ID="gvHabitaciones" runat="server" AutoGenerateColumns="False" CssClass="grid-view control-table" GridLines="None" DataKeyNames="NumHabitacion,IdHotel,IdCategoria" AllowPaging="True" PageSize="10" OnPageIndexChanging="gvHabitaciones_PageIndexChanging" OnRowCommand="gvHabitaciones_RowCommand"><Columns><asp:BoundField DataField="NumHabitacion" HeaderText="Habitación" /><asp:BoundField DataField="NombreCategoria" HeaderText="Categoría" /><asp:BoundField DataField="CaracteristicasTexto" HeaderText="Características" /><asp:TemplateField HeaderText="Acciones"><ItemTemplate><div class="control-row-actions"><asp:Button runat="server" Text="Editar" CommandName="EditarRegistro" CommandArgument="<%# ((System.Web.UI.WebControls.GridViewRow)Container).RowIndex %>" CssClass="control-row-link" CausesValidation="false" /><asp:Button runat="server" Text="Eliminar" CommandName="EliminarRegistro" CommandArgument="<%# ((System.Web.UI.WebControls.GridViewRow)Container).RowIndex %>" CssClass="control-row-link danger" CausesValidation="false" OnClientClick="return confirm('¿Eliminar esta habitación y sus características e inventario? No se eliminará si tiene reservaciones.');" /></div></ItemTemplate></asp:TemplateField></Columns><PagerSettings Mode="NumericFirstLast" FirstPageText="«" LastPageText="»" /><PagerStyle CssClass="grid-pager" /></asp:GridView></div>
                </div>
            </div>
        </section>

        <section class="control-module" id="mobiliario">
            <header class="control-module-head"><span class="control-module-number">04</span><div><span>CATÁLOGO DE MOBILIARIO</span><h2>Objetos con intención</h2><p>Conserva un catálogo limpio antes de distribuir las piezas entre habitaciones.</p></div><span class="control-module-status quiet">CRUD completo</span></header>
            <div class="control-module-layout">
                <div class="control-form-card">
                    <asp:Label ID="lblModoMueble" runat="server" CssClass="control-form-label" Text="NUEVA PIEZA" />
                    <div class="form-group"><label>Código</label><asp:TextBox ID="txtMuebleId" runat="server" CssClass="form-control" TextMode="Number" /></div>
                    <div class="form-group"><label>Valor</label><asp:TextBox ID="txtMueblePrecio" runat="server" CssClass="form-control" TextMode="Number" /></div>
                    <div class="form-group"><label>Descripción</label><asp:TextBox ID="txtMuebleDescripcion" runat="server" CssClass="form-control" placeholder="Sillón de lectura en madera y lino" /></div>
                    <div class="control-form-actions"><asp:Button ID="btnGuardarMueble" runat="server" Text="Registrar pieza" CssClass="btn control-primary" OnClick="btnGuardarMueble_Click" /><asp:Button ID="btnCancelarMueble" runat="server" Text="Cancelar edición" CssClass="btn control-cancel-edit" Visible="false" CausesValidation="false" OnClick="btnCancelarMueble_Click" /></div>
                </div>
                <div class="control-table-card">
                    <div class="control-table-heading"><div><strong>Catálogo de mobiliario</strong><span>Edita descripción y valor o retira piezas sin asignaciones · 10 por página</span></div><span class="control-table-badge gold">Catálogo</span></div>
                    <div class="control-table-scroll"><asp:GridView ID="gvMuebles" runat="server" AutoGenerateColumns="False" CssClass="grid-view control-table" GridLines="None" DataKeyNames="CodMueble,Descripcion,PrecioMueble" AllowPaging="True" PageSize="10" OnPageIndexChanging="gvMuebles_PageIndexChanging" OnRowCommand="gvMuebles_RowCommand"><Columns><asp:BoundField DataField="CodMueble" HeaderText="Código" /><asp:BoundField DataField="Descripcion" HeaderText="Descripción" /><asp:BoundField DataField="PrecioMueble" HeaderText="Valor" DataFormatString="₡ {0:N2}" /><asp:TemplateField HeaderText="Acciones"><ItemTemplate><div class="control-row-actions"><asp:Button runat="server" Text="Editar" CommandName="EditarRegistro" CommandArgument="<%# ((System.Web.UI.WebControls.GridViewRow)Container).RowIndex %>" CssClass="control-row-link" CausesValidation="false" /><asp:Button runat="server" Text="Eliminar" CommandName="EliminarRegistro" CommandArgument="<%# ((System.Web.UI.WebControls.GridViewRow)Container).RowIndex %>" CssClass="control-row-link danger" CausesValidation="false" OnClientClick="return confirm('¿Eliminar este mueble del catálogo? Solo será posible si no está asignado.');" /></div></ItemTemplate></asp:TemplateField></Columns><PagerSettings Mode="NumericFirstLast" FirstPageText="«" LastPageText="»" /><PagerStyle CssClass="grid-pager" /></asp:GridView></div>
                </div>
            </div>
        </section>

        <section class="control-module" id="inventario-habitaciones">
            <header class="control-module-head"><span class="control-module-number">05</span><div><span>INVENTARIO POR HABITACIÓN</span><h2>Cada pieza, en su lugar</h2><p>Asigna cantidades, consulta el reporte de una habitación y traslada mobiliario sin duplicar registros.</p></div><span class="control-module-status"><i></i> Operación de inventario</span></header>
            <div class="control-module-layout">
                <div class="control-form-card">
                    <span class="control-form-label">ASIGNAR MOBILIARIO</span>
                    <div class="form-group"><label>Propiedad</label><asp:DropDownList ID="ddlHotelInventario" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlHotelInventario_SelectedIndexChanged" /></div>
                    <div class="form-group"><label>Habitación</label><asp:DropDownList ID="ddlHabitacionInventario" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlHabitacionInventario_SelectedIndexChanged" /></div>
                    <div class="form-group"><label>Pieza del catálogo</label><asp:DropDownList ID="ddlMuebleInventarioCatalogo" runat="server" CssClass="form-control" /></div>
                    <div class="form-group"><label>Cantidad a agregar</label><asp:TextBox ID="txtCantidadInventario" runat="server" CssClass="form-control" TextMode="Number" Text="1" /></div>
                    <asp:Button ID="btnAsignarMueble" runat="server" Text="Asignar a la habitación" CssClass="btn control-primary" OnClick="btnAsignarMueble_Click" />
                </div>
                <div class="control-table-card">
                    <div class="control-table-heading"><div><strong>Reporte de la habitación</strong><span>Existencias actuales · 10 por página</span></div><span class="control-table-badge">Inventario</span></div>
                    <div class="control-table-scroll"><asp:GridView ID="gvInventarioHabitacion" runat="server" AutoGenerateColumns="False" CssClass="grid-view control-table" GridLines="None" DataKeyNames="cod_mueble" AllowPaging="True" PageSize="10" OnPageIndexChanging="gvInventarioHabitacion_PageIndexChanging" OnRowCommand="gvInventarioHabitacion_RowCommand"><Columns><asp:BoundField DataField="cod_mueble" HeaderText="Código" /><asp:BoundField DataField="descripcion" HeaderText="Pieza" /><asp:BoundField DataField="cantidad" HeaderText="Cantidad" /><asp:TemplateField HeaderText="Acción"><ItemTemplate><asp:Button runat="server" Text="Retirar" CommandName="RetirarRegistro" CommandArgument="<%# ((System.Web.UI.WebControls.GridViewRow)Container).RowIndex %>" CssClass="control-row-link danger" CausesValidation="false" OnClientClick="return confirm('¿Retirar esta pieza de la habitación?');" /></ItemTemplate></asp:TemplateField></Columns><PagerSettings Mode="NumericFirstLast" FirstPageText="«" LastPageText="»" /><PagerStyle CssClass="grid-pager" /></asp:GridView></div>
                    <div class="control-transfer-panel">
                        <div><span class="control-form-label">TRASLADAR TODO EL LOTE</span><p>Mueve una pieza y su cantidad completa desde la habitación seleccionada hacia otra de la misma propiedad.</p></div>
                        <div class="control-transfer-fields"><div class="form-group"><label>Pieza en la habitación</label><asp:DropDownList ID="ddlMuebleTraslado" runat="server" CssClass="form-control" /></div><div class="form-group"><label>Habitación destino</label><asp:DropDownList ID="ddlHabitacionDestino" runat="server" CssClass="form-control" /></div><asp:Button ID="btnTrasladarMueble" runat="server" Text="Trasladar pieza" CssClass="btn control-secondary" OnClick="btnTrasladarMueble_Click" /></div>
                    </div>
                </div>
            </div>
        </section>
    </section>
    <asp:Label ID="lblMensaje" runat="server" CssClass="message control-message" />
</div>
</asp:Content>
