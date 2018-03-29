using GitBasic.FileSystem;
using LibGit2Sharp;
using Reactive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            if (_mainVM.Repo.Value != null)
            {
                ICollection<Item> stagedItems = ItemProvider.GetItems(_mainVM.Repo.Value, StatusShowOption.IndexOnly);
                ICollection<Item> unstagedItems = ItemProvider.GetItems(_mainVM.Repo.Value, StatusShowOption.WorkDirOnly);
                SyncItems(StagedItems, stagedItems);
                SyncItems(UnstagedItems, unstagedItems);
            }
            else
            {
                StagedItems.Clear();
                UnstagedItems.Clear();
            }
        }
        
        /// <summary>
        /// This function adds and removes items from the oldItems list so that it matches
        /// the newItems list. We don't want to simply reset the oldItems list because we
        /// need to keep the old objects for the items which haven't changed.
        /// </summary>       
        private void SyncItems(ICollection<Item> oldItems, ICollection<Item> newItems)
        {
            var itemsToRemove = oldItems.Except(newItems).ToList();
            itemsToRemove.ForEach(i => oldItems.Remove(i));

            foreach (var item in newItems)
            {
                if (oldItems.Contains(item))
                {
                    if (item is DirectoryItem dirItem)
                    {
                        DirectoryItem oldItem = (DirectoryItem)oldItems.First((i) => i.Equals(item));
                        SyncItems(oldItem.Items, dirItem.Items);
                    }
                }
                else
                {
                    oldItems.Add(item);
                }
            }
        }

        private const string GIT_ADD = "git add";
        private const string GIT_RESET_HEAD = "git reset HEAD";
        private MainVM _mainVM;
    }
}