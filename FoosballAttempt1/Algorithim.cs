using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FoosballAttempt1.DbConnection;
using static FoosballAttempt1.Player;
using static FoosballAttempt1.Program;
using System.Data.SqlClient;

namespace FoosballAttempt1
{
    class Algorithim
    {
        //The Skill/Teamskill updates perform calculations to update mu and sigma, then push those updates to the player/team stats table
        public static Player[] TeamSkillUpdate(Player[] players)
        {
            //returns 2 player array
            Player[] teams = MakeTeam(players);
            foreach (Player player in teams)
            {
                DynamicsFactor(player);
            }

            CalculateVandW(teams[0].Sigma, teams[1].Sigma, teams[0].Mu, teams[1].Mu, out double v, out double w, out double c);

            //Purposefully don't update Score here, so that we can report on previous and updated scores
            int i = 0;
            foreach (Player player in teams)
            {
                if (i < 1)
                { player.Mu = player.Mu + MuDelta(player, c, v); }
                else { player.Mu = player.Mu - MuDelta(player, c, v); }
                i++;
            }
            foreach (Player player in teams)
            {
                SigmaUpdate(player, c, w);
            }
            foreach (Player player in teams)
            {
                ExecuteQuery("UPDATE [TeamStats] SET Mu = " + player.Mu + ", Sigma = " + player.Sigma + "WHERE Name = '" + player.Name + "'");
            }
            return teams;
        }

        public static void SkillUpdate(Player[] players)
        {

            foreach (Player player in players)
            {
                DynamicsFactor(player);
            }
            //Create Team Mus and Sigmas used to calculate intermediate variables
            //Team Mu = Mu+Mu
            //Team Sigma = sqrt(sigma^2+sigma^2)
            double winningMu = TeamMu(players[0].Mu, players[1].Mu);
            double winningSigma = TeamSigma(players[0].Sigma, players[1].Sigma);

            double losingMu = TeamMu(players[2].Mu, players[3].Mu);
            double losingSigma = TeamSigma(players[2].Sigma, players[3].Sigma);

            CalculateVandW(winningSigma, losingSigma, winningMu, losingMu, out double v, out double w, out double c);

            int i = 0;
            foreach(Player player in players)
            {
                //first 2 players won, so Mu goes up. Last 2 players lost, so Mu goes down.
                if (i < 2)
                { player.Mu = player.Mu + MuDelta(player, c, v); }
                else { player.Mu = player.Mu - MuDelta(player, c, v); }
                i++;
            }

            foreach(Player player in players)
            {
                SigmaUpdate(player, c, w);
            }

            foreach(Player player in players)
            {
                ExecuteQuery("UPDATE [PlayerStats] SET Mu = " + player.Mu + ", Sigma = " + player.Sigma + "WHERE Name = '" + player.Name + "'");
            }

        }

        public static double MatchQuality(Player player1, Player player2, Player player3, Player player4)
        {
            double sigmasum = Math.Pow(player1.Sigma, 2) + Math.Pow(player2.Sigma, 2) + Math.Pow(player3.Sigma, 2) + Math.Pow(player4.Sigma, 2);
            //Right side of equation is constant for same group of players, no matter the team compositions
            double rightside = Math.Sqrt((4 * Math.Pow(BETA, 2)) / (4 * Math.Pow(BETA, 2) + sigmasum));

            double leftside = Math.Exp(-0.5 * (player1.Mu + player2.Mu - player3.Mu - player4.Mu) * 1 / (4 * Math.Pow(BETA, 2) + sigmasum) * (player1.Mu + player2.Mu - player3.Mu - player4.Mu));

            return leftside * rightside;
        }

        //These functions are only used by Skill/Teamskill update
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

        static void CalculateVandW(double winningSigma, double losingSigma, double winningMu, double losingMu, out double v, out double w, out double c)
        {
            c = Math.Sqrt(2 * Math.Pow(BETA, 2) + Math.Pow(winningSigma, 2) + Math.Pow(losingSigma, 2));
            double t = (winningMu - losingMu) / c;
            double n = Math.Exp(-Math.Pow(t, 2) / (2)) / Math.Sqrt(2 * Math.PI);
            v = n / CumDensity(t - 0);
            w = v * (v + t);
        }

        static void DynamicsFactor(Player player)
        {
            player.Sigma = Math.Sqrt(Math.Pow(player.Sigma, 2) + Math.Pow(TAU, 2));
        }

        static double CumDensity(double z)
        {
            double p = 0.3275911;
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;

            int sign;
            if (z < 0.0)
                sign = -1;
            else
                sign = 1;

            double x = Math.Abs(z) / Math.Sqrt(2.0);
            double t = 1.0 / (1.0 + p * x);
            double erf = 1.0 - (((((a5 * t + a4) * t) + a3)
              * t + a2) * t + a1) * t * Math.Exp(-x * x);
            return 0.5 * (1.0 + sign * erf);
        }
    }
}
