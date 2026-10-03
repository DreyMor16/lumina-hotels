using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUIL
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["IdRol"] == null || Convert.ToInt32(Session["IdRol"]) != 2)
            {
                Response.Redirect("Login.aspx?reason=role");
            }
        }

        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            // Esto asegura que el ID de sesión viejo muera en el navegador
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            Response.Redirect("Login.aspx");
        }
    }
}
