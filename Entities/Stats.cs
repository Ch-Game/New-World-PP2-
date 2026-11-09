

namespace NewWorldPP2.Entities
{
    // Contains the six primary attributes used by character systems.
    public class Stats
    {
        private int _strength;
        private int _dexterity;
        private int _constitution;
        private int _intelligence;
        private int _wisdom;
        private int _charisma;
        private int _luck;
        private int _perception;
        private int _speed;

        public int Strength { get => _strength; set => _strength = Clamp(value); }
        public int Dexterity { get => _dexterity; set => _dexterity = Clamp(value); }
        public int Constitution { get => _constitution; set => _constitution = Clamp(value); }
        public int Intelligence { get => _intelligence; set => _intelligence = Clamp(value); }
        public int Wisdom { get => _wisdom; set => _wisdom = Clamp(value); }
        public int Charisma { get => _charisma; set => _charisma = Clamp(value); }
        public int Luck { get => _luck; set => _luck = Clamp(value); }
        public int Perception { get => _perception; set => _perception = Clamp(value); }
        public int Speed { get => _speed; set => _speed = Clamp(value); }

        public Stats(int strength = 1, int dexterity = 1, int constitution = 1, int intelligence = 1, int wisdom = 1, int charisma = 1, int luck = 1, int perception = 1, int speed = 1)
        {
            Strength = strength;
            Dexterity = dexterity;
            Constitution = constitution;
            Intelligence = intelligence;
            Wisdom = wisdom;
            Charisma = charisma;
            Luck = luck;
            Perception = perception;
            Speed = speed;
        }

        private static int Clamp(int value) => value < 0 ? 0 : value;

        // Returns the combined value of all primary attributes.
        public int Total() => Strength + Dexterity + Constitution + Intelligence + Wisdom + Charisma;

        public override string ToString()
        {
            return $"STR: {Strength}  DEX: {Dexterity}  CON: {Constitution}  INT: {Intelligence}  WIS: {Wisdom}  CHA: {Charisma}  LCK: {Luck}  PER: {Perception}  SPD: {Speed}";
        }
    }
}
