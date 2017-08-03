using System;
namespace FoosballAttempt1
{
    public class Player
    {
        public double Mu { get; set; }
        public double Sigma { get; set; }
        public string Name { get; set; } 
        public double Score { get; set; }
        public int Rank { get; set; }

        public Player(double mu, double sigma, string name)
        {
            Mu = mu;
            Sigma = sigma;
            Name = name;
        }



    }
}
