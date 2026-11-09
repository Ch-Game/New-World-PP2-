using System.Collections.Generic;

namespace NewWorldPP2.Entities
{
    // Represents equipment or consumables that can be stored in the inventory.
    public class Item
    {
        public string Name { get; set; } = string.Empty;
        public ItemRarity Rarity { get; set; } = ItemRarity.Common;
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Magic { get; set; }
        public int Speed { get; set; }
        public int Durability { get; set; }
        public WeaponCategory? WeaponCategory { get; set; }
        public Stats RequiredStats { get; set; } = new Stats();
        // Only mythical items will have abilities populated by factory
        public List<Ability>? Abilities { get; set; }

        // Formats the item's main statistics for display in the console UI.
        public override string ToString()
        {
            var ab = Abilities == null ? "" : $" Abilities: {Abilities.Count}";
            var requirements = WeaponCategory == null ? "" : $" Requires stats: {RequiredStats}";
            return $"{Name} ({Rarity}) A:{Attack} D:{Defense} M:{Magic} S:{Speed} Dur:{Durability}{requirements}{ab}";
        }
    }
}
