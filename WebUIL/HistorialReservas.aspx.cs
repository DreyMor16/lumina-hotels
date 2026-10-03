using BLL;
using EDL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace WebUIL
{
    public partial class HistorialReservas : System.Web.UI.Page
    {
        private const int TamanoPagina = 10;
        ReservacionBLL bll = new ReservacionBLL();

        private int PaginaActual
        {
            get { return ViewState["PaginaHistorial"] == null ? 0 : (int)ViewState["PaginaHistorial"]; }
            set { ViewState["PaginaHistorial"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CedulaLogueada"] == null) { Response.Redirect("Login.aspx"); return; }

            if (!IsPostBack)
            {
                CargarCombosFiltros();
                CargarHistorial();
            }
        }

        private void CargarCombosFiltros()
        {
            try
            {
                string cedula = Session["CedulaLogueada"].ToString();
                var todas = bll.ObtenerHistorialReservas(cedula);

                // Hoteles
                ddlFiltroHotel.DataSource = todas.Select(x => x.NombreHotel).Distinct().ToList();
                ddlFiltroHotel.DataBind();
                ddlFiltroHotel.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Todos los hoteles", ""));

                // Años
                ddlFiltroAnio.DataSource = todas.Select(x => x.FechaSalida.Year).Distinct().OrderByDescending(y => y).ToList();
                ddlFiltroAnio.DataBind();
                ddlFiltroAnio.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Todos los años", ""));
            }
            catch (Exception) { }
        }

        private void CargarHistorial()
        {
            string cedula = Session["CedulaLogueada"].ToString();

            // FILTRO BASE: Solo las que ya pasaron la fecha de salida
            List<ReservaVista> lista = bll.ObtenerHistorialReservas(cedula)
                                          .Where(x => x.FechaSalida < DateTime.Now)
                                          .OrderByDescending(x => x.FechaSalida)
                                          .ToList();

            // Aplicar Filtro Hotel
            if (ddlFiltroHotel.SelectedValue != "")
                lista = lista.Where(x => x.NombreHotel == ddlFiltroHotel.SelectedValue).ToList();

            // Aplicar Filtro Mes
            if (ddlFiltroMes.SelectedValue != "0")
                lista = lista.Where(x => x.FechaSalida.Month == Convert.ToInt32(ddlFiltroMes.SelectedValue)).ToList();

            // Aplicar Filtro Año
            if (ddlFiltroAnio.SelectedValue != "")
                lista = lista.Where(x => x.FechaSalida.Year == Convert.ToInt32(ddlFiltroAnio.SelectedValue)).ToList();

            if (lista.Count > 0)
            {
                var pagina = new PagedDataSource
                {
                    DataSource = lista,
                    AllowPaging = true,
                    PageSize = TamanoPagina
                };
                if (PaginaActual >= pagina.PageCount) PaginaActual = Math.Max(0, pagina.PageCount - 1);
                pagina.CurrentPageIndex = PaginaActual;

                rptHistorial.DataSource = pagina;
                rptHistorial.DataBind();
                divCarousel.Visible = true;
                lblMensajeVacio.Visible = false;
                pnlPaginacionHistorial.Visible = pagina.PageCount > 1;
                btnHistorialAnterior.Enabled = !pagina.IsFirstPage;
                btnHistorialSiguiente.Enabled = !pagina.IsLastPage;
                lblPaginaHistorial.Text = string.Format("Página {0} de {1} · {2} registros", PaginaActual + 1, pagina.PageCount, lista.Count);
            }
            else
            {
                divCarousel.Visible = false;
                pnlPaginacionHistorial.Visible = false;
                lblMensajeVacio.Visible = true;
                lblMensajeVacio.Text = "No se encontraron estancias con los filtros seleccionados.";
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            PaginaActual = 0;
            CargarHistorial();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            ddlFiltroHotel.SelectedIndex = 0;
            ddlFiltroMes.SelectedIndex = 0;
            ddlFiltroAnio.SelectedIndex = 0;
            PaginaActual = 0;
            CargarHistorial();
        }

        protected void btnHistorialAnterior_Click(object sender, EventArgs e)
        {
            if (PaginaActual > 0) PaginaActual--;
            CargarHistorial();
        }

        protected void btnHistorialSiguiente_Click(object sender, EventArgs e)
        {
            PaginaActual++;
            CargarHistorial();
        }

        protected void rptHistorial_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "PDF")
            {
                GenerarPDF(Convert.ToInt32(e.CommandArgument));
            }
        }

        private void GenerarPDF(int idReserva)
        {
            // Limpiamos la respuesta antes de empezar
            Response.Clear();

            try
            {
                // 1. Obtención de datos
                var detalles = bll.ConsultarReservasPorId(idReserva);
                if (detalles == null || detalles.Count == 0)
                {
                    throw new Exception("No se encontraron datos para la reserva #" + idReserva);
                }

                // 2. Configuración del documento en memoria
                using (MemoryStream ms = new MemoryStream())
                {
                    Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
                    PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                    doc.Open();

                    // --- FUENTES ---
                    var fontTitulo = FontFactory.GetFont("Arial", 20, Font.BOLD, new BaseColor(44, 62, 80));
                    var fontSubTitulo = FontFactory.GetFont("Arial", 12, Font.BOLD, BaseColor.DARK_GRAY);
                    var fontNormal = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);
                    var fontBold = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
                    var fontHeader = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.WHITE);
                    var fontTotal = FontFactory.GetFont("Arial", 14, Font.BOLD, BaseColor.WHITE);
                    var fontAzul = FontFactory.GetFont("Arial", 11, Font.ITALIC, new BaseColor(52, 152, 219));

                    BaseColor azulByron = new BaseColor(52, 152, 219);

                    // --- ENCABEZADO ---
                    PdfPTable headerTable = new PdfPTable(1) { WidthPercentage = 100 };
                    headerTable.AddCell(new PdfPCell(new Phrase("CADENA HOTELERA LUXURY", fontTitulo)) { Border = Rectangle.NO_BORDER });
                    headerTable.AddCell(new PdfPCell(new Phrase("COMPROBANTE DE ESTADÍA FINALIZADA", fontSubTitulo)) { Border = Rectangle.NO_BORDER, PaddingTop = 5 });
                    doc.Add(headerTable);
                    doc.Add(new Chunk("\n"));

                    // --- INFO DE RESERVA (Recuadro Gris) ---
                    PdfPTable infoTable = new PdfPTable(2) { WidthPercentage = 100 };
                    infoTable.SetWidths(new float[] { 60f, 40f });

                    var cellId = new PdfPCell(new Phrase("RESERVA No: " + idReserva, fontBold)) { BackgroundColor = new BaseColor(245, 245, 245), Padding = 10, BorderColor = BaseColor.LIGHT_GRAY };
                    var cellFecha = new PdfPCell(new Phrase("Emitido el: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fontNormal)) { BackgroundColor = new BaseColor(245, 245, 245), Padding = 10, BorderColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_RIGHT };

                    infoTable.AddCell(cellId);
                    infoTable.AddCell(cellFecha);
                    doc.Add(infoTable);
                    doc.Add(new Chunk("\n"));

                    // --- TABLA DE DETALLES ---
                    PdfPTable table = new PdfPTable(5) { WidthPercentage = 100 };
                    table.SetWidths(new float[] { 30f, 25f, 15f, 15f, 15f });

                    string[] headers = { "Habitación / Categoría", "Hotel", "Entrada", "Salida", "Subtotal" };
                    foreach (string h in headers)
                    {
                        table.AddCell(new PdfPCell(new Phrase(h, fontHeader)) { BackgroundColor = azulByron, HorizontalAlignment = Element.ALIGN_CENTER, Padding = 10, BorderColor = azulByron });
                    }

                    decimal totalFinal = 0;
                    foreach (var item in detalles)
                    {
                        table.AddCell(new PdfPCell(new Phrase("#" + item.NumHabitacion + " - " + item.NombreCategoria, fontNormal)) { Padding = 8, BorderColor = new BaseColor(230, 230, 230) });
                        table.AddCell(new PdfPCell(new Phrase(item.NombreHotel, fontNormal)) { Padding = 8, BorderColor = new BaseColor(230, 230, 230) });
                        table.AddCell(new PdfPCell(new Phrase(item.FechaLlegada.ToString("dd/MM/yyyy"), fontNormal)) { Padding = 8, HorizontalAlignment = Element.ALIGN_CENTER, BorderColor = new BaseColor(230, 230, 230) });
                        table.AddCell(new PdfPCell(new Phrase(item.FechaSalida.ToString("dd/MM/yyyy"), fontNormal)) { Padding = 8, HorizontalAlignment = Element.ALIGN_CENTER, BorderColor = new BaseColor(230, 230, 230) });
                        table.AddCell(new PdfPCell(new Phrase("₡ " + item.SubTotal.ToString("N2"), fontBold)) { Padding = 8, HorizontalAlignment = Element.ALIGN_RIGHT, BorderColor = new BaseColor(230, 230, 230) });
                        totalFinal += item.SubTotal;
                    }

                    // --- FILA DEL TOTAL ---
                    PdfPCell cellLabel = new PdfPCell(new Phrase("TOTAL NETO PAGADO: ", fontBold))
                    {
                        Colspan = 4,
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        PaddingRight = 15,
                        PaddingTop = 15,
                        PaddingBottom = 15,
                        Border = Rectangle.NO_BORDER
                    };
                    table.AddCell(cellLabel);

                    PdfPCell cellMonto = new PdfPCell(new Phrase("₡ " + totalFinal.ToString("N2"), fontTotal))
                    {
                        BackgroundColor = azulByron,
                        BorderColor = azulByron,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 15
                    };
                    table.AddCell(cellMonto);

                    doc.Add(table);

                    // --- PIE DE PÁGINA ---
                    doc.Add(new Chunk("\n\n"));
                    Paragraph pGracias = new Paragraph("¡Gracias por confiar en nosotros para su estancia!", fontAzul) { Alignment = Element.ALIGN_CENTER };
                    doc.Add(pGracias);

                    doc.Close();

                    // 3. Envío del archivo al navegador
                    byte[] bytes = ms.ToArray();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", $"attachment; filename=Comprobante_Reserva_{idReserva}.pdf");
                    Response.Buffer = true;
                    Response.BinaryWrite(bytes);

                    // Terminamos la respuesta de forma limpia
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception ex)
            {
                // En caso de error, limpiamos y enviamos un mensaje al usuario
                Response.Clear();
                Response.ContentType = "text/html";
                string errorScript = $@"<script>alert('Error al generar el PDF: {ex.Message.Replace("'", "")}'); window.history.back();</script>";
                Response.Write(errorScript);
            }
        }
    }
}
