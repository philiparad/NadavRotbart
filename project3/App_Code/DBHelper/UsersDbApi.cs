using System;
using System.Data;
using System.Globalization;
using System.Data.OleDb;

/// <summary>
/// Summary description for UsersDbApi
/// </summary>
public class UsersDbApi
{
    public UsersDbApi()
    {
        //
        // TODO: Add constructor logic here
        //
    }


    private static string EscapeSqlString(string value)
    {
        return (value ?? string.Empty).Replace("'", "''");
    }

    private static int ParseIntOrDefault(string value, int defaultValue)
    {
        int parsed;
        return int.TryParse(value, out parsed) ? parsed : defaultValue;
    }

    private static bool ParseGender(string value)
    {
        return value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }

    private static DateTime ParseDateOrDefault(string value)
    {
        DateTime parsed;
        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
        {
            return parsed;
        }
        return new DateTime(2000, 1, 1);
    }

    private static string ToAccessDateLiteral(DateTime date)
    {
        return date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
    }

    public static string getCities()
    {
        string sqlQuery = "select * from CitiesTbl";
        DataTable citiesTable = DAL.GetDataTable(sqlQuery);
        string responseStr = "";
        for (int i = 0; i < citiesTable.Rows.Count; i++)
        {
            responseStr += " <option value='" + citiesTable.Rows[i]["id"] + "'> " + citiesTable.Rows[i]["CityName"] + " </option>";
        }

        return responseStr;
    }

    public static string getCities(int selectedCity)
    {
        string sqlQuery = "select * from CitiesTbl";
        DataTable citiesTable = DAL.GetDataTable(sqlQuery);
        string responseStr = "";
        for (int i = 0; i < citiesTable.Rows.Count; i++)
        {
            string selected = "";
            if ((int)citiesTable.Rows[i]["id"] == selectedCity)
                selected = "selected";
            responseStr += " <option value='" + citiesTable.Rows[i]["id"] + "' " + selected + "> " + citiesTable.Rows[i]["CityName"] + " </option>";
        }

        return responseStr;
    }

    public static string getPhonePrefixes()
    {
        string sqlQuery = "select * from PhonePrefixesTbl";
        DataTable citiesTable = DAL.GetDataTable(sqlQuery);
        string responseStr = "";

        foreach (DataRow row in citiesTable.Rows)
        {
            responseStr += " <option value='" + row["id"] + "'>" + row["Prefix"] + " </option>";
        }
        return responseStr;
    }

    public static string getPhonePrefixes(int prefixNumber)
    {
        string sqlQuery = "select * from PhonePrefixesTbl";
        DataTable citiesTable = DAL.GetDataTable(sqlQuery);
        string responseStr = "";
        string selected;
        foreach (DataRow row in citiesTable.Rows)
        {
            if ((int)row["id"] == prefixNumber)
            {
                selected = "selected";
            }
            else
            {
                selected = "";
            }
            responseStr += " <option value='" + row["id"] + "' " + selected + ">" + row["Prefix"] + " </option>";
        }


        return responseStr;
    }

    public static int findUserId(string userName)
    {
        OleDbConnection dbConnection = DAL.GetConnection();
        dbConnection.Open();
        string sqlQuery = "select ID from UsersTbl where UserName = '" + EscapeSqlString(userName) + "'";
        OleDbCommand dbCmd = DAL.GetCommand(dbConnection, sqlQuery);
        Object obj = dbCmd.ExecuteScalar();
        dbConnection.Close();
        int retVal = -1;
        if (obj != null)
        {
            int parsedId;
            if (int.TryParse(obj.ToString(), out parsedId))
            {
                retVal = parsedId;
            }
        }
        return retVal;
    }

    public static DataTable getUserById(int userId)
    {
        string sqlQuery = "select * from UsersTbl where ID = " + userId;
        return DAL.GetDataTable(sqlQuery);
    }

