using System;

namespace GitBasic.FileSystem
{
    public class Item
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public override bool Equals(Object obj)
        {
            if (obj is Item otherItem)
            {
                return Path == otherItem.Path;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }
    }
}