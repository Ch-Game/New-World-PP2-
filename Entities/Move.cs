using System;

namespace NewWorldPP2.Entities
{
    public enum MoveSourceType
    {
        Level,
        Skill,
        Weapon,
        Armor,
        Special
    }

    // A combat move that can be learned and placed in one of four combat slots.
    public class Move
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public MoveSourceType SourceType { get; set; }
        public string Source { get; set; } = string.Empty;
        public int RequiredLevel { get; set; } = 1;
        public int SpecialPointCost { get; set; }
        public WeaponCategory? RequiredWeaponCategory { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; } = 85;
        public int Healing { get; set; }
        public int DefenseBoost { get; set; }
        public bool IgnoresDefense { get; set; }

        public Move Clone() => (Move)MemberwiseClone();
    }
}
