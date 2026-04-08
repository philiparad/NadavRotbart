using System;
using System.Web.UI;

public partial class AdminDeleteUser : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["ID"] == null || Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
        {
            Response.Redirect("EntryPage.aspx");
            return;
        }

        int currentAdminId = (int)Session["ID"];
        int userId;
        string userIdStr = Request.QueryString["id"] ?? Request.QueryString["UserID"];
        if (!int.TryParse(userIdStr, out userId))
        {
            Response.Redirect("AdminPage.aspx?Msg=משתמש לא תקין");
            return;
        }

        if (userId == currentAdminId)
        {
            Response.Redirect("AdminPage.aspx?Msg=מנהל לא יכול למחוק את עצמו");
            return;
        }

        int rowsAffected = UsersDbApi.deleteUser(userId);

        if (rowsAffected > 0)
        {
            Response.Redirect("AdminPage.aspx?Msg=המשתמש נמחק בהצלחה");
        }
        else
        {
            Response.Redirect("AdminPage.aspx?Msg=מחיקת המשתמש נכשלה");
        }
    }
}
