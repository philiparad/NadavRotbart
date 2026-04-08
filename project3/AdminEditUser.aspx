<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AdminEditUser.aspx.cs" Inherits="AdminEditUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    <h1>עדכון משתמש (מנהל)</h1>
    <form id="admin-update-form" action="AdminUpdateUserCheck.aspx" method="post">
        <h2>שם משתמש: </h2>
        <input name="userName" type="text" value="<%=row["UserName"] %>" readonly />
        <br />

        <h2>שם פרטי: </h2>
        <input type="text" name="firstName" value="<%=row["FirstName"] %>" />
        <br />

        <h2>שם משפחה: </h2>
        <input type="text" name="lastName" value="<%=row["LastName"] %>" />
        <br />

        <h2>דואר אלקטרוני: </h2>
        <input type="email" name="email" value="<%=row["Email"] %>">
        <br />

        <label style="font-size: 1.5em; font-weight: bold;">עיר: </label>
        <select name="city">
            <% =UsersDbApi.getCities((int)row["City"])%>
        </select>

        <h2>כתובת - רחוב ומספר בית: </h2>
        <input type="text" name="address" value="<%=row["Address"] %>">

        <h2>מספר טלפון: </h2>
        <span>
            <input type="text" name="phoneNumber" pattern="[0-9]{7}" maxlength="7" value="<%=row["PhoneNumber"] %>">
        </span>
        <span>
            <select name="phonePrefix">
                <% =UsersDbApi.getPhonePrefixes((int)row["PhonePrefix"]) %>
            </select>
        </span>

        <h2>מגדר:</h2>
        <label>
            <input type="radio" name="gender" value="0" <%=ganderMaleCheck %> />
            זכר</label>
        <label>
            <input type="radio" name="gender" value="1" <%=ganderFemaleCheck %> />
            נקבה</label>

        <h2>תאריך לידה:</h2>
        <input type="date" name="birthDate" value="<%=birthDate.ToString("yyyy-MM-dd") %>">

        <h2>הרשאת מנהל:</h2>
        <label>
            <input type="checkbox" name="isAdmin" value="1" <%=adminChecked %> />האם מנהל</label>

        <div style="visibility: <%=errorVisability %>">
            <h2><%= errorMsg %></h2>
        </div>

        <input type="submit" value="שמור שינויים" />
    </form>
</asp:Content>
