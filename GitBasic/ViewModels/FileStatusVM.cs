using GitBasic.FileSystem;
using LibGit2Sharp;
using Reactive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Linq;

namespace GitBasic
{
    public class FileStatusVM
    {
        public Action<string> Stage { get; set; }
        public Action<string> Unstage { get; set; }
        public ObservableCollection<Item> StagedItems { get; set; } = new ObservableCollection<Item>();
        public ObservableCollection<Item> UnstagedItems { get; set; } = new ObservableCollection<Item>();

        public FileStatusVM(MainVM mainVM)
        {
            _mainVM = mainVM;

            Stage = new Action<string>((path) => _mainVM.ConsoleControlVM.ExecuteCommand($"{GIT_ADD} {path}"));
            Unstage = new Action<string>((path) => _mainVM.ConsoleControlVM.ExecuteCommand($"{GIT_RESET_HEAD} {path}"));

            // Call UpdateItems on the dispatcher to avoid threading issues.
            ReactiveAction itemUpdater = new ReactiveAction(() => Application.Current.Dispatcher.Invoke(UpdateItems), _mainVM.RepoNotifier, _mainVM.Repo);
        }

        private void UpdateItems()
        {
            if (_mainVM.Repo.Value != null)
            {
                List<Item> stagedItems = ItemProvider.GetItems(_mainVM.Repo.Value, StatusShowOption.IndexOnly);
                List<Item> unstagedItems = ItemProvider.GetItems(_mainVM.Repo.Value, StatusShowOption.WorkDirOnly);
                
                var stagedItemsToAdd = stagedItems.Except(StagedItems).ToList();
                var stagedItemsToRemove = StagedItems.Except(stagedItems).ToList();
                stagedItemsToAdd.ForEach(i => StagedItems.Add(i));
                stagedItemsToRemove.ForEach(i => StagedItems.Remove(i));

                var unstagedItemsToAdd = unstagedItems.Except(UnstagedItems).ToList();
                var unstagedItemsToRemove = UnstagedItems.Except(unstagedItems).ToList();
                unstagedItemsToAdd.ForEach(i => UnstagedItems.Add(i));
                unstagedItemsToRemove.ForEach(i => UnstagedItems.Remove(i));


                //Debug.Print(stagedItems.Count.ToString());
                //Debug.Print(unstagedItems.Count.ToString());

                //// compare previous lists with updated lists; add changed files to observable collection
                //updateObservableCollection(StagedItems, stagedItems);
                //updateObservableCollection(UnstagedItems, unstagedItems);

                // repopulate all stagedItems to observable collection
                //stagedItems.ForEach(StagedItems.Add);
                // repopulate all unstagedItems to observable collection
                //unstagedItems.ForEach(UnstagedItems.Add);
            }
            else
            {
                StagedItems.Clear();
                UnstagedItems.Clear();
            }
        }

        // Add and remove items from the TreeViews for updated list
        // Utilizes helper class updateChangedItems for directory recursion
        // @param previousFileSystemItems our Observable Collection holding the previous directory structure, before changes
        // @param newFileSystemItems our new directory structure we need to compare against
        private void updateObservableCollection(ObservableCollection<Item> previousFileSystemItems, List<Item> newFileSystemItems)
        {
            // iterate items in Observed Collection list, removing as needed
            for(int i = 0; i < previousFileSystemItems.Count; i++)
            {
                Item item = previousFileSystemItems[i];
                Boolean inNew = newFileSystemItems.Contains(item);

                // if DirectoryItem pass for further processing; delete if needed
                if (item.GetType().ToString().Equals("GitBasic.FileSystem.DirectoryItem"))
                {
                    // if DirectoryItem unchanged, pass directory for further processing
                    if (inNew)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + ": no change");
                        int idx = newFileSystemItems.IndexOf(item);
                        updateChangedItems(ref item, newFileSystemItems[idx]);
                    }
                    // if DirectoryItem deleted
                    if (!inNew)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + ": deleted");
                        updateChangedItems(ref item, new DirectoryItem());
                        previousFileSystemItems.Remove(item);
                    }
                }
                else  // FileItem
                {
                    // if FileItem deleted
                    if (!inNew)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + ": deleted");
                        previousFileSystemItems.Remove(item);
                    }
                    // if FileItem in both file systems (no change)
                }
            }

            // iterate items in updated file system list, adding to Observed Collection as needed
            foreach (Item item in newFileSystemItems)
            {
                Boolean inPrevious = previousFileSystemItems.Contains(item);

                // if DirectoryItem
                if (item.GetType().ToString().Equals("GitBasic.FileSystem.DirectoryItem"))
                {
                    // if new DirectoryItem, create, add and recurse to fill
                    if (!inPrevious)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + " added");
                        Item newDir = new DirectoryItem();
                        newDir.Name = item.Name;
                        newDir.Path = item.Path;
                        previousFileSystemItems.Add(newDir);
                        updateChangedItems(ref newDir, item);
                    }
                }
                else  // FileItem
                {
                    // if new FileItem, add
                    if (!inPrevious)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + " added");
                        previousFileSystemItems.Add(item);
                    }
                }
            }
        }

        // Add and remove items from the Observable Collection based on new file system
        // @param ocDirectory Observable Collection list (ie previous list)
        // @param updatedDirectory the new directory we are comparing to
        private void updateChangedItems(ref Item ocDir, Item updatedDir)
        {
            DirectoryItem ocDirectory = (DirectoryItem)ocDir;
            DirectoryItem updatedDirectory = (DirectoryItem)updatedDir;

            // iterate items in ocDirectory, removing as needed
            for(int i = 0; i < ocDirectory.Items.Count; i++)
            {
                Item item = ocDirectory[i];
                Boolean inNew = updatedDirectory.Contains(item);

                if (item.GetType().ToString().Equals("GitBasic.FileSystem.DirectoryItem"))
                {
                    // if DirectoryItem in both file systems, recurse
                    if (inNew)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + ": no change");
                        int idx = updatedDirectory.Items.IndexOf(item);
                        updateChangedItems(ref item, updatedDirectory.Items[idx]);
                    }
                    // if DirectoryItem deleted 
                    if (!inNew)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + ": deleted");
                        updateChangedItems(ref item, new DirectoryItem());
                        ocDirectory.Items.Remove(item);
                    }
                }
                else  // FileItem
                {
                    // if FileItem deleted 
                    if (!inNew)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + ": deleted");
                        ocDirectory.Items.Remove(item);
                    }
                }
            }
            // iterate items in updatedDirectory, adding to ocDirectory as needed
            foreach (Item item in updatedDirectory.Items)
            {
                Boolean inPrevious = ocDirectory.Items.Contains(item);

                if (item.GetType().ToString().Equals("GitBasic.FileSystem.DirectoryItem"))
                {
                    // if DirectoryItem added
                    if (!inPrevious)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + " added");
                        Item newDir = new DirectoryItem();
                        newDir.Name = item.Name;
                        newDir.Path = item.Path;
                        ocDirectory.Items.Add(newDir);
                        updateChangedItems(ref newDir, (DirectoryItem)item);
                    }
                }
                else  // FileItem
                {
                    // if FileItem added
                    if (!inPrevious)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + " added");
                        ocDirectory.Items.Add(item);
                    }
                }
            }
        }

        private const string GIT_ADD = "git add";
        private const string GIT_RESET_HEAD = "git reset HEAD";
        private MainVM _mainVM;
    }
}