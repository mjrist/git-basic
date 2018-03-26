using GitBasic.FileSystem;
using LibGit2Sharp;
using Reactive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

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
            //StagedItems.Clear();
            //UnstagedItems.Clear();

            if (_mainVM.Repo.Value != null)
            {
                List<Item> stagedItems = ItemProvider.GetItems(_mainVM.Repo.Value, StatusShowOption.IndexOnly);
                List<Item> unstagedItems = ItemProvider.GetItems(_mainVM.Repo.Value, StatusShowOption.WorkDirOnly);

                // compare previous lists with updated lists; add changed files to observable collection
                updateObservableCollection(StagedItems, stagedItems);
                updateObservableCollection(UnstagedItems, unstagedItems);

                // repopulate all stagedItems to observable collection
                //stagedItems.ForEach(StagedItems.Add);
                // repopulate all unstagedItems to observable collection
                //unstagedItems.ForEach(UnstagedItems.Add);
            }
        }

        // Add and remove items from the TreeViews for updated list
        // Utilizes helper class updateChangedItems
        // @param previousFileSystemItems our mirror list holding the previous directory structure, before changes
        // @param newFileSystemItems our new directory structure we need to step through to find changes
        private void updateObservableCollection(ObservableCollection<Item> previousFileSystemItems, List<Item> newFileSystemItems)
        {
            // iterate items in Observed Collection list, removing as needed
            foreach (Item item in previousFileSystemItems)
            {
                Boolean inPrevious = previousFileSystemItems.Contains(item);
                Boolean inNew = newFileSystemItems.Contains(item);

                // evaluate each item and compare to previous list, recurse directories
                if (item.GetType().ToString().Equals("GitBasic.FileSystem.DirectoryItem"))
                {
                    // if DirectoryItem unchanged, pass directory for further processing
                    if (inPrevious && inNew)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + ": no change");
                        updateChangedItems(ref item, newFileSystemItems[newFileSystemItems.IndexOf(item)]);
                    }
                    // if DirectoryItem deleted
                    if (inPrevious && !inNew)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + ": deleted");
                        updateChangedItems(item, new List<Item>);
                        previousFileSystemItems.Remove(item);
                    }
                }
                else  // FileItem
                {
                    // if FileItem deleted
                    if (inPrevious && !inNew)
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
                Boolean inNew = newFileSystemItems.Contains(item);

                if (item.GetType().ToString().Equals("GitBasic.FileSystem.DirectoryItem"))
                {
                    // if DirectoryItem added
                    if (!inPrevious && inNew)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + " added");
                        DirectoryItem newDir = new DirectoryItem();
                        newDir.Name = item.Name;
                        newDir.Path = item.Path;
                        previousFileSystemItems.Add(newDir);
                        updateChangedItems(ref newDir, item);
                    }
                }
                else  // FileItem
                {
                    // if FileItem added
                    if (!inPrevious && inNew)
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
        private void updateChangedItems(ref DirectoryItem ocDirectory, DirectoryItem updatedDirectory)
        {
            // iterate items in ocDirectory, removing as needed
            foreach (Item item in ocDirectory)
            {
                Boolean inPrevious = ocDirectory.Contains(item);
                Boolean inNew = updatedDirectory.Contains(item);

                if (item.GetType().ToString().Equals("GitBasic.FileSystem.DirectoryItem"))
                {
                    // if DirectoryItem in both file systems, recurse
                    if (inPrevious && inNew)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + ": no change");
                        int idx = updatedDirectory.Items.IndexOf(item);
                        updateChangedItems(item, updatedDirectory.Items.IndexOf(item);
                    }
                    // if DirectoryItem deleted 
                    if (inPrevious && !inNew)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + ": deleted");
                        updateChangedItems(item, new List<Item>);
                        ocDirectory.Remove(item);
                    }
                    else  // FileItem
                    {
                        // if FileItem deleted 
                        if (inPrevious && !inNew)
                        {
                            Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + ": deleted");
                            ocDirectory.Remove(item);
                        }
                    }

                }
            }
            // iterate items in updatedDirectory, adding to ocDirectory as needed
            foreach (Item item in newItem)
            {
                Boolean inPrevious = ocDirectory.Contains(item);
                Boolean inNew = newItem.Contains(item);

                if (item.GetType().ToString().Equals("GitBasic.FileSystem.DirectoryItem"))
                {
                    // if DirectoryItem added
                    if (!inPrevious && inNew)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + " added");
                        DirectoryItem newDir = new Directory();
                        newDir.Name = item.Name;
                        newDir.Path = item.Path;
                        ocDirectory.add(newDir);
                        updateChangedItems(newDir, item);
                    }
                }
                else  // FileItem
                {
                    // if FileItem added
                    if (!inPrevious && inNew)
                    {
                        Debug.Print(item.Path + System.IO.Path.DirectorySeparatorChar + item.Name + " added");
                        ocDirectory.Add(item);
                    }
                }
            }
        }

        private const string GIT_ADD = "git add";
        private const string GIT_RESET_HEAD = "git reset HEAD";
        private MainVM _mainVM;
    }
}