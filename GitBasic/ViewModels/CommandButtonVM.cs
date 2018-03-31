using Reactive;
using System;
using System.Windows.Input;

namespace GitBasic
{
    public class CommandButtonVM
    {
        public Prop<HotKeyHelper> HotKeyHelper { get; set; } = new Prop<HotKeyHelper>();

        public Action Status { get; set; }
        public Action Fetch { get; set; }
        public Action Checkout { get; set; }
        public Action Commit { get; set; }
        public Action AddAll { get; set; }
        public Action CommitAll { get; set; }
        public Action Pull { get; set; }
        public Action Push { get; set; }
        public Action Branch { get; set; }
        public Action Merge { get; set; }

        public CommandButtonVM(MainVM mainVM)
        {
            _mainVM = mainVM;
            CreateCommands();
            ReactiveAction hotKeySetter = new ReactiveAction(RegisterHotKeys, HotKeyHelper);
        }

        private void CreateCommands()
        {
            Status = new Action(() => _mainVM.ConsoleControlVM.ExecuteCommand(GIT_STATUS));
            Fetch = new Action(() => _mainVM.ConsoleControlVM.ExecuteCommand(GIT_FETCH));
            Checkout = new Action(() => _mainVM.ConsoleControlVM.SetInputCommand(GIT_CHECKOUT, GIT_CHECKOUT.Length));
            Commit = new Action(() => _mainVM.ConsoleControlVM.SetInputCommand(GIT_COMMIT, GIT_COMMIT.Length - 1));
            AddAll = new Action(() => _mainVM.ConsoleControlVM.ExecuteCommand(GIT_ADD_ALL));
            CommitAll = new Action(() => _mainVM.ConsoleControlVM.SetInputCommand(GIT_COMMIT_ALL, GIT_COMMIT_ALL.Length - 1));
            Pull = new Action(() => _mainVM.ConsoleControlVM.SetInputCommand(GIT_PULL, GIT_PULL.Length));
            Push = new Action(() => _mainVM.ConsoleControlVM.SetInputCommand(GIT_PUSH, GIT_PUSH.Length));
            Branch = new Action(() => _mainVM.ConsoleControlVM.SetInputCommand(GIT_BRANCH, GIT_BRANCH.Length));
            Merge = new Action(() => _mainVM.ConsoleControlVM.SetInputCommand(GIT_MERGE, GIT_MERGE.Length));
        }

        private void RegisterHotKeys()
        {
            if (HotKeyHelper.Value != null)
            {
                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.D1, ModifierKeys.Control, Status));
                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.NumPad1, ModifierKeys.Control, Status));

                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.D2, ModifierKeys.Control, Fetch));
                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.NumPad2, ModifierKeys.Control, Fetch));

                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.D3, ModifierKeys.Control, Checkout));
                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.NumPad3, ModifierKeys.Control, Checkout));

                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.D4, ModifierKeys.Control, Commit));
                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.NumPad4, ModifierKeys.Control, Commit));

                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.D5, ModifierKeys.Control, AddAll));
                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.NumPad5, ModifierKeys.Control, AddAll));

                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.D6, ModifierKeys.Control, CommitAll));
                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.NumPad6, ModifierKeys.Control, CommitAll));

                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.D7, ModifierKeys.Control, Pull));
                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.NumPad7, ModifierKeys.Control, Pull));

                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.D8, ModifierKeys.Control, Push));
                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.NumPad8, ModifierKeys.Control, Push));

                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.D9, ModifierKeys.Control, Branch));
                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.NumPad9, ModifierKeys.Control, Branch));

                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.D0, ModifierKeys.Control, Merge));
                HotKeyHelper.Value.RegisterHotKey(new HotKey(Key.NumPad0, ModifierKeys.Control, Merge));
            }
        }

        private const string GIT_STATUS = "git status";
        private const string GIT_FETCH = "git fetch";
        private const string GIT_CHECKOUT = "git checkout ";
        private const string GIT_COMMIT = "git commit -m \"\"";
        private const string GIT_ADD_ALL = "git add -A";
        private const string GIT_COMMIT_ALL = "git commit -a -m \"\"";
        private const string GIT_PULL = "git pull ";
        private const string GIT_PUSH = "git push ";
        private const string GIT_BRANCH = "git branch ";
        private const string GIT_MERGE = "git merge ";

        private MainVM _mainVM;
    }
}
