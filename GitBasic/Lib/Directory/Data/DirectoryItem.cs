
namespace GitBasic
{
    /// <summary>
    /// Directory class object to hold drive/file/folder type, path, and name
    /// Code adapted from https://youtu.be/U2ZvZwDZmJU
    /// </summary>
    public class DirectoryItem
    {
        /// <summary>
        /// The item type
        /// </summary>
        public DirectoryItemType Type {get; set; }

        /// <summary>
        /// absolute path to item
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// The name of the item
        /// </summary>
        public string Name { get { return this.Type == DirectoryItemType.Drive ? this.FullPath : DirectoryStructure.GetFileOrFolderName(this.FullPath); } }
    }
}
