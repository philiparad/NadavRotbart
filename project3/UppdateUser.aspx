<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="UppdateUser.aspx.cs" Inherits="UppdateUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <h1>עדכון פרטים</h1>
    <form id="register-form" action="UpdateUserCheck.aspx" method="post">
        <h2>שם משתמש: </h2>
        <input id="register-user-name" name="userName" type="text" value="<%=row["UserName"] %>" readonly />
        <br />
        
    
    <h2>שם פרטי: </h2>
    <input id="register-firstname" type="text" name="firstName" value ="<%=row["FirstName"] %>" />
        <br />
        <h2>שם משפחה: </h2>
        <input id="register-lastname" type="text" name="lastName" value="<%=row["LastName"] %>" />
        <br />
        <h2>דואר אלקטרוני: </h2>
        <input type="email" id="register-email" name="email" value="<%=row["Email"] %>">
        <br />
        <label style="font-size: 1.5em; font-weight: bold;">עיר: </label>
        <select id="register-city" name="city">
            <% =UserDbApi.getCities((int)row["City"])%>
        </select>
        <h2>כתובת - רחוב ומספר בית: </h2>
        <input type="text" id="register-address" name="address" value="<%=row["Address"] %>">
        <h2>מספר טלפון: </h2>
        <span>
            <input type="text" id="phoneNumber" name="phoneNumber" pattern="[0-9]{7}" maxlength="7" placeholder="7 ספרות">
        </span>
        <span>
            <select id="register-phonePrefix" name="phonePrefix">
                <% =UserDbApi.getPhonePrefixes((int)row["PhonePrefix"]) %>
            </select>
        </span>

        <h2>מגדר:</h2>
        <label>    
            <input type="radio" name="gender" value="0" <%= ganderMaleCheck %> />
            זכר</label>
        <label>
            <input type="radio" name="gender" value="1"<%=ganderFemaleCheck %> />
            נקבה</label>

        <h2>תאריך לידה:</h2>
        <input type="date" id="rgister-birthDate" name="birthDate" value="<%=birthDate.ToString("yyyy-MM-dd") %>">

        <div style="visibility: <%=errorVisability %>">
            <h2><%= errorMsg %></h2>
        </div>

        <input id="Submit1" type="submit" value="שלח פרטים" />
        <br />
        <br />
        <br />
    </form>
</asp:Content>

