using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using WordTrainerApp.Models;
using System.Text.Encodings.Web;


namespace WordTrainerApp.Data
{
    public static class DataManager
    {
        private const string DataFile = "words.json";

        public static List<WordEntry> LoadWords()
        {
            if (!File.Exists(DataFile))
                return new List<WordEntry>();

            string json = File.ReadAllText(DataFile);
            return JsonSerializer.Deserialize<List<WordEntry>>(json);
        }

        public static void SaveWords(List<WordEntry> words)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // <- вот это нужно
            };

            string json = JsonSerializer.Serialize(words, options);
            File.WriteAllText(DataFile, json);
        }
    }
}