    public static DataTable getUserForLogin(string userName, string password)
    {
        string sqlStr = string.Format("SELECT ID, FirstName, IsAdmin FROM UsersTbl WHERE (UserName = '{0}') AND ([Password] = '{1}')",
            EscapeSqlString(userName), EscapeSqlString(password));
        return DAL.GetDataTable(sqlStr);
    }

    public static int registerUser(string username, string userPass, string firstName, string lastName, string email, string address,
        string city, string phonePrefix, string phoneNum, string gender, string birthDate)
    {
        int cityId = ParseIntOrDefault(city, 0);
        int parsedPhonePrefix = ParseIntOrDefault(phonePrefix, 0);
        int parsedPhoneNum = ParseIntOrDefault(phoneNum, 0);
        bool parsedGender = ParseGender(gender);
        string parsedBirthDate = ToAccessDateLiteral(ParseDateOrDefault(birthDate));

        string sqlQuery = "INSERT INTO UsersTbl (UserName, [Password], FirstName, LastName, Email, Address, City, PhonePrefix, PhoneNumber, Gender, BirthDate, IsAdmin) " +
                            string.Format("VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, {7}, {8}, {9}, #{10}#, False)",
                            EscapeSqlString(username), EscapeSqlString(userPass), EscapeSqlString(firstName), EscapeSqlString(lastName),
                            EscapeSqlString(email), EscapeSqlString(address), cityId, parsedPhonePrefix, parsedPhoneNum, parsedGender ? 1 : 0, parsedBirthDate);
        return DAL.ExecuteNonQuery(sqlQuery);
    }

    public static int updateUser(int userId, string firstName, string lastName, string email, string city, string address,
        string phoneNum, string phonePrefix, string gender, string birthDate)
    {
        int cityId = ParseIntOrDefault(city, 0);
        int parsedPhoneNum = ParseIntOrDefault(phoneNum, 0);
        int parsedPhonePrefix = ParseIntOrDefault(phonePrefix, 0);
        bool parsedGender = ParseGender(gender);
        string parsedBirthDate = ToAccessDateLiteral(ParseDateOrDefault(birthDate));

        string sqlQuery = "UPDATE UsersTbl SET " +
            string.Format("FirstName = '{0}', LastName = '{1}', Address = '{2}', City = {3}, PhoneNumber = {4}, Gender = {5}, BirthDate = #{6}#, PhonePrefix = {7} ,email='{8}' WHERE ID = {9}",
                        EscapeSqlString(firstName), EscapeSqlString(lastName), EscapeSqlString(address), cityId, parsedPhoneNum,
                        parsedGender ? 1 : 0, parsedBirthDate, parsedPhonePrefix, EscapeSqlString(email), userId);
        return DAL.ExecuteNonQuery(sqlQuery);
    }

    public static int updateUserByAdmin(int userId, string firstName, string lastName, string email, string city, string address,
        string phoneNum, string phonePrefix, string gender, string birthDate, bool isAdmin)
    {
        int cityId = ParseIntOrDefault(city, 0);
        int parsedPhoneNum = ParseIntOrDefault(phoneNum, 0);
        int parsedPhonePrefix = ParseIntOrDefault(phonePrefix, 0);
        bool parsedGender = ParseGender(gender);
        string parsedBirthDate = ToAccessDateLiteral(ParseDateOrDefault(birthDate));

        string sqlQuery = "UPDATE UsersTbl SET " +
            string.Format("FirstName = '{0}', LastName = '{1}', Address = '{2}', City = {3}, PhoneNumber = {4}, Gender = {5}, BirthDate = #{6}#, PhonePrefix = {7}, Email = '{8}', IsAdmin = {9} WHERE ID = {10}",
                        EscapeSqlString(firstName), EscapeSqlString(lastName), EscapeSqlString(address), cityId, parsedPhoneNum, parsedGender ? 1 : 0,
                        parsedBirthDate, parsedPhonePrefix, EscapeSqlString(email), isAdmin ? "True" : "False", userId);
        return DAL.ExecuteNonQuery(sqlQuery);
    }

