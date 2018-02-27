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

            var repo = new Repository(path);

            //var dirInfo = new DirectoryInfo(path);

            foreach (var file in repo.RetrieveStatus())
            {
                var item = new FileItem
                {
                    Name = file.FilePath,
                    Path = file.FilePath
                };

                items.Add(item);
            }

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
            return items;
        }
    }
}
