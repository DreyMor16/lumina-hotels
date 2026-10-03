using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUIL
{
    public partial class AdminReservas : Page
    {
        private readonly ReservacionBLL reservacionBll = new ReservacionBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtLlegadaReserva.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtSalidaReserva.Text = DateTime.Today.AddDays(2).ToString("yyyy-MM-dd");
                CargarCatalogos();
            }
        }

        private void CargarCatalogos()
        {
            try
            {
                ddlHotelReserva.DataSource = reservacionBll.ListarHoteles();
                ddlHotelReserva.DataTextField = "Direccion";
                ddlHotelReserva.DataValueField = "IdHotel";
                ddlHotelReserva.DataBind();
                ddlHotelReserva.Items.Insert(0, new ListItem("Todos los hoteles", "0"));

                ddlCategoriaReserva.DataSource = reservacionBll.ListarCategorias();
                ddlCategoriaReserva.DataTextField = "NombreCategoria";
                ddlCategoriaReserva.DataValueField = "IdCategoria";
                ddlCategoriaReserva.DataBind();
                ddlCategoriaReserva.Items.Insert(0, new ListItem("Todas las categorías", "0"));
            }
            catch (Exception ex) { lblMensaje.Text = "No fue posible cargar los catálogos: " + ex.Message; }
        }

        protected void btnBuscarDisponibilidad_Click(object sender, EventArgs e)
        {
            try
            {
                if (ReferenceEquals(sender, btnBuscarDisponibilidad)) gvDisponibles.PageIndex = 0;
                if (!reservacionBll.ValidarCliente(txtCedulaReserva.Text.Trim()))
                    throw new Exception("La cédula no corresponde a un cliente registrado.");

                DateTime llegada = DateTime.Parse(txtLlegadaReserva.Text);
                DateTime salida = DateTime.Parse(txtSalidaReserva.Text);
                if (salida.Date <= llegada.Date) throw new Exception("La salida debe ser posterior a la llegada.");

                int hotelValue;
                int categoriaValue;
                int? hotel = int.TryParse(ddlHotelReserva.SelectedValue, out hotelValue) && hotelValue > 0 ? (int?)hotelValue : null;
                int? categoria = int.TryParse(ddlCategoriaReserva.SelectedValue, out categoriaValue) && categoriaValue > 0 ? (int?)categoriaValue : null;
                var disponibles = reservacionBll.BuscarHabitacionesAdmin(llegada, salida, categoria, hotel);
                gvDisponibles.DataSource = disponibles;
                gvDisponibles.DataBind();
                lblMensaje.Text = disponibles.Count > 0 ? disponibles.Count + " habitaciones disponibles." : "No hay disponibilidad para esos criterios.";
            }
            catch (Exception ex)
            {
                gvDisponibles.DataSource = null;
                gvDisponibles.DataBind();
                lblMensaje.Text = "Revisa la búsqueda: " + ex.Message;
            }
        }

        protected void gvDisponibles_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDisponibles.PageIndex = e.NewPageIndex;
            btnBuscarDisponibilidad_Click(sender, EventArgs.Empty);
        }

        protected void gvDisponibles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Reservar") return;
            try
            {
                int index = Convert.ToInt32(e.CommandArgument);
                var keys = gvDisponibles.DataKeys[index];
                var habitacion = new HabitacionDisponible
                {
                    NumHabitacion = Convert.ToInt32(keys.Values["NumHabitacion"]),
                    IdHotel = Convert.ToInt32(keys.Values["IdHotel"]),
                    Precio = Convert.ToDecimal(keys.Values["Precio"])
                };
                reservacionBll.ReservarAdmin(txtCedulaReserva.Text.Trim(), habitacion, DateTime.Parse(txtLlegadaReserva.Text), DateTime.Parse(txtSalidaReserva.Text));
                lblMensaje.Text = "Reserva creada correctamente para la habitación " + habitacion.NumHabitacion + ".";
                btnBuscarDisponibilidad_Click(sender, EventArgs.Empty);
            }
            catch (Exception ex) { lblMensaje.Text = "No fue posible reservar: " + ex.Message; }
        }

        protected void btnBuscarEstancia_Click(object sender, EventArgs e)
        {
            gvEstancias.PageIndex = 0;
            CargarEstancias();
        }

        protected void gvEstancias_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEstancias.PageIndex = e.NewPageIndex;
            CargarEstancias();
        }

        private void CargarEstancias()
        {
            try
            {
                List<ReservaVista> estancias = reservacionBll.ObtenerReservasPorCedulaHoy(txtCedulaEstancia.Text.Trim());
                gvEstancias.DataSource = estancias;
                gvEstancias.DataBind();
                lblMensaje.Text = estancias.Count > 0 ? "Estancias de hoy cargadas." : "No hay una llegada o salida programada hoy para este cliente.";
            }
            catch (Exception ex) { lblMensaje.Text = "No fue posible consultar: " + ex.Message; }
        }

        protected void gvEstancias_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "CheckIn" && e.CommandName != "CheckOut") return;
            try
            {
                int index = Convert.ToInt32(e.CommandArgument);
                var keys = gvEstancias.DataKeys[index];
                int reserva = Convert.ToInt32(keys.Values["IdReservacion"]);
                int habitacion = Convert.ToInt32(keys.Values["NumHabitacion"]);
                int hotel = Convert.ToInt32(keys.Values["IdHotel"]);
                if (e.CommandName == "CheckIn") reservacionBll.MarcarCheckIn(reserva, habitacion, hotel, true);
                else reservacionBll.MarcarCheckOut(reserva, habitacion, hotel, true);
                lblMensaje.Text = e.CommandName == "CheckIn" ? "Check-in registrado." : "Check-out registrado.";
                CargarEstancias();
            }
            catch (Exception ex) { lblMensaje.Text = "No fue posible actualizar la estancia: " + ex.Message; }
        }
    }
}
