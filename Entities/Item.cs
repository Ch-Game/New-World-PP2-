using System.Collections.Generic;

namespace NewWorldPP2.Entities
{
    public class Item
    {
        public string Name { get; set; } = string.Empty;
        public ItemRarity Rarity { get; set; } = ItemRarity.Common;
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Magic { get; set; }
        public int Speed { get; set; }
        public int Durability { get; set; }
        // Only mythical items will have abilities populated by factory
        public List<Ability>? Abilities { get; set; }

        public override string ToString()
        {
            var ab = Abilities == null ? "" : $" Abilities: {Abilities.Count}";
            return $"{Name} ({Rarity}) A:{Attack} D:{Defense} M:{Magic} S:{Speed} Dur:{Durability}{ab}";
        }
    }
}
