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
        public static void Start(SqlConnection DBConnection)
        {
            string answer = Text("Please choose an option:" +
                "\r\n 1. Add a match and update skills" +
                "\r\n 2. View Leaderboard" +
                "\r\n 3. Delete all Player Stats and recalculate from Match Records" +
                "\r\n Exit");

            switch (answer.ToLower())
            {
                case "1":
                    AddMatch(DBConnection);
                    Start(DBConnection);
                    break;
                case "2":
                    DisplayLeaderboard(DBConnection);
                    Start(DBConnection);
                    break;
                case "3":
                    RefreshPlayerStats(DBConnection);
                    Start(DBConnection);
                    break;
                default:
                    Start(DBConnection);
                    break;
                case "exit":
                    Environment.Exit(1);
                    break;

            }
        }

        public static string Text(string display)
        {
            Console.Clear();
            Console.WriteLine(display);
            return Console.ReadLine();
        }

        public static void AddMatch(SqlConnection DBConnection)
        {
            Player[] players = new Player[4];
            players[0] = GetPlayer(Text("Please enter the first name of one of the winning players"), DBConnection);
            players[1] = GetPlayer(Text("Please enter the first name of the other winning player"), DBConnection);
            players[2] = GetPlayer(Text("Please enter the first name of one of the losing players"), DBConnection);
            players[3] = GetPlayer(Text("Please enter the first name of the other losing player"), DBConnection);
            string date = Text("Please enter the date of the math as YYYY-MM-DD");

            InsertIntoMatchRecord(DBConnection, players, date);
            SkillUpdate(players[0], players[1], players[2], players[3], DBConnection);
            RefreshLeaderboard(DBConnection);
        }

        static void InsertIntoMatchRecord(SqlConnection DBConnection, Player[] players, string date)
        {
            string query = "INSERT INTO [MatchRecords] VALUES ('" + players[0].Name + "', '" + players[1].Name + "', '" + players[2].Name + "', '" + players[3].Name + "', '" + date + "')";
            SqlCommand queryCommand = new SqlCommand(query, DBConnection);
            queryCommand.ExecuteReader();
        }

        public static void DisplayLeaderboard(SqlConnection DBConnection)
        {
            RefreshLeaderboard(DBConnection);

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
