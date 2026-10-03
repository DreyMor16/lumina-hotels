using BLL;
using EDL;
using System;
using System.Web;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace WebUIL
{
    public partial class MisReservas : System.Web.UI.Page
    {
        private const int TamanoPagina = 10;
        ReservacionBLL logica = new ReservacionBLL();

        private int PaginaActual
        {
            get { return ViewState["PaginaReservas"] == null ? 0 : (int)ViewState["PaginaReservas"]; }
            set { ViewState["PaginaReservas"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["CedulaLogueada"] == null)
                {
                    Response.Redirect("Login.aspx");
                }
                if (Session["UltimaReservaPagada"] != null)
                {
                    pnlDescargaPDF.Visible = true;
                }
                else
                {
                    pnlDescargaPDF.Visible = false;
                }
                CargarTarjetas();
            }
        }

        private void CargarTarjetas()
        {
            if (Session["CedulaLogueada"] == null) return;

            string cedula = Session["CedulaLogueada"].ToString();
            var lista = logica.ObtenerReservasPorCliente(cedula);

            if (lista != null && lista.Count > 0)
            {
                var pagina = new PagedDataSource
                {
                    DataSource = lista,
                    AllowPaging = true,
                    PageSize = TamanoPagina
                };
                if (PaginaActual >= pagina.PageCount) PaginaActual = Math.Max(0, pagina.PageCount - 1);
                pagina.CurrentPageIndex = PaginaActual;

                rptReservas.DataSource = pagina;
                rptReservas.DataBind();

                pnlPaginacionReservas.Visible = pagina.PageCount > 1;
                btnReservasAnterior.Enabled = !pagina.IsFirstPage;
                btnReservasSiguiente.Enabled = !pagina.IsLastPage;
                lblPaginaReservas.Text = string.Format("Página {0} de {1} · {2} registros", PaginaActual + 1, pagina.PageCount, lista.Count);

                reservasCarousel.Visible = true;
                carouselIndicators.Visible = true;
                rptReservas.Visible = true;

                decimal granTotal = 0;
                foreach (var item in lista)
                {
                    granTotal += item.SubTotal;
                }

                lblMontoTotal.Text = granTotal.ToString("N2");
                pnlTotal.Visible = true;
                lblMensajeVacio.Visible = false;
            }
            else
            {
                rptReservas.Visible = false;
                reservasCarousel.Visible = false;
                carouselIndicators.Visible = false;
                pnlTotal.Visible = false;
                pnlPaginacionReservas.Visible = false;
                lblMensajeVacio.Visible = true;
                lblMensajeVacio.Text = "No tienes habitaciones reservadas actualmente.";
            }
        }

        protected void btnReservasAnterior_Click(object sender, EventArgs e)
        {
            if (PaginaActual > 0) PaginaActual--;
            CargarTarjetas();
        }

        protected void btnReservasSiguiente_Click(object sender, EventArgs e)
        {
            PaginaActual++;
            CargarTarjetas();
        }

        protected void rptReservas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Cancelar")
            {
                try
                {
                    string[] datos = e.CommandArgument.ToString().Split('|');

                    int idReserva = Convert.ToInt32(datos[0]);
                    int numHabitacion = Convert.ToInt32(datos[1]);
                    int idHotel = Convert.ToInt32(datos[2]);
                    decimal subTotal = Convert.ToDecimal(datos[3]);

                    DateTime fechaLlegada = Convert.ToDateTime(datos[4]);
                    DateTime fechaSalida = Convert.ToDateTime(datos[5]);

                    logica.CancelarHabitacion(idReserva, numHabitacion, idHotel, subTotal, fechaLlegada, fechaSalida);

                    ScriptManager.RegisterStartupScript(this, GetType(), "pop",
                        "mostrarNotificacion('¡Listo!', 'Habitación removida.', 'success');", true);

                    CargarTarjetas();
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "popError",
                        $"mostrarNotificacion('Error', '{ex.Message.Replace("'", "")}', 'error');", true);
                }
            }
        }

        protected void btnConfirmarPago_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["CedulaLogueada"] == null) return;

                string cedula = Session["CedulaLogueada"].ToString();
                string metodo = ddlMetodoPago.SelectedValue;

                int idReserva = logica.ObtenerReservaPendiente(cedula);

                if (idReserva > 0)
                {
                    logica.PagarReservacion(idReserva, metodo);

                    // Guardamos el ID en sesión para que el botón de PDF sepa qué imprimir
                    Session["UltimaReservaPagada"] = idReserva;

                    
                    ScriptManager.RegisterStartupScript(this, GetType(), "pop",
                        "mostrarNotificacion('¡Pago Exitoso!', 'Su comprobante está listo para descargar.', 'success'); " +
                        "setTimeout(function(){ window.location='MisReservas.aspx'; }, 2000);", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "popError",
                        "mostrarNotificacion('Atención', 'No hay reserva pendiente.', 'warning');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "popError",
                    $"mostrarNotificacion('Error', '{ex.Message.Replace("'", "")}', 'error');", true);
            }
        }
        protected void btnDescargarPDF_Click(object sender, EventArgs e)
        {
            if (Session["UltimaReservaPagada"] == null) return;

            try
            {
                int idReserva = (int)Session["UltimaReservaPagada"];
                string cedula = Session["CedulaLogueada"].ToString();
                var detalles = logica.ConsultarReservasPorId(idReserva);

                // 1. Configuración del Documento
                Document doc = new Document(PageSize.A4, 50, 50, 50, 50);
                MemoryStream ms = new MemoryStream();
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                doc.Open();

                // 2. Estilos de Fuente
                var titleFont = FontFactory.GetFont("Arial", 18, iTextSharp.text.Font.BOLD, new BaseColor(44, 62, 80));
                var subTitleFont = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY);
                var textFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                var boldTextFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                var amenitiesFont = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.ITALIC, new BaseColor(52, 152, 219));
                var headerTableFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                // 3. Encabezado
                doc.Add(new Paragraph("COMPROBANTE OFICIAL DE RESERVACIÓN", titleFont));
                doc.Add(new Paragraph("Cliente: " + cedula, textFont));
                doc.Add(new Paragraph("Fecha de pago: " + DateTime.Now.ToString("f"), textFont));
                doc.Add(new Chunk("\n"));

                // 4. Tabla de Detalles
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 30f, 20f, 15f, 15f, 20f });

                string[] headers = { "Habitación / Servicios", "Hotel", "Llegada", "Salida", "Estadía" };
                foreach (string h in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(h, headerTableFont));
                    cell.BackgroundColor = new BaseColor(44, 62, 80);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 8;
                    table.AddCell(cell);
                }

                decimal totalFinal = 0;
                foreach (var item in detalles)
                {
                    Phrase phHab = new Phrase();
                    phHab.Add(new Chunk("#" + item.NumHabitacion + " - " + item.NombreCategoria + "\n", boldTextFont));
                    phHab.Add(new Chunk("✨ " + item.Amenidades, amenitiesFont));
                    table.AddCell(new PdfPCell(phHab) { Padding = 8 });

                    table.AddCell(new PdfPCell(new Phrase(item.NombreHotel, textFont)) { Padding = 8 });
                    table.AddCell(new PdfPCell(new Phrase(item.FechaLlegada.ToString("dd/MM/yyyy"), textFont)) { Padding = 8, HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.FechaSalida.ToString("dd/MM/yyyy"), textFont)) { Padding = 8, HorizontalAlignment = Element.ALIGN_CENTER });

                    int noches = (item.FechaSalida - item.FechaLlegada).Days;
                    Phrase phPrecio = new Phrase();
                    phPrecio.Add(new Chunk(noches + (noches == 1 ? " Noche\n" : " Noches\n"), textFont));

                   
                    phPrecio.Add(new Chunk("₡ " + item.SubTotal.ToString("N2"), boldTextFont));

                    PdfPCell cellPrecio = new PdfPCell(phPrecio) { Padding = 8, HorizontalAlignment = Element.ALIGN_RIGHT, BackgroundColor = new BaseColor(245, 245, 245) };
                    table.AddCell(cellPrecio);

                    totalFinal += item.SubTotal;
                }
                doc.Add(table);

                // 5. Total Final
                PdfPTable totalTable = new PdfPTable(1);
                totalTable.WidthPercentage = 100;

                
                PdfPCell totalCell = new PdfPCell(new Phrase("TOTAL NETO PAGADO: ₡ " + totalFinal.ToString("N2"), titleFont));
                totalCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                totalCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                totalCell.PaddingTop = 20;
                totalTable.AddCell(totalCell);
                doc.Add(totalTable);

                // 6. Notas
                doc.Add(new Chunk("\n\n"));
                doc.Add(new Paragraph("INFORMACIÓN IMPORTANTE:", subTitleFont));
                doc.Add(new Paragraph("• Presente este comprobante al momento del Check-in (3:00 PM).", textFont));
                doc.Add(new Paragraph("• El Check-out debe ser antes de las 11:00 AM.", textFont));

                doc.Close();

                // --- LIMPIEZA DE SESIÓN ---
                // Esto hace que el panel desaparezca para la próxima vez
                Session.Remove("UltimaReservaPagada");

                byte[] bytes = ms.ToArray();
                ms.Close();

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment; filename=Comprobante_Reserva.pdf");
                Response.Buffer = true;
                Response.BinaryWrite(bytes);
                Response.End();
            }
            catch (Exception)
            {
            }
        }
    }
}
