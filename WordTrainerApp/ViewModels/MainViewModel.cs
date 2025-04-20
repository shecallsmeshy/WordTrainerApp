using System;
using System.Collections.Generic;
using System.Linq;
using WordTrainerApp.Models;
using WordTrainerApp.Data;

namespace WordTrainerApp.ViewModels
{
    public class MainViewModel
    {
        private List<WordEntry> words;
        public WordEntry CurrentWord { get; set; }
        public string SelectedCategory { get; set; }

        public int CorrectCount { get; private set; }
        public int WrongCount { get; private set; }

        private Random random = new Random();

        public MainViewModel()
        {
            words = DataManager.LoadWords(); // Загрузка слов из файла при инициализации
            if (words.Count > 0)
            {
                SelectedCategory = words[0].Category;
                GetNextWord();
            }
        }

        // Возвращает все слова
        public List<WordEntry> GetAllWords()
        {
            return words;
        }

        // Загружает слова в модель
        public void LoadWords(List<WordEntry> loadedWords)
        {
            words = loadedWords;
            if (words.Count > 0)
            {
                SelectedCategory = words[0].Category; // Устанавливаем категорию по умолчанию, если слова загружены
                GetNextWord();
            }
        }

        // Получаем все доступные категории
        public List<string> GetAllCategories()
        {
            return words.Select(w => w.Category).Distinct().ToList();
        }

        // Получаем следующее слово в выбранной категории
        public WordEntry GetNextWord()
        {
            var categoryWords = words.Where(w => w.Category == SelectedCategory).ToList();

            if (categoryWords.Count == 0)
            {
                CurrentWord = null;
                return null;
            }

            CurrentWord = categoryWords[random.Next(categoryWords.Count)];
            return CurrentWord;
        }

        // Получаем перемешанные варианты перевода для текущего слова
        public List<string> GetShuffledOptions(WordEntry correctWord)
        {
            var options = new HashSet<string> { correctWord.Translation };

            var wrongOptions = words
                .Where(w => w.Category == SelectedCategory && w.Translation != correctWord.Translation)
                .Select(w => w.Translation)
                .Distinct()
                .OrderBy(x => random.Next())
                .Take(3);

            foreach (var opt in wrongOptions)
                options.Add(opt);

            return options.OrderBy(x => random.Next()).ToList();
        }

        // Проверка ответа
        public bool CheckAnswer(string answer)
        {
            bool isCorrect = answer == CurrentWord?.Translation;
            if (isCorrect) CorrectCount++;
            else WrongCount++;

            return isCorrect;
        }

        // Добавление нового слова
        public void AddWord(WordEntry word)
        {
            words.Add(word);
            DataManager.SaveWords(words);
        }
    }
}
