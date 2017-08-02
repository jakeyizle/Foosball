using System;
using System.Data.SqlClient;
using System.Data;
using static FoosballAttempt1.Program;
using static FoosballAttempt1.Algorithim;

namespace FoosballAttempt1
{
    public class DbConnection
    {

        public static string GetPlayerName()
        {
            Console.WriteLine("Enter Player Name");
            return Console.ReadLine();
        }

        public static Player CreatePlayer(SqlConnection DBConnection)
        {            
            string name = GetPlayerName();
            GetStats(name, DBConnection, out double mu, out double sigma);

            return new Player(mu, sigma, name);
        }

        public static Player CreatePlayerMatchRecord(string name, SqlConnection DBConnection)
        {
            GetStats(name, DBConnection, out double mu, out double sigma);
            return new Player(mu, sigma, name);
        }

        public static void GetStats(string name, SqlConnection DBConnection, out double mu, out double sigma)
        {
            string query = "SELECT TOP 1 * FROM [PlayerStats] WHERE Name='" + name + "'";
            SqlCommand queryCommand = new SqlCommand(query, DBConnection);
            SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(queryCommandReader);

            //Check if empty, if yes then make a new player in DB and fetch their stats
            if (dataTable.Rows.Count == 0)
            {
                dataTable = InitalizePlayerInDB(name, DBConnection);
            }

            mu = dataTable.Rows[0].Field<double>("Mu");
            sigma = dataTable.Rows[0].Field<double>("Sigma");
        }

       public static DataTable InitalizePlayerInDB(string name, SqlConnection DBConnection)
        {
            string query = "INSERT INTO [PlayerStats] VALUES ('" + name + "', " + mu0 + ", " + sigma0 + ")";
            SqlCommand queryCommand = new SqlCommand(query, DBConnection);
            //Better way to execute a SQL Command?
            int a = queryCommand.ExecuteNonQuery();

            string check = "SELECT TOP 1 * FROM [PlayerStats] WHERE Name='" + name + "'";
            SqlCommand checkCommand = new SqlCommand(check, DBConnection);
            SqlDataReader queryCommandReader = checkCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(queryCommandReader);

            return dataTable;
        }

       public static void UpdatePlayer(Player player, SqlConnection DBConnection)
        {
            string query = "UPDATE [PlayerStats] SET Mu = " + player.Mu + ", Sigma = " + player.Sigma + "WHERE Name = '" + player.Name + "'";
            SqlCommand queryCommand = new SqlCommand(query, DBConnection);
            //Better way to execute a SQL Command?
             int a = queryCommand.ExecuteNonQuery();
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

        public static void CalculateFromScratchMatchRecords(SqlConnection DBConnection)
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
                    players[i] = CreatePlayerMatchRecord(name, DBConnection);
                    i++;
                }
                SkillUpdate(players[0], players[1], players[2], players[3], DBConnection);
            }
        }

       public static void Leaderboard(SqlConnection DBConnection)
        {
            //Delete Leaderboard, then repopulate
            string delete = "DELETE FROM [Leaderboard]";
            SqlCommand deleteCommand = new SqlCommand(delete, DBConnection);
            int a = deleteCommand.ExecuteNonQuery();

            string query = "SELECT Name FROM PlayerStats";
            SqlCommand queryCommand = new SqlCommand(query, DBConnection);
            SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(queryCommandReader);

            Player[] players = new Player[dataTable.Rows.Count];

            for (int i = 0; i <= dataTable.Rows.Count - 1; i++)
            {
                players[i] = CreatePlayerMatchRecord(dataTable.Rows[i].Field<string>("Name"), DBConnection);
            }

            for (int i = 0; i < players.Length; i++)
            {
                CalculateScore(players[i]);
            }

            CalculateRanks(players);

            foreach (Player player in players)
            {
                string insert = "INSERT INTO Leaderboard VALUES ('" + player.Name + "', " + player.Rank + ", " + player.Score + ")";
                SqlCommand insertCommand = new SqlCommand(insert, DBConnection);
                int b = insertCommand.ExecuteNonQuery();
            }
        }
    }
}
