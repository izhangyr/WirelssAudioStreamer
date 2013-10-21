




using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.ViewModel;
using Strings = Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.Resources.Localization;

namespace Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.View
{
    /// <summary>
    /// Interaction logic for InputFolderWindow.xaml
    /// </summary>
    public partial class InputFolderWindow : Window
    {
        private readonly int _maxPathLength;
        private readonly string _parentFolder;
        private InputFolderWindowViewModel _viewModel;
        
        public InputFolderWindow(string parentFolder, int maxPath)
        {
            InitializeComponent();
            _parentFolder = parentFolder;
            _maxPathLength = maxPath;
            Loaded += (obj, args) => _viewModel = DataContext as InputFolderWindowViewModel;
        }

        public string NewFolderName { get; set; }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = !string.IsNullOrEmpty(NewFolderName);
            base.OnClosing(e);
        }

        private void ConfirmName(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_viewModel.NewFolderName) || string.IsNullOrEmpty(_viewModel.NewFolderName))
            {
                _viewModel.Error = Strings.EmptyFolderNameError;
                return;
            }

            var invalidCharacters = CollectInvalidCharacters(_viewModel.NewFolderName, Path.GetInvalidFileNameChars());
            if (!string.IsNullOrEmpty(invalidCharacters.ToString()))
            {
                _viewModel.Error = string.Format(CultureInfo.InvariantCulture, Strings.InvalidCharactersFormat, invalidCharacters);
                return;
            }

            var newFolderName = Path.Combine(_parentFolder, _viewModel.NewFolderName);
            if (newFolderName.Length > _maxPathLength)
            {
                _viewModel.Error = Strings.PathTooLong;
                return;
            }

            if (Directory.Exists(Path.Combine(_parentFolder, _viewModel.NewFolderName)))
            {
                _viewModel.Error = Strings.DirectoryExists;
                return;
            }

            NewFolderName = _viewModel.NewFolderName;
            Close();
        }

        private static StringBuilder CollectInvalidCharacters(string path, IEnumerable<char> invalidCharacterList)
        {
            var invalidCharList = new StringBuilder();
            foreach (var character in invalidCharacterList)
            {
                var s = Convert.ToString(character, CultureInfo.InvariantCulture);
                if (path.Contains(s))
                {
                    invalidCharList.AppendFormat(CultureInfo.InvariantCulture, ", '{0}'", s);
                }
            }

            if (invalidCharList.Length > 0)
            {
                return invalidCharList.Remove(0, 2);
            }

            return invalidCharList;
        }

        private void CloseDialog(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
