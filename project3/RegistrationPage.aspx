<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="RegistrationPage.aspx.cs" Inherits="RegistrationPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <h1>טופס הרשמה</h1>
    <form id="register-form" action="RegistrationCheck.aspx" method="post">
        <h2>שם משתמש: </h2>
        <input id="register-user-name" name="userName" type="text" />
        <br />
        <h2>סיסמה: </h2>
        <input id="register-user-pass" name="userPass" type="password" />
        <br />
        <h2>שם פרטי: </h2>
        <input id="register-firstname" type="text" name="firstName" />
        <br />
        <h2>שם משפחה: </h2>
        <input id="register-lastname" type="text" name="lastName" />
        <br />
        <h2>דואר אלקטרוני: </h2>
        <input type="email" id="register-email" name="email">
        <br />
        <label style="font-size: 1.5em; font-weight: bold;">עיר: </label>
        <select id="register-city" name="city">
            <% = UsersDbApi.getCities()%>
        </select>
        <h2>כתובת - רחוב ומספר בית: </h2>
        <input type="text" id="register-address" name="address">
        <h2>מספר טלפון: </h2>
        <span>
            <input type="text" id="phoneNumber" name="phoneNumber" pattern="[0-9]{7}" maxlength="7" placeholder="7 ספרות">
        </span>
        <span>
            <select id="register-phonePrefix" name="phonePrefix">
                <% = UsersDbApi.getPhonePrefixes() %>
            </select>
        </span>

        <h2>מגדר:</h2>
        <label>
            <input type="radio" name="gender" value="0" checked>
            זכר</label>
        <label>
            <input type="radio" name="gender" value="1">
            נקבה</label>

        <h2>תאריך לידה:</h2>
        <input type="date" id="rgister-birthDate" name="birthDate">

        <div style="visibility: <%=errorVisability %>">
            <h2><%= errorMsg %></h2>
        </div>

        <input id="Submit1" type="submit" value="שלח פרטים" />
        <br />
        <br />
        <br />
    </form>

</asp:Content>

