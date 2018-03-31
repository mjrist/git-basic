using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
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
            Execute = RunCommand;
            SetInput = SetInputText;
            InputBox.Focus();
            StartWatchingSelectionChange();
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
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.Tab)
            {
                AutoComplete(Selection.Previous);
                e.Handled = true;
            }
            else if (e.Key == Key.Tab)
            {
                AutoComplete(Selection.Next);
                e.Handled = true;
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
                    PrintStandardOutput();
                    PrintStandardError();
                    Dispatcher.Invoke(() => WriteLine());
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is NullReferenceException)
                {
                    // There is no way to cancel pending reads on StandardOutput and StandardError.
                    // When Ctrl+C is pressed cmd.exe is restarted. When this happens, if there is
                    // a pending read or a read is about to be called, an exception will be thrown.
                    // In this case we don't care so we should just swallow it and move on.
                }
            });
        }

        private void PrintStandardOutput()
        {
            string line;
            // Skip any initial empty output lines.
            while (string.IsNullOrWhiteSpace(line = _cmd.StandardOutput.ReadLine())) { }

            // First output line is green.
            Dispatcher.Invoke(() => WriteLine(line, Colors.LimeGreen));
            while (!(line = _cmd.StandardOutput.ReadLine()).EndsWith(DELIMITER))
            {
                Dispatcher.Invoke(() => WriteLine(line));
            }

            _cmd.StandardOutput.DiscardBufferedData();
            SetWorkingDirectory();
            Dispatcher.Invoke(() => RemoveLastLineIfEmpty());
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
                Dispatcher.Invoke(() => WriteLine(line));
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

        private void StartWatchingSelectionChange()
        {
            InputBox.SelectionChanged += InputBox_SelectionChanged;
        }

        private void StopWatchingSelectionChange()
        {
            InputBox.SelectionChanged -= InputBox_SelectionChanged;
        }

        private void InputBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SetCurrentToken();
        }

        private void SetCurrentToken()
        {
            _token.Reset();

            int i = InputBox.CaretIndex - 1;
            while (i > -1 && !char.IsWhiteSpace(InputBox.Text[i]))
            {
                char nextChar = InputBox.Text[i];
                if (nextChar == '"' && TryGetQuotedToken(i))
                {
                    return;
                }
                _token.Text = nextChar + _token.Text;
                i--;
            }
            _token.StartIndex = i + 1;
        }

        private bool TryGetQuotedToken(int currentIndex)
        {
            int firstQuoteIndex = InputBox.Text.Substring(0, currentIndex).LastIndexOf('"');
            if (firstQuoteIndex > -1)
            {
                _token.Text = InputBox.Text.Substring(firstQuoteIndex + 1, currentIndex - (firstQuoteIndex + 1)) + _token.Text;
                _token.StartIndex = firstQuoteIndex;
                return true;
            }
            return false;
        }

        private void AutoComplete(Selection selection)
        {
            string completionText = (selection == Selection.Next) ?
                _autoCompletion.GetNext(_token.Text, WorkingDirectory) :
                _autoCompletion.GetPrevious(_token.Text, WorkingDirectory);

            if (string.IsNullOrEmpty(completionText))
            {
                return;
            }

            StopWatchingSelectionChange();
            int lengthToRemove = InputBox.CaretIndex - _token.StartIndex;
            InputBox.Text = InputBox.Text.Remove(_token.StartIndex, lengthToRemove);
            InputBox.Text = InputBox.Text.Insert(_token.StartIndex, completionText);
            InputBox.CaretIndex = _token.StartIndex + completionText.Length;
            StartWatchingSelectionChange();
        }

        private void ConsoleControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            InputBox.Focus();

            if (sender == OutputBox)
            {
                CopyOutputBoxSelection();
            }
            else if (sender == CurrentDirectoryIndicator)
            {
                CopyCurrentDirectoryIndicatorSelection();
            }
        }

        private void CopyOutputBoxSelection()
        {
            if (!OutputBox.Selection.IsEmpty)
            {
                Clipboard.SetText(OutputBox.Selection.Text);
            }
            CurrentDirectoryIndicator.Select(0, 0);
        }

        private void CopyCurrentDirectoryIndicatorSelection()
        {
            if (!string.IsNullOrEmpty(CurrentDirectoryIndicator.SelectedText))
            {
                Clipboard.SetText(CurrentDirectoryIndicator.SelectedText);
            }
            OutputBox.DeselectAll();
        }

        private void ConsoleControl_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            PasteClipboardText();
            InputBox.Focus();
        }

        private void PasteClipboardText()
        {
            InputBox.SelectedText = Regex.Replace(Clipboard.GetText(), @"\r\n|\n\r|\n|\r", " ");
            InputBox.Select(InputBox.SelectionStart + InputBox.SelectionLength, 0);
        }

        private enum Selection { Next, Previous };

        private const string CD_COMMAND = "cd";
        private const string DELIMITER = "\x01";

        private object _lockKey = new object();
        private Process _cmd;
        private CommandHistory _commandHistory = new CommandHistory();
        private Token _token = new Token();
        private AutoCompletion _autoCompletion = new AutoCompletion();
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

        public Action<string, int> SetInput
        {
            get { return (Action<string, int>)GetValue(SetInputProperty); }
            set { SetValue(SetInputProperty, value); }
        }
        public static readonly DependencyProperty SetInputProperty =
            DependencyProperty.Register("SetInput", typeof(Action<string, int>), typeof(ConsoleControl), new PropertyMetadata(new Action<string, int>((input, caretIndex) => { })));

        public Action<string> Execute
        {
            get { return (Action<string>)GetValue(ExecuteProperty); }
            set { SetValue(ExecuteProperty, value); }
        }
        public static readonly DependencyProperty ExecuteProperty =
            DependencyProperty.Register("Execute", typeof(Action<string>), typeof(ConsoleControl), new PropertyMetadata(new Action<string>((input) => { })));

        #endregion
    }
}
