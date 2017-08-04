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
            using (SqlConnection DBConnection = new SqlConnection(@CONNSTRING))
            {
                DBConnection.Open();
                string query = "SELECT TOP 1 * FROM [PlayerStats] WHERE Name='" + name + "'";
                SqlCommand queryCommand = new SqlCommand(query, DBConnection);
                SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(queryCommandReader);

                if (dataTable.Rows.Count == 0)
                {
                    InitalizePlayerInDB(name);
                    return GetPlayer(name);
                }
                return new Player(dataTable.Rows[0].Field<double>("Mu"), dataTable.Rows[0].Field<double>("Sigma"), name);
            }
        }

        public static void RefreshPlayerStats()
        {
            //Deletes PlayerStats table, then goes through MatchRecords table and recalculates stats
            using (SqlConnection DBConnection = new SqlConnection(@CONNSTRING))
            {
                
                string delete = "TRUNCATE TABLE [PlayerStats]";
                ExecuteQuery(delete);

                DBConnection.Open();
                string query =   "SELECT [Win1], [Win2], [Lose1], [Lose2] FROM [MatchRecords] ORDER BY Date ASC";
                SqlCommand queryCommand = new SqlCommand(query, DBConnection);
                SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(queryCommandReader);
                DBConnection.Close();
                Player[] players = new Player[4];

                for (int j = 0; j < dataTable.Rows.Count; j++)
                {
                    int i = 0;
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        string name = dataTable.Rows[j][column.ColumnName].ToString();
                        players[i] = GetPlayer(name);
                        i++;
                    }
                    SkillUpdate(players[0], players[1], players[2], players[3]);
                }
            }
        }
        
        public static void GetScore(Player[] players)
        {
            using (SqlConnection DBConnection = new SqlConnection(@CONNSTRING))
            {
                DBConnection.Open();
                foreach (Player player in players)
                {
                    string query = "SELECT Score FROM [Leaderboard] WHERE Name = '" + player.Name + "'";
                    SqlCommand queryCommand = new SqlCommand(query, DBConnection);
                    SqlDataReader queryCommandReader = queryCommand.ExecuteReader();


                    if(queryCommandReader.HasRows)
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(queryCommandReader);
                        player.Score = dataTable.Rows[0].Field<double>("Score");
                    }
                    else
                    {

                        CalculateScore(player);
                    }
                }
            }
        }

        public static void RefreshLeaderboard()
        {
            using (SqlConnection DBConnection = new SqlConnection(@CONNSTRING))
            {
                //Delete Leaderboard, then recalculate Scores/Ranks, then repopulate Leaderboard
                              
                string delete = "TRUNCATE TABLE [Leaderboard]";
                ExecuteQuery(delete);
                DBConnection.Open();
                string query = "SELECT Name FROM PlayerStats";
                SqlCommand queryCommand = new SqlCommand(query, DBConnection);
                SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(queryCommandReader);

                Player[] players = new Player[dataTable.Rows.Count];

                for (int i = 0; i <= dataTable.Rows.Count - 1; i++)
                {
                    players[i] = GetPlayer(dataTable.Rows[i].Field<string>("Name"));
                }

                CalculateRanks(players);

                foreach (Player player in players)
                {
                    string insert = "INSERT INTO Leaderboard VALUES ('" + player.Name + "', " + player.Rank + ", " + player.Score + ")";
                    SqlCommand insertCommand = new SqlCommand(insert, DBConnection);
                    insertCommand.ExecuteReader();
                }
            }
        }

        private static void ExecuteQuery(string query)
        {
            using (SqlConnection DBConnection = new SqlConnection(@CONNSTRING))
            {
                DBConnection.Open();
                SqlCommand queryCommand = new SqlCommand(query, DBConnection);
                queryCommand.ExecuteReader();
            }
        }

        public static void InitalizePlayerInDB(string name)
        {
            string query = "INSERT INTO [PlayerStats] VALUES ('" + name + "', " + MU0 + ", " + SIGMA0 + ")";
            ExecuteQuery(query);
        }

        public static void UpdatePlayer(Player player)
        {
            string query = "UPDATE [PlayerStats] SET Mu = " + player.Mu + ", Sigma = " + player.Sigma + "WHERE Name = '" + player.Name + "'";
            ExecuteQuery(query);
        }

        public static void InsertIntoMatchRecord(Player[] players, string date)
        {
            string query = "INSERT INTO [MatchRecords] VALUES ('" + players[0].Name + "', '" + players[1].Name + "', '" + players[2].Name + "', '" + players[3].Name + "', '" + date + "')";
            ExecuteQuery(query);
        }


    }
}
