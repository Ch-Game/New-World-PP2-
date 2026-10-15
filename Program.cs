using System;
using NewWorldPP2.Core;
using NewWorldPP2.Entities;

namespace NewWorldPP2
{
    class Program
    {
        static void Main(string[] args)
        {
            PlayerCharacter? player = null;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Character Menu ===");
                Console.WriteLine("1) Create New Character");
                Console.WriteLine("2) Customize Character");
                Console.WriteLine("3) View Character");
                Console.WriteLine("4) Save Character");
                Console.WriteLine("5) Load Character");
                Console.WriteLine("6) Exit");
                Console.Write("Choose an option: ");
                var key = Console.ReadLine() ?? string.Empty;

                switch (key)
                {
                    case "1":
                        player = CharacterCreation.CreateCharacter();
                        break;
                    case "2":
                        if (player == null)
                        {
                            Console.WriteLine("No character yet. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            CharacterCreation.CustomizeCharacter(player);
                        }
                        break;
                    case "3":
                        if (player == null)
                        {
                            Console.WriteLine("No character yet. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            Console.WriteLine(player.GetFullDescription());
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        break;
                    case "4":
                        if (player == null)
                        {
                            Console.WriteLine("No character to save. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            Console.Write("Enter save username: ");
                            var user = Console.ReadLine()?.Trim() ?? string.Empty;
                            Console.Write("Enter password: ");
                            var pass = ReadPassword();
                            if (SaveSystem.SaveExists(user))
                            {
                                Console.WriteLine("A save with that username already exists. Choose a different username.");
                            }
                            else
                            {
                                var ok = SaveSystem.SaveCharacter(player, user, pass);
                                Console.WriteLine(ok ? "Saved successfully." : "Save failed.");
                            }
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        break;
                    case "5":
                        Console.Write("Enter save username: ");
                        var loadUser = Console.ReadLine()?.Trim() ?? string.Empty;
                        Console.Write("Enter password: ");
                        var loadPass = ReadPassword();
                        var loaded = SaveSystem.LoadCharacter(loadUser, loadPass);
                        if (loaded == null)
                        {
                            Console.WriteLine("Failed to load save (incorrect user/password or missing save). Press Enter...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            player = loaded;
                            Console.WriteLine("Save loaded. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        break;
                    case "6":
                        return;
                        default:
                            Console.WriteLine("Invalid option. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                }
            }
        }

        // Minimal password masking for console input
        private static string ReadPassword()
        {
            var pass = string.Empty;
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass[..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return pass;
        }
    }
}
