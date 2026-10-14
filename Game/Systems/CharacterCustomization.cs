using System;

namespace NewWorldPP2.Systems
{
    public static class CharacterCustomization
    {
        public static PlayerCharacter CreateCharacter(string name, string race, string @class)
        {
            var pc = new PlayerCharacter
            {
                Name = name,
                Race = race,
                Class = @class,
                Level = 1,
                Morality = 0,
                MaxHealth = 100,
                Health = 100
            };
            Console.WriteLine($"Character created: {pc.Name} the {pc.Race} {pc.Class}.");
            return pc;
        }
    }

    public class PlayerCharacter
    {
        public string Name { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
        public int Level { get; set; }
        public int Morality { get; set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }
    }
}
