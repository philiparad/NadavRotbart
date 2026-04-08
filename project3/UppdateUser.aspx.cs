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

        int userId = Convert.ToInt32(Session["ID"]);
        DataTable dt = UsersDbApi.getUserById(userId);
        if (dt.Rows.Count == 0)
        {
            Session.Abandon();
            Response.Redirect("EntryPage.aspx");
            return;
        }

        row = dt.Rows[0];

        bool gender = ParseGender(row["Gender"]);
        if (gender)
        {
            ganderFemaleCheck = "checked";
        }
        else
        {
            ganderMaleCheck = "checked";
        }

        birthDate = ParseBirthDate(row["BirthDate"]);
    }

    private static bool ParseGender(object value)
    {
        if (value is bool)
        {
            return (bool)value;
        }

        int numberValue;
        return int.TryParse(Convert.ToString(value), out numberValue) && numberValue == 1;
    }

    private static DateTime ParseBirthDate(object value)
    {
        if (value is DateTime)
        {
            return (DateTime)value;
        }

        DateTime parsedDate;
        if (DateTime.TryParse(Convert.ToString(value), out parsedDate))
        {
            return parsedDate;
        }

        return new DateTime(2000, 1, 1);
    }
}
