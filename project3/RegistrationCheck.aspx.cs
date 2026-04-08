using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RegistrationCheck : System.Web.UI.Page
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
        string sqlQuery = "INSERT INTO UsersTbl (UserName, [Password], FirstName, LastName, Email, Address, City, PhonePrefix, PhoneNumber, Gender, BirthDate) " +
                            string.Format("VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, {7}, {8}, {9}, #{10}#)",
                            username, userPass, firstName, lastName, email, address, city, phonePrefix, phoneNum, gender, birthDate);
        int rowsAffected = DAL.ExecuteNonQuery(sqlQuery);
        if (rowsAffected > 0)
        {
            Session["name"] = firstName;
            Session["ID"] = UserDbApi.findUserId(username);
            Response.Redirect("HomePage.aspx");
        }
        else
        {
            Response.Redirect("RegistrationPage.aspx?Err=Registration Failed, Please Try Again");
        }
    }
}
