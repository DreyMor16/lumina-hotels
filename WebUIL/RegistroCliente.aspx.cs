using BLL;
using EDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUIL
{
    public partial class RegistroCliente : System.Web.UI.Page
    {
        ClienteLN logica = new ClienteLN();

        // Propiedad para manejar la lista de teléfonos 
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
            if (IsPostBack)
            {
                // recuperar lo que el usuario escribió y lo vuelve a poner en el campo
                txtPassword.Attributes.Add("value", txtPassword.Text);
            }
        }

        protected void btnAgregarTel_Click(object sender, EventArgs e)
        {
            string tel = txtTelefono.Text.Trim();

            if (tel.Length != 8)
            {
                lblMensaje.Text = "El teléfono debe tener 8 dígitos.";
                return;
            }

            if (TelefonosTemp.Contains(tel))
            {
                lblMensaje.Text = "Teléfono ya está en la lista.";
                return;
            }

            if (logica.ExisteTelefono(tel))
            {
                lblMensaje.Text = "Este teléfono ya pertenece a otro cliente.";
                return;
            }

            TelefonosTemp.Add(tel);
            ActualizarLista();
            txtTelefono.Text = "";
            lblMensaje.Text = "";
        }

        protected void btnEliminarTel_Click(object sender, EventArgs e)
        {
            if (LstTelefonos.SelectedIndex != -1)
            {
                TelefonosTemp.RemoveAt(LstTelefonos.SelectedIndex);
                ActualizarLista();
            }
        }

        private void ActualizarLista()
        {
            LstTelefonos.DataSource = TelefonosTemp;
            LstTelefonos.DataBind();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            // 1. Limpiar mensaje previo
            lblMensaje.Text = "";
            lblMensaje.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60); 

            // 2. Validar campos obligatorios (Cédula, Nombre, Usuario, Password)
            if (string.IsNullOrWhiteSpace(txtCedula.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblMensaje.Text = "Por favor, llene todos los campos marcados con asterisco (*).";
                return;
            }

            // 3. Validar que exista al menos un teléfono en la lista (Campo obligatorio de contacto)
            if (TelefonosTemp == null || TelefonosTemp.Count == 0)
            {
                lblMensaje.Text = "Debe agregar al menos un número de teléfono de contacto.";
                return;
            }

            // 4. Validar complejidad de contraseña (Mayúscula, Número y Carácter Especial)
            string pass = txtPassword.Text;
            var regex = new System.Text.RegularExpressions.Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$");

            if (!regex.IsMatch(pass))
            {
                lblMensaje.Text = "La contraseña debe incluir al menos una mayúscula, un número y un carácter especial.";
                return;
            }

            // --- PROCESO DE GUARDADO SI PASA LAS VALIDACIONES ---

            Cliente cliente = new Cliente()
            {
                Cedula = txtCedula.Text,
                Nombre = txtNombre.Text
            };

            Usuario usuario = new Usuario()
            {
                Username = txtUsuario.Text,
                Password = txtPassword.Text,
                IdRol = 2 // Forzamos que sea Cliente
            };

            
            List<TelefonoCliente> listaTelefonos = TelefonosTemp.Select(t => new TelefonoCliente { Telefono = t }).ToList();

            // Llamada a la capa de negocio
            logica.RegistrarCliente(cliente, listaTelefonos, usuario);

            // 5. Verificar si hubo errores en la base de datos (Cédula duplicada, usuario existe, etc.)
            if (logica.stringBuilder.Length > 0)
            {
                lblMensaje.Text = logica.stringBuilder.ToString();
            }
            else
            {
                // ÉXITO: Limpiamos la lista temporal y redirigimos
                ViewState["Telefonos"] = null;
                Response.Redirect("Login.aspx?reg=success");
            }
        }
    }
}