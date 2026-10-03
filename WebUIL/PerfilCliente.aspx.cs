using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUIL
{
    public partial class PerfilCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CedulaLogueada"] == null || Session["IdRol"] == null || Convert.ToInt32(Session["IdRol"]) != 2)
            {
                Response.Redirect("Login.aspx");
            }
        }
    }
}
