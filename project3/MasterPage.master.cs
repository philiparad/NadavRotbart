using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected string menu;
    protected void Page_Load(object sender, EventArgs e)
    {
        adminLink.Visible = false;
        menu = "<a href = 'EntryPage.aspx'> <input class='btn' type = 'button' value = 'דף השער'/> </a><br /><br />";
        if (Session["ID"] == null)
        {
            menu += "<a href = 'LoginPage.aspx'> <input class='btn' type = 'button' value = 'כניסת משתמש'/> </a><br /><br />";
            menu += "<a href = 'RegistrationPage.aspx'> <input class='btn' type = 'button' value = 'רישום משתמש חדש'/> </a><br />";
        }
        else
        {
            menu += "<a href = 'HomePage.aspx'> <input class='btn' type = 'button' value = 'דף הבית'/> </a><br /><br />";
            menu += "<a href = 'UppdateUser.aspx'> <input class='btn' type = 'button' value = 'עדכון פרטים'/> </a><br /><br />";
            if (Session["IsAdmin"] != null && Convert.ToBoolean(Session["IsAdmin"]))
            {
                adminLink.Visible = true;
            }
            menu += "<input class='btn' type='button' value='התנתק' onclick=\"window.location='Logout.aspx'\" /><br />";
        }

    }
}
