using GitBasic.FileSystem;
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
            // Run once on startup. After this the ReactiveAction itemUpdater will keep the lists up to date.
            UpdateItems();
        }

        private void UpdateItems()
        {
            StagedItems.Clear();
            UnstagedItems.Clear();

            if (_mainVM.Repo.Value != null)
            {
                var itemProvider = new ItemProvider();
                List<Item> stagedItems = itemProvider.GetItems(_mainVM.Repo.Value, "Staged");
                List<Item> unstagedItems = itemProvider.GetItems(_mainVM.Repo.Value, "Unstaged");

                stagedItems.ForEach(StagedItems.Add);
                unstagedItems.ForEach(UnstagedItems.Add);
            }
        }

        private MainVM _mainVM;
    }
}
