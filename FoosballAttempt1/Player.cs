using System;
using static FoosballAttempt1.DbConnection;
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
            Score = mu - 3 * sigma;
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
        //sorts each team (0, 1 and 2, 3) in alphabetical order
        //returns 2 player array, winning team and losing team
        public static Player[] MakeTeam(Player[] players)
        {
            for (int j = 0; j <= 2; j = j + 2)
            {
                if (string.Compare(players[j].Name, players[j + 1].Name, true) > 0)
                {
                    Player buffer = players[j];
                    players[j] = players[j + 1];
                    players[j + 1] = buffer;
                }

            }
            return new Player[] { GetTeam(players[0].Name + " and " + players[1].Name), GetTeam(players[2].Name + " and " + players[3].Name) };
        }

    }
}
