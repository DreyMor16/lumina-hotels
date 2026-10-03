namespace EDL
{
    public class Habitacion
    {
        public int NumHabitacion { get; set; }
        public int IdHotel { get; set; }
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
        public string CaracteristicasTexto { get; set; } 

        public Habitacion() { }

        public Habitacion(int num, int idHotel, int idCat)
        {
            NumHabitacion = num;
            IdHotel = idHotel;
            IdCategoria = idCat;
        }
    }
}