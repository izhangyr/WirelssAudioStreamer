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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Replay.Common.Wpf.Behaviors;
using Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.CustomControls;
using Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.ExternalReferences;
using Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.View;
using Strings = Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.Resources.Localization;

namespace Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.ViewModel
{
    public class FolderBrowseDialogViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<TreeViewItemLabeled> _folderStructure;
        private readonly ICommand _closeCommand;
        private readonly ICommand _confirmCommand;
        private readonly ICommand _newFolderCommand;
        private readonly Window _mainWindowHandle;
        private readonly string _defaultPath;
        private readonly List<string> _createdFolders = new List<string>();
        private readonly int _maxPath;
        private string _error;
        
        public FolderBrowseDialogViewModel(Action<string> closeAction, Window mainWindowHandle, string defaultPath, int maxPath)
        {
            _closeCommand = new RelayCommand(() =>
                {
                    RemoveCreatedFolders();
                    closeAction(string.Empty);
                });

            _confirmCommand = new RelayCommand(() => { RemoveCreatedFolders(); closeAction(SelectedElement.Tag.ToString()); }, IsEnabledConfirmAndNewFolder);

            _newFolderCommand = new RelayCommand(GetNewFolderNameAndCreate, IsEnabledConfirmAndNewFolder);
            _defaultPath = defaultPath;
            _folderStructure = PrepareStructure();
            SetDefaultSelection();
            _mainWindowHandle = mainWindowHandle;
            _maxPath = maxPath;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Action<string, bool> CloseAction { get; set; }

        public string NewFolderName { get; set; }

        public string Error
        {
            get
            {
                return _error;
            }

            set 
            { 
                _error = value;
                OnPropertyChanged("Error");
            }
        }

        public ObservableCollection<TreeViewItemLabeled> FolderStructure
        {
            get { return _folderStructure; }
        }
        
        public ICommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public virtual ICommand ConfirmCommand
        {
            get { return _confirmCommand; }
        }

        public ICommand NewFolderCommand
        {
            get { return _newFolderCommand; }
        }

        public static string Title { get { return Strings.FolderBrowseDialogTitle; } }

        public static Visibility IsNewFolderEnabled { get { return Visibility.Visible; } }
        
        public virtual double TreeViewWidth { get { return 410; } }

        public virtual double ListViewWidth { get { return 0; } }

        protected TreeViewItemLabeled SelectedElement { get; set; }

        protected virtual bool ShouldShowNonFixedDrives { get { return false; } }

        protected virtual bool IsEnabledConfirmAndNewFolder()
        {
            return SelectedElement != null && SelectedElement.Tag != null;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "It's ok. I don't care about folders I can't delete.")]
        protected void RemoveCreatedFolders()
        {
            _createdFolders.Reverse();
            foreach (var createdFolder in _createdFolders)
            {
                try
                {
                    Directory.Delete(createdFolder);
                }
                catch (Exception)
                {
                }
            }
        }

