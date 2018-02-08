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
        }

        private void StartCMD()
        {
            // Attempting to access WorkingDirectory from the Task seen below would
            // cause a threading exception. Therefore copy it to a local variable first.
            string workingDirectory = WorkingDirectory;            

            Task.Factory.StartNew(() =>
            {
                _process = new Process();
                _process.StartInfo.FileName = "cmd.exe";
                _process.StartInfo.WorkingDirectory = workingDirectory;
                _process.StartInfo.UseShellExecute = false;
                _process.StartInfo.ErrorDialog = false;
                _process.StartInfo.CreateNoWindow = true;
                _process.StartInfo.RedirectStandardError = true;
                _process.StartInfo.RedirectStandardInput = true;
                _process.StartInfo.RedirectStandardOutput = true;
                _process.EnableRaisingEvents = true;

                _process.ErrorDataReceived += _process_ErrorDataReceived;
                _process.OutputDataReceived += _process_OutputDataReceived;

                _process.Start();
                _process.BeginErrorReadLine();
                _process.BeginOutputReadLine();

                _process.WaitForExit();
            });
        }

        private Process _process;        
        private bool _isInputLine = false;
        private bool _setDirectory = false;

        private void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(() => PrintStandardOutput(e.Data));
        }

        private void _process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(() => PrintStandardError(e.Data));
        }

        private void PrintStandardError(string text)
        {
            Color errorColor = Colors.Red;
            OutputBox.AppendText($"{text}{Environment.NewLine}", errorColor);
            OutputBox.ScrollToEnd();
        }

        private void PrintStandardOutput(string text)
        {
            Color textColor = Colors.White;

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
                string command = text.Split('>')[1].Trim();
                if (command.StartsWith(CD, StringComparison.InvariantCultureIgnoreCase) && command.Length > 2)
                {
                    _setDirectory = true;
                    RunCommand(CD);
                }
                textColor = Colors.LimeGreen;
            }

            OutputBox.AppendText($"{text}{Environment.NewLine}", textColor);
            OutputBox.ScrollToEnd();
        }

        void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string command = InputBox.Text.Trim();

                if (command.ToLower() == "exit")
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
            _process.StandardInput.WriteLine(input);
        }

        /////////////
        // Buttons
        /////////////

        private void SelectRepo_Click(object sender, RoutedEventArgs e)
        {
            //using (var folderDialog = new System.Windows.Forms.FolderBrowserDialog())
            //{
            //    if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    {
            //        EnterText($"{CD} {folderDialog.SelectedPath}");
            //    }
            //}
        }

        private void CommitAll_Click(object sender, RoutedEventArgs e)
        {
            EnterText(COMMIT_ALL, COMMIT_ALL.Length - 1);
        }

        private void Status_Click(object sender, RoutedEventArgs e)
        {
            EnterText(GIT_STATUS);
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

        private const string CD = "cd";
        private const string GIT_STATUS = "git status";
        private const string COMMIT_ALL = "git commit -a -m \"\"";


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
    }
}
