using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Data.OleDb;
using System.Web;

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

    private static OleDbConnection GetConnection()
    {
        string dbPhysicalPath = ResolveDatabasePath();
        string connectionString = BuildConnectionString(dbPhysicalPath);
        return new OleDbConnection(connectionString);
    }

    private static string ResolveDatabasePath()
    {
        if (HttpContext.Current != null)
        {
            string mappedPath = HttpContext.Current.Server.MapPath("~/App_Data/ProjectDB2.accdb");
            if (!string.IsNullOrWhiteSpace(mappedPath) && File.Exists(mappedPath))
            {
                return mappedPath;
            }
        }

        string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
        string candidatePath = Path.Combine(appBasePath ?? string.Empty, @"App_Data\ProjectDB2.accdb");
        if (File.Exists(candidatePath))
        {
            return candidatePath;
        }

        return @"|DataDirectory|\ProjectDB2.accdb";
    }

    private static string BuildConnectionString(string dbPath)
    {
        string sanitizedPath = EscapeSqlString(dbPath);
        string[] providers = new string[]
        {
            "Microsoft.ACE.OLEDB.16.0",
            "Microsoft.ACE.OLEDB.12.0",
            "Microsoft.Jet.OLEDB.4.0"
        };

        Exception lastError = null;

        foreach (string provider in providers)
        {
            string connectionString = string.Format(
                CultureInfo.InvariantCulture,
                "Provider={0};Data Source={1};Persist Security Info=False;",
                provider,
                sanitizedPath);

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                }

                return connectionString;
            }
            catch (Exception ex)
            {
                lastError = ex;
            }
        }

        throw new InvalidOperationException(
            "Could not initialize Access database connection. Install Microsoft Access Database Engine (ACE) x86/x64 to match the app pool.",
            lastError);
    }

    private static OleDbCommand GetCommand(OleDbConnection conn, string sqlQuery)
    {
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        cmd.CommandText = sqlQuery;
        return cmd;
    }

    private static DataTable GetDataTable(string sqlQuery)
    {
        DataTable dt = new DataTable();
        using (OleDbConnection dbConnection = GetConnection())
        using (OleDbCommand dbCommand = GetCommand(dbConnection, sqlQuery))
        using (OleDbDataAdapter adapter = new OleDbDataAdapter())
        {
            adapter.SelectCommand = dbCommand;
            adapter.Fill(dt);
        }
        return dt;
    }

    private static int ExecuteNonQuery(string sqlQuery)
    {
        using (OleDbConnection dbConnection = GetConnection())
        using (OleDbCommand dbCommand = GetCommand(dbConnection, sqlQuery))
        {
            dbConnection.Open();
            return dbCommand.ExecuteNonQuery();
        }
    }

    private static object ExecuteScalar(string sqlQuery)
    {
        using (OleDbConnection dbConnection = GetConnection())
        using (OleDbCommand dbCommand = GetCommand(dbConnection, sqlQuery))
        {
            dbConnection.Open();
            return dbCommand.ExecuteScalar();
        }
    }

    private static string ResolveUsersTableName()
    {
        string[] tableCandidates = new string[] { "Usertbl", "UsersTbl" };
        foreach (string tableName in tableCandidates)
        {
            try
            {
                ExecuteScalar("SELECT COUNT(*) FROM [" + tableName + "]");
                return tableName;
            }
            catch
            {
                // Try the next candidate.
            }
        }

        throw new InvalidOperationException("Could not find user table. Expected Usertbl or UsersTbl.");
    }

    public static void EnsureAdminSupport()
    {
        string usersTableName = ResolveUsersTableName();

        try
        {
            ExecuteNonQuery("ALTER TABLE [" + usersTableName + "] ADD COLUMN IsAdmin BIT DEFAULT False");
        }
        catch
        {
            // Column already exists.
        }

        try
        {
            ExecuteNonQuery("ALTER TABLE [" + usersTableName + "] ALTER COLUMN IsAdmin BIT DEFAULT False");
        }
        catch
        {
            // Ignore if default already exists or ALTER syntax differs by provider.
        }

        try
        {
            ExecuteNonQuery("UPDATE [" + usersTableName + "] SET IsAdmin = False WHERE IsAdmin IS NULL");
        }
        catch
        {
            // Ignore if table is empty or column conversion fails on older data.
        }

        try
        {
            int promotedRows = ExecuteNonQuery("UPDATE [" + usersTableName + "] SET IsAdmin = True WHERE UserName = 'admin'");
            if (promotedRows == 0)
            {
                ExecuteNonQuery("UPDATE [" + usersTableName + "] SET IsAdmin = True WHERE ID IN (SELECT TOP 1 ID FROM [" + usersTableName + "] ORDER BY ID)");
            }
        }
        catch
        {
            // Ignore; app can still run with admin set manually.
        }

        try
        {
            object countResult = ExecuteScalar("SELECT COUNT(*) FROM [" + usersTableName + "]");
            int usersCount = Convert.ToInt32(countResult);
            if (usersCount == 0)
            {
                ExecuteNonQuery(
                    "INSERT INTO [" + usersTableName + "] (UserName, [Password], FirstName, LastName, Email, Address, City, PhonePrefix, PhoneNumber, Gender, BirthDate, IsAdmin) " +
                    "VALUES ('admin', 'admin123', 'System', 'Admin', 'admin@example.com', '1 Admin St', 1, 1, 1111111, False, #01/01/1990#, True)");

                ExecuteNonQuery(
                    "INSERT INTO [" + usersTableName + "] (UserName, [Password], FirstName, LastName, Email, Address, City, PhonePrefix, PhoneNumber, Gender, BirthDate, IsAdmin) " +
                    "VALUES ('alice', 'alice123', 'Alice', 'Cohen', 'alice@example.com', '12 Main St', 2, 2, 2222222, True, #02/02/1992#, False)");

                ExecuteNonQuery(
                    "INSERT INTO [" + usersTableName + "] (UserName, [Password], FirstName, LastName, Email, Address, City, PhonePrefix, PhoneNumber, Gender, BirthDate, IsAdmin) " +
                    "VALUES ('bob', 'bob123', 'Bob', 'Levi', 'bob@example.com', '34 Oak Ave', 3, 3, 3333333, False, #03/03/1993#, False)");

                ExecuteNonQuery(
                    "INSERT INTO [" + usersTableName + "] (UserName, [Password], FirstName, LastName, Email, Address, City, PhonePrefix, PhoneNumber, Gender, BirthDate, IsAdmin) " +
                    "VALUES ('dana', 'dana123', 'Dana', 'Mizrahi', 'dana@example.com', '56 Pine Rd', 4, 4, 4444444, True, #04/04/1994#, False)");
            }
        }
        catch
        {
            // Ignore seeding failures when lookup tables/constraints differ.
        }
    }

    public static string getCities()
    {
        string sqlQuery = "select * from CitiesTbl";
        DataTable citiesTable = GetDataTable(sqlQuery);
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
        DataTable citiesTable = GetDataTable(sqlQuery);
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
        DataTable citiesTable = GetDataTable(sqlQuery);
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
        DataTable citiesTable = GetDataTable(sqlQuery);
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
        string sqlQuery = "select ID from UsersTbl where UserName = '" + EscapeSqlString(userName) + "'";
        Object obj;
        using (OleDbConnection dbConnection = GetConnection())
        using (OleDbCommand dbCmd = GetCommand(dbConnection, sqlQuery))
        {
            dbConnection.Open();
            obj = dbCmd.ExecuteScalar();
        }
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
        return GetDataTable(sqlQuery);
    }

    private static string FindColumnName(DataTable table, string expectedName)
    {
        foreach (DataColumn column in table.Columns)
        {
            if (string.Equals(column.ColumnName, expectedName, StringComparison.OrdinalIgnoreCase))
            {
                return column.ColumnName;
            }
        }

        return null;
    }

    public static DataTable getUserForLogin(string userName, string password)
    {
        string sqlQuery = string.Format(
            "SELECT * FROM [UsersTbl] WHERE [UserName] = '{0}' AND [Password] = '{1}'",
            EscapeSqlString(userName),
            EscapeSqlString(password));

        DataTable userTable = GetDataTable(sqlQuery);
        if (!userTable.Columns.Contains("IsAdmin"))
        {
            string adminColumnName = FindColumnName(userTable, "Admin");
            userTable.Columns.Add("IsAdmin", typeof(bool));

            foreach (DataRow row in userTable.Rows)
            {
                if (!string.IsNullOrEmpty(adminColumnName))
                {
                    row["IsAdmin"] = Convert.ToBoolean(row[adminColumnName]);
                }
                else
                {
                    row["IsAdmin"] = false;
                }
            }
        }

        return userTable;
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
        return ExecuteNonQuery(sqlQuery);
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
        return ExecuteNonQuery(sqlQuery);
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
        return ExecuteNonQuery(sqlQuery);
    }

    public static int deleteUser(int userId)
    {
        string sqlQuery = "DELETE FROM UsersTbl WHERE ID = " + userId;
        return ExecuteNonQuery(sqlQuery);
    }

    public static string getUsersTableForAdmin(int currentAdminId)
    {
        string sqlQuery = "select * from CitiesTbl";
        DataTable citiesTable = GetDataTable(sqlQuery);

        sqlQuery = "select * from PhonePrefixesTbl";
        DataTable phonePrefixesTable = GetDataTable(sqlQuery);

        sqlQuery = "select * from UsersTbl";
        DataTable usersTable = GetDataTable(sqlQuery);
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
            string cityName = cityRows.Length > 0 ? cityRows[0]["CityName"].ToString() : "לא ידוע";
            responseStr += "<td>" + cityName + "</td>";
            int phonePrefixId = int.Parse(row["PhonePrefix"].ToString());
            DataRow[] phonePrefixRows = phonePrefixesTable.Select("id = " + phonePrefixId);
            string phonePrefix = phonePrefixRows.Length > 0 ? phonePrefixRows[0]["Prefix"].ToString() : "";
            string phoneDisplay = string.IsNullOrEmpty(phonePrefix)
                ? row["PhoneNumber"].ToString()
                : phonePrefix + "-" + row["PhoneNumber"];
            responseStr += "<td>" + phoneDisplay + "</td>";
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
