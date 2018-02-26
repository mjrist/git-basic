using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBasic
{
    public class FileStatusVM
    {
        public FileStatusVM(MainVM mainVM)
        {
            _mainVM = mainVM;
        }

        private MainVM _mainVM;
    }
}
