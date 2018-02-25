using Reactive;
using System;
using System.Windows.Input;

namespace GitBasic
{
    public class CommandButtonVM
    {
        public Prop<HotKeyHelper> HotKeyHelper { get; set; } = new Prop<HotKeyHelper>();
        public Action Fetch { get; set; }
        public Action Commit { get; set; }
        public Action Status { get; set; }

        public CommandButtonVM(MainVM mainVM)
        {
            _mainVM = mainVM;
            CreateCommands();
            ReactiveAction hotKeySetter = new ReactiveAction(RegisterHotKeys, HotKeyHelper);
        }

        private void CreateCommands()
        {
            Fetch = new Action(() => _mainVM.ConsoleControlVM.ExecuteCommand(GIT_FETCH));
            Commit = new Action(() => _mainVM.ConsoleControlVM.SetInputCommand(GIT_COMMIT_ALL, GIT_COMMIT_ALL.Length - 1));
            Status = new Action(() => _mainVM.ConsoleControlVM.ExecuteCommand(GIT_STATUS));
        }

        private void RegisterHotKeys()
        {
            if (HotKeyHelper.Value != null)
            {
                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.D1, ModifierKeys.Control, Fetch));
                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.NumPad1, ModifierKeys.Control, Fetch));

                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.D2, ModifierKeys.Control, Commit));
                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.NumPad2, ModifierKeys.Control, Commit));

                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.D3, ModifierKeys.Control, Status));
                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.NumPad3, ModifierKeys.Control, Status));

            }
        }                       
        
        private const string GIT_FETCH = "git fetch";
        private const string GIT_COMMIT_ALL = "git commit -a -m \"\"";
        private const string GIT_STATUS = "git status";
        private MainVM _mainVM;
    }
}
