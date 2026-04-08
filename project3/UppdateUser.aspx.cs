using System;
using System.Data;
using System.Web.UI;

public partial class UppdateUser : Page
{
    protected string errorVisability = "hidden";
    protected string errorMsg = string.Empty;
    protected DataRow row;
    protected string ganderMaleCheck = "";
    protected string ganderFemaleCheck = "";
    protected DateTime birthDate;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["ID"] == null)
        {
            Response.Redirect("EntryPage.aspx");
            return;
        }

        errorMsg = Request.QueryString["Err"];
        if (errorMsg != null)
        {
            errorVisability = "visible";
        }

        int userId = (int)Session["ID"];
        DataTable dt = UsersDbApi.getUserById(userId);
        if (dt.Rows.Count == 0)
        {
            Session.Abandon();
            Response.Redirect("EntryPage.aspx");
            return;
        }

        row = dt.Rows[0];

        bool gender = (bool)row["gender"];
        if (gender)
        {
            ganderFemaleCheck = "checked";
        }
        else
        {
            ganderMaleCheck = "checked";
        }

        birthDate = (DateTime)row["BirthDate"];
    }
}
