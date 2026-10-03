using System;

namespace EDL
{
    public class ReservaVista
    {
        public int IdReservacion { get; set; }
        public int NumHabitacion { get; set; }
        public int IdHotel { get; set; }
        public string NombreHotel { get; set; } 
        public DateTime FechaLlegada { get; set; }
        public DateTime FechaSalida { get; set; }
        public decimal PrecioNoche { get; set; }
        public int TotalDias { get; set; }
        public decimal SubTotal { get; set; }
        public string NombreCategoria { get; set; }
        public string Amenidades { get; set; }
        public string CedulaCliente { get; set; }
        public bool CheckIn { get; set; }
        public bool CheckOut { get; set; }
    }
}