        private ObservableCollection<TreeViewItemLabeled> PrepareStructure()
        {
            var avalibleDisks = GetAvalibleDrives();
            var resultCollection = new ObservableCollection<TreeViewItemLabeled>();
            foreach (var avalibleDisk in avalibleDisks)
            {
                var modifyableVariable = avalibleDisk;
                modifyableVariable.Items.Add(new object());
                modifyableVariable.Expanded += ElementExpanded;
                modifyableVariable.Collapsed += ElementCollapsed;
                modifyableVariable.Selected += (sender, args) =>
                    {
                        args.Handled = true;
                        SelectedElement = args.Source as TreeViewItemLabeled;
                        OnPropertyChanged("FolderStucture");
                    };
                resultCollection.Add(modifyableVariable);
            }

            return resultCollection;
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "I need an exact copy, not one object.")]
        private void ElementCollapsed(object sender, RoutedEventArgs routedEventArgs)
        {
            routedEventArgs.Handled = true;
            var collapsedElement = sender as TreeViewItemLabeled;
            var originalSender = sender as TreeViewItemLabeled;
            if (collapsedElement == null)
            {
                return;    
            }

            collapsedElement.IsSelected = true;
            collapsedElement.Items.Clear();
            var children = GetChildrenItems(collapsedElement.Tag.ToString());
            foreach (var child in children)
            {
                collapsedElement.Items.Add(child);
            }

            var parent = originalSender.Parent as TreeViewItemLabeled;
            if (parent == null)
            {
                ExecuteWithPreserveSelection(() => ReplaceElement(originalSender, collapsedElement));
            }
            else
            {
                ExecuteWithPreserveSelection(() => ReplaceAllTopLevelElements(collapsedElement, originalSender));
            }

            OnPropertyChanged("FolderStucture");
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "I need an exact copy, not one object.")]
        private void ElementExpanded(object sender, RoutedEventArgs args)
        {
            args.Handled = true;
            var expandedElement = sender as TreeViewItemLabeled;
            var originalSender = sender as TreeViewItemLabeled;
            if (expandedElement == null)
            {
                return;
            }

            expandedElement.IsSelected = true;
            expandedElement.Items.Clear();
            var modifyableCollection = GetChildrenItems(expandedElement.Tag.ToString());
            foreach (var item in modifyableCollection)
            {
                var modifyableVariable = item;
                if (GetChildrenFolders(modifyableVariable.Tag.ToString()).Any())
                {
                    modifyableVariable.Items.Add(new object());
                }

                modifyableVariable.Expanded += ElementExpanded;
                modifyableVariable.Collapsed += ElementCollapsed;
                modifyableVariable.Selected += (element, eventArgs) =>
                    {
                        eventArgs.Handled = true;
                        SelectedElement = eventArgs.Source as TreeViewItemLabeled;
                        OnPropertyChanged("FolderStucture");
                    };

                expandedElement.Items.Add(modifyableVariable);
            }

            var parent = expandedElement.Parent as TreeViewItemLabeled;
            if (parent == null)
            {
                ExecuteWithPreserveSelection(() => ReplaceElement(originalSender, expandedElement));
            }
            else
            {
                ExecuteWithPreserveSelection(() => ReplaceAllTopLevelElements(expandedElement, originalSender));
            }

            if (!string.IsNullOrEmpty(NewFolderName))
            {
                UpdateSelection(NewFolderName);
                NewFolderName = string.Empty;
            }

            OnPropertyChanged("FolderStucture");
        }

        private void ReplaceAllTopLevelElements(TreeViewItemLabeled childElement, FrameworkElement originalSender)
        {
            var parent = originalSender.Parent as TreeViewItemLabeled;
            if (parent == null)
            {
                return;
            }

            var elementIndex = parent.Items.IndexOf(originalSender);
            var originalParent = new TreeViewItemLabeled();
            while (parent != null)
            {
                originalParent = parent;
                parent.Items.RemoveAt(elementIndex);
                parent.Items.Insert(elementIndex, childElement);
                childElement = parent;
                parent = childElement.Parent as TreeViewItemLabeled;
                if (parent != null)
                {
                    elementIndex = parent.Items.IndexOf(originalParent);
                }
            }

            ReplaceElement(originalParent, childElement);
        }

        private static IEnumerable<object> SafeExecuteDirectoryOperation(Func<IEnumerable<object>> operation)
        {
            try
            {
                return operation();
            }
            catch (DirectoryNotFoundException)
            {
                // Nothing to do
            }
            catch (UnauthorizedAccessException)
            {
                // Nothing to do
            }

            return Enumerable.Empty<object>();
        }

        private static IEnumerable<TreeViewItemLabeled> GetChildrenItems(string path)
        {
            var result = SafeExecuteDirectoryOperation(() => Directory.GetDirectories(path).Select(x =>
                                                                                  new TreeViewItemLabeled
                                                                                      {
                                                                                          Header = Path.GetFileName(x),
                                                                                          Tag = x,
                                                                                          IconSource = NativeMethods.GetLargeIcon(x)
                                                                                      })
                                                     .OrderBy(x => x.Header)
                                                     .ToList());
            return result.Any() ? (IEnumerable<TreeViewItemLabeled>)result : Enumerable.Empty<TreeViewItemLabeled>();
        }

        private static IEnumerable<string> GetChildrenFolders(string path)
        {
            var result = SafeExecuteDirectoryOperation(() => Directory.GetDirectories(path).ToList());
            return result.Any() ? (IEnumerable<string>)result : Enumerable.Empty<string>();
        }

        private void GetNewFolderNameAndCreate()
        {
            var newFolderDialog = new InputFolderWindow(SelectedElement.Tag.ToString(), _maxPath)
                {
                    DataContext = new InputFolderWindowViewModel(),
                    Owner = _mainWindowHandle
                };

            var dialogResult = newFolderDialog.ShowDialog();

            if (dialogResult == null || !(bool)dialogResult)
            {
                return;
            }

            NewFolderName = newFolderDialog.NewFolderName;
            var directoryToCreate = Path.Combine(SelectedElement.Tag.ToString(), NewFolderName);
            Directory.CreateDirectory(directoryToCreate);
            _createdFolders.Add(directoryToCreate);
            ResetElementContent(SelectedElement);
        }

        private void ExecuteWithPreserveSelection(Action function)
        {
            var oldSelection = SelectedElement;
            function();
            oldSelection.IsSelected = true;
        }

        private void ReplaceElement(TreeViewItemLabeled oldElement, TreeViewItemLabeled newElement)
        {
            var originalParentIndex = _folderStructure.IndexOf(oldElement);
            _folderStructure.RemoveAt(originalParentIndex);
            _folderStructure.Insert(originalParentIndex, newElement);
        }

        private void UpdateSelection(string itemHeader)
        {
            var newSelection =
                (from TreeViewItemLabeled item in SelectedElement.Items
                 where string.Equals(item.Header, itemHeader, StringComparison.OrdinalIgnoreCase)
                 select item).FirstOrDefault();
            var newSelectionIndex = SelectedElement.Items.IndexOf(newSelection);
            if (newSelectionIndex == -1)
            {
                return;
            }

            newSelection = SelectedElement.Items[newSelectionIndex] as TreeViewItemLabeled;
            if (newSelection == null)
            {
                return;
            }

            newSelection.IsSelected = true;
            newSelection.Focus();
        }

        private void SetDefaultSelection()
        {
            var splittedDefaultPath = _defaultPath.Split(new[] { ":\\", "\\" }, StringSplitOptions.None).ToList();
            if (!splittedDefaultPath.Any())
            {
                return;    
            }

            var currentSelection = (from TreeViewItemLabeled item in _folderStructure
                                    where item.Header.Contains(splittedDefaultPath[0])
                                    select item).FirstOrDefault();
            currentSelection.IsSelected = true;
            currentSelection.IsExpanded = true;
            splittedDefaultPath.RemoveAt(0);

            foreach (var entry in splittedDefaultPath)
            {
                var currentDirectory = Path.Combine(SelectedElement.Tag.ToString(), entry);
                if (!Directory.Exists(currentDirectory))
                {
                    Directory.CreateDirectory(currentDirectory);
                    _createdFolders.Add(currentDirectory);
                }

                ResetElementContent(SelectedElement);
                UpdateSelection(entry);
            }
        }

        private IEnumerable<TreeViewItemLabeled> GetAvalibleDrives()
        {
            var allDrives = DriveInfo.GetDrives();
            foreach (var driveInfo in allDrives)
            {
                if (driveInfo.IsReady && (ShouldShowNonFixedDrives || driveInfo.DriveType == DriveType.Fixed))
                {
                    TreeViewItemLabeled item;
                    try
                    {
                        item = new TreeViewItemLabeled
                        {
                            Header = string.Join(" ", driveInfo.Name, driveInfo.VolumeLabel),
                            Tag = driveInfo.Name,
                            IconSource = NativeMethods.GetLargeIcon(driveInfo.Name)
                        };
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    yield return item;
                }
            }
        }

        //Hack!

        /// <summary>
        /// Here we reset element content by raising Expanded/Collapsed event which causes element to reload it's contents
        /// </summary>
        /// <param name="element">Element to reset</param>
        private static void ResetElementContent(TreeViewItemLabeled element)
        {
            element.IsExpanded = false;
            element.IsExpanded = true;
        }
    }
}
