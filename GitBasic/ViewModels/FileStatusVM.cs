using GitBasic.FileSystem;
using LibGit2Sharp;
using Reactive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
                updateChangedItems(ref StagedItems, stagedItems);
                updateChangedItems(ref UnstagedItems, unstagedItems);

                // repopulate all stagedItems to observable collection
                //stagedItems.ForEach(StagedItems.Add);
                // repopulate all unstagedItems to observable collection
                //unstagedItems.ForEach(UnstagedItems.Add);
            }
        }

        // Add and remove items from the TreeViews for updated Items in newDirectoryItems
        // @param previousDirectoryItems our mirror list holding the previous directory structure, before changes
        // @param newDirectoryItem our new directory structure we need to step through to find changes
        // @param ocDir the Observed List to make changes on so our TreeView updates
        private void updateChangedItems(ref ObservableCollection<Item> previousFileSystemItems, List<Item> newFileSystemItems)
        {
            foreach (Item item in newFileSystemItems)
            {
                Boolean inPrevious = previousFileSystemItems.Contains(item);
                Boolean inNew = newFileSystemItems.Contains(item);

                // evaluate each item and compare to previous list, recurse directories
                if (item.GetType().ToString().Equals("GitBasic.FileSystem.DirectoryItem"))
                {
                    // if DirectoryItem in both file systems (no change)
                    // REMOVE -- NOT NEEDED
                    if (inPrevious && inNew)
                    {
                        Debug.Print(item.Path + "\\" + item.Name + ": no change");
                    }
                    // if DirectoryItem in previous, but not new file system (deleted)
                    if (inPrevious && !inNew)
                    {
                        Debug.Print(item.Path + "\\" + item.Name + ": deleted");
                        previousFileSystemItems.Remove(item);
                        //ocDir.Remove(item); or Staged.Remove(item) ???
                    }
                    // if DirectoryItem in new, but not previous file system (added)
                    if (!inPrevious && inNew)
                    {
                        Debug.Print(item.Path + "\\" + item.Name + " added");
                        previousFileSystemItems.Add(item);
                        //ocDir.Add(item); or Staged.Add(item) ???
                    }
                }
                else  // FileItem
                {

                    // if FileItem in both file systems (no change)
                    // REMOVE -- NOT NEEDED
                    if (inPrevious && inNew)
                    {
                        Debug.Print(item.Path + "\\" + item.Name + ": no change");
                    }
                    // if FileItem in previous, but not new file system (deleted)
                    if (inPrevious && !inNew)
                    {
                        Debug.Print(item.Path + "\\" + item.Name + ": deleted");
                        previousFileSystemItems.Remove(item);
                        //ocDir.Remove(item); or Staged.Remove(item) ???
                    }
                    // if FileItem in new, but not previous file system (added)
                    if (!inPrevious && inNew)
                    {
                        Debug.Print(item.Path + "\\" + item.Name + " added");
                        previousFileSystemItems.Add(item);
                        //ocDir.Add(item); or Staged.Add(item) ???
                    }
                }
            }
            List<Item> list = new List<Item>();
        }

        private const string GIT_ADD = "git add";
        private const string GIT_RESET_HEAD = "git reset HEAD";
        private MainVM _mainVM;
    }
}
