using System;
using System.Data.SqlClient;
using System.Data;
using static FoosballAttempt1.Program;
using static FoosballAttempt1.Algorithim;
using static FoosballAttempt1.Player;
namespace FoosballAttempt1
{
    //This class has way too much in it
    public class DbConnection
    {

        //Retrieves player stats from DB, then creates player with that info. 
        //If no player in DB, creates player with default values and adds to DB            
        public static Player GetPlayer(string name)
        {
            DataTable dataTable = ExecuteQuery("SELECT TOP 1 * FROM [PlayerStats] WHERE Name='" + name + "'");
            if (dataTable.Rows.Count == 0)
            {
                ExecuteQuery("INSERT INTO [PlayerStats] VALUES ('" + name + "', " + MU0 + ", " + SIGMA0 + ")");
                return GetPlayer(name);
             }
            return new Player(dataTable.Rows[0].Field<double>("Mu"), dataTable.Rows[0].Field<double>("Sigma"), name);           
        }
        //GetPlayer, but for teamstats
        public static Player GetTeam(string name)
        {           
            DataTable dataTable = ExecuteQuery("SELECT TOP 1 * FROM [TeamStats] WHERE Name ='" + name + "'");           
            if (dataTable.Rows.Count == 0)
            {
                ExecuteQuery("INSERT INTO [TeamStats] VALUES ('" + name + "', " + MU0 + ", " + SIGMA0 + ")");
                return GetTeam(name);
            }
            return new Player(dataTable.Rows[0].Field<double>("Mu"), dataTable.Rows[0].Field<double>("Sigma"), name);            
        }
        //Deletes PlayerStats/teamstats table, then goes through MatchRecords table and recalculates stats
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
        //Delete Leaderboard, pull all teams from teamstats, calculate scores/ranks, insert into leaderboard
        public static void RefreshTeamLeaderboard()
        {
            ExecuteQuery("TRUNCATE TABLE [TeamLeaderboard]");
            DataTable dataTable = ExecuteQuery("SELECT Name FROM TeamStats");
            Player[] teams = new Player[dataTable.Rows.Count];

            for (int i = 0; i <= dataTable.Rows.Count - 1; i++)
            {
                teams[i] = GetTeam(dataTable.Rows[i].Field<string>("Name"));
            }

            CalculateRanks(teams);
            foreach (Player team in teams)
            {
                ExecuteQuery("INSERT INTO TeamLeaderboard VALUES ('" + team.Name + "', " + team.Rank + ", " + team.Score + ")");
            }
        }
        //Delete Leaderboard, pull all players from playerstats, calculate scores/ranks, insert into leaderboard
        public static void RefreshLeaderboard()
        {             
            ExecuteQuery("TRUNCATE TABLE [Leaderboard]");
            DataTable dataTable = ExecuteQuery("SELECT Name FROM PlayerStats");

            Player[] players = new Player[dataTable.Rows.Count];
            for (int i = 0; i <= dataTable.Rows.Count - 1; i++)
            {
                players[i] = GetPlayer(dataTable.Rows[i].Field<string>("Name"));
            }
            CalculateRanks(players);

            foreach (Player player in players)
            {
                ExecuteQuery("INSERT INTO Leaderboard VALUES ('" + player.Name + "', " + player.Rank + ", " + player.Score + ")");
            }
            
        }
        public static DataTable ExecuteQuery(string query)
        {
            using (SqlConnection DBConnection = new SqlConnection(@CONNSTRING))
            {
                DBConnection.Open();
                SqlCommand queryCommand = new SqlCommand(query, DBConnection);
                SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(queryCommandReader);
                DBConnection.Close();
                return dataTable;
            }
        }
    }
}
