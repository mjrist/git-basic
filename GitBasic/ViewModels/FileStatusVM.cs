using GitBasic.FileSystem;
using LibGit2Sharp;
using Reactive;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace GitBasic
{
    public class FileStatusVM
    {
        public ObservableCollection<Item> StagedItems { get; set; } = new ObservableCollection<Item>();
        public ObservableCollection<Item> UnstagedItems { get; set; } = new ObservableCollection<Item>();

        public FileStatusVM(MainVM mainVM)
        {
            _mainVM = mainVM;
            // Call UpdateItems on the dispatcher to avoid threading issues.
            ReactiveAction itemUpdater = new ReactiveAction(() => Application.Current.Dispatcher.Invoke(UpdateItems), _mainVM.RepoNotifier, _mainVM.Repo);
        }

        private void UpdateItems()
        {
            StagedItems.Clear();
            UnstagedItems.Clear();

            if (_mainVM.Repo.Value != null)
            {
                List<Item> stagedItems = ItemProvider.GetItems(_mainVM.Repo.Value, StatusShowOption.IndexOnly);
                List<Item> unstagedItems = ItemProvider.GetItems(_mainVM.Repo.Value, StatusShowOption.WorkDirOnly);

                stagedItems.ForEach(StagedItems.Add);
                unstagedItems.ForEach(UnstagedItems.Add);
            }
        }

        private MainVM _mainVM;
    }
}
