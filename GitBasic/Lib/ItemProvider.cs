using GitBasic.FileSystem;
using LibGit2Sharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitBasic
{
    public static class ItemProvider
    {
        public static List<Item> GetItems(Repository repo, StatusShowOption showOption)
        {
            StatusOptions stagedOptions = new StatusOptions() { Show = showOption, IncludeIgnored = false };
            var fileNames = repo.RetrieveStatus(stagedOptions).Select(x => x.FilePath).ToList();
            return BuildTreeFromRepo.RecurseRepoItems(fileNames, repo.Info.WorkingDirectory);
        }

        public static class BuildTreeFromRepo
        {
            public static List<Item> RecurseRepoItems(List<string> repo_file_list, string working_directory)
            {
                Dictionary<string, List<string>> folders_with_remaining_strings = new Dictionary<string, List<string>>();
                var items = new List<Item>();

                foreach (var repo_item in repo_file_list)
                {
                    // if file in a directory
                    if (repo_item.IndexOf(Path.DirectorySeparatorChar) > 0)
                    {
                        // split the next level directory from the the string
                        string sub_directory_name = repo_item.Substring(0, repo_item.IndexOf(Path.DirectorySeparatorChar));
                        string remaining_file_path = repo_item.Substring(repo_item.IndexOf(Path.DirectorySeparatorChar) + 1);

                        // if this directory name has already been added to the item list, find and add remaining string to directory list
                        if (folders_with_remaining_strings.ContainsKey(sub_directory_name))
                        {
                            folders_with_remaining_strings[sub_directory_name].Add(remaining_file_path);
                        }
                        else  // else create a new directory item and add remaining string to item list
                        {
                            List<string> sub_directory_file_list = new List<string>();
                            sub_directory_file_list.Add(remaining_file_path);
                            folders_with_remaining_strings.Add(sub_directory_name, sub_directory_file_list);
                        }

                    }
                    else  // just a file
                    {
                        var item = new FileItem
                        {
                            Name = repo_item.ToString(),
                            Path = Path.Combine(working_directory, repo_item.ToString())
                        };

                        items.Add(item);
                    }
                }

                // recursively build up remaining directory structures
                foreach (string key in folders_with_remaining_strings.Keys)
                {
                    var item = new DirectoryItem
                    {
                        Name = key,
                        Path = Path.Combine(working_directory, key),
                        Items = RecurseRepoItems(folders_with_remaining_strings[key], Path.Combine(working_directory, key))
                    };

                    items.Add(item);
                }

                return items;
            }
        }
    }
}
