using System;
using System.Data;
using System.Web.UI;

public partial class LoginCheck : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string userName = Request.Form.Get("userName");
        string password = Request.Form.Get("userPass");
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            Response.Redirect("LoginPage.aspx?Err=חסר שם משתמש או סיסמה או שניהם");
            return;
        }

        DataTable userTable = UsersDbApi.getUserForLogin(userName, password);
        if (userTable.Rows.Count > 0)
        {
            Session["name"] = userTable.Rows[0]["FirstName"].ToString();
            Session["ID"] = Convert.ToInt32(userTable.Rows[0]["ID"]);
            Session["IsAdmin"] = Convert.ToBoolean(userTable.Rows[0]["IsAdmin"]);
            Response.Redirect("HomePage.aspx");
        }
        else
        {
            Response.Redirect("LoginPage.aspx?Err=שם משתמש או סיסמה שגויים");
        }
    }
}
