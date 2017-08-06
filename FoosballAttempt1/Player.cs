using System;
using System.Data;
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
        public int GameCount { get; set; }

        public Player(double mu, double sigma, string name)
        {
            Mu = mu;
            Sigma = sigma;
            Name = name;
            Score = mu - 3 * sigma;
            //this is solution to make sure that "teams" get a gamecount
            //only team names should contain " and ", then code is to generate first players name and second players name
            //then count MatchRecords where both players are on same team
            if (name.Contains(" and "))
            {
                string name1 = "";
                string name2 = "";
                foreach (char c in name)
                {
                    if (char.IsWhiteSpace(c))
                    {
                        break;
                    }
                    name1 = name1 + c;
                }
                int counter = 0;
                foreach (char c in name)
                {
                    if (counter==2)
                    { name2 = name2 + c; }
                    if (char.IsWhiteSpace(c))
                    { counter++; }
                }
                DataTable dataTable = ExecuteQuery("SELECT(COUNT(*)) FROM MatchRecords WHERE('" + name1 + "' in (win1, win2) and '" + name2 + "' in (win1, win2)) OR('" + name1 + "' in (lose1, lose2) and '" + name2 + "' in (lose1, lose2))");
                GameCount = dataTable.Rows[0].Field<int>(0);
            }
            else
            {
                DataTable dataTable = ExecuteQuery("SELECT COUNT(*) FROM MatchRecords WHERE '" +  name + "' in (win1, win2, lose1, lose2)");
                GameCount = dataTable.Rows[0].Field<int>(0);
            }

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
