using GitBasic.FileSystem;
using System;
using System.Collections.Generic;
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

        // variable used to hold the item we will be dragging between controls
        Item dragged_item;

        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {

            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    dragged_item = (Item)Unstaged.SelectedItem;

                    if (dragged_item != null)
                    {
                        DragDropEffects finalDropEffect = DragDrop.DoDragDrop(Unstaged, Unstaged.SelectedValue,
                            DragDropEffects.Move);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
        }

        private void treeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                if (dragged_item != null)
                {
                    // No need to modify control, the TreeView will refresh automagically upon repo change
                    Stage_Items(dragged_item);
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
        }

        private void Stage_Items(Item item)
        {
            List<Item> directory_items;

            // if a DirectoryItem
            if (item is DirectoryItem)
            {
                directory_items = ((DirectoryItem)item).Items;

                Debug.Print(item.Name + " directory added to Staged TreeView");
                // Iterate directory Items
                foreach (Item dir_item in directory_items)
                {
                    // if DirectoryItem, recurse. else stage FileItem
                    if (dir_item is DirectoryItem)
                    {
                        Debug.Print(dir_item.Name + " directory added to Staged TreeView");
                        Stage_Items(dir_item);
                    }
                    else
                    {
                        //Commands.Stage(repo, dir_item.Path);
                        Debug.Print(dir_item.Name + " file added to Staged TreeView");
                    }
                }
            }
            else  // it's a FileItem, stage it
            {

                //Commands.Stage(repo, item.Path);
                Debug.Print(item.Name + " file added to Staged TreeView");
            }
        }

        private void FileStatus_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is FileItem file)
            {
                SelectedFile = file.Path;
            }
        }

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
    }
}
