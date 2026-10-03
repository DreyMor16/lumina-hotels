namespace EDL
{
    public class Reservacion
    {
        public int IdReservacion { get; set; }

        public string CedulaCliente { get; set; }

        public string MetodoPago { get; set; }

        public decimal MontoTotal { get; set; }

        public string Estado { get; set; }
    }
}