

namespace NewWorldPP2.Entities
{
    public class Stats
    {
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        public int Total() => Strength + Dexterity + Constitution + Intelligence + Wisdom + Charisma;

        public override string ToString()
        {
            return $"STR: {Strength}  DEX: {Dexterity}  CON: {Constitution}  INT: {Intelligence}  WIS: {Wisdom}  CHA: {Charisma}";
        }
    }
}
