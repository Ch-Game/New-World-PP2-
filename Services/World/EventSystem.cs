using System;
using NewWorldPP2.Entities;
using NewWorldPP2.Services;

namespace NewWorldPP2.Services.World
{
    // Resolves random events and applies their outcomes to the player.
    public class EventSystem
    {
        // Trigger an event that is influenced by a d20 roll. High rolls are better for player.
        public void TriggerEvent(string eventName, PlayerCharacter player)
        {
            var roll = Math.Min(20, Dice.Roll() + StatCalculator.SocialBonus(player.Stats) + StatCalculator.RewardBonus(player.Stats));
            Console.WriteLine($"Event '{eventName}' triggered. d20 roll: {roll}");
            if (roll == 20)
            {
                Console.WriteLine("Critical success! The event turns in the player's favor spectacularly.");
                // grant a small permanent boon as an example
                player.MaxHealth += 5 + StatCalculator.HealingBonus(player.Stats);
            }
            else if (roll >= 12)
            {
                Console.WriteLine("Success: The player handles the event well.");
            }
            else if (roll > 1)
            {
                Console.WriteLine("Partial failure: consequences apply.");
                player.Morality -= 1;
            }
            else
            {
                Console.WriteLine("Critical failure: disastrous outcome.");
                player.Health = Math.Max(1, player.Health / 2);
                player.Morality -= 5;
            }
        }
    }
}

