using System;
using System.Web.UI;

public partial class UpdateUserCheck : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack && Request.HttpMethod != "POST")
        {
            Response.Redirect("UppdateUser.aspx");
            return;
        }

        if (Session["ID"] == null)
        {
            Response.Redirect("EntryPage.aspx");
            return;
        }

        int userId = (int)Session["ID"];
        string firstName = Request.Form["firstName"];
        string lastName = Request.Form["lastName"];
        string email = Request.Form["email"];
        string city = Request.Form["city"];
        string address = Request.Form["address"];
        string phoneNum = Request.Form["phoneNumber"];
        string phonePrefix = Request.Form["phonePrefix"];
        string gender = Request.Form["gender"];
        string birthDate = Request.Form["birthDate"];

        int rowsAffected = UsersDbApi.updateUser(userId, firstName, lastName, email, city, address,
            phoneNum, phonePrefix, gender, birthDate);

        if (rowsAffected > 0)
        {
            Session["name"] = firstName;
            Response.Redirect("HomePage.aspx");
        }
        else
        {
            Response.Redirect("UppdateUser.aspx?Err=Update Failed, Please Try Again");
        }
    }
}
