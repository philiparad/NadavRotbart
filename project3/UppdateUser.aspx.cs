using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;

public partial class UppdateUser : System.Web.UI.Page
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
        }


        errorMsg = Request.QueryString["Err"];
        if (errorMsg != null)
        {
            errorVisability = "visible";
        }
        int userId = (int)Session["ID"];
        string sqlQuery = "select * from UsersTbl where ID = " + userId;
        DataTable dt = DAL.GetDataTable(sqlQuery);
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