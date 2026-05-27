using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EventVenueApp
{
    public static class DBHelper
    {
        // *** CHANGE THIS IF NEEDED ***
        // For SQL Server Express: @"Server=.\SQLEXPRESS;Database=EventVenueManagment;Integrated Security=True;"
        // For default instance:   @"Server=(local);Database=EventVenueManagment;Integrated Security=True;"
        // With username/password:  @"Server=.;Database=EventVenueManagment;User Id=sa;Password=YourPassword;"
        public static string ConnectionString =
    @"Data Source=.\SQLEXPRESS;Initial Catalog=EventVenueManagment;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        /// <summary>Returns a DataTable from a SELECT query.</summary>
        public static DataTable GetData(string query, SqlParameter[] prms = null)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (prms != null) cmd.Parameters.AddRange(prms);
                        new SqlDataAdapter(cmd).Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        /// <summary>Executes INSERT / UPDATE / DELETE. Returns true on success.</summary>
        public static bool Execute(string query, SqlParameter[] prms = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (prms != null) cmd.Parameters.AddRange(prms);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
