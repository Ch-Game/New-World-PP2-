using System;

namespace NewWorldPP2.Entities
{
    public class Civilization
    {
        public string Name { get; }
        public int Prosperity { get; set; } = 50; // 0-100
        public bool IsFriendly { get; set; } = true;

        public Civilization(string name)
        {
            Name = name;
        }

        public void HandleCreatureVisit(Creature creature)
        {
            Console.WriteLine($"{Name} handles visit from {creature.Name}.");
            // Example: friendly civs may feed or recruit creatures
            var roll = Core.Dice.Roll();
            if (roll >= 15)
            {
                Console.WriteLine($"{creature.Name} integrates peacefully into {Name}.");
                Prosperity += 2;
            }
            else
            {
                Console.WriteLine($"{Name} cannot accommodate the {creature.Name}.");
                Prosperity -= 1;
            }
        }
    }
}
