using System.Windows;
using WordTrainerApp.Models;
using WordTrainerApp.ViewModels;

namespace WordTrainerApp.Views
{
    public partial class AddWordDialog : Window
    {
        private MainViewModel viewModel;

        public AddWordDialog(MainViewModel vm)
        {
            InitializeComponent();
            viewModel = vm;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var newWord = new WordEntry
            {
                ForeignWord = foreignInput.Text,
                Translation = translationInput.Text,
                Category = categoryInput.Text
            };

            viewModel.AddWord(newWord);
            this.Close();
        }
    }
}
