using Reactive;
using System;
using System.Windows;

namespace GitBasic
{
    public class DiffViewerVM
    {
        public DiffViewerVM(MainVM mainVM)
        {
            _mainVM = mainVM;

            // If the diff viewer's currently selected file gets changed, the diff viewer needs to be refreshed.
            // Since the diff viewer is not currently watching its file for changes, I will just refresh it on any repository changes.
            ReactiveAction itemUpdater = new ReactiveAction(() => Application.Current.Dispatcher.Invoke(RefreshDiffViewer), _mainVM.RepoNotifier, _mainVM.Repo);
        }

        public Action RefreshDiffViewer { get; set; } = new Action(() => { });

        private MainVM _mainVM;
    }
}
