using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class LoginCheck : System.Web.UI.Page
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
        string sqlStr = string.Format("SELECT ID, FirstName FROM UsersTbl WHERE (UserName = '{0}') AND (Password = '{1}')", userName, password);
        OleDbConnection con = DAL.GetConnection();
        con.Open();
        OleDbCommand cmd = DAL.GetCommand(con, sqlStr);
        OleDbDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            Session["name"] = reader.GetString(1);
            Session["ID"] = reader.GetValue(0);
            con.Close();
            Response.Redirect("HomePage.aspx");
        }
        else
        {
            con.Close();
            Response.Redirect("LoginPage.aspx?Err=שם משתמש או סיסמה שגויים");
        }       

    }
}