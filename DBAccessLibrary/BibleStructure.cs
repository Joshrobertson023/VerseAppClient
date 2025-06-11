using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace DBAccessLibrary
{
    public static class BibleStructure
    {
        // Helper to pull and parse the embedded JSON
        private static Dictionary<string, Dictionary<string, int>> LoadStructure()
        {
            var asm = Assembly.GetExecutingAssembly();
            string? resourceName = null;

            // find the resource that ends with our JSON filename
            foreach (var name in asm.GetManifestResourceNames())
            {
                if (name.EndsWith("bible_structure.json", StringComparison.OrdinalIgnoreCase))
                {
                    resourceName = name;
                    break;
                }
            }

            if (resourceName == null)
                throw new FileNotFoundException("Embedded resource 'bible_structure.json' not found.");

            using var stream = asm.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);
            string json = reader.ReadToEnd();
            return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(json)
                   ?? throw new JsonException("Failed to deserialize bible_structure.json");
        }

        public static int GetNumberChapters(string book)
        {
            int numChapters = 0;
            var map = LoadStructure();

            if (map.ContainsKey(book))
            {
                numChapters = map[book].Count;
            }

            return numChapters;
        }

        public static int GetNumberVerses(string book, int chapter)
        {
            int numVerses = 0;
            var map = LoadStructure();

            if (map.ContainsKey(book))
            {
                string key = chapter.ToString();
                if (map[book].ContainsKey(key))
                {
                    numVerses = map[book][key];
                }
            }

            return numVerses;
        }
    }
}
