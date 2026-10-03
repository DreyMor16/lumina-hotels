using System;
using System.Collections.Generic;
using DAL;
using EDL;

namespace BLL
{
    public class ReservacionBLL
    {
        HabitacionDisponibleDAL habitacionDAL = new HabitacionDisponibleDAL();
        ReservacionDAL reservacionDAL = new ReservacionDAL();

        public List<HabitacionDisponible> BuscarHabitaciones(DateTime? llegada, DateTime? salida, int? categoria, int? idHotel)
        {
            // Si el usuario no elige fechas, el sistema asume hoy y mañana
            DateTime fechaLlegada = llegada ?? DateTime.Today;
            DateTime fechaSalida = salida ?? DateTime.Today.AddDays(1);

            return habitacionDAL.BuscarHabitaciones(fechaLlegada, fechaSalida, categoria, idHotel);
        }
        public List<Hotel> ListarHoteles()
        {
            return habitacionDAL.ListarHoteles();
        }

        public List<Categoria> ListarCategorias()
        {
            return habitacionDAL.ListarCategorias();
        }
        public void Reservar(string cedula, HabitacionDisponible hab, DateTime llegada, DateTime salida)
        {
       
            int noches = (salida.Date - llegada.Date).Days;

            if (noches <= 0) noches = 1;

            decimal montoEstaHabitacion = noches * hab.Precio;

      
            int idReservaActual = reservacionDAL.ObtenerReservaPendiente(cedula);

            if (idReservaActual == 0)
            {

                Reservacion nueva = new Reservacion
                {
                    CedulaCliente = cedula,
                    MontoTotal = montoEstaHabitacion,
                    Estado = "Pendiente"
                };
                idReservaActual = reservacionDAL.CrearReservacion(nueva);
            }
            else
            {
                // Segunda habitación en adelante: Sumamos al monto existente
                reservacionDAL.ActualizarMontoReserva(idReservaActual, montoEstaHabitacion);
            }

            // Detalle de la habitación (aquí queda guardado cuánto costó CADA UNA)
            Detalle_Reservacion detalle = new Detalle_Reservacion
            {
                IdReservacion = idReservaActual,
                NumHabitacion = hab.NumHabitacion,
                IdHotel = hab.IdHotel,
                FechaLlegada = llegada,
                FechaSalida = salida,
                PrecioNoche = hab.Precio // Precio unitario por noche
            };

            reservacionDAL.CrearDetalle(detalle);
        }

        //Para reporte
        public List<ReservaVista> ObtenerReservasPorCliente(string cedula)
        {
            // Ahora devuelve la lista con categorías y amenidades llenas
            return reservacionDAL.ConsultarReservasPorCliente(cedula);
        }

        public void CancelarHabitacion(int idReservacion, int numHabitacion, int idHotel, decimal montoARestar, DateTime fechaLlegada, DateTime fechaSalida)
        {
            reservacionDAL.CancelarDetalleReserva(idReservacion, numHabitacion, idHotel, montoARestar, fechaLlegada, fechaSalida);
        }


        public int ObtenerReservaPendiente(string cedula)
        {
            return reservacionDAL.ObtenerReservaPendiente(cedula);
        }

        public void PagarReservacion(int idReservacion, string metodo)
        {
            reservacionDAL.FinalizarPago(idReservacion, metodo);
        }

        public List<ReservaVista> ConsultarReservasPorId(int idReserva)
        {
            return reservacionDAL.ConsultarReservasPorId(idReserva);
        }

        //Historial de reservas pagadas
        public List<ReservaVista> ObtenerReservasActualesPagadas(string cedula)
        {
            return reservacionDAL.ConsultarReservasActualesPagadas(cedula);
        }

        public List<ReservaVista> ObtenerHistorialReservas(string cedula)
        {
            return reservacionDAL.ConsultarHistorialReservas(cedula);
        }

  
        public List<ReservaVista> ObtenerReservasPagadas(string cedula)
        {
            if (string.IsNullOrEmpty(cedula)) return new List<ReservaVista>();

            return reservacionDAL.ConsultarReservasPagadas(cedula);
        }
        public List<HabitacionDisponible> BuscarHabitacionesAdmin(DateTime llegada, DateTime salida, int? idCategoria, int? idHotel)
        {
            return BuscarHabitaciones(llegada, salida, idCategoria, idHotel);
        }

        public void ReservarAdmin(string cedula, HabitacionDisponible hab, DateTime llegada, DateTime salida)
        {
            Reservar(cedula, hab, llegada, salida);
        }
        public bool ValidarCliente(string cedula)
        {
            if (string.IsNullOrEmpty(cedula))
                return false;

            return reservacionDAL.ExisteCliente(cedula);
        }
        //check-in y check-out
        public List<ReservaVista> ObtenerReservasPorCedulaHoy(string cedula)
        {
            return reservacionDAL.ObtenerReservasPorCedulaHoy(cedula);
        }
        public void MarcarCheckIn(int idReserva, int numHabitacion, int idHotel, bool estado)
        {
            reservacionDAL.MarcarCheckIn(idReserva, numHabitacion, idHotel, estado);
        }

        public void MarcarCheckOut(int idReserva, int numHabitacion, int idHotel, bool estado)
        {
            reservacionDAL.MarcarCheckOut(idReserva, numHabitacion, idHotel, estado);
        }
    }
}