using LibGit2Sharp;
using Playground.Lib;
using Playground.Lib.FileSystem;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Playground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point _lastMouseDown;
        TreeViewItem _draggedItem, _target;
        Item draggedItem;
        Repository repo;

        public MainWindow()
        {
            InitializeComponent();

            var itemProvider = new ItemProvider();

            var items = itemProvider.GetItems("C:\\Users\\shaama\\Desktop\\Test Directory");

            DataContext = items;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {             
            //Diff();

            // Comment out Diff() above and uncomment this for file status example.
            GetFileStatus();
        }

        private void Diff()
        {
            // Make a small change to this file, save (don't commit), and you will see side-by-side diff.

            // Modify this to point to your repo root dir.
            string repoRoot = "C:\\source\\git-basic";
            string relativeFilePath = "Playground\\MainWindow.xaml.cs";

            using (var repo = new Repository(repoRoot))
            {
                var change = repo.Diff.Compare<TreeChanges>(new string[] { relativeFilePath }, true).FirstOrDefault();

                if (change != null)
                {
                    Blob oldBlob = repo.Lookup<Blob>(change.OldOid);
                    string oldText = oldBlob.GetContentText();
                    string newText = File.ReadAllText(Path.Combine(repoRoot, relativeFilePath));

                    // Have to normalize the line endings because LibGit2Sharp is using '\n' but Windows in '\r\n'.
                    oldText = Regex.Replace(oldText, @"\r\n|\n\r|\n|\r", "\r\n");
                    newText = Regex.Replace(newText, @"\r\n|\n\r|\n|\r", "\r\n");

                    ShowDiff(oldText, newText);
                }
            }
        }

        private void ShowDiff(string oldText, string newText)
        {
            GitSharp.Diff diff = new GitSharp.Diff(oldText, newText);
            foreach (var section in diff.Sections)
            {
                // This is a quick and easy way to get the sections of the diff to line up.
                // We don't really want to do it this way because you can't select the text.
                // Use the properties on section (BeginA, BeginB, EditWithRespectToA, ...)
                // in order to insert the appropriate whitespace. These properties will also
                // help you determine highlighting. It would be nice to have line numbers
                // as well if possible. The closer it looks to visual studio's built-in
                // diff viewer, the better.

                TextBlock oldTextBlock = new TextBlock();
                Grid.SetColumn(oldTextBlock, 0);
                oldTextBlock.Text = section.TextA.TrimEnd('\r', '\n');

                TextBlock newTextBlock = new TextBlock();
                Grid.SetColumn(newTextBlock, 1);
                newTextBlock.Text = section.TextB.TrimEnd('\r', '\n');

                Grid newSection = new Grid();
                ColumnDefinition oldColumn = new ColumnDefinition();
                ColumnDefinition newColumn = new ColumnDefinition();
                newSection.ColumnDefinitions.Add(oldColumn);
                newSection.ColumnDefinitions.Add(newColumn);
                newSection.Children.Add(oldTextBlock);
                newSection.Children.Add(newTextBlock);

                LayoutRoot.Children.Add(newSection);
            }
        }
        
        private void GetFileStatus()
        {
            // Didn't spend a lot of time on this yet. Still some to be explored here.

            // Modify this to point to your repo root dir.
            string repoRoot = "C:\\Users\\shaama\\Desktop\\Test Directory";            

            using (repo = new Repository(repoRoot))
            {
                // Haven't tested these, but this should stage/unstage a file.

                //Commands.Stage(repo, "pathtofile");
                //Commands.Unstage(repo, "pathtofile");

                foreach (var item in repo.RetrieveStatus())
                {
                    // You'll have to figure out how to tell what is staged/unstaged/untracked
                    if (item.State != FileStatus.Unaltered)
                    {
                        TextBlock filePathTextBlock = new TextBlock();
                        filePathTextBlock.Text = item.FilePath;
                        
                        LayoutRoot.Children.Add(filePathTextBlock);
                    }                    
                }                
            }
        }

        private void treeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                //MessageBox.Show(e.GetType().ToString());
                _lastMouseDown = e.GetPosition(Unstaged);

            }

        }

        private void treeView2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                //MessageBox.Show(e.GetType().ToString());
                _lastMouseDown = e.GetPosition(Unstaged);

            }

        }

        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {

            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)   
                {
                    //MessageBox.Show(sender.ToString());
                    Point currentPosition = e.GetPosition(Unstaged);

                    //if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                    //    (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                    if (true)
                    {
                        //_draggedItem = (TreeViewItem)Unstaged.SelectedItem;
                        draggedItem = (Item)Unstaged.SelectedItem;
                        //MessageBox.Show(draggedItem.Name);
                        if (draggedItem != null)
                        {
                            DragDropEffects finalDropEffect = DragDrop.DoDragDrop(Unstaged, Unstaged.SelectedValue,
                                DragDropEffects.Move);
                            /*
                            //Checking target is not null and item is dragging(moving)
                            if ((finalDropEffect == DragDropEffects.Move) && (_target != null))
                            {
                                // A Move drop was accepted 
                                if (!draggedItem.Path.Equals(_target.Path))
                                {
                                    MessageBox.Show("Drop move!");
                                    CopyItem(_draggedItem, _target);
                                    _target = null;
                                    _draggedItem = null;
                                }

                            }
                            */
                        }
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show(w.ToString());
            }
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {

                Point currentPosition = e.GetPosition(Unstaged);


                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                    (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    MessageBox.Show(e.Source.ToString());
                    // Verify that this is a valid drop and then store the drop target
                    TreeViewItem item = GetNearestContainer(e.OriginalSource as UIElement);
                    if (CheckDropTarget(_draggedItem, item))
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
                e.Handled = true;
            }
            catch (Exception y)
            {
                MessageBox.Show(y.ToString());
            }
        }

        private void Staged_DragOver(object sender, DragEventArgs e)
        {
            try
            {

                Point currentPosition = e.GetPosition(Staged);

                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                    (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    //MessageBox.Show(e.Source.ToString());
                    // Verify that this is a valid drop and then store the drop target
                    //MessageBox.Show(draggedItem.GetType().ToString());
                    //TreeViewItem item = GetNearestContainer(e.OriginalSource as UIElement);
                    if (draggedItem.GetType().Equals(typeof(FileItem)) || draggedItem.GetType().Equals(typeof(DirectoryItem)))
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
                e.Handled = true;
            }
            catch (Exception y)
            {
                MessageBox.Show(y.ToString());
            }
        }

        private void treeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                // Verify that this is a valid drop and then store the drop target
                MessageBox.Show(e.Source.ToString());
                TreeViewItem TargetItem = GetNearestContainer(e.OriginalSource as UIElement);
                if (TargetItem != null && _draggedItem != null)
                {
                    _target = TargetItem;
                    e.Effects = DragDropEffects.Move;

                }
            }
            catch (Exception)
            {
            }
        }

        private void Staged_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                // Verify that this is a valid drop and then store the drop target
                MessageBox.Show(nameof(IEnumerable));
                
                //TreeViewItem TargetItem = GetNearestContainer(e.OriginalSource as UIElement);
                if (draggedItem != null)
                {
                    // If directory, attempt to stage all files
                    if (draggedItem.GetType().GetInterface(nameof(IEnumerable)) != null)
                    {
                        Stage_Directory((DirectoryItem)draggedItem);
                        MessageBox.Show("items added to Staged TreeView");
                    }
                    //else
                    {
                        Commands.Stage(repo, draggedItem.Path);
                        MessageBox.Show(draggedItem.Name + " added to Staged TreeView");
                    }

                    // else stage file

                    // No need to add file to tree, the TreeView will refresh automagically upon repo change
                    //_target = TargetItem;
                    //e.Effects = DragDropEffects.Move;

                }
            }
            catch (Exception z)
            {
                MessageBox.Show(z.ToString());
            }
        }

        private void Stage_Directory(DirectoryItem dir_item)
        {
            // Stage all items in directory dir_item
        }

        /*
        private bool CheckDropTarget(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            //Check whether the target item is meeting your condition
            bool _isEqual = false;
            if (!_sourceItem.Header.ToString().Equals(_targetItem.Header.ToString()))
            {
                _isEqual = true;
            }
            return _isEqual;

        }
        */

        private bool CheckDropTarget(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            //Check whether the target item is meeting your condition
            bool _isEqual = false;
            if (!_sourceItem.Header.ToString().Equals(_targetItem.Header.ToString()))
            {
                _isEqual = true;
            }
            return _isEqual;

        }

        private void CopyItem(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {

            //Asking user wether he want to drop the dragged TreeViewItem here or not
            if (MessageBox.Show("Would you like to drop " + _sourceItem.Header.ToString() + " into " + _targetItem.Header.ToString() + "", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    //adding dragged TreeViewItem in target TreeViewItem
                    addChild(_sourceItem, _targetItem);

                    //finding Parent TreeViewItem of dragged TreeViewItem 
                    TreeViewItem ParentItem = FindVisualParent<TreeViewItem>(_sourceItem);
                    // if parent is null then remove from TreeView else remove from Parent TreeViewItem
                    if (ParentItem == null)
                    {
                        Unstaged.Items.Remove(_sourceItem);
                    }
                    else
                    {
                        ParentItem.Items.Remove(_sourceItem);
                    }
                }
                catch
                {

                }
            }

        }

        public void addChild(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            // add item in target TreeViewItem 
            TreeViewItem item1 = new TreeViewItem();
            item1.Header = _sourceItem.Header;
            _targetItem.Items.Add(item1);
            foreach (TreeViewItem item in _sourceItem.Items)
            {
                addChild(item, item1);
            }
        }

        static T FindVisualParent<T>(UIElement child) where T : UIElement
        {
            if (child == null)
            {
                return null;
            }

            UIElement parent = VisualTreeHelper.GetParent(child) as UIElement;

            while (parent != null)
            {
                T found = parent as T;
                if (found != null)
                {
                    return found;
                }
                else
                {
                    parent = VisualTreeHelper.GetParent(parent) as UIElement;
                }
            }

            return null;
        }

        private TreeViewItem GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }

    }
}
