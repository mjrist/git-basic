using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using LibGit2Sharp;

namespace GitBasic.Controls
{
    /// <summary>
    /// Interaction logic for DiffViewer.xaml
    /// </summary>
    public partial class DiffViewer : UserControl
    {
        public DiffViewer()
        {
            InitializeComponent();
            Loaded += DiffViewer_Loaded;
        }

        private void DiffViewer_Loaded(object sender, RoutedEventArgs e)
        {
            Diff();
        }

        private void Diff()
        {
            //Temporary static setting until I work out how to share data between VMs.
            //As well, I'm a bit confused on how we're comparing a staged file to the base branch.

            RepoRoot += "C:\\Users\\reidd\\Documents\\Winter 2018\\Comp 394\\Practicum\\git-basic";
            string relativeFilePath = "Playground\\MainWindow.xaml.cs";

            using(var repo = new Repository(RepoRoot)) 
            {
                var change = repo.Diff.Compare<TreeChanges>(new string[] { relativeFilePath }, true).FirstOrDefault();

                if (change != null)
                {
                    Blob oldBlob = repo.Lookup<Blob>(change.OldOid);
                    string oldText = oldBlob.GetContentText();
                    string newText = File.ReadAllText(Path.Combine(RepoRoot, relativeFilePath));

                    // Have to normalize the line endings because LibGit2Sharp is using '\n' but Windows in '\r\n'.
                    oldText = Regex.Replace(oldText, @"\r\n|\n\r|\n|\r", "\r\n");
                    newText = Regex.Replace(newText, @"\r\n|\n\r|\n|\r", "\r\n");

                    ShowDiff(oldText, newText);
                }
            }
        }

        private void ShowDiff(string oldText, string newText)
        {
            GitSharp.Diff diff = new GitSharp.Diff(oldText, newText);
            
            OldDiff.Document.Blocks.Clear();
            NewDiff.Document.Blocks.Clear();

            //I need to ask Matthew what exactly the sections properties mean in order to get more detailed here.
            foreach (var section in diff.Sections)
            {
                OldDiff.Document.Blocks.Add(new Paragraph(new Run(section.TextA.TrimEnd('\r', '\n'))));
                NewDiff.Document.Blocks.Add(new Paragraph(new Run(section.TextB.TrimEnd('\r', '\n'))));
            }
        }

        private string RepoRoot { get; set; }

        private string _RepoRoot;

        #region Dependency Properties

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public static readonly DependencyProperty FileNameProperty = 
            DependencyProperty.Register("FilePath", typeof(string), typeof(DiffViewer), new PropertyMetadata(string.Empty, OnFileNameChanged));

        private static void OnFileNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            
        }


        #endregion

    }
}
