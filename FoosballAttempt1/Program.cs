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
        public const double MU0 = 25;
        public const double SIGMA0 = MU0 / 3;
        public const double BETA = SIGMA0/2;
        public const double TAU = SIGMA0/100;

        static void Main()
        {
            SqlConnection DBConnection = new SqlConnection(@"Data Source = 423ZTF2; Initial Catalog = Foosball; Integrated Security=True; MultipleActiveResultSets=True");
            DBConnection.Open();
            
            Start(DBConnection);

            DBConnection.Close();
        }

        


        

        






    }
}
    

