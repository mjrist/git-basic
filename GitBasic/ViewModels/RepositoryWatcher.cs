using LibGit2Sharp;
using Reactive;
using System;
using System.ComponentModel;
using System.IO;

namespace GitBasic
{
    public class RepositoryChangeNotifier : INotifyPropertyChanged, IDisposable
    {
        public RepositoryChangeNotifier(Prop<Repository> repo)
        {
            _repo = repo;
            SetupWatcher();
            new ReactiveAction(ToggleWatcher, _repo);
        }

        private void SetupWatcher()
        {
            _watcher = new FileSystemWatcher() { IncludeSubdirectories = true };
            _watcher.Created += Changed;
            _watcher.Changed += Changed;
            _watcher.Deleted += Changed;
            _watcher.Renamed += Changed;
        }

        private void ToggleWatcher()
        {
            if (_repo.Value == null)
            {
                _watcher.EnableRaisingEvents = false;
            }
            else
            {
                _watcher.Path = _repo.Value.Info.WorkingDirectory;
                _watcher.EnableRaisingEvents = true;
            }
        }

        private void Changed(object sender, FileSystemEventArgs e) => _throttledChangeNotifier.Execute(() => Notify());

        private void Notify()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RepositoryWatcher"));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose() => _watcher.Dispose();

        private Prop<Repository> _repo;
        private FileSystemWatcher _watcher;
        private ThrottledAction _throttledChangeNotifier = new ThrottledAction();
    }
}
