using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUIL
{
    public partial class AdminDashboard : Page
    {
        private readonly HotelBLL hotelBll = new HotelBLL();
        private readonly CategoriaBLL categoriaBll = new CategoriaBLL();
        private readonly HabitacionBLL habitacionBll = new HabitacionBLL();
        private readonly MobiliarioBLL mobiliarioBll = new MobiliarioBLL();
        private readonly CaracteristicaBLL caracteristicaBll = new CaracteristicaBLL();
        private readonly InventarioHabitacionBLL inventarioBll = new InventarioHabitacionBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                lblMensaje.Text = string.Empty;

            if (!IsPostBack)
                CargarTodo();
        }

        private void CargarTodo()
        {
            try
            {
                List<Hotel> hoteles = hotelBll.ListarHoteles();
                List<Categoria> categorias = categoriaBll.ListarCategorias();
                List<Mobiliario> muebles = mobiliarioBll.ListarCatalogo();
                int totalHabitaciones = hoteles.Sum(h => habitacionBll.ListarPorHotel(h.IdHotel).Count);

                lblHoteles.Text = hoteles.Count.ToString();
                lblHabitaciones.Text = totalHabitaciones.ToString();
                lblCategorias.Text = categorias.Count.ToString();
                lblMuebles.Text = muebles.Count.ToString();

                gvHoteles.DataSource = hoteles;
                gvHoteles.DataBind();
                gvCategorias.DataSource = categorias;
                gvCategorias.DataBind();
                gvMuebles.DataSource = muebles;
                gvMuebles.DataBind();

                CargarComboHoteles(ddlHotelHabitacion, hoteles, "Seleccione un hotel");
                CargarComboCategorias(categorias);
                CargarCaracteristicas();
                CargarHabitaciones();

                CargarComboHoteles(ddlHotelInventario, hoteles, "Seleccione una propiedad");
                CargarComboMuebles(muebles);
                LimpiarHabitacionesInventario();
            }
            catch (Exception ex)
            {
                MostrarMensaje("No fue posible cargar los datos: " + ex.Message, true);
            }
        }

        private static void CargarComboHoteles(DropDownList combo, IEnumerable<Hotel> hoteles, string textoInicial)
        {
            combo.DataSource = hoteles;
            combo.DataTextField = "Direccion";
            combo.DataValueField = "IdHotel";
            combo.DataBind();
            combo.Items.Insert(0, new ListItem(textoInicial, "0"));
        }

        private void CargarComboCategorias(IEnumerable<Categoria> categorias)
        {
            ddlCategoriaHabitacion.DataSource = categorias;
            ddlCategoriaHabitacion.DataTextField = "NombreCategoria";
            ddlCategoriaHabitacion.DataValueField = "IdCategoria";
            ddlCategoriaHabitacion.DataBind();
            ddlCategoriaHabitacion.Items.Insert(0, new ListItem("Seleccione una categoría", "0"));
        }

        private void CargarComboMuebles(IEnumerable<Mobiliario> muebles)
        {
            ddlMuebleInventarioCatalogo.DataSource = muebles;
            ddlMuebleInventarioCatalogo.DataTextField = "Descripcion";
            ddlMuebleInventarioCatalogo.DataValueField = "CodMueble";
            ddlMuebleInventarioCatalogo.DataBind();
            ddlMuebleInventarioCatalogo.Items.Insert(0, new ListItem("Seleccione una pieza", "0"));
        }

        private void CargarCaracteristicas()
        {
            cblCaracteristicasHabitacion.DataSource = caracteristicaBll.ListarCatalogo();
            cblCaracteristicasHabitacion.DataTextField = "NombreCaracteristica";
            cblCaracteristicasHabitacion.DataValueField = "IdCaracteristica";
            cblCaracteristicasHabitacion.DataBind();
        }

        private void CargarHabitaciones()
        {
            int idHotel;
            if (int.TryParse(ddlHotelHabitacion.SelectedValue, out idHotel) && idHotel > 0)
                gvHabitaciones.DataSource = habitacionBll.ListarPorHotel(idHotel);
            else
                gvHabitaciones.DataSource = null;
            gvHabitaciones.DataBind();
        }

        private void CargarHabitacionesInventario()
        {
            int idHotel;
            if (!int.TryParse(ddlHotelInventario.SelectedValue, out idHotel) || idHotel <= 0)
            {
                LimpiarHabitacionesInventario();
                return;
            }

            List<Habitacion> habitaciones = habitacionBll.ListarPorHotel(idHotel);
            ddlHabitacionInventario.DataSource = habitaciones;
            ddlHabitacionInventario.DataTextField = "NumHabitacion";
            ddlHabitacionInventario.DataValueField = "NumHabitacion";
            ddlHabitacionInventario.DataBind();
            ddlHabitacionInventario.Items.Insert(0, new ListItem("Seleccione una habitación", "0"));

            ddlHabitacionDestino.DataSource = new List<Habitacion>(habitaciones);
            ddlHabitacionDestino.DataTextField = "NumHabitacion";
            ddlHabitacionDestino.DataValueField = "NumHabitacion";
            ddlHabitacionDestino.DataBind();
            ddlHabitacionDestino.Items.Insert(0, new ListItem("Seleccione el destino", "0"));

            CargarInventarioSeleccionado();
        }

        private void LimpiarHabitacionesInventario()
        {
            ddlHabitacionInventario.Items.Clear();
            ddlHabitacionInventario.Items.Add(new ListItem("Seleccione primero una propiedad", "0"));
            ddlHabitacionDestino.Items.Clear();
            ddlHabitacionDestino.Items.Add(new ListItem("Seleccione primero una propiedad", "0"));
            ddlMuebleTraslado.Items.Clear();
            ddlMuebleTraslado.Items.Add(new ListItem("Seleccione una habitación de origen", "0"));
            gvInventarioHabitacion.DataSource = null;
            gvInventarioHabitacion.DataBind();
        }

        private void CargarInventarioSeleccionado()
        {
            int idHotel;
            int numHabitacion;
            if (!int.TryParse(ddlHotelInventario.SelectedValue, out idHotel) || idHotel <= 0 ||
                !int.TryParse(ddlHabitacionInventario.SelectedValue, out numHabitacion) || numHabitacion <= 0)
            {
                gvInventarioHabitacion.DataSource = null;
                gvInventarioHabitacion.DataBind();
                ddlMuebleTraslado.Items.Clear();
                ddlMuebleTraslado.Items.Add(new ListItem("Seleccione una habitación de origen", "0"));
                return;
            }

            var inventario = inventarioBll.ListarMueblesDeHabitacion(numHabitacion, idHotel);
            gvInventarioHabitacion.DataSource = inventario;
            gvInventarioHabitacion.DataBind();

            ddlMuebleTraslado.DataSource = inventario;
            ddlMuebleTraslado.DataTextField = "descripcion";
            ddlMuebleTraslado.DataValueField = "cod_mueble";
            ddlMuebleTraslado.DataBind();
            ddlMuebleTraslado.Items.Insert(0, new ListItem("Seleccione una pieza", "0"));
        }

        protected void ddlHotelHabitacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvHabitaciones.PageIndex = 0;
            RestablecerHabitacion(false);
            CargarHabitaciones();
            IrA("habitaciones");
        }

        protected void ddlHotelInventario_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvInventarioHabitacion.PageIndex = 0;
            CargarHabitacionesInventario();
            IrA("inventario-habitaciones");
        }

        protected void ddlHabitacionInventario_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvInventarioHabitacion.PageIndex = 0;
            CargarInventarioSeleccionado();
            IrA("inventario-habitaciones");
        }

        protected void gvHoteles_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvHoteles.PageIndex = e.NewPageIndex;
            gvHoteles.DataSource = hotelBll.ListarHoteles();
            gvHoteles.DataBind();
        }

        protected void gvCategorias_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCategorias.PageIndex = e.NewPageIndex;
            gvCategorias.DataSource = categoriaBll.ListarCategorias();
            gvCategorias.DataBind();
        }

        protected void gvHabitaciones_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvHabitaciones.PageIndex = e.NewPageIndex;
            CargarHabitaciones();
        }

        protected void gvMuebles_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMuebles.PageIndex = e.NewPageIndex;
            gvMuebles.DataSource = mobiliarioBll.ListarCatalogo();
            gvMuebles.DataBind();
        }

        protected void gvInventarioHabitacion_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvInventarioHabitacion.PageIndex = e.NewPageIndex;
            CargarInventarioSeleccionado();
        }

        protected void btnGuardarHotel_Click(object sender, EventArgs e)
        {
            EjecutarCrud(() =>
            {
                Hotel hotel = new Hotel
                {
                    IdHotel = int.Parse(txtHotelId.Text),
                    Direccion = txtHotelDireccion.Text.Trim(),
                    Telefono = txtHotelTelefono.Text.Trim()
                };

                if (HotelEditando.HasValue)
                    hotelBll.ActualizarHotel(hotel);
                else
                    hotelBll.GuardarHotel(hotel);
            }, HotelEditando.HasValue ? "Hotel actualizado correctamente." : "Hotel registrado correctamente.", RestablecerHotel, "hoteles");
        }

        protected void btnGuardarCategoria_Click(object sender, EventArgs e)
        {
            EjecutarCrud(() =>
            {
                Categoria categoria = new Categoria
                {
                    IdCategoria = int.Parse(txtCategoriaId.Text),
                    NombreCategoria = txtCategoriaNombre.Text.Trim(),
                    PrecioActual = ParseDecimal(txtCategoriaPrecio.Text)
                };

                if (CategoriaEditando.HasValue)
                    categoriaBll.ActualizarPrecio(categoria);
                else
                    categoriaBll.GuardarCategoria(categoria);
            }, CategoriaEditando.HasValue ? "Categoría actualizada correctamente." : "Categoría registrada correctamente.", RestablecerCategoria, "categorias");
        }

        protected void btnGuardarHabitacion_Click(object sender, EventArgs e)
        {
            EjecutarCrud(() =>
            {
                int idHotel = HabitacionHotelEditando ?? int.Parse(ddlHotelHabitacion.SelectedValue);
                int numero = HabitacionNumeroEditando ?? int.Parse(txtHabitacionNumero.Text);
                Habitacion habitacion = new Habitacion
                {
                    NumHabitacion = numero,
                    IdHotel = idHotel,
                    IdCategoria = int.Parse(ddlCategoriaHabitacion.SelectedValue)
                };

                if (HabitacionNumeroEditando.HasValue)
                    habitacionBll.ActualizarHabitacion(habitacion);
                else
                    habitacionBll.GuardarHabitacion(habitacion);

                caracteristicaBll.ReemplazarCaracteristicas(numero, idHotel, CaracteristicasSeleccionadas());
            }, HabitacionNumeroEditando.HasValue ? "Habitación y características actualizadas." : "Habitación agregada con sus características.", () => RestablecerHabitacion(true), "habitaciones");
        }

        protected void btnGuardarMueble_Click(object sender, EventArgs e)
        {
            EjecutarCrud(() =>
            {
                Mobiliario mueble = new Mobiliario
                {
                    CodMueble = int.Parse(txtMuebleId.Text),
                    Descripcion = txtMuebleDescripcion.Text.Trim(),
                    PrecioMueble = ParseDecimal(txtMueblePrecio.Text)
                };

                if (MuebleEditando.HasValue)
                    mobiliarioBll.ActualizarMueble(mueble);
                else
                    mobiliarioBll.RegistrarMueble(mueble);
            }, MuebleEditando.HasValue ? "Mueble actualizado correctamente." : "Mueble registrado correctamente.", RestablecerMueble, "mobiliario");
        }

        protected void btnAsignarMueble_Click(object sender, EventArgs e)
        {
            try
            {
                InventarioHabitacion inventario = new InventarioHabitacion
                {
                    IdHotel = int.Parse(ddlHotelInventario.SelectedValue),
                    NumHabitacion = int.Parse(ddlHabitacionInventario.SelectedValue),
                    CodMueble = int.Parse(ddlMuebleInventarioCatalogo.SelectedValue),
                    Cantidad = int.Parse(txtCantidadInventario.Text)
                };
                inventarioBll.AsignarMuebleAHabitacion(inventario);
                txtCantidadInventario.Text = "1";
                CargarInventarioSeleccionado();
                MostrarMensaje("Mobiliario asignado. Si ya existía, la cantidad fue sumada.", false);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Revisa la asignación: " + ex.Message, true);
            }
            IrA("inventario-habitaciones");
        }

        protected void btnTrasladarMueble_Click(object sender, EventArgs e)
        {
            try
            {
                inventarioBll.TrasladarMueble(
                    int.Parse(ddlHotelInventario.SelectedValue),
                    int.Parse(ddlHabitacionInventario.SelectedValue),
                    int.Parse(ddlHabitacionDestino.SelectedValue),
                    int.Parse(ddlMuebleTraslado.SelectedValue));
                CargarInventarioSeleccionado();
                MostrarMensaje("El lote de mobiliario fue trasladado correctamente.", false);
            }
            catch (Exception ex)
            {
                MostrarMensaje("No fue posible trasladar la pieza: " + ex.Message, true);
            }
            IrA("inventario-habitaciones");
        }

        protected void gvHoteles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!EsComandoRegistro(e.CommandName)) return;
            int index = Convert.ToInt32(e.CommandArgument);
            DataKey key = gvHoteles.DataKeys[index];
            int id = Convert.ToInt32(key.Values["IdHotel"]);

            if (e.CommandName == "EditarRegistro")
            {
                HotelEditando = id;
                txtHotelId.Text = id.ToString();
                txtHotelDireccion.Text = Convert.ToString(key.Values["Direccion"]);
                txtHotelTelefono.Text = Convert.ToString(key.Values["Telefono"]);
                txtHotelId.Enabled = false;
                lblModoHotel.Text = "EDITANDO PROPIEDAD";
                btnGuardarHotel.Text = "Guardar cambios";
                btnCancelarHotel.Visible = true;
                MostrarMensaje("Editando el hotel " + id + ". El código se mantiene sin cambios.", false);
            }
            else
            {
                EjecutarCrud(() => hotelBll.EliminarHotel(id), "Hotel eliminado correctamente.", RestablecerHotel, "hoteles");
                return;
            }
            IrA("hoteles");
        }

        protected void gvCategorias_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!EsComandoRegistro(e.CommandName)) return;
            int index = Convert.ToInt32(e.CommandArgument);
            DataKey key = gvCategorias.DataKeys[index];
            int id = Convert.ToInt32(key.Values["IdCategoria"]);

            if (e.CommandName == "EditarRegistro")
            {
                CategoriaEditando = id;
                txtCategoriaId.Text = id.ToString();
                txtCategoriaNombre.Text = Convert.ToString(key.Values["NombreCategoria"]);
                txtCategoriaPrecio.Text = Convert.ToDecimal(key.Values["PrecioActual"]).ToString("0.##", CultureInfo.InvariantCulture);
                txtCategoriaId.Enabled = false;
                lblModoCategoria.Text = "EDITANDO CATEGORÍA";
                btnGuardarCategoria.Text = "Guardar cambios";
                btnCancelarCategoria.Visible = true;
                MostrarMensaje("Editando la categoría " + id + ".", false);
            }
            else
            {
                EjecutarCrud(() => categoriaBll.EliminarCategoria(id), "Categoría eliminada correctamente.", RestablecerCategoria, "categorias");
                return;
            }
            IrA("categorias");
        }

        protected void gvHabitaciones_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!EsComandoRegistro(e.CommandName)) return;
            int index = Convert.ToInt32(e.CommandArgument);
            DataKey key = gvHabitaciones.DataKeys[index];
            int numero = Convert.ToInt32(key.Values["NumHabitacion"]);
            int hotel = Convert.ToInt32(key.Values["IdHotel"]);
            int categoria = Convert.ToInt32(key.Values["IdCategoria"]);

            if (e.CommandName == "EditarRegistro")
            {
                HabitacionNumeroEditando = numero;
                HabitacionHotelEditando = hotel;
                ddlHotelHabitacion.SelectedValue = hotel.ToString();
                txtHabitacionNumero.Text = numero.ToString();
                ddlCategoriaHabitacion.SelectedValue = categoria.ToString();
                ddlHotelHabitacion.Enabled = false;
                txtHabitacionNumero.Enabled = false;
                MarcarCaracteristicas(caracteristicaBll.ListarPorHabitacion(numero, hotel));
                lblModoHabitacion.Text = "EDITANDO HABITACIÓN";
                btnGuardarHabitacion.Text = "Guardar cambios";
                btnCancelarHabitacion.Visible = true;
                MostrarMensaje("Editando la habitación " + numero + ". Puedes cambiar categoría y características.", false);
            }
            else
            {
                EjecutarCrud(() => habitacionBll.EliminarHabitacion(numero, hotel), "Habitación eliminada junto con sus características e inventario.", () => RestablecerHabitacion(true), "habitaciones");
                return;
            }
            IrA("habitaciones");
        }

        protected void gvMuebles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!EsComandoRegistro(e.CommandName)) return;
            int index = Convert.ToInt32(e.CommandArgument);
            DataKey key = gvMuebles.DataKeys[index];
            int codigo = Convert.ToInt32(key.Values["CodMueble"]);

            if (e.CommandName == "EditarRegistro")
            {
                MuebleEditando = codigo;
                txtMuebleId.Text = codigo.ToString();
                txtMuebleDescripcion.Text = Convert.ToString(key.Values["Descripcion"]);
                txtMueblePrecio.Text = Convert.ToDecimal(key.Values["PrecioMueble"]).ToString("0.##", CultureInfo.InvariantCulture);
                txtMuebleId.Enabled = false;
                lblModoMueble.Text = "EDITANDO PIEZA";
                btnGuardarMueble.Text = "Guardar cambios";
                btnCancelarMueble.Visible = true;
                MostrarMensaje("Editando el mueble " + codigo + ".", false);
            }
            else
            {
                EjecutarCrud(() => mobiliarioBll.EliminarMueble(codigo), "Mueble eliminado del catálogo.", RestablecerMueble, "mobiliario");
                return;
            }
            IrA("mobiliario");
        }

        protected void gvInventarioHabitacion_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "RetirarRegistro") return;
            try
            {
                int index = Convert.ToInt32(e.CommandArgument);
                int codigo = Convert.ToInt32(gvInventarioHabitacion.DataKeys[index].Value);
                inventarioBll.EliminarMuebleDeHabitacion(
                    int.Parse(ddlHabitacionInventario.SelectedValue),
                    int.Parse(ddlHotelInventario.SelectedValue),
                    codigo);
                CargarInventarioSeleccionado();
                MostrarMensaje("La pieza fue retirada de la habitación.", false);
            }
            catch (Exception ex)
            {
                MostrarMensaje("No fue posible retirar la pieza: " + ex.Message, true);
            }
            IrA("inventario-habitaciones");
        }

        protected void btnCancelarHotel_Click(object sender, EventArgs e)
        {
            RestablecerHotel();
            MostrarMensaje("Edición de hotel cancelada.", false);
            IrA("hoteles");
        }

        protected void btnCancelarCategoria_Click(object sender, EventArgs e)
        {
            RestablecerCategoria();
            MostrarMensaje("Edición de categoría cancelada.", false);
            IrA("categorias");
        }

        protected void btnCancelarHabitacion_Click(object sender, EventArgs e)
        {
            RestablecerHabitacion(false);
            MostrarMensaje("Edición de habitación cancelada.", false);
            IrA("habitaciones");
        }

        protected void btnCancelarMueble_Click(object sender, EventArgs e)
        {
            RestablecerMueble();
            MostrarMensaje("Edición de mobiliario cancelada.", false);
            IrA("mobiliario");
        }

        private IEnumerable<int> CaracteristicasSeleccionadas()
        {
            return cblCaracteristicasHabitacion.Items.Cast<ListItem>()
                .Where(item => item.Selected)
                .Select(item => int.Parse(item.Value))
                .ToList();
        }

        private void MarcarCaracteristicas(IEnumerable<Caracteristica> seleccionadas)
        {
            var ids = new HashSet<int>(seleccionadas.Select(c => c.IdCaracteristica));
            foreach (ListItem item in cblCaracteristicasHabitacion.Items)
                item.Selected = ids.Contains(int.Parse(item.Value));
        }

        private void EjecutarCrud(Action action, string success, Action reset, string anchor)
        {
            try
            {
                action();
                reset();
                gvHoteles.PageIndex = 0;
                gvCategorias.PageIndex = 0;
                gvHabitaciones.PageIndex = 0;
                gvMuebles.PageIndex = 0;
                CargarTodo();
                MostrarMensaje(success, false);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Revisa la información: " + ex.Message, true);
            }
            IrA(anchor);
        }

        private void RestablecerHotel()
        {
            HotelEditando = null;
            txtHotelId.Text = string.Empty;
            txtHotelDireccion.Text = string.Empty;
            txtHotelTelefono.Text = string.Empty;
            txtHotelId.Enabled = true;
            lblModoHotel.Text = "NUEVA PROPIEDAD";
            btnGuardarHotel.Text = "Agregar propiedad";
            btnCancelarHotel.Visible = false;
        }

        private void RestablecerCategoria()
        {
            CategoriaEditando = null;
            txtCategoriaId.Text = string.Empty;
            txtCategoriaNombre.Text = string.Empty;
            txtCategoriaPrecio.Text = string.Empty;
            txtCategoriaId.Enabled = true;
            lblModoCategoria.Text = "NUEVA CATEGORÍA";
            btnGuardarCategoria.Text = "Guardar categoría";
            btnCancelarCategoria.Visible = false;
        }

        private void RestablecerHabitacion(bool limpiarHotel)
        {
            HabitacionNumeroEditando = null;
            HabitacionHotelEditando = null;
            txtHabitacionNumero.Text = string.Empty;
            txtHabitacionNumero.Enabled = true;
            ddlHotelHabitacion.Enabled = true;
            if (limpiarHotel && ddlHotelHabitacion.Items.Count > 0)
                ddlHotelHabitacion.SelectedIndex = 0;
            if (ddlCategoriaHabitacion.Items.Count > 0)
                ddlCategoriaHabitacion.SelectedIndex = 0;
            foreach (ListItem item in cblCaracteristicasHabitacion.Items)
                item.Selected = false;
            lblModoHabitacion.Text = "NUEVA HABITACIÓN";
            btnGuardarHabitacion.Text = "Agregar habitación";
            btnCancelarHabitacion.Visible = false;
        }

        private void RestablecerMueble()
        {
            MuebleEditando = null;
            txtMuebleId.Text = string.Empty;
            txtMuebleDescripcion.Text = string.Empty;
            txtMueblePrecio.Text = string.Empty;
            txtMuebleId.Enabled = true;
            lblModoMueble.Text = "NUEVA PIEZA";
            btnGuardarMueble.Text = "Registrar pieza";
            btnCancelarMueble.Visible = false;
        }

        private static bool EsComandoRegistro(string commandName)
        {
            return commandName == "EditarRegistro" || commandName == "EliminarRegistro";
        }

        private static decimal ParseDecimal(string value)
        {
            decimal parsed;
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out parsed) ||
                decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out parsed))
                return parsed;
            throw new Exception("El precio debe ser un número válido.");
        }

        private void MostrarMensaje(string texto, bool esError)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = esError ? "message control-message error" : "message control-message success";
        }

        private void IrA(string anchor)
        {
            ClientScript.RegisterStartupScript(GetType(), "admin-anchor", "window.location.hash='" + anchor + "';", true);
        }

        private int? HotelEditando
        {
            get { return ViewState["HotelEditando"] == null ? (int?)null : Convert.ToInt32(ViewState["HotelEditando"]); }
            set { ViewState["HotelEditando"] = value; }
        }

        private int? CategoriaEditando
        {
            get { return ViewState["CategoriaEditando"] == null ? (int?)null : Convert.ToInt32(ViewState["CategoriaEditando"]); }
            set { ViewState["CategoriaEditando"] = value; }
        }

        private int? HabitacionNumeroEditando
        {
            get { return ViewState["HabitacionNumeroEditando"] == null ? (int?)null : Convert.ToInt32(ViewState["HabitacionNumeroEditando"]); }
            set { ViewState["HabitacionNumeroEditando"] = value; }
        }

        private int? HabitacionHotelEditando
        {
            get { return ViewState["HabitacionHotelEditando"] == null ? (int?)null : Convert.ToInt32(ViewState["HabitacionHotelEditando"]); }
            set { ViewState["HabitacionHotelEditando"] = value; }
        }

        private int? MuebleEditando
        {
            get { return ViewState["MuebleEditando"] == null ? (int?)null : Convert.ToInt32(ViewState["MuebleEditando"]); }
            set { ViewState["MuebleEditando"] = value; }
        }
    }
}
