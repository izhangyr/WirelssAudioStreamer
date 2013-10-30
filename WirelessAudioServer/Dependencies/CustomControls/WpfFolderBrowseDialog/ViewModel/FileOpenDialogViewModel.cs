/*
    Copyright 2013 Roman Fortunatov

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/






using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Replay.Common.Wpf.Behaviors;
using Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.CustomControls;
using Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.ExternalReferences;
using Strings = Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.Resources.Localization;

namespace Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.ViewModel
{
    public class FileOpenDialogViewModel : FolderBrowseDialogViewModel
    {
        private readonly ObservableCollection<ListViewItemLabeled> _folderFiles;
        private readonly ICommand _confirmCommand;
        private readonly string[] _filters;
        private ListViewItemLabeled _selectedElement;

        public FileOpenDialogViewModel(Action<string> closeAction, Window mainWindowHandle, string defaultPath, int maxPath, params string[] filters)
            : base(closeAction, mainWindowHandle, defaultPath, maxPath)
        {
            _folderFiles = new ObservableCollection<ListViewItemLabeled>();
            _confirmCommand = new RelayCommand(() => { RemoveCreatedFolders(); closeAction(_selectedElement.Tag.ToString()); }, IsEnabledConfirmAndNewFolder);

            PropertyChanged += (sender, args) => UpdateFolderFiles();
            _filters = filters;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded in view model.")]
        public ObservableCollection<ListViewItemLabeled> FolderFiles { get { return _folderFiles; } }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded in view model.")]
        public static new string Title { get { return Strings.FileOpenDialogTitle; } }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded in view model.")]
        public static new Visibility IsNewFolderEnabled { get { return Visibility.Collapsed; } }

        public override double ListViewWidth { get { return 300; } }

        public override double TreeViewWidth { get { return 330;} }

        public override ICommand ConfirmCommand { get { return _confirmCommand; } }

        protected override bool ShouldShowNonFixedDrives { get { return true; } }

        public void UpdateFolderFiles()
        {
            if (SelectedElement.Tag == null)
            {
                return;    
            }
            
            _folderFiles.Clear();
            _selectedElement = null;
            var directoryInfo = new DirectoryInfo(SelectedElement.Tag.ToString());
            var files = new List<FileInfo>();
            try
            {
                foreach (var filter in _filters)
                {
                    files.AddRange(directoryInfo.GetFiles(filter));
                }
            }
            catch (UnauthorizedAccessException)
            {
                var newItem = new ListViewItemLabeled {Header = Strings.InsufficientRightsError};
                newItem.Selected += (sender, args) => ((ListViewItemLabeled)sender).IsSelected = false;
                _folderFiles.Add(newItem);
                return;
            }

            foreach (var file in files)
            {
                var newItem = new ListViewItemLabeled
                    {
                        Header = file.Name,
                        IconSource = NativeMethods.GetLargeIcon(file.FullName),
                        Tag = file.FullName
                    };

                newItem.Selected += (sender, args) => _selectedElement = (ListViewItemLabeled)sender;
                _folderFiles.Add(newItem);
            }
        }

        protected override bool IsEnabledConfirmAndNewFolder()
        {
            return base.IsEnabledConfirmAndNewFolder() && _selectedElement != null;
        }
    }
}
