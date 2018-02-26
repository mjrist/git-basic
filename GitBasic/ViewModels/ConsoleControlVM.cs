using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBasic
{
    public class ConsoleControlVM
    {
        public ConsoleControlVM(MainVM mainVM)
        {
            _mainVM = mainVM;
        }

        public Action<string> ExecuteCommand { get; set; }
        public Action<string, int> SetInputCommand { get; set; }       

        private MainVM _mainVM;
    }
}
