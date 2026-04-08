using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class LoginPage : System.Web.UI.Page
{
    protected string errorVisability = "hidden";
    protected string errorMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        errorMsg = Request.QueryString.Get("Err");
        if (!string.IsNullOrEmpty(errorMsg))
        {
            errorVisability = "visible";            
        }
    }
}