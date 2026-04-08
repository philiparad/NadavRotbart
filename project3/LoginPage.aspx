<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="LoginPage.aspx.cs" Inherits="LoginPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <title>Login</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    <h1>כניסת משתמש</h1>
    <form id="form33" action="LoginCheck.aspx" method="post">
        <h2>שם משתמש: </h2>
        <input id="Text1" name="userName" type="text" />
        <br />
        <h2>סיסמה: </h2>
        <input id="Password1" name="userPass" type="password" />
        <br />
        <br />
        <input id="Submit1" type="submit" value="שלח פרטים" />
    </form>
    <div style ="visibility: <%=errorVisability %>">
        <h2><%= errorMsg %></h2>
    </div>
</asp:Content>

