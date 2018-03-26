using System;

namespace GitBasic.FileSystem
{
    public class Item : Object
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public override bool Equals(Object obj)
        {
            return (this.Path.Equals(((Item)obj).Path) && this.Name.Equals(((Item)obj).Name));
        }

        public override int GetHashCode()
        {
            return (this.Path + System.IO.Path.DirectorySeparatorChar + this.Name).GetHashCode();
        }
    }
}