using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for UserDbApi
/// </summary>
public class UserDbApi
{
    public UserDbApi()
    {
        //
        // TODO: Add constructor logic here
        //
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

        //foreach (DataRow row in citiesTable.Rows) 
        //{
        //    responseStr += " <option value='" + row["id"] + "'> " + row["CityName"] + " </option>";
        //}

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
            responseStr += " <option value='" + citiesTable.Rows[i]["id"] + "'" + selected + "> " + citiesTable.Rows[i]["CityName"] + " </option>";
        }

        //foreach (DataRow row in citiesTable.Rows) 
        //{
        //    responseStr += " <option value='" + row["id"] + "'> " + row["CityName"] + " </option>";
        //}

        return responseStr;
    }

    public static string getPhonePrefixes()
    {
        string sqlQuery = "select * from PhonePrefixesTbl";
        DataTable citiesTable = DAL.GetDataTable(sqlQuery);
        string responseStr = "";
        //for (int i = 0; i < citiesTable.Rows.Count; i++)
        //{
        //    responseStr += " <option value='" + citiesTable.Rows[i]["id"] + "'> " + citiesTable.Rows[i]["city"] + " </option>";
        //}

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
            responseStr += " <option value='" + row["id"] + "'" + selected + ">" + row["Prefix"] + " </option>";
        }


        return responseStr;
    }

    public static int findUserId(string userName)
    {
        OleDbConnection dbConnection = DAL.GetConnection();
        dbConnection.Open();
        string sqlQuery = "select ID from UsersTbl where userName = '" + userName + "'";
        OleDbCommand dbCmd = DAL.GetCommand(dbConnection, sqlQuery);
        Object obj = dbCmd.ExecuteScalar();
        dbConnection.Close();
        int retVal = -1;
        if (obj != null)
        {
            retVal = (int)obj;
        }
        return retVal;
    }
}