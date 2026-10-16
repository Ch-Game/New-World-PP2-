using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NewWorldPP2.Core
{
    public static class Dice
    {
        private static readonly Random _rng = new();

        // Roll an n-sided dice (default d20)
        public static int Roll(int sides = 20)
        {
            if (sides < 2) return 1;
            return _rng.Next(1, sides + 1);
        }
    }

    // Saves and related helpers (keeps file-per-user JSON with SHA-256 hashing)
    public static class SaveSystem
    {
        private const string SaveFolder = "Saves";

        private sealed class SaveData
        {
            public string Username { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;
            public NewWorldPP2.Entities.PlayerCharacter? Character { get; set; }
        }

        public static bool SaveCharacter(NewWorldPP2.Entities.PlayerCharacter pc, string username, string password)
        {
            if (pc == null) return false;
            if (string.IsNullOrWhiteSpace(pc.Name) || string.IsNullOrWhiteSpace(username)) return false;

            try
            {
                Directory.CreateDirectory(SaveFolder);
                var file = Path.Combine(SaveFolder, Sanitize(username) + ".json");
                if (File.Exists(file)) return false; // do not overwrite

                var data = new SaveData
                {
                    Username = username,
                    PasswordHash = ComputeHash(username, password),
                    Character = pc
                };

                var opts = new JsonSerializerOptions { WriteIndented = true };
                opts.Converters.Add(new JsonStringEnumConverter());
                File.WriteAllText(file, JsonSerializer.Serialize(data, opts), Encoding.UTF8);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SaveExists(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            var file = Path.Combine(SaveFolder, Sanitize(username) + ".json");
            return File.Exists(file);
        }

        public static NewWorldPP2.Entities.PlayerCharacter? LoadCharacter(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            try
            {
                var file = Path.Combine(SaveFolder, Sanitize(username) + ".json");
                if (!File.Exists(file)) return null;

                var opts = new JsonSerializerOptions();
                opts.Converters.Add(new JsonStringEnumConverter());
                var json = File.ReadAllText(file, Encoding.UTF8);
                var data = JsonSerializer.Deserialize<SaveData>(json, opts);
                if (data == null) return null;

                var expected = ComputeHash(data.Username ?? string.Empty, password);
                if (!string.Equals(expected, data.PasswordHash ?? string.Empty, StringComparison.OrdinalIgnoreCase)) return null;

                var character = data.Character;
                if (character == null) return null;

                character.RecalculateDerivedStats();
                return character;
            }
            catch
            {
                return null;
            }
        }

        private static string Sanitize(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return string.Empty;
            var invalid = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(username.Length);
            foreach (var c in username)
            {
                sb.Append(Array.IndexOf(invalid, c) >= 0 ? '_' : c);
            }
            return sb.ToString().ToLowerInvariant();
        }

        private static string ComputeHash(string username, string password)
        {
            using var sha = SHA256.Create();
            var combined = (username ?? string.Empty) + ":" + (password ?? string.Empty);
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
