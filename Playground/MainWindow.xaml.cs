using System.IO;
using System.Linq;
using System.Windows;
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

                    ShowDiff(oldText, newText);
                }


                //var repoDifferences = repo.Diff.Compare<Patch>(new string[] { "Playground\\MainWindow.xaml.cs" }, true);
                //DiffViewer.Text = repoDifferences.Content;
            }
        }

        private void ShowDiff(string oldText, string newText)
        {
            GitSharp.Diff diff = new GitSharp.Diff(oldText, newText);
            foreach (var section in diff.Sections)
            {
                DiffViewer.Text = section.TextA;
            }
        }
    }
}
