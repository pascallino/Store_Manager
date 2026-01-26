using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace StoreManager
{
    public static class TrialManager
    {
        public static void CheckTrialStatus()
        {
            int daysLeft = GetRemainingTrialDays();

            if (daysLeft <= 0)
            {
                MessageBox.Show(
                    "Your 30-day trial has expired.\n\n" +
                    "Please purchase a new Microsoft SQL Server database license immediately.",
                    "Microsoft SQL Trial Warniog",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                Application.Exit();
            }
            else
            {
                MessageBox.Show(
                    $"Trial Version Notice\n\n" +
                    $"Your trial will expire in {daysLeft} day(s).\n\n" +
                    "Please purchase a new Microsoft SQL Server database license.",
                     "Microsoft SQL Trial Warniog",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private static int GetRemainingTrialDays()
        {
            DateTime firstRun = GetOrCreateFirstRunDate();
            int totalTrialDays = 30;

            int daysUsed = (DateTime.Now - firstRun).Days;
            return totalTrialDays - daysUsed;
        }

        private static DateTime GetOrCreateFirstRunDate()
        {
            using (SqlConnection con = DB.GetCon())
            using (SqlCommand cmd = new SqlCommand(
                "SELECT TOP 1 FirstRunDate FROM AppLicense", con))
            {
                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    return Convert.ToDateTime(result);

                DateTime now = DateTime.Now;

                using (SqlCommand insertCmd = new SqlCommand(
                    "INSERT INTO AppLicense (FirstRunDate) VALUES (@date)", con))
                {
                    insertCmd.Parameters.AddWithValue("@date", now);
                    insertCmd.ExecuteNonQuery();
                }

                return now;
            }
        }
    }

}
