using System;
using NewWorldPP2.Entities;

namespace NewWorldPP2.Systems
{
    public class MagicSystem
    {
        // Cast a simple spell; power scales with caster level (very simplified)
        public void CastSpell(PlayerCharacter caster, string spellName, int casterLevel)
        {
            var roll = Core.Dice.Roll();
            Console.WriteLine($"{caster.Name} attempts to cast {spellName} (level {casterLevel}). Roll: {roll}");
            var successThreshold = Math.Max(5, 10 - casterLevel);
            if (roll >= successThreshold)
            {
                Console.WriteLine($"Spell {spellName} succeeds with potency {casterLevel + (roll / 5)}.");
                // example: heal
                if (spellName.Contains("Heal", StringComparison.OrdinalIgnoreCase) || spellName.Contains("Light"))
                {
                    caster.Health = Math.Min(caster.Health + 10 + casterLevel, caster.MaxHealth);
                    Console.WriteLine($"{caster.Name} healed to {caster.Health}/{caster.MaxHealth}.");
                }
            }
            else
            {
                Console.WriteLine($"Spell {spellName} fizzles.");
            }
        }
    }
}
