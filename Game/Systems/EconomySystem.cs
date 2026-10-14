using System;

namespace NewWorldPP2.Systems
{
    public class EconomySystem
    {
        // Very small economy model that can react to player transactions
        public int GlobalWealthIndex { get; private set; } = 50; // 0..100

        public void ProcessTransaction(int amount, bool playerIsBuyer)
        {
            Console.WriteLine($"Processing transaction of {amount} (playerIsBuyer={playerIsBuyer})");
            if (playerIsBuyer)
            {
                GlobalWealthIndex = Math.Max(0, GlobalWealthIndex - (amount / 10));
            }
            else
            {
                GlobalWealthIndex = Math.Min(100, GlobalWealthIndex + (amount / 10));
            }
            Console.WriteLine($"Global wealth index now {GlobalWealthIndex}");
        }
    }
}
