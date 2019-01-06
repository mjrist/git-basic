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

        private void Unstage_Click(object sender, RoutedEventArgs e)
        {
            var item = (FileSystemNode)((MenuItem)sender).DataContext;
            UnstageAction(item);
        }

        private void Stage_Click(object sender, RoutedEventArgs e)
        {
            var item = (FileSystemNode)((MenuItem)sender).DataContext;
            StageAction(item);
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var item = (FileSystemNode)((MenuItem)sender).DataContext;           
            if (item.IsFile)
            {
                Process.Start(item.Path);
            }
        }

        private void ShowInExplorer_Click(object sender, RoutedEventArgs e)
        {
            var item = (FileSystemNode)((MenuItem)sender).DataContext;
            if (item.IsFile)
            {
                Process.Start("explorer.exe", $"/select, {item.Path}");
            }
            else
            {
                Process.Start("explorer.exe", item.Path);
            }
        }

        private void UndoChanges_Click(object sender, RoutedEventArgs e)
        {
            var item = (FileSystemNode)((MenuItem)sender).DataContext;
            UndoAction(item);
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((TreeViewItem)sender).IsSelected = true;
        }

        private void FileStatus_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = (FileSystemNode)e.NewValue;
            SelectedFile = (item != null && item.IsFile) ? item.Path : string.Empty;
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
                FileSystemNode item = (FileSystemNode)e.Data.GetData(DATA_FROM_UNSTAGED);
                StageAction(item);
            }
        }

        private void Unstaged_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DATA_FROM_STAGED))
            {
                FileSystemNode item = (FileSystemNode)e.Data.GetData(DATA_FROM_STAGED);                
                UnstageAction(item);
            }
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

        public ObservableCollection<FileSystemNode> StagedItems
        {
            get { return (ObservableCollection<FileSystemNode>)GetValue(StagedItemsProperty); }
            set { SetValue(StagedItemsProperty, value); }
        }
        public static readonly DependencyProperty StagedItemsProperty =
            DependencyProperty.Register("StagedItems", typeof(ObservableCollection<FileSystemNode>), typeof(FileStatusControl), new PropertyMetadata(new ObservableCollection<FileSystemNode>()));

        public ObservableCollection<FileSystemNode> UnstagedItems
        {
            get { return (ObservableCollection<FileSystemNode>)GetValue(UnstagedItemsProperty); }
            set { SetValue(UnstagedItemsProperty, value); }
        }
        public static readonly DependencyProperty UnstagedItemsProperty =
            DependencyProperty.Register("UnstagedItems", typeof(ObservableCollection<FileSystemNode>), typeof(FileStatusControl), new PropertyMetadata(new ObservableCollection<FileSystemNode>()));

        public Action<FileSystemNode> StageAction
        {
            get { return (Action<FileSystemNode>)GetValue(StageActionProperty); }
            set { SetValue(StageActionProperty, value); }
        }
        public static readonly DependencyProperty StageActionProperty =
            DependencyProperty.Register("StageAction", typeof(Action<FileSystemNode>), typeof(FileStatusControl), new PropertyMetadata(new Action<FileSystemNode>((filePath) => { })));

        public Action<FileSystemNode> UnstageAction
        {
            get { return (Action<FileSystemNode>)GetValue(UnstageActionProperty); }
            set { SetValue(UnstageActionProperty, value); }
        }
        public static readonly DependencyProperty UnstageActionProperty =
            DependencyProperty.Register("UnstageAction", typeof(Action<FileSystemNode>), typeof(FileStatusControl), new PropertyMetadata(new Action<FileSystemNode>((filePath) => { })));

        public Action<FileSystemNode> UndoAction
        {
            get { return (Action<FileSystemNode>)GetValue(UndoActionProperty); }
            set { SetValue(UndoActionProperty, value); }
        }
        public static readonly DependencyProperty UndoActionProperty =
            DependencyProperty.Register("UndoAction", typeof(Action<FileSystemNode>), typeof(FileStatusControl), new PropertyMetadata(new Action<FileSystemNode>((fileSystemNode) => { })));

        #endregion        
    }
}
