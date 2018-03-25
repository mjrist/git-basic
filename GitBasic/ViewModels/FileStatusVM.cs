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
        // lists to hold current copy of displayed Items; use this to detect changes in new updates
        private List<Item> _mirroredStagedList { get; set; } = new List<Item>();
        private List<Item> _mirroredUnstagedList { get; set; } = new List<Item>();

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
            List<Item> itemsToAdd;
            List<Item> itemsToRemove;

            //StagedItems.Clear();
            //UnstagedItems.Clear();

            if (_mainVM.Repo.Value != null)
            {
                List<Item> stagedItems = ItemProvider.GetItems(_mainVM.Repo.Value, StatusShowOption.IndexOnly);
                List<Item> unstagedItems = ItemProvider.GetItems(_mainVM.Repo.Value, StatusShowOption.WorkDirOnly);

                // compare previous lists with updated lists; add changed files to observable collection
                updateChangedItems(_mirroredStagedList, stagedItems, StagedItems);
                updateChangedItems(_mirroredUnstagedList, unstagedItems, UnstagedItems);

                // repopulate all stagedItems to observable collection
                stagedItems.ForEach(StagedItems.Add);
                // repopulate all unstagedItems to observable collection
                unstagedItems.ForEach(UnstagedItems.Add);
            }
        }

        // Add and remove items from the TreeViews for updated Items in newDirectoryItems
        // @param previousDirectoryItems our mirror list holding the previous directory structure, before changes
        // @param newDirectoryItem our new directory structure we need to step through to find changes
        // @param ocDir the Observed List to make changes on so our TreeView updates
        private void updateChangedItems(List<Item> previousDirectoryItems, List<Item> newDirectoryItems, ObservableCollection<Item> ocDir)
        {
            Debug.Print("made it");
            foreach (Item item in previousDirectoryItems)
            {
                Debug.Print(item.GetType().ToString());
                // evaluate each item and compare to previous list, recurse directories
                if (item.GetType() is DirectoryItem)
                {
                    Debug.Print(item.GetType().ToString());
                }
            }
            List<Item> list = new List<Item>();
        }

        private const string GIT_ADD = "git add";
        private const string GIT_RESET_HEAD = "git reset HEAD";
        private MainVM _mainVM;
    }
}
