namespace EDL
{
    public class TelefonoCliente
    {
        public int IdTelefono { get; set; }
        public string CedulaCliente { get; set; }
        public string Telefono { get; set; }

        public TelefonoCliente() { }

        public TelefonoCliente(string cedulaCliente, string telefono)
        {
            CedulaCliente = cedulaCliente;
            Telefono = telefono;
        }
    }
}