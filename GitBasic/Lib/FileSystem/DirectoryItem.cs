using System.Collections.Generic;
using System.Collections;
using System;

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