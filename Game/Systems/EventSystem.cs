using System;
using NewWorldPP2.Systems;
using NewWorldPP2.Entities;

namespace NewWorldPP2.Systems
{
    public class EventSystem
    {
        // Trigger an event that is influenced by a d20 roll. High rolls are better for player.
        public void TriggerEvent(string eventName, PlayerCharacter player)
        {
            var roll = Core.Dice.Roll();
            Console.WriteLine($"Event '{eventName}' triggered. d20 roll: {roll}");
            if (roll == 20)
            {
                Console.WriteLine("Critical success! The event turns in the player's favor spectacularly.");
                // grant a small permanent boon as an example
                player.MaxHealth += 5;
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
