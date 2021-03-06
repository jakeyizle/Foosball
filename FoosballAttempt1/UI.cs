﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using static FoosballAttempt1.Program;
using static FoosballAttempt1.Algorithim;
using static FoosballAttempt1.DbConnection;
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
        //This doesn't have to refresh leaderboard, as the leaderboard is also refreshed before being displayed
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

            Console.WriteLine("{0,-20}{1,-15}{2,-15}{3,-15}",
                "Player Name",
                "Old Rating",
                "New Rating",
                "Rating Change");
            foreach (Player player in players)
            {
                double oldscore = player.Rating;
                CalculateRating(player);
                Console.WriteLine("{0,-20}{1,-15}{2,-15}{3,-15}",
                    player.Name,
                    Math.Round(oldscore, 3),
                    Math.Round(player.Rating, 3),
                    Math.Round(player.Rating - oldscore, 3));
            }
            Console.Write("\r\n");
            Console.WriteLine("{0,-20}{1,-15}{2,-15}{3,-15}",
                "Team Name",
                "Old Rating",
                "New Rating",
                "Rating Change");
            foreach (Player team in teams)
            {
                double oldscore = team.Rating;
                CalculateRating(team);
                Console.WriteLine("{0,-20}{1,-15}{2,-15}{3,-15}",
                    team.Name,
                    Math.Round(oldscore, 3),
                    Math.Round(team.Rating, 3),
                    Math.Round(team.Rating - oldscore, 3));
            }
            RefreshLeaderboard();
            RefreshTeamLeaderboard();
            Console.WriteLine("\r\nPress any key...");
            Console.ReadKey();
        }

        public static void DisplayTeamLeaderboard()
        {
            RefreshTeamLeaderboard();
            DataTable dataTable = ExecuteQuery("SELECT * FROM TeamLeaderboard");
            Console.Write(String.Format("\r\n{0,-6}{1,-20}{2,7}{3,15}",
                "Rank", 
                "Team Name",
                "Rating",
                "Games Played"));
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                Console.Write(String.Format("\r\n{0,-6}{1,-20}{2,7}{3,15}",
                    dataTable.Rows[j].Field<int>("Rank"), 
                    dataTable.Rows[j].Field<string>("Name"), 
                    Math.Round(dataTable.Rows[j].Field<double>("Score"), 3), 
                    dataTable.Rows[j].Field<int>("Game Count")));
            }
            Console.WriteLine("\r\nPress any key...");
            Console.ReadKey();           
        }
        public static void DisplayLeaderboard()
        {
            RefreshLeaderboard();
            DataTable dataTable = ExecuteQuery("SELECT * FROM Leaderboard");
            Console.Write(String.Format("\r\n{0,-6}{1,-20}{2,7}{3,15}", 
                "Rank", 
                "Player Name", 
                "Rating", 
                "Games Played"));
            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                Console.Write(String.Format("\r\n{0,-6}{1,-20}{2,7}{3,15}", 
                    dataTable.Rows[j].Field<int>("Rank"), 
                    dataTable.Rows[j].Field<string>("Name"), 
                    Math.Round(dataTable.Rows[j].Field<double>("Score"), 3), 
                    dataTable.Rows[j].Field<int>("Game Count")));
            }
            Console.WriteLine("\r\nPress any key...");
            Console.ReadKey();           
        }
        //Deletes PlayerStats/teamstats table, then goes through MatchRecords table and recalculates stats
        //this is useful for when you enter 'test matches'. Just delete the matches from the database, then run this. itll delete the extra players from playerstats/leaderboard
        public static void RefreshStats()
        {
            ExecuteQuery("TRUNCATE TABLE [PlayerStats]");
            ExecuteQuery("TRUNCATE TABLE [teamStats]");

            DataTable dataTable = ExecuteQuery("SELECT [Win1], [Win2], [Lose1], [Lose2] FROM [MatchRecords] ORDER BY Date ASC");
            Player[] players = new Player[4];

            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                int i = 0;
                foreach (DataColumn column in dataTable.Columns)
                {
                    string name = dataTable.Rows[j][column.ColumnName].ToString().ToLower();
                    players[i] = GetPlayer(name);
                    i++;
                }
                SkillUpdate(players);
                TeamSkillUpdate(players);
            }
        }
        public static void DisplayMatchQuality()
        {
            Player player1 = GetPlayer(Text("Please enter the first player"));
            Player player2 = GetPlayer(Text("Please enter the second player"));
            Player player3 = GetPlayer(Text("Please enter the third player"));
            Player player4 = GetPlayer(Text("Please enter the fourth player"));

            Console.WriteLine("\r\n{0,-20}{1,-20}{2,15}",
                "Team 1",
                "Team 2",
                "Match Quality");
            Console.WriteLine("{0,-20}{1,-20}{2,15}",
                player1.Name + " and " + player2.Name,
                player3.Name + " and " + player4.Name,
                Math.Round(MatchQuality(player1, player2, player3, player4),3));
            Console.WriteLine("{0,-20}{1,-20}{2,15}",
                player1.Name + " and " + player3.Name,
                player2.Name + " and " + player4.Name,
                Math.Round(MatchQuality(player1, player3, player2, player4), 3));
            Console.WriteLine("{0,-20}{1,-20}{2,15}",
                player1.Name + " and " + player4.Name,
                player3.Name + " and " + player2.Name,
                Math.Round(MatchQuality(player1, player4, player3, player2), 3));

            Console.WriteLine("\r\nPress any key...");
            Console.ReadKey();
        }
    }
}
