using System;
using System.Web.UI;

public partial class RegistrationCheck : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string username = Request.Form["userName"];
        string userPass = Request.Form["userPass"];
        string firstName = Request.Form["firstName"];
        string lastName = Request.Form["lastName"];
        string email = Request.Form["email"];
        string city = Request.Form["city"];
        string address = Request.Form["address"];
        string phoneNum = Request.Form["phoneNumber"];
        string phonePrefix = Request.Form["phonePrefix"];
        string gender = Request.Form["gender"];
        string birthDate = Request.Form["birthDate"];

        int rowsAffected = UsersDbApi.registerUser(username, userPass, firstName, lastName, email, address,
            city, phonePrefix, phoneNum, gender, birthDate);

        if (rowsAffected > 0)
        {
            Session["name"] = firstName;
            Session["ID"] = UsersDbApi.findUserId(username);
            Session["IsAdmin"] = false;
            Response.Redirect("HomePage.aspx");
        }
        else
        {
            Response.Redirect("RegistrationPage.aspx?Err=Registration Failed, Please Try Again");
        }
    }
}
