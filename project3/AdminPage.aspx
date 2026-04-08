<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AdminPage.aspx.cs" Inherits="AdminPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <title>Admin Page</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    <h1>דף מנהל</h1>
    <div style="visibility: <%=messageVisibility %>">
        <h3><%=message %></h3>
    </div>

    <table border="1" style="width:95%; text-align:center; margin-bottom:100px;">
        <%=usersTableHtml %>
    </table>
</asp:Content>
