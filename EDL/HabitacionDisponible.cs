namespace EDL
{
    public class HabitacionDisponible
    {
        public int NumHabitacion { get; set; }
        public int IdHotel { get; set; }
        public string DireccionHotel { get; set; } 
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
        public decimal Precio { get; set; }
        public string Caracteristicas { get; set; } 
    }
}
