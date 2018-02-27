using GongSolutions.Wpf.DragDrop;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBasic
{
    public class FileStatusVM : IDropTarget
    {
        // set repository root
        string repoRoot = "C:\\Users\\shaama\\Desktop\\Test Directory";

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
        
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
        
        }
    }
}
