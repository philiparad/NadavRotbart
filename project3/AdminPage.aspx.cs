using System;
using System.Web.UI;

public partial class AdminPage : Page
{
    protected string usersTableHtml = string.Empty;
    protected string messageVisibility = "hidden";
    protected string message = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["ID"] == null || Session["IsAdmin"] == null || !Convert.ToBoolean(Session["IsAdmin"]))
        {
            Response.Redirect("LoginPage.aspx");
            return;
        }

        message = Request.QueryString["Msg"];
        if (!string.IsNullOrEmpty(message))
        {
            messageVisibility = "visible";
        }

        int currentAdminId = Convert.ToInt32(Session["ID"]);
        usersTableHtml = UsersDbApi.getUsersTableForAdmin(currentAdminId);
    }
}
