using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibGit2Sharp;

namespace GitBasic.Controls
{
    /// <summary>
    /// Interaction logic for FileStatusControl.xaml
    /// </summary>
    public partial class FileStatusControl : UserControl
    {
        public FileStatusControl()
        {
            InitializeComponent();
            Loaded += FileStatusControl_Loaded;
        }

        private void FileStatusControl_Loaded(object sender, RoutedEventArgs e)
        {
            StartTimer();

            // File watcher alternative.
            // An event based aproach like the file watcher is technically better since it doesn't involve constant polling.
            // However, I'm not sure the file watcher watches sub directories and I've had problems with it in the past.
            // You can give it a go if you want, but for what we have to deliver, I'm okay with the timer approach.
            //StartFileWatcher();
        }       

        private void StartTimer()
        {
            double twoSeconds = 2000;
            Timer timer = new Timer(twoSeconds);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Because we are dispatching back to the main UI thread, we don't need to worry about the concurrency problem I was talking about.
            Dispatcher.Invoke(() =>
            {
                // The file status control really shouldn't know that its DataContext is a MainVM.
                // This code should be moved out into a view model. Its okay to start here though.
                if (DataContext is MainVM mainVM && mainVM.Repo.Value != null)
                {
                    StagedListBox.Items.Clear();

                    foreach (var item in mainVM.Repo.Value.RetrieveStatus())
                    {
                        // You'll have to figure out how to tell what is staged/unstaged/untracked
                        // Look at FileStatus.ModifiedInIndex versus FileStatus.ModifiedInWorkdir
                        if (item.State != FileStatus.Unaltered)
                        {
                            TextBlock textBlock = new TextBlock();
                            textBlock.Text = item.FilePath;
                            StagedListBox.Items.Add(textBlock);
                        }
                    }
                }
            });
        }

        
        //private void StartFileWatcher()
        //{
        //    // Does this watch subdirs? I don't think so. Polling is probably easier.
        //    FileSystemWatcher fileSystemWatcher = new FileSystemWatcher("the repository directory");
        //    fileSystemWatcher.Changed += FileSystemWatcher_Changed;
        //    fileSystemWatcher.BeginInit();
        //}

        //private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        //{
        //    // Run code here to check repository status.           
        //}


        // This would get set when the user double clicks a file in either of the listboxes.
        public string SelectedFile
        {
            get { return (string)GetValue(SelectedFileProperty); }
            set { SetValue(SelectedFileProperty, value); }
        }
        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register("SelectedFile", typeof(string), typeof(FileStatusControl), new PropertyMetadata(string.Empty));
    }
}
