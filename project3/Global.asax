<%@ Application Language="C#" %>

<script runat="server">
    void Application_Start(object sender, EventArgs e)
    {
        UsersDbApi.EnsureAdminSupport();
    }

    void Session_Start(object sender, EventArgs e)
    {
        Session["IsAdmin"] = false;
    }
</script>
