using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WordTrainerApp.Models;
using WordTrainerApp.ViewModels;
using WordTrainerApp.Views;
using Microsoft.Win32;
using System.IO;
using Newtonsoft.Json;

namespace WordTrainerApp.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            this.DataContext = viewModel;

            categoryComboBox.ItemsSource = viewModel.GetAllCategories();
            if (categoryComboBox.Items.Count > 0)
                categoryComboBox.SelectedIndex = 0;
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categoryComboBox.SelectedItem is string category)
            {
                viewModel.SelectedCategory = category;
                ShowNextWord();
            }
        }

        private void ShowNextWord()
        {
            optionsGrid.Children.Clear();
            resultTextBlock.Text = "";

            var currentWord = viewModel.GetNextWord();
            if (currentWord == null)
            {
                wordTextBlock.Text = "Нет слов в категории.";
                return;
            }

            wordTextBlock.Text = currentWord.ForeignWord;

            var options = viewModel.GetShuffledOptions(currentWord);
            foreach (var option in options)
            {
                var btn = new Button
                {
                    Content = option,
                    Margin = new Thickness(5),
                    Tag = option,
                    Padding = new Thickness(10)
                };
                btn.Click += Option_Click;
                optionsGrid.Children.Add(btn);
            }

            UpdateStats();
        }

        private void Option_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string selected = btn.Tag.ToString();
                bool isCorrect = viewModel.CheckAnswer(selected);

                resultTextBlock.Text = isCorrect ? "Верно!" : $"Неверно. Правильно: {viewModel.CurrentWord.Translation}";
                resultTextBlock.Foreground = isCorrect ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;

                foreach (Button b in optionsGrid.Children)
                    b.IsEnabled = false;

                UpdateStats();
            }
        }

        private void AddWord_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddWordDialog(viewModel);
            dialog.Owner = this;
            dialog.ShowDialog();

            categoryComboBox.ItemsSource = null;
            categoryComboBox.ItemsSource = viewModel.GetAllCategories();
        }

        private void NextWord_Click(object sender, RoutedEventArgs e)
        {
            ShowNextWord();
        }

        private void UpdateStats()
        {
            correctCountText.Text = viewModel.CorrectCount.ToString();
            wrongCountText.Text = viewModel.WrongCount.ToString();
        }

        // Новый метод для сохранения в файл words.json
        private void SaveWords_Click(object sender, RoutedEventArgs e)
        {
            var words = viewModel.GetAllWords(); // Получение всех слов
            string json = JsonConvert.SerializeObject(words, Formatting.Indented);
            File.WriteAllText("words.json", json);
            MessageBox.Show("Словарь сохранён в words.json.");
        }

        // Новый метод для загрузки из файла words.json
        private void LoadWords_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("words.json"))
            {
                string json = File.ReadAllText("words.json");
                var words = JsonConvert.DeserializeObject<List<WordEntry>>(json);
                viewModel.LoadWords(words); // Загрузка слов в модель
                MessageBox.Show("Словарь загружен из words.json.");
            }
            else
            {
                MessageBox.Show("Файл words.json не найден.");
            }
        }

        // Новый метод для сохранения в произвольный файл
        private void SaveWordsToCustomFile_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*",
                DefaultExt = "json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var words = viewModel.GetAllWords();
                string json = JsonConvert.SerializeObject(words, Formatting.Indented);
                File.WriteAllText(saveFileDialog.FileName, json);
                MessageBox.Show($"Словарь сохранён в {saveFileDialog.FileName}.");
            }
        }

        // Новый метод для загрузки из произвольного файла
        private void LoadWordsFromCustomFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*",
                DefaultExt = "json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                var words = JsonConvert.DeserializeObject<List<WordEntry>>(json);
                viewModel.LoadWords(words); // Загрузка слов в модель
                MessageBox.Show($"Словарь загружен из {openFileDialog.FileName}.");
            }
        }
    }
}
