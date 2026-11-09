using System;
using NewWorldPP2.Entities;

namespace NewWorldPP2.Services
{
    // Provides consistent gameplay modifiers derived from every character attribute.
    public static class StatCalculator
    {
        public static int AttackBonus(Stats stats) => Modifier(stats.Strength);
        public static int DefenseBonus(Stats stats) => Modifier(stats.Constitution) + Modifier(stats.Wisdom);
        public static int MagicBonus(Stats stats) => Modifier(stats.Intelligence) + Modifier(stats.Wisdom);
        public static int InitiativeBonus(Stats stats) => Modifier(stats.Speed) + Modifier(stats.Dexterity);
        public static int CriticalChance(Stats stats) => Math.Clamp(5 + stats.Luck * 2, 5, 35);
        public static int AccuracyBonus(Stats stats) => Modifier(stats.Dexterity) + Modifier(stats.Perception);
        public static int HealingBonus(Stats stats) => Modifier(stats.Wisdom) + Modifier(stats.Constitution);
        public static int FleeChance(Stats stats) => Math.Clamp(35 + stats.Dexterity * 5 + stats.Speed * 3, 10, 90);
        public static int TrapReduction(Stats stats) => Modifier(stats.Dexterity) + Modifier(stats.Perception);
        public static int DiscoveryChance(Stats stats) => Math.Clamp(10 + stats.Luck * 4 + stats.Perception * 5, 10, 90);
        public static int TravelEncounterChance(Stats stats) => Math.Clamp(50 - stats.Perception * 4 - stats.Speed * 2, 10, 50);
        public static int ShopDiscountPercent(Stats stats) => Math.Clamp(stats.Charisma * 3 + stats.Luck, 0, 40);
        public static int RewardBonus(Stats stats) => Math.Max(0, Modifier(stats.Charisma) + Modifier(stats.Luck));
        public static int SocialBonus(Stats stats) => Modifier(stats.Charisma) + Modifier(stats.Wisdom);

        public static int Modifier(int value) => (value - 1) / 2;

        public static bool RollCritical(Stats stats)
        {
            return Dice.Roll(100) <= CriticalChance(stats);
        }
    }
}
