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
        //gets names and date from user, inserts match into matchrecord, updates player/team stats, displays difference in scores, refreshes leaderboard
        public static void AddMatch()
        {
            Player[] players = new Player[4];
            string date;
            do
            {               
                players[0] = GetPlayer(Text("Please enter the first name of one of the winning players"));
                players[1] = GetPlayer(Text("Please enter the first name of the other winning player"));
                players[2] = GetPlayer(Text("Please enter the first name of one of the losing players"));
                players[3] = GetPlayer(Text("Please enter the first name of the other losing player"));
                date = Text("Please enter the date of the match as YYYY-MM-DD");
            }
            while ((!Text(" Winning team: " + players[0].Name + " and " + players[1].Name +
                    "\r\n Losing Team: " + players[2].Name + " and " + players[3].Name +
                    "\r\n Date: " + date +
                    "\r\n\r\n Is this correct? Y/N").ToLower().Equals("y")));
            Console.Clear();
            ExecuteQuery("INSERT INTO [MatchRecords] VALUES ('" + players[0].Name + "', '" + players[1].Name + "', '" + players[2].Name + "', '" + players[3].Name + "', '" + date + "')");
            SkillUpdate(players);            
            Player[] teams = TeamSkillUpdate(players);
            foreach (Player player in players)
            {
                double oldscore = player.Score;
                CalculateScore(player);
                Console.WriteLine(player.Name +
                    "\r\n Old Score: " + Math.Round(oldscore,3) +
                    "\r\n New Score: " + Math.Round(player.Score,3) +
                    "\r\n Score Change: " + Math.Round(player.Score - oldscore,3));
            }
            Console.Write("\r\n");
            foreach(Player team in teams)
            {
                double oldscore = team.Score;
                CalculateScore(team);
                Console.WriteLine("Team " + team.Name +
                    "\r\n Old Score: " + Math.Round(oldscore, 3) +
                    "\r\n New Score: " + Math.Round(team.Score, 3) +
                    "\r\n Score Change: " + Math.Round(team.Score - oldscore, 3));
            }
            RefreshLeaderboard();
            RefreshTeamLeaderboard();
            Console.WriteLine("\r\n Press any key...");
            Console.ReadKey();
        }
        public static void DisplayTeamLeaderboard()
        {
            RefreshTeamLeaderboard();
            DataTable dataTable = ExecuteQuery("SELECT * FROM TeamLeaderboard");
            Console.WriteLine("The current Team Leaderboard is:");
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                Console.Write("Rank " + dataTable.Rows[j].Field<int>("Rank") + ". " + dataTable.Rows[j].Field<string>("Name") + " -- Score: " + Math.Round(dataTable.Rows[j].Field<double>("Score"), 3));
                Console.WriteLine("");
            }
            Console.WriteLine("Press any key...");
            Console.ReadKey();           
        }
        public static void DisplayLeaderboard()
        {
            RefreshLeaderboard();
            DataTable dataTable = ExecuteQuery("SELECT * FROM Leaderboard");
            Console.WriteLine("The current Leaderboard is:");
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                Console.Write("Rank " + dataTable.Rows[j].Field<int>("Rank") + ". " + dataTable.Rows[j].Field<string>("Name") + " -- Score: " + Math.Round(dataTable.Rows[j].Field<double>("Score"),3));
                Console.WriteLine("");
            }
            Console.WriteLine("Press any key...");
            Console.ReadKey();           
        }
    }
}
