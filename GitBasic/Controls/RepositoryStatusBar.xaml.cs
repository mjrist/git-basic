using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GitBasic.Controls
{
    /// <summary>
    /// Interaction logic for StatusControl.xaml
    /// </summary>
    public partial class RepositoryStatusBar : UserControl
    {
        public RepositoryStatusBar()
        {
            InitializeComponent();
        }

        private void RepoButton_Click(object sender, RoutedEventArgs e)
        {
            RepoButtonAction();
        }

        private void BranchButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            InputHelper.SendRightClick(sender);
        }

        private void BypassRightClick(object sender, MouseButtonEventArgs e) => e.Handled = true;

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string brancName = ((MenuItem)sender).Header.ToString();
            CheckoutAction(brancName);
        }

        #region Dependency Properties

        public string RepositoryName
        {
            get { return (string)GetValue(RepositoryNameProperty); }
            set { SetValue(RepositoryNameProperty, value); }
        }
        public static readonly DependencyProperty RepositoryNameProperty =
            DependencyProperty.Register("RepositoryName", typeof(string), typeof(RepositoryStatusBar), new PropertyMetadata(string.Empty));

        public Action RepoButtonAction
        {
            get { return (Action)GetValue(RepoButtonActionProperty); }
            set { SetValue(RepoButtonActionProperty, value); }
        }
        public static readonly DependencyProperty RepoButtonActionProperty =
            DependencyProperty.Register("RepoButtonAction", typeof(Action), typeof(RepositoryStatusBar), new PropertyMetadata(new Action(() => { })));

        public string BranchName
        {
            get { return (string)GetValue(BranchNameProperty); }
            set { SetValue(BranchNameProperty, value); }
        }
        public static readonly DependencyProperty BranchNameProperty =
            DependencyProperty.Register("BranchName", typeof(string), typeof(RepositoryStatusBar), new PropertyMetadata(string.Empty));

        public ObservableCollection<string> BranchNames
        {
            get { return (ObservableCollection<string>)GetValue(BranchNamesProperty); }
            set { SetValue(BranchNamesProperty, value); }
        }
        public static readonly DependencyProperty BranchNamesProperty =
            DependencyProperty.Register("BranchNames", typeof(ObservableCollection<string>), typeof(RepositoryStatusBar), new PropertyMetadata(new ObservableCollection<string>()));

        public Action<string> CheckoutAction
        {
            get { return (Action<string>)GetValue(CheckoutActionProperty); }
            set { SetValue(CheckoutActionProperty, value); }
        }
        public static readonly DependencyProperty CheckoutActionProperty =
            DependencyProperty.Register("CheckoutAction", typeof(Action<string>), typeof(RepositoryStatusBar), new PropertyMetadata(new Action<string>((branchName) => { })));

        #endregion
    }
}
