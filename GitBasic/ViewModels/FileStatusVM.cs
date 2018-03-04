using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitBasic.FileSystem;

namespace GitBasic
{
    public class FileStatusVM
    {

        public List<Item> StagedItems { get; set; }
        public List<Item> UnstagedItems { get; set; }

        public FileStatusVM(MainVM mainVM)
        {
            _mainVM = mainVM;
        }

        private MainVM _mainVM;
    }
}
