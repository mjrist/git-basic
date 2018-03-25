using System.Collections.Generic;
using System.Collections;

namespace GitBasic.FileSystem
{
    public class DirectoryItem : Item, IEnumerable<Item>
    {
        public List<Item> Items { get; set; }

        public DirectoryItem()
        {
            Items = new List<Item>();
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

    }
}