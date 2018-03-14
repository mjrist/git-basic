using LibGit2Sharp;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

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
            _oldDiff = new DiffFormatter(OldDiff);
            _newDiff = new DiffFormatter(NewDiff);
        }

        private void Diff(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                ClearDiffViewer();
                return;
            }

            string repoPath = Repository.Discover(fileName);

            if (string.IsNullOrEmpty(repoPath))
            {
                return;
            }

            using (var repo = new Repository(repoPath))
            {

                var change = repo.Diff.Compare<TreeChanges>(new string[] { fileName }, true).FirstOrDefault();

                if (change != null)
                {
                    Blob oldBlob = repo.Lookup<Blob>(change.OldOid);
                    //Blob newBlob = repo.Lookup<Blob>(change.Oid);
                    string localFile = oldBlob.GetContentText();
                    //string newText = newBlob.GetContentText();
                    string headFile = System.IO.File.ReadAllText(fileName);

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

            ClearDiffViewer();
            SetDiffTitles();

            foreach (var section in diff.Sections)
            {
                if (section.Status == GitSharp.Diff.SectionStatus.Unchanged)
                {
                    _oldDiff.AddSection(section.TextA);
                    _newDiff.AddSection(section.TextB);
                }
                else if (section.EditWithRespectToA == GitSharp.Diff.EditType.Replaced
                    || section.EditWithRespectToB == GitSharp.Diff.EditType.Replaced)
                {
                    int sectionASize = section.EndA - section.BeginA;
                    int sectionBSize = section.EndB - section.BeginB;

                    _oldDiff.AddSection(section.TextA, DiffSectionType.Removed);
                    _newDiff.AddSection(section.TextB, DiffSectionType.Added);

                    if (sectionASize < sectionBSize)
                    {
                        int lineCount = sectionBSize - sectionASize;
                        _oldDiff.AddPadding(lineCount);
                    }
                    else if (sectionBSize < sectionASize)
                    {
                        int lineCount = sectionASize - sectionBSize;
                        _newDiff.AddPadding(lineCount);
                    }
                }
                else if (section.EditWithRespectToA == GitSharp.Diff.EditType.Inserted)
                {
                    int lineCount = section.EndB - section.BeginB;
                    _oldDiff.AddPadding(lineCount);
                    _newDiff.AddSection(section.TextB, DiffSectionType.Added);
                }
                else if (section.EditWithRespectToB == GitSharp.Diff.EditType.Inserted)
                {
                    int lineCount = section.EndA - section.BeginA;
                    _newDiff.AddPadding(lineCount);
                    _oldDiff.AddSection(section.TextA, DiffSectionType.Removed);
                }
            }
        }

        private void ClearDiffViewer()
        {
            _oldDiff.Clear();
            _newDiff.Clear();
            oldTitle.Text = string.Empty;
            newTitle.Text = string.Empty;
        }

        private void SetDiffTitles()
        {
            string fileName = System.IO.Path.GetFileName(FileName);
            oldTitle.Text = $"{fileName} - HEAD";
            newTitle.Text = $"{fileName} - MODIFIED";
        }

        private DiffFormatter _oldDiff;
        private DiffFormatter _newDiff;

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
            ((DiffViewer)d).Diff((string)e.NewValue);
        }

        #endregion
    }
}

