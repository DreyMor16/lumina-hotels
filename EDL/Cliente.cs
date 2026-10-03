namespace EDL
{
    public class Cliente
    {
        public string Cedula { get; set; }
        public string Nombre { get; set; }

        public Cliente() { }

        public Cliente(string cedula, string nombre)
        {
            Cedula = cedula;
            Nombre = nombre;
        }
    }
}