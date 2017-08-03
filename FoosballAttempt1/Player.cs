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

        //Conservative score estimate: 99% of the time you will play at a level at or above this value
        public static void CalculateScore(Player player)
        {
            player.Score = (player.Mu - 3 * player.Sigma);
        }

        //Sorts player array by Score (largest to smallest) then assigns rank
        public static void CalculateRanks(Player[] players)
        {
            Array.Sort(players, (a, b) => a.Score.CompareTo(b.Score));
            Array.Reverse(players);
            for (int i = 0; i < players.Length; i++)
            {
                players[i].Rank = i + 1;
            }
        }


    }
}
