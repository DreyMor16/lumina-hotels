using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUIL
{
    public partial class HacerReserva : System.Web.UI.Page
    {
        ReservacionBLL logica = new ReservacionBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtLlegada.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtSalida.Text = DateTime.Today.AddDays(7).ToString("yyyy-MM-dd");

                CargarCatalogos();
                CargarGrid();
            }
        }

        private void CargarCatalogos()
        {
            ddlHotel.DataSource = logica.ListarHoteles();
            ddlHotel.DataTextField = "Direccion";
            ddlHotel.DataValueField = "IdHotel";
            ddlHotel.DataBind();
            ddlHotel.Items.Insert(0, new ListItem("-- Seleccione un Hotel --", "0"));

            ddlCategoria.DataSource = logica.ListarCategorias();
            ddlCategoria.DataTextField = "NombreCategoria";
            ddlCategoria.DataValueField = "IdCategoria";
            ddlCategoria.DataBind();
            ddlCategoria.Items.Insert(0, new ListItem("-- Todas las Categorías --", "0"));
        }

        private void CargarGrid()
        {
            try
            {
                lblMensaje.Text = "";

                if (string.IsNullOrEmpty(txtLlegada.Text) || string.IsNullOrEmpty(txtSalida.Text))
                {
                    lblMensaje.Text = "⚠️ Por favor, seleccione un rango de fechas.";
                    lblMensaje.ForeColor = System.Drawing.Color.OrangeRed;
                    return;
                }

                DateTime llegada = Convert.ToDateTime(txtLlegada.Text);
                DateTime salida = Convert.ToDateTime(txtSalida.Text);

                if (llegada.Date >= salida.Date)
                {
                    gvHabitaciones.Visible = false;
                    lblTituloGrid.Text = "❌ Rango de fechas inválido";
                    lblTituloGrid.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60);

                    lblMensaje.Text = "La fecha de salida debe ser posterior al día de llegada.";
                    lblMensaje.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60);
                    return;
                }

                int? idHotel = ddlHotel.SelectedValue != "0" ? (int?)Convert.ToInt32(ddlHotel.SelectedValue) : null;
                int? idCategoria = ddlCategoria.SelectedValue != "0" ? (int?)Convert.ToInt32(ddlCategoria.SelectedValue) : null;

                var lista = logica.BuscarHabitaciones(llegada, salida, idCategoria, idHotel);

                if (lista != null && lista.Count > 0)
                {
                    gvHabitaciones.DataSource = lista;
                    gvHabitaciones.DataBind();
                    gvHabitaciones.Visible = true;

                    lblTituloGrid.Text = "🏨 Habitaciones Disponibles";
                    lblTituloGrid.ForeColor = System.Drawing.Color.FromArgb(44, 62, 80);
                }
                else
                {
                    gvHabitaciones.DataSource = null;
                    gvHabitaciones.DataBind();
                    gvHabitaciones.Visible = false;

                    lblTituloGrid.Text = "❌ No hay disponibilidad para estas fechas";
                    lblTituloGrid.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60);

                    lblMensaje.ForeColor = System.Drawing.Color.Gray;
                    lblMensaje.Text = "Sugerencia: Pruebe iniciando en otra fecha o cambiando el hotel.";
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.ForeColor = System.Drawing.Color.Red;
            }
        }
        protected void gvHabitaciones_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvHabitaciones.PageIndex = e.NewPageIndex;

            CargarGrid();
        }
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvHabitaciones.PageIndex = 0;
            CargarGrid();
        }

        protected void gvHabitaciones_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Reservar")
            {
                try
                {
                    if (Session["CedulaLogueada"] == null)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "mostrarError('Acceso Denegado', 'Debe iniciar sesión para reservar.');", true);
                        return;
                    }

                    int index = Convert.ToInt32(e.CommandArgument);
                    int idHotel = Convert.ToInt32(gvHabitaciones.DataKeys[index].Values["IdHotel"]);
                    decimal precio = Convert.ToDecimal(gvHabitaciones.DataKeys[index].Values["Precio"]);
                    int numHabitacion = Convert.ToInt32(gvHabitaciones.DataKeys[index].Values["NumHabitacion"]);

                    DateTime fLlegada = Convert.ToDateTime(txtLlegada.Text);
                    DateTime fSalida = Convert.ToDateTime(txtSalida.Text);

                    HabitacionDisponible hab = new HabitacionDisponible { NumHabitacion = numHabitacion, IdHotel = idHotel, Precio = precio };
                    string cedula = Session["CedulaLogueada"].ToString();

                    // Ejecutar Reserva
                    logica.Reservar(cedula, hab, fLlegada, fSalida);

                    // POP-UP DE ÉXITO
                    string script = $"mostrarExito('¡Reserva Exitosa!', 'La habitación {numHabitacion} ha sido reservada correctamente.');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "popExito", script, true);

                    CargarGrid();
                }
                catch (Exception ex)
                {
                    string scriptErr = $"mostrarError('Error', '{ex.Message.Replace("'", "")}');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "popError", scriptErr, true);
                }
            }
        }
    }
}
