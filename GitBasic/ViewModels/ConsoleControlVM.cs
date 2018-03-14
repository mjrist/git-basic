using System;

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
