using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using LibGit2Sharp;


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
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {   
            // New line
            Diff();
        }

        private void Diff()
        {
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
    }
}
