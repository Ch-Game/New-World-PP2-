using System;
using NewWorldPP2.Entities;

namespace NewWorldPP2.Entities
{
    public class Creature
    {
        public string Name { get; set; }
        public int Health { get; set; }

        public Creature(string name, int health)
        {
            Name = name;
            Health = health;
        }

        // Simple interaction hook. Real game would be more complex.
        public void InteractWith(Civilization civ)
        {
            Console.WriteLine($"{Name} encounters the civilization '{civ.Name}'.");
            // Very small example: if civ is friendly, creature may flee or join.
            if (civ.IsFriendly)
            {
                Console.WriteLine($"{Name} is pacified by friendly civilians.");
                civ.HandleCreatureVisit(this);
            }
            else
            {
                Console.WriteLine($"{Name} shows hostility toward {civ.Name}.");
                // trigger a simple battle result based on dice
                var roll = Core.Dice.Roll();
                Console.WriteLine($"Battle roll: {roll}");
                if (roll >= 12)
                {
                    Console.WriteLine($"{Name} survives the skirmish.");
                }
                else
                {
                    Console.WriteLine($"{Name} is driven off or defeated.");
                    Health = 0;
                }
            }
        }
    }
}
