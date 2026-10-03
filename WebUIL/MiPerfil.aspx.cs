using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using EDL;

namespace WebUIL
{
    public partial class MiPerfil : System.Web.UI.Page
    {
        ClienteLN logica = new ClienteLN();

        private List<string> TelefonosTemp
        {
            get
            {
                if (ViewState["Telefonos"] == null) ViewState["Telefonos"] = new List<string>();
                return (List<string>)ViewState["Telefonos"];
            }
            set { ViewState["Telefonos"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //  Guardar la cédula en Session["CedulaLogueada"]
                if (Session["CedulaLogueada"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                CargarDatosPerfil(Session["CedulaLogueada"].ToString());
            }

        }
        private void CargarDatosPerfil(string cedula)
        {
            Cliente cliente = logica.TraerCliente(cedula);
            Usuario usuario = logica.TraerUsuarioPorCedula(cedula);
            List<TelefonoCliente> telefonos = logica.TraerTelefonos(cedula);

            if (cliente != null && usuario != null)
            {

                lblCedulaVista.Text = cliente.Cedula;
                lblNombreVista.Text = cliente.Nombre;
                lblUsuarioVista.Text = usuario.Username;

                
                txtCedula.Text = cliente.Cedula;
                txtNombre.Text = cliente.Nombre;
                txtUsuario.Text = usuario.Username;

                // 3. Cargar teléfonos
                TelefonosTemp = telefonos.Select(t => t.Telefono).ToList();
                ActualizarListaTelefonos();
            }
        }

        protected void btnAgregarTel_Click(object sender, EventArgs e)
        {
            string tel = txtTelefono.Text.Trim();
            if (!string.IsNullOrEmpty(tel) && !TelefonosTemp.Contains(tel))
            {
                TelefonosTemp.Add(tel);
                ActualizarListaTelefonos();
                txtTelefono.Text = "";
            }
        }

        protected void btnEliminarTel_Click(object sender, EventArgs e)
        {
            if (LstTelefonos.SelectedIndex != -1)
            {
                TelefonosTemp.RemoveAt(LstTelefonos.SelectedIndex);
                ActualizarListaTelefonos();
            }
        }
        private void ActualizarListaTelefonos()
        {
            LstTelefonos.DataSource = TelefonosTemp;
            LstTelefonos.DataBind();
        }

        protected void btnGuardarDatos_Click(object sender, EventArgs e)
        {
            lblMensaje.ForeColor = System.Drawing.Color.Red;

            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                lblMensaje.Text = "El nombre y el usuario no pueden estar vacíos.";
                return;
            }

            if (TelefonosTemp.Count == 0)
            {
                lblMensaje.Text = "Debe tener al menos un teléfono de contacto.";
                return;
            }

            // Mantenemos la contraseña actual que ya está en la BD
            Usuario usuarioActualBD = logica.TraerUsuarioPorCedula(txtCedula.Text);

            Cliente clienteMod = new Cliente { Cedula = txtCedula.Text, Nombre = txtNombre.Text };
            Usuario usuarioMod = new Usuario { CedulaCliente = txtCedula.Text, Username = txtUsuario.Text, Password = usuarioActualBD.Password };
            List<TelefonoCliente> telefonosMod = TelefonosTemp.Select(t => new TelefonoCliente { Telefono = t }).ToList();

            logica.ModificarPerfilCompleto(clienteMod, telefonosMod, usuarioMod);

            if (logica.stringBuilder.Length > 0)
            {
                lblMensaje.Text = logica.stringBuilder.ToString();
            }
            else
            {
                lblMensaje.ForeColor = System.Drawing.Color.Green;
                lblMensaje.Text = "¡Datos personales actualizados con éxito!";
            }
        }
        protected void btnGuardarPass_Click(object sender, EventArgs e)
        {
            lblMensaje.ForeColor = System.Drawing.Color.Red;

            Usuario usuarioActualBD = logica.TraerUsuarioPorCedula(txtCedula.Text);

            // Validar la contraseña actual
            if (txtPasswordActual.Text != usuarioActualBD.Password)
            {
                lblMensaje.Text = "La contraseña actual es incorrecta. No se pueden guardar los cambios.";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPasswordNueva.Text))
            {
                lblMensaje.Text = "Debe ingresar una nueva contraseña.";
                return;
            }

            // Validar la seguridad de la nueva contraseña
            var regex = new System.Text.RegularExpressions.Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$");
            if (!regex.IsMatch(txtPasswordNueva.Text))
            {
                lblMensaje.Text = "La nueva contraseña debe incluir mayúscula, número y carácter especial.";
                return;
            }

            // Preparamos los objetos manteniendo el nombre y usuario actuales, pero cambiando el password
            Cliente clienteMod = new Cliente { Cedula = txtCedula.Text, Nombre = txtNombre.Text };
            Usuario usuarioMod = new Usuario { CedulaCliente = txtCedula.Text, Username = txtUsuario.Text, Password = txtPasswordNueva.Text };
            List<TelefonoCliente> telefonosMod = TelefonosTemp.Select(t => new TelefonoCliente { Telefono = t }).ToList();

            logica.ModificarPerfilCompleto(clienteMod, telefonosMod, usuarioMod);

            if (logica.stringBuilder.Length > 0)
            {
                lblMensaje.Text = logica.stringBuilder.ToString();
            }
            else
            {
                lblMensaje.ForeColor = System.Drawing.Color.Green;
                lblMensaje.Text = "¡Contraseña actualizada con éxito!";
                txtPasswordActual.Text = "";
                txtPasswordNueva.Text = "";
            }
        }
        protected void btnIrModificar_Click(object sender, EventArgs e)
        {
            pnlVista.Visible = false;
            pnlModificarDatos.Visible = true;
            pnlModificarPass.Visible = false;
            lblMensaje.Text = "";
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            pnlVista.Visible = true;
            pnlModificarDatos.Visible = false;
            pnlModificarPass.Visible = false;
            lblMensaje.Text = "";

            //  recargar labels con lo que hay en la base de datos
            if (Session["CedulaLogueada"] != null)
            {
                CargarDatosPerfil(Session["CedulaLogueada"].ToString());
            }
        }
        protected void btnIrPassword_Click(object sender, EventArgs e)
        {
            pnlVista.Visible = false;
            pnlModificarDatos.Visible = false;
            pnlModificarPass.Visible = true;
            lblMensaje.Text = "";
        }
    }
}