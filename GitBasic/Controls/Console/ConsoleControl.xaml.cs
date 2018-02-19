using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            if (!Directory.Exists(WorkingDirectory))
            {
                WorkingDirectory = _defaultDirectory;
            }

            StartCMD();
            InputBox.Focus();
            RegisterHotKeys();            
        }

        private void StartCMD()
        {
            // Attempting to access WorkingDirectory from the Task seen below would
            // cause a threading exception. Therefore copy it to a local variable first.
            string workingDirectory = WorkingDirectory;            

            Task.Factory.StartNew(() =>
            {
                _cmd = new Process();
                _cmd.StartInfo.FileName = "cmd.exe";
                _cmd.StartInfo.WorkingDirectory = workingDirectory;
                _cmd.StartInfo.UseShellExecute = false;
                _cmd.StartInfo.ErrorDialog = false;
                _cmd.StartInfo.CreateNoWindow = true;
                _cmd.StartInfo.RedirectStandardError = true;
                _cmd.StartInfo.RedirectStandardInput = true;
                _cmd.StartInfo.RedirectStandardOutput = true;
                _cmd.EnableRaisingEvents = true;

                _cmd.ErrorDataReceived += (s, e) => { Dispatcher.Invoke(() => PrintStandardError(e.Data)); };
                _cmd.OutputDataReceived += (s, e) => { Dispatcher.Invoke(() => PrintStandardOutput(e.Data)); };

                _cmd.Start();
                _cmd.BeginErrorReadLine();
                _cmd.BeginOutputReadLine();

                _cmd.WaitForExit();
            });
        }

        

        private Process _cmd;

        private void PrintStandardError(string text)
        {
            Color errorColor = Colors.Red;
            OutputBox.AppendText($"{text}{Environment.NewLine}", errorColor);
            OutputBox.ScrollToEnd();
        }

        private void PrintStandardOutput(string text)
        {
            Color textColor = Colors.White;
            
            // TODO: This code is a hack to get the working directory to update.
            // Move it into its own function.
            if (_setDirectory)
            {
                if (text != string.Empty)
                {
                    _setDirectory = false;
                    WorkingDirectory = text.Split('>')[0];
                }
                return;
            }

            if (_isInputLine)
            {
                _isInputLine = false;

                string[] tokens = text.Split('>');
                if (tokens.Length > 1)
                {
                    string command = tokens[1].Trim();
                    if (command.StartsWith(CD, StringComparison.InvariantCultureIgnoreCase) && command.Length > 2)
                    {
                        _setDirectory = true;
                        RunCommand(CD);
                    }
                }

                textColor = Colors.LimeGreen;
            }

            OutputBox.AppendText($"{text}{Environment.NewLine}", textColor);
            OutputBox.ScrollToEnd();
        }

        private bool _isInputLine = false;
        private bool _setDirectory = false;

        void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string command = InputBox.Text;
                _commandHistory.AddCommand(command);

                if (command.Trim().ToLower() == "exit")
                {
                    Application.Current.Shutdown();
                }
                else
                {
                    _isInputLine = true;
                    RunCommand(command);
                    InputBox.Text = string.Empty;
                    InputBox.Focus();
                }
            }            
        }

        public void RunCommand(string input)
        {
            _cmd.StandardInput.WriteLine(input);
        }

        private void EnterText(string text)
        {
            EnterText(text, text.Length);
        }

        private void EnterText(string text, int selectionIndex)
        {
            InputBox.Text = text;
            InputBox.Select(selectionIndex, 0);
            InputBox.Focus();
        }

        private void InputBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                EnterText(_commandHistory.GetOlderCommand());
            }
            else if (e.Key == Key.Down)
            {
                EnterText(_commandHistory.GetNewerCommand());
            }
        }

        private CommandHistory _commandHistory = new CommandHistory();
        private const string CD = "cd";       
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
            EnterText(GIT_FETCH);
        }

        private void CommitAll_Click(object sender, RoutedEventArgs e)
        {
            EnterText(GIT_COMMIT_ALL, GIT_COMMIT_ALL.Length - 1);
        }

        private void Status_Click(object sender, RoutedEventArgs e)
        {
            EnterText(GIT_STATUS);
        }

        private const string GIT_FETCH = "git fetch";
        private const string GIT_COMMIT_ALL = "git commit -a -m \"\"";
        private const string GIT_STATUS = "git status";        

        #endregion
    }
}
