




using System;
using System.IO;
using System.Linq;
using Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.Contracts;
using Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.Resources;
using Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.ViewModel;

namespace Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.View
{
    public partial class FolderBrowseDialog
    {
        private readonly FolderBrowseDialogViewModel _viewModel;

        public FolderBrowseDialog(string defaultPath, int maxPath, DialogType dialogType = DialogType.FolderBrowseDialog, string[] filters = null)
        {
            InitializeComponent();
            switch (dialogType)
            {
                case DialogType.FolderBrowseDialog:
                {
                    _viewModel = new FolderBrowseDialogViewModel(SetSelectedFolderAndClose, this, defaultPath, maxPath);
                    break;
                }

                case DialogType.FileOpenDialog:
                {
                    _viewModel = new FileOpenDialogViewModel(SetSelectedFileAndClose, this, defaultPath, maxPath, filters ?? new[] {"*.*"});
                    break;
                }

                default:
                {
                    throw new InvalidOperationException("Unknown dialog type was passed.");
                }
            }
            
            DataContext = _viewModel;
            SelectedItem = string.Empty;
        }

        public string SelectedItem { get; set; }

        private void SetSelectedFolderAndClose(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                DialogResult = false;
                Close();
                return;
            }

            if (Directory.Exists(path))
            {
                try
                {
                    if (Directory.EnumerateFileSystemEntries(path).Any())
                    {
                        _viewModel.Error = Localization.DirectoryNotEmpyError;
                        return;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    _viewModel.Error = Localization.InsufficientRightsError;
                    return;
                }
            }

            SelectedItem = path;
            DialogResult = true;
            Close();
        }

        private void SetSelectedFileAndClose(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                DialogResult = false;
                Close();
                return;
            }

            if (!File.Exists(path))
            {
                DialogResult = false;
                Close();
                return;
            }

            if (!string.Equals(Path.GetExtension(path), ".inf", StringComparison.OrdinalIgnoreCase))
            {
                DialogResult = false;
                Close();
                return;
            }

            SelectedItem = path;
            DialogResult = true;
            Close();
        }
    }
}