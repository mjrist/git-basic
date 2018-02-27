using LibGit2Sharp;
using Playground.Lib;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;


namespace Playground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var itemProvider = new ItemProvider();

            var items = itemProvider.GetItems("C:\\Users\\shaama\\Desktop\\Test Directory");

            DataContext = items;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {             
            //Diff();

            // Comment out Diff() above and uncomment this for file status example.
            GetFileStatus();
        }

        private void Diff()
        {
            // Make a small change to this file, save (don't commit), and you will see side-by-side diff.

            // Modify this to point to your repo root dir.
            string repoRoot = "C:\\source\\git-basic";
            string relativeFilePath = "Playground\\MainWindow.xaml.cs";

            using (var repo = new Repository(repoRoot))
            {
                var change = repo.Diff.Compare<TreeChanges>(new string[] { relativeFilePath }, true).FirstOrDefault();

                if (change != null)
                {
                    Blob oldBlob = repo.Lookup<Blob>(change.OldOid);
                    string oldText = oldBlob.GetContentText();
                    string newText = File.ReadAllText(Path.Combine(repoRoot, relativeFilePath));

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
            foreach (var section in diff.Sections)
            {
                // This is a quick and easy way to get the sections of the diff to line up.
                // We don't really want to do it this way because you can't select the text.
                // Use the properties on section (BeginA, BeginB, EditWithRespectToA, ...)
                // in order to insert the appropriate whitespace. These properties will also
                // help you determine highlighting. It would be nice to have line numbers
                // as well if possible. The closer it looks to visual studio's built-in
                // diff viewer, the better.

                TextBlock oldTextBlock = new TextBlock();
                Grid.SetColumn(oldTextBlock, 0);
                oldTextBlock.Text = section.TextA.TrimEnd('\r', '\n');

                TextBlock newTextBlock = new TextBlock();
                Grid.SetColumn(newTextBlock, 1);
                newTextBlock.Text = section.TextB.TrimEnd('\r', '\n');

                Grid newSection = new Grid();
                ColumnDefinition oldColumn = new ColumnDefinition();
                ColumnDefinition newColumn = new ColumnDefinition();
                newSection.ColumnDefinitions.Add(oldColumn);
                newSection.ColumnDefinitions.Add(newColumn);
                newSection.Children.Add(oldTextBlock);
                newSection.Children.Add(newTextBlock);

                LayoutRoot.Children.Add(newSection);
            }
        }
        
        private void GetFileStatus()
        {
            // Didn't spend a lot of time on this yet. Still some to be explored here.

            // Modify this to point to your repo root dir.
            string repoRoot = "C:\\Users\\shaama\\Desktop\\Test Directory";            

            using (var repo = new Repository(repoRoot))
            {
                // Haven't tested these, but this should stage/unstage a file.

                //Commands.Stage(repo, "pathtofile");
                //Commands.Unstage(repo, "pathtofile");

                foreach (var item in repo.RetrieveStatus())
                {
                    // You'll have to figure out how to tell what is staged/unstaged/untracked
                    if (item.State != FileStatus.Unaltered)
                    {
                        TextBlock filePathTextBlock = new TextBlock();
                        filePathTextBlock.Text = item.FilePath;
                        
                        LayoutRoot.Children.Add(filePathTextBlock);
                    }                    
                }                
            }
        }
    }
}
