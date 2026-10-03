using System.Text;
using DAL;
using EDL;

namespace BLL
{
    public class UsuarioBLL
    {
        private readonly UsuarioDAL _usuarioDAL;
        public readonly StringBuilder stringBuilder = new StringBuilder();

        public UsuarioBLL()
        {
            _usuarioDAL = new UsuarioDAL();
        }

        public Usuario Login(string username, string password)
        {
            try
            {
                stringBuilder.Clear();

                if (string.IsNullOrEmpty(username))
                {
                    stringBuilder.Append("El usuario es obligatorio.");
                    return null;
                }

                if (string.IsNullOrEmpty(password))
                {
                    stringBuilder.Append("La contraseña es obligatoria.");
                    return null;
                }

                return _usuarioDAL.Login(username, password);
            }
            catch
            {
                stringBuilder.Clear();
                stringBuilder.Append("Error del sistema.");
                return null;
            }
        }
    }
}
