using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace GitBasic.Controls
{
    /// <summary>
    /// Interaction logic for ConsoleControl.xaml
    /// </summary>
    public partial class ConsoleControl : UserControl
    {
        public ConsoleControl()
        {
            InitializeComponent();
            Loaded += ConsoleControl_Loaded;
        }

        private void ConsoleControl_Loaded(object sender, RoutedEventArgs e)
        {
            WriteLine("Welcome to Git Basic!");
            WriteLine(string.Empty);

            StartCmdExe();
            InputBox.Focus();
            RegisterHotKeys();
        }

        private void StartCmdExe()
        {
            if (!Directory.Exists(WorkingDirectory))
            {
                WorkingDirectory = _defaultDirectory;
            }

            Task.Factory.StartNew(() =>
            {
                lock (_lockKey)
                {
                    // If this is a restart, the old cmd and child processes get killed first.
                    _cmd?.KillProcessTree();

                    _cmd = new Process();
                    _cmd.StartInfo.FileName = "cmd.exe";
                    _cmd.StartInfo.UseShellExecute = false;
                    _cmd.StartInfo.ErrorDialog = false;
                    _cmd.StartInfo.CreateNoWindow = true;
                    _cmd.EnableRaisingEvents = true;
                    _cmd.StartInfo.RedirectStandardError = true;
                    _cmd.StartInfo.RedirectStandardInput = true;
                    _cmd.StartInfo.RedirectStandardOutput = true;
                    Dispatcher.Invoke(() => { _cmd.StartInfo.WorkingDirectory = WorkingDirectory; });

                    _cmd.Start();
                    Dispatcher.Invoke(ClearInitialOutput);
                }

                _cmd?.WaitForExit();
            });
        }

        private void ClearInitialOutput()
        {
            _cmd.StandardInput.WriteLine(DELIMITER);
            while (!(_cmd.StandardOutput.ReadLine()).EndsWith(DELIMITER)) { }
            _cmd.StandardOutput.DiscardBufferedData();
            while (!(_cmd.StandardError.ReadLine()).StartsWith($"'{DELIMITER}'")) { }
            _cmd.StandardError.DiscardBufferedData();
        }

        private void InputBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessInputCommand();
            }
            else if (e.Key == Key.Up)
            {
                SetInputText(_commandHistory.GetOlderCommand());
            }
            else if (e.Key == Key.Down)
            {
                SetInputText(_commandHistory.GetNewerCommand());
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                ProcessCtrlC();
            }
        }

        private void ProcessInputCommand()
        {
            string command = InputBox.Text;
            _commandHistory.AddCommand(command);

            if (command.Trim().ToLower() == "exit")
            {
                Application.Current.Shutdown();
            }
            else
            {
                RunCommand(command);
                InputBox.Text = string.Empty;
                InputBox.Focus();
            }
        }

        public void RunCommand(string input)
        {
            _cmd.StandardInput.WriteLine(input);
            // A delimiter must be input to determine when to stop
            // reading standard output and standard error.
            _cmd.StandardInput.WriteLine(DELIMITER);

            _backgroundQueue.QueueTask(() =>
            {
                try
                {
                    PrintFirstLine();
                    PrintStandardError();
                    PrintStandardOutput();                    
                    //Dispatcher.Invoke(() => WriteLine());
                }
                catch (InvalidOperationException)
                {
                    // There is no way to cancel pending reads on StandardOutput and StandardError.
                    // When Ctrl+C is pressed cmd.exe is restarted. When this happens, if there is
                    // a pending read or a read is about to be called, an exception will be thrown.
                    // In this case we don't care so we should just swallow it and move on.
                }
            });
        }

        private void PrintFirstLine()
        {
            string line;
            // Skip any initial empty output lines.
            while (string.IsNullOrWhiteSpace(line = _cmd.StandardOutput.ReadLine())) { }

            // First output line is green.
            Dispatcher.Invoke(() => WriteLine(line, Colors.LimeGreen));
        }

        private void PrintStandardOutput()
        {
            string line;            
            while (!(line = _cmd.StandardOutput.ReadLine()).EndsWith(DELIMITER))
            {
                Dispatcher.Invoke(() => WriteLine(line));
            }

            _cmd.StandardOutput.DiscardBufferedData();
            SetWorkingDirectory();
            //Dispatcher.Invoke(() => RemoveLastLineIfEmpty());
        }

        private void SetWorkingDirectory()
        {
            _cmd.StandardInput.WriteLine(CD_COMMAND);
            while (!(_cmd.StandardOutput.ReadLine()).EndsWith(CD_COMMAND)) { }
            string dir = _cmd.StandardOutput.ReadLine();
            Dispatcher.Invoke(() => WorkingDirectory = dir);
            _cmd.StandardOutput.DiscardBufferedData();
        }

        private void RemoveLastLineIfEmpty()
        {
            var inlines = ((Paragraph)OutputBox.Document.Blocks.LastBlock).Inlines;
            if (string.IsNullOrWhiteSpace(((Run)inlines.LastInline).Text))
            {
                inlines.Remove(inlines.LastInline);
            }
        }

        private void PrintStandardError()
        {
            string line = string.Empty;
            while (!(line = _cmd.StandardError.ReadLine()).StartsWith($"'{DELIMITER}'"))
            {
                Dispatcher.Invoke(() => WriteLine(line, Colors.Red));
            }
            _cmd.StandardError.DiscardBufferedData();
        }

        private void WriteLine(string text = "", Color color = default(Color))
        {
            if (color == default(Color))
            {
                color = Colors.White;
            }

            OutputBox.AppendLine(text, color);
            OutputBox.ScrollToEnd();
        }

        private void ProcessCtrlC()
        {
            WriteLine("Control-C pressed.");
            RestartCmdExe();
        }

        private void RestartCmdExe()
        {
            _backgroundQueue = new BackgroundQueue();
            StartCmdExe();
        }

        public void SetInputText(string text, int selectionIndex = -1)
        {
            if (selectionIndex == -1)
            {
                selectionIndex = text.Length;
            }

            InputBox.Text = text;
            InputBox.Select(selectionIndex, 0);
            InputBox.Focus();
        }

        private const string CD_COMMAND = "cd";
        private const string DELIMITER = "\x01";

        private object _lockKey = new object();
        private Process _cmd;
        private CommandHistory _commandHistory = new CommandHistory();
        private BackgroundQueue _backgroundQueue = new BackgroundQueue();
        private string _defaultDirectory => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        #region Dependency Properties

        public string WorkingDirectory
        {
            get { return (string)GetValue(WorkingDirectoryProperty); }
            set { SetValue(WorkingDirectoryProperty, value); }
        }
        public static readonly DependencyProperty WorkingDirectoryProperty =
            DependencyProperty.Register("WorkingDirectory", typeof(string), typeof(ConsoleControl), new PropertyMetadata(string.Empty, OnWorkingDirectoryChanged));

        private static void OnWorkingDirectoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string newWorkingDirectory = e.NewValue.ToString();
            // Save the new working directory. This way it can be restored if the app is restarted.
            Properties.Settings.Default.WorkingDirectory = newWorkingDirectory;
            Properties.Settings.Default.Save();
        }

        #endregion

        #region Command Buttons

        private void RegisterHotKeys()
        {
            if (DataContext is MainVM mainVM)
            {
                mainVM.HotKeyHelper.RegisterHotKey(new HotKey(Key.D1, ModifierKeys.Control, () => { Fetch_Click(null, null); }));
                mainVM.HotKeyHelper.RegisterHotKey(new HotKey(Key.D2, ModifierKeys.Control, () => { CommitAll_Click(null, null); }));
                mainVM.HotKeyHelper.RegisterHotKey(new HotKey(Key.D3, ModifierKeys.Control, () => { Status_Click(null, null); }));
            }
        }

        // TODO: Consider breaking command buttons out into separate control.

        private void Fetch_Click(object sender, RoutedEventArgs e)
        {
            SetInputText(GIT_FETCH);
        }

        private void CommitAll_Click(object sender, RoutedEventArgs e)
        {
            SetInputText(GIT_COMMIT_ALL, GIT_COMMIT_ALL.Length - 1);
        }

        private void Status_Click(object sender, RoutedEventArgs e)
        {
            SetInputText(GIT_STATUS);
        }

        private const string GIT_FETCH = "git fetch";
        private const string GIT_COMMIT_ALL = "git commit -a -m \"\"";
        private const string GIT_STATUS = "git status";

        #endregion
    }
}
