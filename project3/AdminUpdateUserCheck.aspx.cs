using System;
using System.Web.UI;

public partial class AdminUpdateUserCheck : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["ID"] == null || Session["IsAdmin"] == null || !Convert.ToBoolean(Session["IsAdmin"]))
        {
            Response.Redirect("EntryPage.aspx");
            return;
        }

        int currentAdminId = Convert.ToInt32(Session["ID"]);
        if (Session["EditUserID"] == null)
        {
            Response.Redirect("AdminPage.aspx?Msg=משתמש לא תקין");
            return;
        }
        int userId = Convert.ToInt32(Session["EditUserID"]);

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
        bool isAdmin = Request.Form["isAdmin"] != null;

        int rowsAffected = UsersDbApi.updateUserByAdmin(userId, firstName, lastName, email, city, address,
            phoneNum, phonePrefix, gender, birthDate, isAdmin);

        if (rowsAffected > 0)
        {
            Session.Remove("EditUserID");
            Response.Redirect("AdminPage.aspx?Msg=פרטי המשתמש עודכנו בהצלחה");
        }
        else
        {
            Response.Redirect("AdminPage.aspx?Msg=עדכון המשתמש נכשל");
        }
    }
}
