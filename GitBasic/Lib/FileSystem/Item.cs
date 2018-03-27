using System;

namespace GitBasic.FileSystem
{
    public class Item
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public override bool Equals(Object obj)
        {
            if (obj.GetType().ToString().Equals(this.GetType().ToString()))
            {
                return (this.Path.Equals(((Item)obj).Path) && this.Name.Equals(((Item)obj).Name));
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (this.Path + System.IO.Path.DirectorySeparatorChar + this.Name).GetHashCode();
        }
    }
}