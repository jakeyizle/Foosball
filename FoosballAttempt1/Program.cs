using System;
using System.Data.SqlClient;
using System.Data;
using static FoosballAttempt1.DbConnection;
using static FoosballAttempt1.Algorithim;
using static FoosballAttempt1.UI;

namespace FoosballAttempt1
{
    class Program
    {
 
        //mu0 and sigma0 are the inital values players have
        //if Player1.Mu - Player2.Mu = beta, then Player1 has an ~80% chance of beating Player2
        //tau is the additive dynamics factor - we add it to sigma before updating. otherwise the algorithim would always decrease sigma - higher tau = more volatile leaderboard positions
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

        


        

        






    }
}
    