    public static int deleteUser(int userId)
    {
        string sqlQuery = "DELETE FROM UsersTbl WHERE ID = " + userId;
        return DAL.ExecuteNonQuery(sqlQuery);
    }

    public static string getUsersTableForAdmin(int currentAdminId)
    {
        string sqlQuery = "select * from CitiesTbl";
        DataTable citiesTable = DAL.GetDataTable(sqlQuery);

        sqlQuery = "select * from PhonePrefixesTbl";
        DataTable phonePrefixesTable = DAL.GetDataTable(sqlQuery);

        sqlQuery = "select * from UsersTbl";
        DataTable usersTable = DAL.GetDataTable(sqlQuery);
        string responseStr = "<caption>טבלת משתמשים</caption>\r\n\t\t\t<tr bgcolor =\"orange\" width = 90%>\r\n\t\t\t\t" +
            "<th>שם משתמש</th>\r\n\t\t\t\t<th>סיסמה</th>\r\n\t\t\t\t<th>שם פרטי</th>\r\n\t\t\t\t<th>שם משפחה</th>\r\n\t\t\t\t" +
            "<th>כתובת דואל</th>\r\n\t\t\t\t<th>כתובת</th>\r\n\t\t\t\t<th>עיר</th>\r\n\t\t\t\t" +
            "<th>מספר טלפון</th>\r\n\t\t\t\t<th>מין</th>\r\n\t\t\t\t<th>תאריך לידה</th>\r\n\t\t\t\t" +
            "<th>מנהל?</th>\r\n\t\t\t\t<th>עדכון</th>\r\n\t\t\t\t<th>מחיקה</th>\r\n\t\t\t</tr>\r\n\t\t\t";
        foreach (DataRow row in usersTable.Rows)
        {
            int userId = int.Parse(row["ID"].ToString());
            responseStr += "<tr width = 90%>";
            responseStr += "<td>" + row["UserName"] + "</td>";
            responseStr += "<td>" + row["Password"] + "</td>";
            responseStr += "<td>" + row["FirstName"] + "</td>";
            responseStr += "<td>" + row["LastName"] + "</td>";
            responseStr += "<td>" + row["Email"] + "</td>";
            responseStr += "<td>" + row["Address"] + "</td>";
            int cityId = int.Parse(row["City"].ToString());
            DataRow[] cityRows = citiesTable.Select("id = " + cityId);
            responseStr += "<td>" + cityRows[0]["CityName"] + "</td>";
            int phonePrefixId = int.Parse(row["PhonePrefix"].ToString());
            DataRow[] phonePrefixRows = phonePrefixesTable.Select("id = " + phonePrefixId);
            responseStr += "<td>" + phonePrefixRows[0]["Prefix"] + "-" + row["PhoneNumber"] + "</td>";
            bool genderBool = bool.Parse(row["gender"].ToString());
            string genderName;
            if (genderBool)
            {
                genderName = "נקבה";
            }
            else
            {
                genderName = "זכר";
            }
            responseStr += "<td>" + genderName + "</td>";
            DateTime birthDate = (DateTime)row["BirthDate"];
            responseStr += "<td>" + birthDate.ToString("yyyy-MM-dd") + "</td>";
            bool isAdmin = bool.Parse(row["IsAdmin"].ToString());
            if (isAdmin)
            {
                responseStr += "<td>מנהל</td>";
            }
            else
            {
                responseStr += "<td></td>";
            }

            if (userId == currentAdminId)
            {
                responseStr += "<td>לא זמין</td><td>לא זמין</td>";
            }
            else
            {
                responseStr += "<td><a href='AdminEditUser.aspx?id=" + userId + "'>עדכון</a></td>";
                responseStr += "<td><a href='AdminDeleteUser.aspx?id=" + userId + "' onclick=\"return confirm('למחוק משתמש זה?');\">מחיקה</a></td>";
            }

            responseStr += "</tr>";
        }
        return responseStr;
    }
}
