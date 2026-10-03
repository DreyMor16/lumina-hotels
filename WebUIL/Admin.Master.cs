using System;
using System.Web;
using System.Web.UI;

namespace WebUIL
{
    public partial class AdminMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["IdRol"] == null || Convert.ToInt32(Session["IdRol"]) != 1)
            {
                Response.Redirect("Login.aspx?reason=role");
            }
        }

        protected void btnCerrarAdmin_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-1);
            }
            Response.Redirect("Login.aspx");
        }
    }
}
