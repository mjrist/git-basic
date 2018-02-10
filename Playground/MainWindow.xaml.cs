using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
//using LibGit2Sharp;
using GitSharp;
using GitSharp.Commands;

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
            //TestLibGit2Sharp();
            TestGitSharp();
        }

        //private void TestLibGit2Sharp()
        //{
        //    using (var repo = new Repository("C:\\source\\git-basic"))
        //    {
        //        var repoDifferences = repo.Diff.Compare<Patch>(new string[] { "Playground\\MainWindow.xaml.cs" }, true);
        //        DiffViewer.Text = repoDifferences.Content;
        //    }
        //}

        private void TestGitSharp()
        {
            Diff diff = new Diff(Properties.Settings.Default.OldString, Properties.Settings.Default.NewString);
            foreach (var section in diff.Sections)
            {

            }
        }
    }
}
