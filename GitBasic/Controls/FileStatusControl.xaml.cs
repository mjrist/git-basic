using GitBasic.FileSystem;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GitBasic.Controls
{
    /// <summary>
    /// Interaction logic for FileStatusControl.xaml
    /// </summary>
    public partial class FileStatusControl : UserControl
    {
        public FileStatusControl()
        {
            InitializeComponent();
        }

        private void ShowInExplorer_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem.DataContext is FileItem fileItem)
            {
                Process.Start("explorer.exe", $"/select, {fileItem.Path}");
            }
            else if (menuItem.DataContext is DirectoryItem directoryItem)
            {
                Process.Start("explorer.exe", directoryItem.Path);
            }
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((TreeViewItem)sender).IsSelected = true;
        }       

        private void Staged_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            _treeViewSource = "Staged";
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _draggedItem = (Item)Staged.SelectedItem;

                if (_draggedItem != null)
                {
                    DragDropEffects finalDropEffect = DragDrop.DoDragDrop(Staged, Staged.SelectedValue,
                        DragDropEffects.Move);
                }
            }
        }

        private void Unstaged_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            _treeViewSource = "Unstaged";
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _draggedItem = (Item)Unstaged.SelectedItem;

                if (_draggedItem != null)
                {
                    DragDropEffects finalDropEffect = DragDrop.DoDragDrop(Unstaged, Unstaged.SelectedValue,
                        DragDropEffects.Move);
                }
            }
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            if (_treeViewSource != ((TreeView)sender).Name)
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Staged_Drop(object sender, DragEventArgs e)
        {
            if (_draggedItem != null)
            {
                string path = GetPathForCommand(_draggedItem);
                StageAction(path);
            }
        }

        private void Unstaged_Drop(object sender, DragEventArgs e)
        {
            if (_draggedItem != null)
            {
                string path = GetPathForCommand(_draggedItem);
                UnstageAction(path);
            }
        }

        private string GetPathForCommand(Item item)
        {
            if (item is FileItem fileItem)
            {
                return fileItem.Path;
            }
            else // Directory item
            {
                return $"{((DirectoryItem)item).Path}\\*";
            }
        }

        // variables used to hold the item we will be dragging between controls
        private Item _draggedItem;
        private string _treeViewSource = string.Empty;

        private void FileStatus_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedFile = (e.NewValue is FileItem file) ? file.Path : string.Empty;
        }

        #region Dependency Properties

        public string SelectedFile
        {
            get { return (string)GetValue(SelectedFileProperty); }
            set { SetValue(SelectedFileProperty, value); }
        }
        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register("SelectedFile", typeof(string), typeof(FileStatusControl), new PropertyMetadata(string.Empty));

        public ObservableCollection<Item> StagedItems
        {
            get { return (ObservableCollection<Item>)GetValue(StagedItemsProperty); }
            set { SetValue(StagedItemsProperty, value); }
        }
        public static readonly DependencyProperty StagedItemsProperty =
            DependencyProperty.Register("StagedItems", typeof(ObservableCollection<Item>), typeof(FileStatusControl), new PropertyMetadata(new ObservableCollection<Item>()));

        public ObservableCollection<Item> UnstagedItems
        {
            get { return (ObservableCollection<Item>)GetValue(UnstagedItemsProperty); }
            set { SetValue(UnstagedItemsProperty, value); }
        }
        public static readonly DependencyProperty UnstagedItemsProperty =
            DependencyProperty.Register("UnstagedItems", typeof(ObservableCollection<Item>), typeof(FileStatusControl), new PropertyMetadata(new ObservableCollection<Item>()));

        public Action<string> StageAction
        {
            get { return (Action<string>)GetValue(StageActionProperty); }
            set { SetValue(StageActionProperty, value); }
        }
        public static readonly DependencyProperty StageActionProperty =
            DependencyProperty.Register("StageAction", typeof(Action<string>), typeof(FileStatusControl), new PropertyMetadata(new Action<string>((filePath) => { })));

        public Action<string> UnstageAction
        {
            get { return (Action<string>)GetValue(UnstageActionProperty); }
            set { SetValue(UnstageActionProperty, value); }
        }
        public static readonly DependencyProperty UnstageActionProperty =
            DependencyProperty.Register("UnstageAction", typeof(Action<string>), typeof(FileStatusControl), new PropertyMetadata(new Action<string>((filePath) => { })));

        #endregion
    }
}
