using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitBasic.FileSystem;
using Reactive;

namespace GitBasic
{
    public class FileStatusVM
    {

        public List<Item> StagedItems { get; set; } = new List<Item>();
        public List<Item> UnstagedItems { get; set; } = new List<Item>();

        public FileStatusVM(MainVM mainVM)
        {
            _mainVM = mainVM;

            ReactiveAction listUpdater = new ReactiveAction(UpdateItems, _mainVM.RepoNotifier, _mainVM.Repo);
            UpdateItems();
        }

        private MainVM _mainVM;

        private void UpdateItems()
        {
            var itemProvider = new ItemProvider();
            List<Item> stagedItems = itemProvider.GetItems(_mainVM.Repo.Value, "Staged");
            List<Item> unstagedItems = itemProvider.GetItems(_mainVM.Repo.Value, "Unstaged");

            StagedItems.Clear();
            UnstagedItems.Clear();

            StagedItems.AddRange(stagedItems);
            UnstagedItems.AddRange(unstagedItems);
        }
    }
}
