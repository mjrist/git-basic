using LibGit2Sharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace GitBasic
{
    public static class ItemProvider
    {
        public static ICollection<FileSystemNode> GetItems(Repository repo, StatusShowOption showOption)
        {
            StatusOptions options = new StatusOptions() { Show = showOption, IncludeIgnored = false };
            var fileNames = repo.RetrieveStatus(options).Select(x => x.FilePath).ToList();

            string repoRootDirectory = repo.Info.WorkingDirectory.TrimEnd('/', '\\');
            FileSystemNode repoRoot = new FileSystemNode()
            {
                Name = repoRootDirectory,
                Path = repoRootDirectory
            };
            BuildDirectoryTree(repoRoot, fileNames);

            var items = new ObservableCollection<FileSystemNode>();
            if (repoRoot.Children.Count > 0)
            {
                items.Add(repoRoot);
            }
            return items;
        }

        private static void BuildDirectoryTree(FileSystemNode root, List<string> relativePaths)
        {
            foreach (string path in relativePaths)
            {
                FileSystemNode parentNode = root;

                string[] tokens = path.Split('/', '\\');
                foreach (string part in tokens)
                {
                    var matchingNode = parentNode.Children.FirstOrDefault(x => x.Name == part);
                    if (matchingNode == null)
                    {
                        var newNode = new FileSystemNode() { Name = part, Path = Path.Combine(parentNode.Path, part) };
                        parentNode.Children.Add(newNode);
                        parentNode = newNode;
                    }
                    else
                    {
                        parentNode = matchingNode;
                    }
                }
            }
        }
    }
}
