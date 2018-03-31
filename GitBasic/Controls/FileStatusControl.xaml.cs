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

        private void FileStatus_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedFile = (e.NewValue is FileItem file) ? file.Path : string.Empty;
        }

        private void FileStatus_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
            _itemToDrag = ((DependencyObject)e.OriginalSource).FindAnchestor<TreeViewItem>();
        }

        private Point _dragStartPoint;
        private TreeViewItem _itemToDrag;

        private void Staged_MouseMove(object sender, MouseEventArgs e) => StartDrag(sender as TreeView, e, DATA_FROM_STAGED);

        private void Unstaged_MouseMove(object sender, MouseEventArgs e) => StartDrag(sender as TreeView, e, DATA_FROM_UNSTAGED);

        private void StartDrag(TreeView sender, MouseEventArgs e, string dragDataId)
        {
            if (_itemToDrag != null && IsReadyToDrag(e))
            {
                DataObject dragData = new DataObject(dragDataId, _itemToDrag.Header);
                DragDrop.DoDragDrop(sender, dragData, DragDropEffects.Move);
            }
        }

        private bool IsReadyToDrag(MouseEventArgs e)
        {
            Vector delta = _dragStartPoint - e.GetPosition(null);
            return e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(delta.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(delta.Y) > SystemParameters.MinimumVerticalDragDistance);
        }

        private void Staged_DragOver(object sender, DragEventArgs e) => ProcessDragOver(e, DATA_FROM_UNSTAGED);

        private void Unstaged_DragOver(object sender, DragEventArgs e) => ProcessDragOver(e, DATA_FROM_STAGED);

        private void ProcessDragOver(DragEventArgs e, string dragDataId)
        {
            if (!e.Data.GetDataPresent(dragDataId))
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Staged_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DATA_FROM_UNSTAGED))
            {
                Item item = (Item)e.Data.GetData(DATA_FROM_UNSTAGED);
                string path = GetPathForCommand(item);
                StageAction($"\"{path}\"");
            }
        }

        private void Unstaged_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DATA_FROM_STAGED))
            {
                Item item = (Item)e.Data.GetData(DATA_FROM_STAGED);
                string path = GetPathForCommand(item);
                UnstageAction($"\"{path}\"");
            }
        }

        private string GetPathForCommand(Item item)
        {
            return (item is FileItem fileItem) ? fileItem.Path : $"{((DirectoryItem)item).Path}\\*";
        }

        private const string DATA_FROM_STAGED = "Staged";
        private const string DATA_FROM_UNSTAGED = "Unstaged";

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
