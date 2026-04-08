using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DAL
/// </summary>
public class DAL
{
    public DAL()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public static OleDbConnection GetConnection()
    {
        OleDbConnection conn = new OleDbConnection();
        conn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\ProjectDB2.accdb;Persist Security Info=True";
        return conn;
    }

    public static OleDbCommand GetCommand(OleDbConnection conn, string sqlStr)
    {
        OleDbCommand cmd = new OleDbCommand();
        cmd.CommandText = sqlStr;
        cmd.Connection = conn;
        cmd.CommandType = CommandType.Text;
        return cmd;
    }

    public static DataTable GetDataTable(string sqlQuery)
    {
        OleDbConnection dbConnection = GetConnection();
        OleDbCommand dbCommand = GetCommand(dbConnection, sqlQuery);
        DataTable dt = new DataTable();
        OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter();
        oleDbDataAdapter.SelectCommand = dbCommand;
        oleDbDataAdapter.Fill(dt);
        return dt;
    }

    public static int ExecuteNonQuery(string sqlStr)
    {
        OleDbConnection dbConnection = GetConnection();
        dbConnection.Open();
        OleDbCommand dbCommand = GetCommand(dbConnection, sqlStr);

        int rowsAffected = dbCommand.ExecuteNonQuery();
        dbConnection.Close();
        return rowsAffected;
    }
}