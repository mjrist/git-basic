using System.Collections.Generic;

namespace GitBasic.FileSystem
{
    public class DirectoryItem : Item
    {
        public List<Item> Items { get; set; }

        public DirectoryItem()
        {
            Items = new List<Item>();
        }
    }
}