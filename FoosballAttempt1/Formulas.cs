using System;
using static FoosballAttempt1.Program;

namespace FoosballAttempt1
{
    public class Formulas
    {
        public static void DynamicsFactor(Player player)
        {
            player.Sigma = Math.Sqrt(Math.Pow(player.Sigma, 2) + Math.Pow(Program.tau, 2));
        }

        public static double CalculateC(double winningSigma, double losingSigma)
        {
            return Math.Sqrt(2 * Math.Pow(beta, 2) + Math.Pow(winningSigma, 2) + Math.Pow(losingSigma, 2));
        }

        public static double CalculateT(double winningMu, double losingMu, double c)
        {
            return (winningMu - losingMu) / c;
        }

        public static double CalculateN(double t) //This equation uses t-a, but a is always 0
        {
            return Math.Exp(-Math.Pow(t - 0, 2) / (2)) / Math.Sqrt(2 * Math.PI);
        }

        public static double CalculateV(double n, double t)
        {
            return n / CumDensity(t - 0);
        }

        public static double CalculateW(double v, double t)
        {
            return v * (v + t);
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
