using System.Collections.Generic;

namespace Playground.Lib.FileSystem
{
    class DirectoryItem : Item
    {
        public List<Item> Items { get; set; }

        public DirectoryItem()
        {
            Items = new List<Item>();
        }
    }
}
