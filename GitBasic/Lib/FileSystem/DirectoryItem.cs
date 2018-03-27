using System.Collections.Generic;
using System.Collections;
using System;

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

        public Boolean Contains(Item item)
        {
            return Items.Contains(item);
        }

        public int IndexOf(Item item)
        {
            return Items.IndexOf(item);
        }

        public Item this[int index]
        {
            get
            {
                return Items[index];
            }
            set
            {
                Items[index] = value;
            }
        }

    }
}