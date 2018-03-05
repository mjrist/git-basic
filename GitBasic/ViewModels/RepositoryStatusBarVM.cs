using Reactive;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

namespace GitBasic
{
    public class RepositoryStatusBarVM
    {
        public Action SelectDirectoryCommand { get; set; }
        public Action<string> Checkout { get; set; }
        public ObservableCollection<string> BranchNames { get; set; } = new ObservableCollection<string>();

        public RepositoryStatusBarVM(MainVM mainVM)
        {
            _mainVM = mainVM;
            SelectDirectoryCommand = new Action(SelectDirectory);
            Checkout = new Action<string>((branchName) => _mainVM.ConsoleControlVM.ExecuteCommand($"{GIT_CHECKOUT} {branchName}"));
            ReactiveAction branchNamesUpdater = new ReactiveAction(() => App.Current.Dispatcher.Invoke(UpdateBranchNames), _mainVM.Repo, _mainVM.RepoNotifier);
        }

        private void SelectDirectory()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = false;
            folderBrowser.SelectedPath = _mainVM.WorkingDirectory.Value;

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                _mainVM.ConsoleControlVM.ExecuteCommand($"cd {folderBrowser.SelectedPath}");
            }
        }

        private void UpdateBranchNames()
        {
            if (_mainVM.Repo.Value != null)
            {
                BranchNames.Clear();
                var newBranchNames = _mainVM.Repo.Value.Branches.Where(b => !b.IsRemote).Select(b => b.FriendlyName);
                newBranchNames.ForEach(BranchNames.Add);
            }
        }

        private const string GIT_CHECKOUT = "git checkout";

        private MainVM _mainVM;
    }
}
