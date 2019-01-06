using System;
using System.Collections.ObjectModel;
using System.IO;

namespace GitBasic
{
    public class FileSystemNode
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public bool IsFile => Children.Count == 0;

        public bool Exists => IsFile ? File.Exists(Path) : Directory.Exists(Path);

        public bool IsFileAndExists => IsFile && Exists;

        public ObservableCollection<FileSystemNode> Children { get; set; } = new ObservableCollection<FileSystemNode>();

        public override bool Equals(Object obj) => obj is FileSystemNode otherItem && Path == otherItem.Path;

        public override int GetHashCode() => Path.GetHashCode();
    }
}