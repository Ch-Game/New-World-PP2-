namespace NewWorldPP2.Entities
{
    public enum Race
    {
        Human = 1,
        Elf,
        Dwarf,
        Orc
    }

    public class Stats
    {
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        public int Total()
        {
            return Strength + Dexterity + Constitution + Intelligence + Wisdom + Charisma;
        }

        public override string ToString()
        {
            return $"STR: {Strength}  DEX: {Dexterity}  CON: {Constitution}  INT: {Intelligence}  WIS: {Wisdom}  CHA: {Charisma}";
        }
    }

    public class PlayerCharacter
    {
        public string Name { get; set; } = string.Empty;
        public Race Race { get; set; }
        // BaseStats represent the allocated points before racial modifiers.
        public Stats BaseStats { get; set; } = new Stats();
        // Stats reflect BaseStats plus race modifiers.
        public Stats Stats { get; set; } = new Stats();
        public string Backstory { get; set; } = string.Empty;
        public int MaxHealth { get; set; } = 10;
        public int Health { get; set; } = 10;
        public int Morality { get; set; } = 0;

        public void RecalculateDerivedStats()
        {
            // Simple derived stat: MaxHealth scales with Constitution
            MaxHealth = 10 + (BaseStats?.Constitution ?? Stats.Constitution) * 2;
            if (Health > MaxHealth) Health = MaxHealth;
        }

        public string GetShortDescription()
        {
            return $"{Name} the {Race} - {Stats} (HP {Health}/{MaxHealth})";
        }

        public string GetFullDescription()
        {
            return $"Name: {Name}\nRace: {Race}\nStats: {Stats}\nHP: {Health}/{MaxHealth}\nMorality: {Morality}\nBackstory:\n{Backstory}";
        }
    }
}
