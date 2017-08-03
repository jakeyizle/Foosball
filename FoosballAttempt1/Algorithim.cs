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
        public static void SkillUpdate(Player winPlayer1, Player winPlayer2, Player losePlayer1, Player losePlayer2, SqlConnection DBConnection)
        {
            double winningMu = TeamMu(winPlayer1.Mu, winPlayer2.Mu);
            double winningSigma = TeamSigma(winPlayer1.Sigma, winPlayer2.Sigma);

            double losingMu = TeamMu(losePlayer1.Mu, losePlayer2.Mu);
            double losingSigma = TeamSigma(losePlayer1.Sigma, losePlayer2.Sigma);

            double c = CalculateC(winningSigma, losingSigma);
            double t = CalculateT(winningMu, losingMu, c);
            double n = CalculateN(t);
            double v = CalculateV(n, t);
            double w = CalculateW(v, t);

            Player[] players = new Player[] { winPlayer1, winPlayer2, losePlayer1, losePlayer2 };
            
            //Add dynamics factor to each players sigma
            foreach(Player player in players)
            {
                DynamicsFactor(player);
            }
            //Mu updates for each player
            int i = 0;
            foreach(Player player in players)
            {
                //TODO: better solution for this lol
                if (i < 2)
                { player.Mu = player.Mu + MuDelta(player, c, v); }
                else { player.Mu = player.Mu - MuDelta(player, c, v); }
                i++;
            }
            //Sigma updates for each player
            foreach(Player player in players)
            {
                SigmaUpdate(player, c, w);
            }
            //Push updates to DB
            foreach(Player player in players)
            {
                UpdatePlayer(player, DBConnection);
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
