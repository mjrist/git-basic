using System.Collections.ObjectModel;

namespace GitBasic.FileSystem
{
    public class DirectoryItem : Item
    {
        public ObservableCollection<Item> Items { get; set; }

        public DirectoryItem()
        {
            Items = new ObservableCollection<Item>();
        }
    }
}