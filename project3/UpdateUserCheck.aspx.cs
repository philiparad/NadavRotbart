using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UpdateUserCheck : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["ID"] == null)
        {
            Response.Redirect("EntryPage.aspx");
        }
        int userId = (int)Session["ID"];
        string username = Request.Form["username"];
        //string userPass = Request.Form["userPass"];
        string firstName = Request.Form["firstName"];
        string lastName = Request.Form["lastName"];
        string email = Request.Form["email"];
        string city = Request.Form["city"];
        string address = Request.Form["address"];
        string phoneNum = Request.Form["phoneNumber"];
        string phonePrefix = Request.Form["phonePrefix"];
        string gender = Request.Form["gender"];
        string birthDate = Request.Form["birthDate"];
        string sqlQuery = "UPDATE UsersTbl SET " +
            string.Format("FirstName = '{0}', LastName = '{1}', Address = '{2}', City = {3}, PhoneNumber = {4}, Gender = {5}, BirthDate = #{6}#, PhonePrefix = {7} ,email='{8}' WHERE ID = {9}",
                        firstName, lastName, address, city, phoneNum, gender, birthDate, phonePrefix, email , userId);
        int rowsAffected = DAL.ExecuteNonQuery(sqlQuery);
        if (rowsAffected > 0)
        {
            Session["name"] = firstName;
            Response.Redirect("HomePage.aspx");
        }
        else
        {
            Response.Redirect("UppdateUser.aspx?Err=Registration Failed, Please Try Again");
        }




    }
}