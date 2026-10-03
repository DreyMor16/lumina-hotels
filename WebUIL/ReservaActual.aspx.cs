using BLL;
using EDL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; 
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUIL
{
    public partial class ReservaActual : System.Web.UI.Page
    {
        private const int TamanoPagina = 10;
        ReservacionBLL bll = new ReservacionBLL();

        private int PaginaActual
        {
            get { return ViewState["PaginaActuales"] == null ? 0 : (int)ViewState["PaginaActuales"]; }
            set { ViewState["PaginaActuales"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CedulaLogueada"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarCombosFiltros();
                CargarReservas();
            }
        }

        // Carga los DropDownLists con datos únicos de tus reservas
        private void CargarCombosFiltros()
        {
            try
            {
                string cedula = Session["CedulaLogueada"].ToString();
                List<ReservaVista> todas = bll.ObtenerReservasActualesPagadas(cedula);

                if (todas != null && todas.Count > 0)
                {
                    // Llenar Filtro de Hoteles
                    ddlFiltroHotel.DataSource = todas.Select(x => x.NombreHotel).Distinct().ToList();
                    ddlFiltroHotel.DataBind();
                    ddlFiltroHotel.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Todos los hoteles", ""));

                    // Llenar Filtro de Categorías
                    ddlFiltroCategoria.DataSource = todas.Select(x => x.NombreCategoria).Distinct().ToList();
                    ddlFiltroCategoria.DataBind();
                    ddlFiltroCategoria.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Todas las categorías", ""));
                }
            }
            catch (Exception) { }
        }

        // Método principal de carga con lógica de filtrado
        private void CargarReservas()
        {
            try
            {
                string cedula = Session["CedulaLogueada"].ToString();
                List<ReservaVista> lista = bll.ObtenerReservasActualesPagadas(cedula);

                if (lista != null && lista.Count > 0)
                {
                    // Aplicar Filtro de Hotel
                    if (!string.IsNullOrEmpty(ddlFiltroHotel.SelectedValue))
                    {
                        lista = lista.Where(x => x.NombreHotel == ddlFiltroHotel.SelectedValue).ToList();
                    }

                    // Aplicar Filtro de Categoría
                    if (!string.IsNullOrEmpty(ddlFiltroCategoria.SelectedValue))
                    {
                        lista = lista.Where(x => x.NombreCategoria == ddlFiltroCategoria.SelectedValue).ToList();
                    }

                    // Aplicar Filtro de Fecha
                    if (!string.IsNullOrEmpty(txtFiltroFecha.Text))
                    {
                        DateTime fechaFiltro = DateTime.Parse(txtFiltroFecha.Text);
                        lista = lista.Where(x => x.FechaLlegada.Date == fechaFiltro.Date).ToList();
                    }

                    var pagina = new PagedDataSource
                    {
                        DataSource = lista,
                        AllowPaging = true,
                        PageSize = TamanoPagina
                    };
                    if (PaginaActual >= pagina.PageCount) PaginaActual = Math.Max(0, pagina.PageCount - 1);
                    pagina.CurrentPageIndex = PaginaActual;

                    rptReservasActuales.DataSource = pagina;
                    rptReservasActuales.DataBind();

                    pnlPaginacionActuales.Visible = pagina.PageCount > 1;
                    btnActualesAnterior.Enabled = !pagina.IsFirstPage;
                    btnActualesSiguiente.Enabled = !pagina.IsLastPage;
                    lblPaginaActuales.Text = string.Format("Página {0} de {1} · {2} registros", PaginaActual + 1, pagina.PageCount, lista.Count);

                    // Control de visibilidad
                    bool hayDatos = lista.Count > 0;
                    divCarousel.Visible = hayDatos;
                    lblMensajeVacio.Visible = !hayDatos;
                    if (!hayDatos) lblMensajeVacio.Text = "No se encontraron reservas con los filtros seleccionados.";
                    if (!hayDatos) pnlPaginacionActuales.Visible = false;
                }
                else
                {
                    divCarousel.Visible = false;
                    pnlPaginacionActuales.Visible = false;
                    lblMensajeVacio.Visible = true;
                    lblMensajeVacio.Text = "No tienes estancias pagadas próximas a disfrutar.";
                }
            }
            catch (Exception ex)
            {
                lblMensajeVacio.Visible = true;
                lblMensajeVacio.Text = "Error al cargar las reservas: " + ex.Message;
            }
        }

        // Eventos de botones de filtrado
        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            PaginaActual = 0;
            CargarReservas();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            ddlFiltroHotel.SelectedIndex = 0;
            ddlFiltroCategoria.SelectedIndex = 0;
            txtFiltroFecha.Text = "";
            PaginaActual = 0;
            CargarReservas();
        }

        protected void btnActualesAnterior_Click(object sender, EventArgs e)
        {
            if (PaginaActual > 0) PaginaActual--;
            CargarReservas();
        }

        protected void btnActualesSiguiente_Click(object sender, EventArgs e)
        {
            PaginaActual++;
            CargarReservas();
        }

        // Manejo de comandos del Repeater (Impresión)
        protected void rptReservasActuales_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Imprimir")
            {
                int idReserva = Convert.ToInt32(e.CommandArgument);
                GenerarPDF(idReserva);
            }
        }

        private void GenerarPDF(int idReserva)
        {
            Response.Clear();
            try
            {
                if (Session["CedulaLogueada"] == null) throw new Exception("Sesión expirada.");
                string cedula = Session["CedulaLogueada"].ToString();

                var detalles = bll.ConsultarReservasPorId(idReserva);
                if (detalles == null || detalles.Count == 0) throw new Exception("No hay datos.");

                using (MemoryStream ms = new MemoryStream())
                {
                    Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
                    PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                    doc.Open();

                    // --- FUENTES ---
                    BaseColor verdeExito = new BaseColor(46, 204, 113);
                    var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD, new BaseColor(44, 62, 80));
                    var textFont = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);
                    var boldTextFont = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
                    var headerTableFont = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.WHITE);

                    var totalMontoFont = FontFactory.GetFont("Arial", 12, Font.BOLD, BaseColor.WHITE);

                    // --- ENCABEZADO ---
                    doc.Add(new Paragraph("COMPROBANTE DE ESTADÍA CONFIRMADA", titleFont));
                    doc.Add(new Paragraph("Reserva #: " + idReserva, boldTextFont));
                    doc.Add(new Paragraph("Cliente: " + cedula, textFont));
                    doc.Add(new Chunk("\n"));

                    // --- TABLA DE DETALLES ---
                    PdfPTable table = new PdfPTable(5) { WidthPercentage = 100 };

                    table.SetWidths(new float[] { 30f, 20f, 12f, 13f, 25f });

                    string[] headers = { "Detalle Habitación", "Hotel", "Entrada", "Salida", "Subtotal" };
                    foreach (string h in headers)
                    {
                        table.AddCell(new PdfPCell(new Phrase(h, headerTableFont))
                        {
                            BackgroundColor = verdeExito,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 8,
                            BorderColor = verdeExito
                        });
                    }

                    decimal totalFinal = 0;
                    foreach (var item in detalles)
                    {
                        table.AddCell(new PdfPCell(new Phrase("#" + item.NumHabitacion + " - " + item.NombreCategoria, textFont)) { Padding = 8 });
                        table.AddCell(new PdfPCell(new Phrase(item.NombreHotel, textFont)) { Padding = 8 });
                        table.AddCell(new PdfPCell(new Phrase(item.FechaLlegada.ToString("dd/MM/yyyy"), textFont)) { Padding = 8, HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(item.FechaSalida.ToString("dd/MM/yyyy"), textFont)) { Padding = 8, HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase("₡ " + item.SubTotal.ToString("N2"), boldTextFont)) { Padding = 8, HorizontalAlignment = Element.ALIGN_RIGHT });
                        totalFinal += item.SubTotal;
                    }

                    PdfPCell cellLabelTotal = new PdfPCell(new Phrase("TOTAL NETO PAGADO: ", boldTextFont))
                    {
                        Colspan = 4,
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        PaddingRight = 10,
                        Border = Rectangle.NO_BORDER
                    };
                    table.AddCell(cellLabelTotal);

                    PdfPCell cellMontoTotal = new PdfPCell(new Phrase("₡ " + totalFinal.ToString("N2"), totalMontoFont))
                    {
                        BackgroundColor = verdeExito,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 10,
                        BorderColor = verdeExito,
                        NoWrap = true 
                    };
                    table.AddCell(cellMontoTotal);

                    doc.Add(table);
                    doc.Close();

                    byte[] bytes = ms.ToArray();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", $"attachment; filename=Comprobante_{idReserva}.pdf");
                    Response.BinaryWrite(bytes);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
            }
        }
    }
}
