using GitBasic.FileSystem;
using LibGit2Sharp;
using Reactive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            }
            else
            {
                StagedItems.Clear();
                UnstagedItems.Clear();
            }
        }
        
        private const string GIT_ADD = "git add";
        private const string GIT_RESET_HEAD = "git reset HEAD";
        private MainVM _mainVM;
    }
}