using System;
using System.Web;
using System.Web.UI;

namespace WebUIL
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtUserWeb.Text = string.Empty;
                txtPassWeb.Text = string.Empty;

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();

                if (string.Equals(Request.QueryString["reason"], "role", StringComparison.OrdinalIgnoreCase))
                {
                    lblError.Text = "Inicia sesión nuevamente para continuar.";
                }
            }
        }

        protected void btnEntrarWeb_Click(object sender, EventArgs e)
        {
            string username = txtUserWeb.Text.Trim();
            string password = txtPassWeb.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblError.Text = "Ingresa tu usuario y contraseña para continuar.";
                return;
            }

            try
            {
                BLL.UsuarioBLL logica = new BLL.UsuarioBLL();
                var usuario = logica.Login(username, password);

                if (usuario == null)
                {
                    lblError.Text = "No encontramos una cuenta con esas credenciales.";
                    return;
                }

                Session["CedulaLogueada"] = usuario.CedulaCliente;
                Session["Username"] = usuario.Username;
                Session["IdRol"] = usuario.IdRol;
                Response.Redirect(usuario.IdRol == 1 ? "AdminDashboard.aspx" : "PerfilCliente.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception)
            {
                lblError.Text = "No fue posible conectar con el sistema. Verifica que SQL Server esté disponible e intenta de nuevo.";
            }
        }
    }
}
