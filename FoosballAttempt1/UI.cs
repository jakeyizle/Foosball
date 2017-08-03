using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using static FoosballAttempt1.Program;
using static FoosballAttempt1.Algorithim;
using static FoosballAttempt1.DbConnection;
using static FoosballAttempt1.Formulas;
using static FoosballAttempt1.Player;
using System.Data;

namespace FoosballAttempt1
{
    public class UI
    {

        public static string Text(string display)
        {
            Console.Clear();
            Console.WriteLine(display);
            return Console.ReadLine();
        }

        public static void AddMatch()
        {
            Player[] players = new Player[4];
            players[0] = GetPlayer(Text("Please enter the first name of one of the winning players"));
            players[1] = GetPlayer(Text("Please enter the first name of the other winning player"));
            players[2] = GetPlayer(Text("Please enter the first name of one of the losing players"));
            players[3] = GetPlayer(Text("Please enter the first name of the other losing player"));
            string date = Text("Please enter the date of the match as YYYY-MM-DD");

            InsertIntoMatchRecord(players, date);
            SkillUpdate(players[0], players[1], players[2], players[3]);
            RefreshLeaderboard();
        }

        public static void DisplayLeaderboard()
        {
            using (SqlConnection DBConnection = new SqlConnection(@CONNSTRING))
            {
                DBConnection.Open();
                RefreshLeaderboard();

                string query = "SELECT * FROM Leaderboard";
                SqlCommand queryCommand = new SqlCommand(query, DBConnection);
                SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(queryCommandReader);

                Console.WriteLine("The current Leaderboard is:");
                for (int j = 0; j < dataTable.Rows.Count; j++)
                {
                    Console.Write("Rank " + dataTable.Rows[j].Field<int>("Rank") + ". " + dataTable.Rows[j].Field<string>("Name") + " -- Score: " + dataTable.Rows[j].Field<double>("Score"));
                    Console.WriteLine("");
                }
                Console.WriteLine("Press any key...");
                Console.ReadKey();
            }
        }
    }
}
