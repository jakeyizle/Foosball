using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FoosballAttempt1.Formulas;
using static FoosballAttempt1.DbConnection;
using static FoosballAttempt1.Player;
using System.Data.SqlClient;

namespace FoosballAttempt1
{
    class Algorithim
    {
        public static Player[] TeamSkillUpdate(Player[] players)
        {
            //Sort each team into alphabetical order
            //returns 2 player array
            Player[] teams = MakeTeam(players);

            double c = CalculateC(teams[0].Sigma, teams[1].Sigma);
            double t = CalculateT(teams[0].Mu, teams[1].Mu, c);
            double n = CalculateN(t);
            double v = CalculateV(n, t);
            double w = CalculateW(v, t);

            foreach (Player player in teams)
            {
                DynamicsFactor(player);
            }
            //Purposefully don't update Score here, so that we can report on previous and updated scores
            //MU delta = sigma^2/c * v
            int i = 0;

            foreach (Player player in teams)
            {
                if (i < 1)
                { player.Mu = player.Mu + MuDelta(player, c, v); }
                else { player.Mu = player.Mu - MuDelta(player, c, v); }
                i++;
            }
            //sigma = sigma * sqrt( 1 - sigma^2/c^2 * w)
            foreach (Player player in teams)
            {
                SigmaUpdate(player, c, w);
            }
            //Push updates to DB
            foreach (Player player in teams)
            {
                ExecuteQuery("UPDATE [TeamStats] SET Mu = " + player.Mu + ", Sigma = " + player.Sigma + "WHERE Name = '" + player.Name + "'");
            }
            return teams;
        }



        public static void SkillUpdate(Player[] players)
        {
            //Create Team Mus and Sigmas used to calculate intermediate variables
            //Team Mu = Mu+Mu
            //Team Sigma = sqrt(sigma^2+sigma^2)
            double winningMu = TeamMu(players[0].Mu, players[1].Mu);
            double winningSigma = TeamSigma(players[0].Sigma, players[1].Sigma);

            double losingMu = TeamMu(players[2].Mu, players[3].Mu);
            double losingSigma = TeamSigma(players[2].Sigma, players[3].Sigma);

            //c^2 = sigma^2+sigma^2+beta^2
            double c = CalculateC(winningSigma, losingSigma);
            //t = (mu-mu)/c
            double t = CalculateT(winningMu, losingMu, c);
            //n = NormalDistribution(t;0, 1)
            double n = CalculateN(t);
            //v = n/cdf(t)
            double v = CalculateV(n, t);
            //w = v*(v+t)
            double w = CalculateW(v, t);
         
            //Add tau^2 to sigma^2
            foreach(Player player in players)
            {
                DynamicsFactor(player);
            }
            //MU delta = sigma^2/c * v
            int i = 0;
            foreach(Player player in players)
            {
                //first 2 players won, so Mu goes up. Last 2 players lost, so Mu goes down.
                if (i < 2)
                { player.Mu = player.Mu + MuDelta(player, c, v); }
                else { player.Mu = player.Mu - MuDelta(player, c, v); }
                i++;
            }
            //sigma = sigma * sqrt( 1 - sigma^2/c^2 * w)
            foreach(Player player in players)
            {
                SigmaUpdate(player, c, w);
            }
            //Push updates to DB
            foreach(Player player in players)
            {
                ExecuteQuery("UPDATE [PlayerStats] SET Mu = " + player.Mu + ", Sigma = " + player.Sigma + "WHERE Name = '" + player.Name + "'");
            }

        }

        static double MuDelta(Player player, double c, double v)
        {
            return Math.Pow(player.Sigma, 2) / c * v;
        }

        static void SigmaUpdate(Player player, double c, double w)
        {
            player.Sigma = player.Sigma * Math.Sqrt(1 - Math.Pow(player.Sigma, 2) / Math.Pow(c, 2) * w);
        }

        static double TeamMu(double muOne, double muTwo)
        {
            return muOne + muTwo;
        }

        static double TeamSigma(double sigmaOne, double sigmaTwo)
        {
            return Math.Sqrt(Math.Pow(sigmaOne, 2) + Math.Pow(sigmaTwo, 2));
        }
    }
}
