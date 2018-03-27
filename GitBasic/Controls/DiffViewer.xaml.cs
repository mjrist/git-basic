using LibGit2Sharp;
using Reactive;
using System;
using System.Globalization;
using System.IO;
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
            Loaded += DiffViewer_Loaded;
            _oldDiff = new DiffFormatter(OldDiff);
            _newDiff = new DiffFormatter(NewDiff);
            SetupTextWidthWatchers();
        }

        private void DiffViewer_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshAction = () => { Diff(FileName); };
        }

        private void Diff(string fileName)
        {
            // Future: These File.ReadAllText calls can throw IOExceptions.
            // This code should handle those cases.

            ClearDiffViewer();

            if (string.IsNullOrWhiteSpace(fileName) || Repository == null)
            {
                return;
            }

            var change = Repository.Diff.Compare<TreeChanges>(new string[] { fileName }, true).FirstOrDefault();
            if (change != null)
            {
                if (change.Status == ChangeKind.Deleted)
                {
                    Blob oldBlob = Repository.Lookup<Blob>(change.OldOid);
                    string oldContent = oldBlob.GetContentText();
                    DisplayDeletedFile(oldContent);
                }
                else if (change.Status == ChangeKind.Added)
                {
                    string newContent = File.ReadAllText(fileName);
                    DisplayAddedFile(newContent);
                }
                else
                {
                    Blob oldBlob = Repository.Lookup<Blob>(change.OldOid);
                    string oldContent = oldBlob.GetContentText();
                    string newContent = File.ReadAllText(fileName);

                    // Have to normalize the line endings because LibGit2Sharp is using '\n' but Windows in '\r\n'.                        
                    oldContent = Regex.Replace(oldContent, @"\r\n|\n\r|\n|\r", "\r\n");
                    newContent = Regex.Replace(newContent, @"\r\n|\n\r|\n|\r", "\r\n");
                    DisplayDiff(oldContent, newContent);
                }
            }
        }

        private void DisplayDeletedFile(string oldContent)
        {
            string fileNameWithoutPath = Path.GetFileName(FileName);
            oldTitle.Text = $"{fileNameWithoutPath} - DELETED";
            _oldDiffTextWidth.Value = GetTextWidth(oldContent);
            _oldDiff.AddSection(oldContent);
        }

        private void DisplayAddedFile(string newContent)
        {
            string fileNameWithoutPath = Path.GetFileName(FileName);
            newTitle.Text = $"{fileNameWithoutPath} - NEW";
            _newDiffTextWidth.Value = GetTextWidth(newContent);
            _newDiff.AddSection(newContent);
        }

        private void DisplayDiff(string oldContent, string newContent)
        {
            GitSharp.Diff diff = new GitSharp.Diff(oldContent, newContent);
            SetDiffTitles();
            _oldDiffTextWidth.Value = GetTextWidth(oldContent);
            _newDiffTextWidth.Value = GetTextWidth(newContent);

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
            string fileName = Path.GetFileName(FileName);
            oldTitle.Text = $"{fileName} - HEAD";
            newTitle.Text = $"{fileName} - MODIFIED";
        }

        private void ClearDiffViewer()
        {
            // Clear the diff viewer text.
            _oldDiff.Clear();
            _newDiff.Clear();
            // Reset the text width to 0.
            _oldDiffTextWidth.Value = 0;
            _newDiffTextWidth.Value = 0;
            // Clear the titles.
            oldTitle.Text = string.Empty;
            newTitle.Text = string.Empty;
        }

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

        private double GetTextWidth(string text)
        {
            // Even though the diff viewer font size is 12, I have
            // to use font size 13 here to get the width right.            
            FormattedText formattedText = new FormattedText(text,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Consolas"),
                13, Brushes.WhiteSmoke);

            // Also, I have to add a small margin of 10.
            return formattedText.WidthIncludingTrailingWhitespace + 10;
        }

        private void SetupTextWidthWatchers()
        {
            new ReactiveAction(UpdateOldDiffMinWidth, _oldDiffTextWidth);
            new ReactiveAction(UpdateNewDiffMinWidth, _newDiffTextWidth);
        }

        private void LeftScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateOldDiffMinWidth();

        private void RightScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateNewDiffMinWidth();

        private void UpdateOldDiffMinWidth()
        {
            OldDiff.MinWidth = (_oldDiffVisibleWidth > _oldDiffTextWidth.Value) ? _oldDiffVisibleWidth : _oldDiffTextWidth.Value;
        }

        private void UpdateNewDiffMinWidth()
        {
            NewDiff.MinWidth = (_newDiffVisibleWidth > _newDiffTextWidth.Value) ? _newDiffVisibleWidth : _newDiffTextWidth.Value;
        }

        private Prop<double> _oldDiffTextWidth = new Prop<double>(0);
        private Prop<double> _newDiffTextWidth = new Prop<double>(0);
        private double _oldDiffVisibleWidth => LeftScrollViewer.ActualWidth - DIFF_SIDE_MARGINS;
        private double _newDiffVisibleWidth => RightScrollViewer.ActualWidth - (SCROLLBAR_WIDTH + DIFF_SIDE_MARGINS);
        private const int DIFF_SIDE_MARGINS = 4;
        private const int SCROLLBAR_WIDTH = 20;

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

        public Repository Repository
        {
            get { return (Repository)GetValue(RepositoryProperty); }
            set { SetValue(RepositoryProperty, value); }
        }
        public static readonly DependencyProperty RepositoryProperty =
            DependencyProperty.Register("Repository", typeof(Repository), typeof(DiffViewer), new PropertyMetadata(null));

        public Action RefreshAction
        {
            get { return (Action)GetValue(RefreshActionProperty); }
            set { SetValue(RefreshActionProperty, value); }
        }
        public static readonly DependencyProperty RefreshActionProperty =
            DependencyProperty.Register("RefreshAction", typeof(Action), typeof(DiffViewer), new PropertyMetadata(new Action(() => { })));

        #endregion
    }
}

