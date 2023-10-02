using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Helpers
{
    public static class HelperClass
    {
        private static bool isConnected( SqlConnection dbConnection)
        {
            using (var cmd = new SqlCommand("SELECT 1", dbConnection))
            {
                try
                {
                    cmd.ExecuteScalar();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        //public static DataTable callQuery(SqlConnection dbConnection, string query)
        //{
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.CommandTimeout = 300;
        //    cmd.CommandText = query;
        //    cmd.CommandType = CommandType.Text;

        //    if (!isConnected(dbConnection)) dbConnection.Open();
        //    cmd.Connection = dbConnection;
        //    //Console.WriteLine(cmd.CommandText);
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        SqlDataAdapter sda = new SqlDataAdapter(cmd);
        //        sda.Fill(dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    dbConnection.Close();
        //    return dt;
        //}

        public static DataTable callQuery(SqlConnection dbConnection, string query)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 300;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;

            if (!isConnected(dbConnection)) dbConnection.Open();
            cmd.Connection = dbConnection;

            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            catch (System.InvalidOperationException ex)
            {
                // Display the exception message
                MessageBox.Show($"Error: {ex.Message}\n\nStackTrace: {ex.StackTrace}");

                // If the exception has an inner exception, display its details too
                if (ex.InnerException != null)
                {
                    MessageBox.Show($"Inner Exception Error: {ex.InnerException.Message}\n\nStackTrace: {ex.InnerException.StackTrace}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }

            return dt;
        }

        public static string callCmd(SqlConnection dbConnection, string command)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandTimeout = 300;
                cmd.CommandText = command;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = dbConnection;
                if (!isConnected(dbConnection)) dbConnection.Open();
                Console.WriteLine(cmd.CommandText);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                dbConnection.Close();
                return ex.Message;
            }
            dbConnection.Close();
            return "";
        }
    }
}
