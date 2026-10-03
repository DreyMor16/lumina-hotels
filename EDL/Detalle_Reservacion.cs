using System;

namespace EDL
{
    public class Detalle_Reservacion
    {
        public int IdReservacion { get; set; }
        public int NumHabitacion { get; set; }
        public int IdHotel { get; set; }
        public DateTime FechaLlegada { get; set; }
        public DateTime FechaSalida { get; set; }
        public decimal PrecioNoche { get; set; }
    }
}