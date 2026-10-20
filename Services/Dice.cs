using System;

namespace NewWorldPP2.Services
{
    public static class Dice
    {
        private static readonly Random _rng = new();

        // Roll an n-sided dice (default d20)
        public static int Roll(int sides = 20)
        {
            if (sides < 2) return 1;
            return _rng.Next(1, sides + 1);
        }
    }

}

