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

            //Add dynamics factor to each players sigma
            DynamicsFactor(winPlayer1);
            DynamicsFactor(winPlayer2);
            DynamicsFactor(losePlayer1);
            DynamicsFactor(losePlayer2);

            //Mu updates for each player
            winPlayer1.Mu = winPlayer1.Mu + MuDelta(winPlayer1, c, v);
            winPlayer2.Mu = winPlayer2.Mu + MuDelta(winPlayer2, c, v); ;
            losePlayer1.Mu = losePlayer1.Mu - MuDelta(losePlayer1, c, v);
            losePlayer2.Mu = losePlayer2.Mu - MuDelta(losePlayer2, c, v);

            //Sigma updates for each player
            SigmaUpdate(winPlayer1, c, w);
            SigmaUpdate(winPlayer2, c, w);
            SigmaUpdate(losePlayer1, c, w);
            SigmaUpdate(losePlayer2, c, w);

            //Push updates to DB
            UpdatePlayer(winPlayer1, DBConnection);
            UpdatePlayer(winPlayer2, DBConnection);
            UpdatePlayer(losePlayer1, DBConnection);
            UpdatePlayer(losePlayer2, DBConnection);
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
