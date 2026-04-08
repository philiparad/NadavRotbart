using System;
using System.Data;
using System.Web.UI;

public partial class AdminEditUser : Page
{
    protected string errorVisability = "hidden";
    protected string errorMsg = string.Empty;
    protected DataRow row;
    protected string ganderMaleCheck = "";
    protected string ganderFemaleCheck = "";
    protected string adminChecked = "";
    protected DateTime birthDate;

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
            Response.Redirect("AdminPage.aspx?Msg=לא ניתן לעדכן את המשתמש המחובר כמנהל");
            return;
        }

        errorMsg = Request.QueryString["Err"];
        if (!string.IsNullOrEmpty(errorMsg))
        {
            errorVisability = "visible";
        }

        DataTable dt = UsersDbApi.getUserById(userId);
        if (dt.Rows.Count == 0)
        {
            Response.Redirect("AdminPage.aspx?Msg=משתמש לא נמצא");
            return;
        }

        row = dt.Rows[0];
        Session["EditUserID"] = userId;

        bool gender = (bool)row["gender"];
        if (gender)
        {
            ganderFemaleCheck = "checked";
        }
        else
        {
            ganderMaleCheck = "checked";
        }

        bool isAdmin = (bool)row["IsAdmin"];
        if (isAdmin)
        {
            adminChecked = "checked";
        }

        birthDate = (DateTime)row["BirthDate"];
    }
}
