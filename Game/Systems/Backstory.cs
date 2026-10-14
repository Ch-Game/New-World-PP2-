using System;
using NewWorldPP2.Entities;

namespace NewWorldPP2.Systems
{
    public static class Backstory
    {
        // Generate a short backstory summary for a civilization
        public static string GenerateCivilizationBackstory(Civilization civ)
        {
            // In a real game this would combine templates, recorded history, and events.
            var roll = Core.Dice.Roll();
            var era = roll <= 7 ? "Founding Era" : roll <= 14 ? "Expansion Era" : "Golden Era";
            var summary = $"{civ.Name} — {era}: A people shaped by trade and conflict. Prosperity {civ.Prosperity}.";
            Console.WriteLine(summary);
            return summary;
        }
    }
}
