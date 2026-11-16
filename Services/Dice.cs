using System;

namespace NewWorldPP2.Services
{
    public readonly record struct DiceCheckResult(int Roll, int Modifier)
    {
        public int Total => Roll + Modifier;
        public bool IsCriticalSuccess => Roll == 20;
        public bool IsCriticalFailure => Roll == 1;
    }

    public static class Dice
    {
        private static readonly Random _rng = new();

        // Roll an n-sided dice (default d20)
        public static int Roll(int sides = 20)
        {
            if (sides < 2) return 1;
            return _rng.Next(1, sides + 1);
        }

        public static DiceCheckResult RollD20(int modifier = 0)
        {
            return new DiceCheckResult(Roll(20), modifier);
        }
    }

}

