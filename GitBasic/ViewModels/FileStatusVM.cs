using LibGit2Sharp;
using Reactive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace GitBasic
{
    public class FileStatusVM
    {
        public Action<FileSystemNode> Stage { get; set; }
        public Action<FileSystemNode> Unstage { get; set; }
        public Action<FileSystemNode> Undo { get; set; }
        public ObservableCollection<FileSystemNode> StagedItems { get; set; } = new ObservableCollection<FileSystemNode>();
        public ObservableCollection<FileSystemNode> UnstagedItems { get; set; } = new ObservableCollection<FileSystemNode>();

        public FileStatusVM(MainVM mainVM)
        {
            _mainVM = mainVM;

            Stage = new Action<FileSystemNode>((fileNode) => _mainVM.ConsoleControlVM.ExecuteCommand($"{GIT_ADD} \"{fileNode.Path}\""));
            Unstage = new Action<FileSystemNode>((fileNode) => _mainVM.ConsoleControlVM.ExecuteCommand($"{GIT_RESET_HEAD} \"{fileNode.Path}\""));
            Undo = new Action<FileSystemNode>(UndoChanges);

            // Call UpdateItems on the dispatcher to avoid threading issues.
            ReactiveAction itemUpdater = new ReactiveAction(() => Application.Current.Dispatcher.Invoke(UpdateItems), _mainVM.RepoNotifier, _mainVM.Repo);
        }

        private void UndoChanges(FileSystemNode fileNode)
        {
            var result = MessageBox.Show("This will discard changes and deleted untracked files.", "Warning", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                DeleteUntrackedFiles(fileNode);
                // Using GitSharp here instead of LibGit2Sharp because I could only get LibGit2Sharp to checkout files from HEAD and not INDEX.
                new GitSharp.Repository(_mainVM.Repo.Value.Info.WorkingDirectory).Index.Checkout(GetFilePaths(fileNode).ToArray());
                UpdateItems();
            }
        }

        private void DeleteUntrackedFiles(FileSystemNode fileNode)
        {
            foreach (var node in fileNode.Children)
            {
                DeleteUntrackedFiles(node);
            }

            if (fileNode.IsFile)
            {
                var status = _mainVM.Repo.Value.RetrieveStatus(fileNode.Path);
                if (status == FileStatus.NewInWorkdir)
                {
                    IOHelper.TryRepeatIOAction(() => { File.Delete(fileNode.Path); });
                }
            }
        }

        private List<string> GetFilePaths(FileSystemNode fileNode)
        {
            List<string> paths = new List<string>();

            foreach (var node in fileNode.Children)
            {
                paths.AddRange(GetFilePaths(node));
            }

            if (fileNode.IsFile)
            {
                paths.Add(fileNode.Path);
            }

            return paths;
        }

        private void UpdateItems()
        {
            if (_mainVM.Repo.Value != null)
            {
                ICollection<FileSystemNode> stagedItems = ItemProvider.GetItems(_mainVM.Repo.Value, StatusShowOption.IndexOnly);
                ICollection<FileSystemNode> unstagedItems = ItemProvider.GetItems(_mainVM.Repo.Value, StatusShowOption.WorkDirOnly);
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
        private void SyncItems(ICollection<FileSystemNode> oldItems, ICollection<FileSystemNode> newItems)
        {
            var itemsToRemove = oldItems.Except(newItems).ToList();
            itemsToRemove.ForEach(i => oldItems.Remove(i));

            foreach (var item in newItems)
            {
                if (oldItems.Contains(item))
                {
                    if (!item.IsFile)
                    {
                        var oldItem = oldItems.First((i) => i.Equals(item));
                        SyncItems(oldItem.Children, item.Children);
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