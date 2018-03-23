using LibGit2Sharp;
using Reactive;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            SetupWidthWatchers();
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

            _oldDiffTextWidth.Value = GetTextWidth(oldText);
            _newDiffTextWidth.Value = GetTextWidth(newText);

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

        private void SetDiffTitles()
        {
            string fileName = System.IO.Path.GetFileName(FileName);
            oldTitle.Text = $"{fileName} - HEAD";
            newTitle.Text = $"{fileName} - MODIFIED";
        }
        private void ClearDiffViewer()
        {
            _oldDiff.Clear();
            _newDiff.Clear();
            _oldDiffTextWidth.Value = 0;
            _newDiffTextWidth.Value = 0;
            oldTitle.Text = string.Empty;
            newTitle.Text = string.Empty;
        }

        private double GetTextWidth(string text)
        {
            FormattedText formattedText = new FormattedText(text,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                12,
                Brushes.WhiteSmoke);

            return formattedText.WidthIncludingTrailingWhitespace + 20;
        }

        private void LeftScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e) => SetMinWidth(OldDiff, e.NewSize.Width - 4, _oldDiffTextWidth.Value);

        private void RightScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e) => SetMinWidth(NewDiff, e.NewSize.Width - 24, _newDiffTextWidth.Value);

        private void SetMinWidth(RichTextBox textBox, double controlWidth, double textWidth)
        {
            textBox.MinWidth = (controlWidth > textWidth) ? controlWidth : textWidth;
        }

        private void SetupWidthWatchers()
        {
            new ReactiveAction(() => SetMinWidth(OldDiff, LeftScrollViewer.ActualWidth - 4, _oldDiffTextWidth.Value), _oldDiffTextWidth);
            new ReactiveAction(() => SetMinWidth(NewDiff, RightScrollViewer.ActualWidth - 24, _newDiffTextWidth.Value), _newDiffTextWidth);
        }

        private Prop<double> _oldDiffTextWidth = new Prop<double>(0);
        private Prop<double> _newDiffTextWidth = new Prop<double>(0);
        private DiffFormatter _oldDiff;
        private DiffFormatter _newDiff;

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender == LeftScrollViewer)
            {
                SyncScrollViewer(RightScrollViewer, e);
            }
            else // RightScrollViewer
            {
                SyncScrollViewer(LeftScrollViewer, e);
            }
        }

        private void SyncScrollViewer(ScrollViewer scrollViewer, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange != 0)
            {
                scrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
            }

            if (e.HorizontalChange != 0)
            {
                scrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
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
            ((DiffViewer)d).Diff((string)e.NewValue);
        }

        #endregion
    }
}

