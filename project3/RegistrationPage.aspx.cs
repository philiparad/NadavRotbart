using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RegistrationPage : System.Web.UI.Page
{
    protected string errorVisability = "hidden";
    protected string errorMsg = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        errorMsg = Request.QueryString["Err"];
        if (errorMsg != null)
        {
            errorVisability = "visible";
        }
    }

    }
