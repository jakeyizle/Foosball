using System;
using System.Data.SqlClient;
using System.Data;
using static FoosballAttempt1.Program;
using static FoosballAttempt1.Algorithim;
using static FoosballAttempt1.Player;
namespace FoosballAttempt1
{
    public class DbConnection
    {
        //Retrieves player stats from DB, then creates player with that info. 
        //If no player in DB, creates player with default values and adds to DB
        public static Player GetPlayer(string name, SqlConnection DBConnection)
        {
            string query = "SELECT TOP 1 * FROM [PlayerStats] WHERE Name='" + name + "'";
            SqlCommand queryCommand = new SqlCommand(query, DBConnection);
            SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(queryCommandReader);
            
            if (dataTable.Rows.Count == 0)
            {
                InitalizePlayerInDB(name, DBConnection);
                return GetPlayer(name, DBConnection);
            }
            return new Player(dataTable.Rows[0].Field<double>("Mu"), dataTable.Rows[0].Field<double>("Sigma"), name);
        }


       public static void  InitalizePlayerInDB(string name, SqlConnection DBConnection)
        {
            string query = "INSERT INTO [PlayerStats] VALUES ('" + name + "', " + mu0 + ", " + sigma0 + ")";
            SqlCommand queryCommand = new SqlCommand(query, DBConnection);
            queryCommand.ExecuteReader();
        }

       public static void UpdatePlayer(Player player, SqlConnection DBConnection)
        {
            string query = "UPDATE [PlayerStats] SET Mu = " + player.Mu + ", Sigma = " + player.Sigma + "WHERE Name = '" + player.Name + "'";
            SqlCommand queryCommand = new SqlCommand(query, DBConnection);
            queryCommand.ExecuteReader();
        }


        static void CalculateScore(Player player)
        {
            player.Score = (player.Mu - 3 * player.Sigma);
        }

        static void CalculateRanks(Player[] players)
        {
            Array.Sort(players, (a, b) => a.Score.CompareTo(b.Score));
            Array.Reverse(players);
            for (int i = 0; i < players.Length; i++)
            {
                players[i].Rank = i + 1;
            }
        }

        public static void RefreshPlayerStats(SqlConnection DBConnection)
        {
            //Deletes PlayerStats table, then goes through MatchRecords table and calculates stats
            string delete = "DELETE FROM [PlayerStats]";
            SqlCommand deleteCommand = new SqlCommand(delete, DBConnection);
            int a = deleteCommand.ExecuteNonQuery();

            string query = "SELECT [Win1], [Win2], [Lose1], [Lose2] FROM [MatchRecords] ORDER BY Date ASC";
            SqlCommand queryCommand = new SqlCommand(query, DBConnection);
            SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(queryCommandReader);

            Player[] players = new Player[4];

            for (int j = 0; j < dataTable.Rows.Count; j++)
            {
                int i = 0;
                foreach (DataColumn column in dataTable.Columns)
                {
                    string name = dataTable.Rows[j][column.ColumnName].ToString();
                    players[i] = GetPlayer(name, DBConnection);
                    i++;
                }
                SkillUpdate(players[0], players[1], players[2], players[3], DBConnection);
            }
        }

       public static void RefreshLeaderboard(SqlConnection DBConnection)
        {
            //Delete Leaderboard, then repopulate
            string delete = "DELETE FROM [Leaderboard]";
            SqlCommand deleteCommand = new SqlCommand(delete, DBConnection);
            deleteCommand.ExecuteReader();

            string query = "SELECT Name FROM PlayerStats";
            SqlCommand queryCommand = new SqlCommand(query, DBConnection);
            SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(queryCommandReader);

            Player[] players = new Player[dataTable.Rows.Count];

            for (int i = 0; i <= dataTable.Rows.Count - 1; i++)
            {
                players[i] = GetPlayer(dataTable.Rows[i].Field<string>("Name"), DBConnection);
                CalculateScore(players[i]);
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
}
