using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using NewWorldPP2.Entities;

namespace NewWorldPP2.Core
{
    public static class SaveSystem
    {
        private const string SaveFolder = "Saves";

        private class SaveData
        {
            public string Username { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;
            public PlayerCharacter? Character { get; set; }
        }

        public static bool SaveCharacter(PlayerCharacter pc, string username, string password)
        {
            try
            {
                if (pc == null) return false;
                // Do not allow saving a character without a name
                if (string.IsNullOrWhiteSpace(pc.Name)) return false;
                if (string.IsNullOrWhiteSpace(username)) return false;
                Directory.CreateDirectory(SaveFolder);
                var file = Path.Combine(SaveFolder, Sanitize(username) + ".json");

                // Do not allow duplicate save names
                if (File.Exists(file)) return false;

                var data = new SaveData
                {
                    Username = username,
                    PasswordHash = ComputeHash(username, password),
                    Character = pc
                };

                var opts = new JsonSerializerOptions { WriteIndented = true };
                opts.Converters.Add(new JsonStringEnumConverter());

                var json = JsonSerializer.Serialize(data, opts);
                File.WriteAllText(file, json, Encoding.UTF8);
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

        private static string Sanitize(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return string.Empty;
            var invalid = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(username.Length);
            foreach (var c in username)
            {
                if (Array.IndexOf(invalid, c) >= 0) sb.Append('_'); else sb.Append(c);
            }
            return sb.ToString().ToLowerInvariant();
        }

        public static PlayerCharacter? LoadCharacter(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username)) return null;
                var file = Path.Combine(SaveFolder, username.ToLowerInvariant() + ".json");
                if (!File.Exists(file)) return null;

                var json = File.ReadAllText(file, Encoding.UTF8);
                var opts = new JsonSerializerOptions();
                opts.Converters.Add(new JsonStringEnumConverter());
                var data = JsonSerializer.Deserialize<SaveData>(json, opts);
                if (data == null) return null;

                // Validate credentials, handling potential nulls from deserialization
                var expected = ComputeHash(data.Username ?? string.Empty, password);
                if (!string.Equals(expected, data.PasswordHash ?? string.Empty, StringComparison.OrdinalIgnoreCase)) return null;

                // Ensure derived stats are recomputed after loading and avoid returning a possible null
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

        private static string ComputeHash(string username, string password)
        {
            using var sha = SHA256.Create();
            var combined = (username ?? string.Empty) + ":" + (password ?? string.Empty);
            var bytes = Encoding.UTF8.GetBytes(combined);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
