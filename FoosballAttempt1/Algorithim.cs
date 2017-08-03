using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FoosballAttempt1.Formulas;
using static FoosballAttempt1.DbConnection;
using System.Data.SqlClient;

namespace FoosballAttempt1
{
    class Algorithim
    {
        public static void SkillUpdate(Player winPlayer1, Player winPlayer2, Player losePlayer1, Player losePlayer2)
        {
            //Create Team Mus and Sigmas used to calculate intermediate variables
            double winningMu = TeamMu(winPlayer1.Mu, winPlayer2.Mu);
            double winningSigma = TeamSigma(winPlayer1.Sigma, winPlayer2.Sigma);

            double losingMu = TeamMu(losePlayer1.Mu, losePlayer2.Mu);
            double losingSigma = TeamSigma(losePlayer1.Sigma, losePlayer2.Sigma);

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

            Player[] players = new Player[] { winPlayer1, winPlayer2, losePlayer1, losePlayer2 };
            
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
                UpdatePlayer(player);
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
