using System.IO;
using System.Collections.Generic;
using Playground.Lib.FileSystem;
using LibGit2Sharp;

namespace Playground.Lib
{
    public class ItemProvider
    {
        public List<Item> GetItems(string path)
        {
            var items = new List<Item>();
;           List<string> repo_items = new List<string>;
            
            if (Repository.IsValid(path))
            {

                var repo = new Repository(path);

                var dir_info = new DirectoryInfo(path);

                BuildTreeFromRepo file_tree = new BuildTreeFromRepo();

                foreach (var repo_item in repo.RetrieveStatus())
                {
                    repo_items.Add(repo_item.FilePath);
                }
                items = file_tree.RecurseRepoItems(repo_items, path);


                /*
                foreach (var directory in dirInfo.GetDirectories())
                {
                    var item = new DirectoryItem
                    {
                        Name = directory.Name,
                        Path = directory.FullName,
                        Items = GetItems(directory.FullName)
                    };

                    items.Add(item);
                }

                foreach (var file in dirInfo.GetFiles())
                {
                    var item = new FileItem
                    {
                        Name = file.Name,
                        Path = file.FullName
                    };

                    items.Add(item);
                }
                */
            }
            return items;
        }

        class BuildTreeFromRepo
        {
            public List<Item> RecurseRepoItems(List<string> repo_file_list, string working_directory)
            {
                Dictionary<string, List<string>> item_files_directories = new Dictionary<string, List<string>>();
                var items = new List<Item>();

                foreach (var repo_item in repo_file_list)
                {
                    // if file in a directory
                    if (repo_item.IndexOf("\\") > -1)
                    {
                        // split the next level directory from the the strinng
                        string sub_directory_name = repo_item.Substring(0, repo_item.IndexOf("\\"));
                        string remaining_file_path = repo_item.Substring(repo_item.IndexOf("\\"));

                        // if this directory name has already been added to the item list, find and add remaining string to directory
                        if (item_files_directories.ContainsKey(sub_directory_name))
                        {
                            item_files_directories[sub_directory_name].Add(remaining_file_path);
                        }
                        else
                        {
                            List<string> sub_directory_file_list = new List<string>();
                            sub_directory_file_list.Add(remaining_file_path);
                            item_files_directories.Add(sub_directory_name, sub_directory_file_list);
                        }
                        // else create a new directory item and add remaining string to item list

                        items.Add(item);
                    }
                    else  // just a file
                    {
                        var item = new FileItem
                        {
                            Name = repo_item.FilePath,
                            Path = Path.Combine(working_directory, repo_item.FilePath)
                        };

                        items.Add(item);
                    }
                }
                return items;

            }
        }
    }
}
