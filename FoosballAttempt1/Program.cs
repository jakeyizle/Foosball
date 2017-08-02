using System;
using System.Data.SqlClient;
using System.Data;
using static FoosballAttempt1.DbConnection;
using static FoosballAttempt1.Algorithim;


namespace FoosballAttempt1
{
    class Program
    {
 
        //mu0 and sigma0 are the inital values players have
        //if Player1.Mu - Player2.Mu = beta, then Player1 has an ~80% chance of beating Player2
        //tau is the additive dynamics factor - we add it to sigma before updating. otherwise the algorithim would always decrease sigma
        //not pictured is epsilon, the probability that there is a draw. for foosball, it isnt possible to draw so epsilon is 0.
        public static double mu0 = 25;
        public static double sigma0 = mu0 / 3;
        public static double beta = sigma0/2;
        public static double tau = sigma0/100;

        static void Main()
        {
            string ConnectionString = @"Data Source = 423ZTF2; Initial Catalog = Foosball; Integrated Security=True; MultipleActiveResultSets=True";
            SqlConnection DBConnection = new SqlConnection(ConnectionString);
            DBConnection.Open();

            
            Start(DBConnection);

            
            DBConnection.Close();
        }
        static void Start(SqlConnection DBConnection)
        {
            string answer = Text("Please choose an option: 1. Add a match and update skills, 2. View Leaderboard, 3. Delete all Player Stats and recalculate from Match Records");
    
            switch (answer)
            {
                case "1":
                    AddMatch(DBConnection);
                    break;
                case "2":
                    DisplayLeaderboard(DBConnection);
                    break;
                case "3":
                    CalculateFromScratchMatchRecords(DBConnection);
                    break;
                default:
                    Start(DBConnection);
                    break;
            }


        }

        static string Text(string display)
        {
            Console.Clear();
            Console.WriteLine(display);
            return Console.ReadLine();
        }

        static void AddMatch(SqlConnection DBConnection)
        {
            Player[] players = new Player[4];
            players[0] = CreatePlayerMatchRecord(Text("Please enter the first name of one of the winning players"), DBConnection);
            players[1] = CreatePlayerMatchRecord(Text("Please enter the first name of the other winning player"), DBConnection);
            players[2] = CreatePlayerMatchRecord(Text("Please enter the first name of one of the losing players"), DBConnection);
            players[3] = CreatePlayerMatchRecord(Text("Please enter the first name of the other losing player"), DBConnection);
            // DateTime date = DateTime.Parse(Text("Please enter the date of the math as YYYY-MM-DD"));
            string date = Text("Please enter the date of the math as YYYY-MM-DD");

            InsertIntoMatchRecord(DBConnection, players, date);
            SkillUpdate(players[0], players[1], players[2], players[3], DBConnection);
            Leaderboard(DBConnection);
        }
        
        static void InsertIntoMatchRecord(SqlConnection DBConnection, Player[] players, string date)
        {
            string query = "INSERT INTO [MatchRecords] VALUES ('" + players[0].Name + "', '" + players[1].Name + "', '" + players[2].Name + "', '" + players[3].Name + "', '" + date +"')";
            SqlCommand queryCommand = new SqlCommand(query, DBConnection);
            //Better way to execute a SQL Command?
            queryCommand.ExecuteReader();
        }

        static void DisplayLeaderboard(SqlConnection DBConnection)
        {
            Leaderboard(DBConnection);

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
    

