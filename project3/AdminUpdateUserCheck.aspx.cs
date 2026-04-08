using System;
using System.Web.UI;

public partial class AdminUpdateUserCheck : Page
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
        if (!int.TryParse(Request.Form["userId"], out userId))
        {
            Response.Redirect("AdminPage.aspx?Msg=משתמש לא תקין");
            return;
        }

        if (userId == currentAdminId)
        {
            Response.Redirect("AdminPage.aspx?Msg=מנהל לא יכול לעדכן את עצמו");
            return;
        }

        string firstName = Request.Form["firstName"];
        string lastName = Request.Form["lastName"];
        string email = Request.Form["email"];
        string city = Request.Form["city"];
        string address = Request.Form["address"];
        string phoneNum = Request.Form["phoneNumber"];
        string phonePrefix = Request.Form["phonePrefix"];
        string gender = Request.Form["gender"];
        string birthDate = Request.Form["birthDate"];
        bool isAdmin = Request.Form["isAdmin"] == "1";

        int rowsAffected = UsersDbApi.updateUserByAdmin(userId, firstName, lastName, email, city, address,
            phoneNum, phonePrefix, gender, birthDate, isAdmin);

        if (rowsAffected > 0)
        {
            Response.Redirect("AdminPage.aspx?Msg=פרטי המשתמש עודכנו בהצלחה");
        }
        else
        {
            Response.Redirect("AdminPage.aspx?Msg=עדכון המשתמש נכשל");
        }
    }
}
