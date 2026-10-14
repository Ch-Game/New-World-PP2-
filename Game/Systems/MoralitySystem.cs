using System;
using NewWorldPP2.Entities;

namespace NewWorldPP2.Systems
{
    public class MoralitySystem
    {
        // Adjust morality for a character (positive = good, negative = bad)
        public void AdjustMorality(PlayerCharacter character, int delta, string reason = null)
        {
            character.Morality += delta;
            Console.WriteLine($"{character.Name} morality adjusted by {delta}. Reason: {reason}. New morality: {character.Morality}");
            // Example: if very negative, tougher world reactions
            if (character.Morality < -20)
            {
                Console.WriteLine("The world begins to distrust and hinder the character due to low morality.");
            }
        }
    }
}
