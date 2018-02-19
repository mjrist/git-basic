using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitBasic
{
    /// <summary>
    /// Get Directory Structure class
    /// Code adapted from https://youtu.be/U2ZvZwDZmJU
    /// </summary>
    public static class DirectoryStructure
    {
      
        /// <summary>
        /// Gets contents of the top level of fullPath
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static List<DirectoryItem> GetDirectoryContents(string fullPath)
        {
            var items = new List<DirectoryItem>();

            #region Get Directories

            // try and get directories from folder, ignore issues
            try
            {
                var dirs = Directory.GetDirectories(fullPath);

                if (dirs.Length > 0)
                    items.AddRange(dirs.Select(dir => new DirectoryItem { FullPath = dir, Type = DirectoryItemType.Folder }));
            }
            catch
            { }

            #endregion

            #region Get Files

            // try and get files from folder, ignore issues
            try
            {
                var fs = Directory.GetFiles(fullPath);

                if (fs.Length > 0)
                    items.AddRange(fs.Select(file => new DirectoryItem { FullPath = file, Type = DirectoryItemType.File }));
            }
            catch
            { }

            #endregion

            return items;
        }

        /// <summary>
        /// Find file/folder name from full path
        /// </summary>
        /// <param name="path"></param>
        public static string GetFileOrFolderName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            var normalizedPath = path.Replace('/', '\\');

            var lastSlash = normalizedPath.LastIndexOf('\\');

            // if no slash, return path
            if (lastSlash <= 0)
            {
                return path;
            }

            // return name substring
            return path.Substring(lastSlash + 1);
        }

    }
}
