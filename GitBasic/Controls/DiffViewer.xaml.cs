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
        }

        private void Diff(string fileName)
        {
            string repoPath = Repository.Discover(fileName);

            if (string.IsNullOrEmpty(repoPath)) 
            {
                return;
            }

            using(var repo = new Repository(repoPath)) 
            {
                //GitSharp's new file oid doesn't work so this is a workaround.

                var change = repo.Diff.Compare<TreeChanges>(new string[] { fileName }, true).FirstOrDefault();

                if (change != null)
                {
                    Blob oldBlob = repo.Lookup<Blob>(change.OldOid);
                    //Blob newBlob = repo.Lookup<Blob>(change.Oid);
                    string localFile = oldBlob.GetContentText();
                    //string newText = newBlob.GetContentText();
                    string headFile = File.ReadAllText(fileName);

                    // Have to normalize the line endings because LibGit2Sharp is using '\n' but Windows in '\r\n'.
                    localFile = Regex.Replace(localFile, @"\r\n|\n\r|\n|\r", "\r\n");
                    headFile = Regex.Replace(headFile, @"\r\n|\n\r|\n|\r", "\r\n");

                    ShowDiff(localFile, headFile);
                }
            }
        }

        private void ShowDiff(string oldText, string newText)
        {
            GitSharp.Diff diff = new GitSharp.Diff(oldText, newText);
            
            OldDiff.Document.Blocks.Clear();
            NewDiff.Document.Blocks.Clear();

            //File name at top of document
            string fileName = Path.GetFileName(FileName);
            oldTitle.Text = $"{fileName} - HEAD";
            newTitle.Text = $"{fileName} - MODIFIED";

            Color changed = Color.FromArgb(100,255,255,200);

            foreach (var section in diff.Sections)
            {
                OldDiff.Document.Blocks.Add(new Paragraph(new Run(section.TextA.TrimEnd('\r', '\n'))));
                NewDiff.Document.Blocks.Add(new Paragraph(new Run(section.TextB.TrimEnd('\r', '\n'))));
            }
        }

        #region Dependency Properties

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public static readonly DependencyProperty FileNameProperty = 
            DependencyProperty.Register("FileName", typeof(string), typeof(DiffViewer), new PropertyMetadata(string.Empty, OnFileNameChanged));

        private static void OnFileNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            ((DiffViewer) d).Diff((string) e.NewValue);
        }

        #endregion

    }
}
