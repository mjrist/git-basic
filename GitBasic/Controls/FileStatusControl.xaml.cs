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

        // variables used to hold the item we will be dragging between controls
        private Item dragged_item;
        private String tree_view_source = "";

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

        private void Staged_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tree_view_source = "Staged";
            TreeViewItem unstaged_selected_item = Unstaged.SelectedItem as TreeViewItem;
            if (unstaged_selected_item != null)
            {
                unstaged_selected_item.IsSelected = false;
            }
        }

        private void Unstaged_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tree_view_source = "Unstaged";
            TreeViewItem staged_selected_item = Staged.SelectedItem as TreeViewItem;
            if (staged_selected_item != null)
            {
                staged_selected_item.IsSelected = false;
            }
        }

        private void Staged_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            try
            {
                tree_view_source = "Staged";
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    dragged_item = (Item)Staged.SelectedItem;

                    if (dragged_item != null)
                    {
                        DragDropEffects finalDropEffect = DragDrop.DoDragDrop(Staged, Staged.SelectedValue,
                            DragDropEffects.Move);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
        }

        private void Unstaged_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            try
            {
                tree_view_source = "Unstaged";
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
                if (!tree_view_source.Equals(((TreeView)sender).Name))
                {
                    e.Effects = DragDropEffects.Move;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
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
                    Stage_Unstage_Items(dragged_item, ((TreeView)sender).Name);
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
        }

        private void Stage_Unstage_Items(Item item, String tree_view_name)
        {
            List<Item> directory_items;

            // if a DirectoryItem
            if (item is DirectoryItem)
            {
                directory_items = ((DirectoryItem)item).Items;

                Debug.Print(item.Name + " directory added to " + tree_view_name + " TreeView");
                // Iterate directory Items
                foreach (Item dir_item in directory_items)
                {
                    // if DirectoryItem, recurse. else stage FileItem
                    if (dir_item is DirectoryItem)
                    {
                        Debug.Print(dir_item.Name + " directory added to " + tree_view_name + " TreeView");
                        Stage_Unstage_Items(dir_item, tree_view_name);
                    }
                    else
                    {
                        // *** NEED GIT ADD/REMOVE COMMAND HERE ***
                        Debug.Print(dir_item.Name + " file added to " + tree_view_name + " TreeView");
                    }
                }
            }
            else  // it's a FileItem, stage/unstage it
            {
                // *** NEED GIT ADD/REMOVE COMMAND HERE ***
                Debug.Print(item.Name + " file added to " + tree_view_name + " TreeView");
            }
        }

        private void Staged_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is FileItem file)
            {
                SelectedFile = file.Path;;
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
