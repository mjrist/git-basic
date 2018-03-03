using System;
using System.Windows;
using System.Windows.Controls;

namespace GitBasic.Controls
{
    /// <summary>
    /// Interaction logic for StatusControl.xaml
    /// </summary>
    public partial class StatusControl : UserControl
    {
        public StatusControl()
        {
            InitializeComponent();
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            LeftButtonAction();
        }

        #region Dependency Properties

        public string RepositoryName
        {
            get { return (string)GetValue(RepositoryNameProperty); }
            set { SetValue(RepositoryNameProperty, value); }
        }
        public static readonly DependencyProperty RepositoryNameProperty =
            DependencyProperty.Register("RepositoryName", typeof(string), typeof(StatusControl), new PropertyMetadata(string.Empty));

        public string BranchName
        {
            get { return (string)GetValue(BranchNameProperty); }
            set { SetValue(BranchNameProperty, value); }
        }
        public static readonly DependencyProperty BranchNameProperty =
            DependencyProperty.Register("BranchName", typeof(string), typeof(StatusControl), new PropertyMetadata(string.Empty));

        public Action LeftButtonAction
        {
            get { return (Action)GetValue(LeftButtonActionProperty); }
            set { SetValue(LeftButtonActionProperty, value); }
        }
        public static readonly DependencyProperty LeftButtonActionProperty =
            DependencyProperty.Register("LeftButtonAction", typeof(Action), typeof(StatusControl), new PropertyMetadata(new Action(() => { })));

        #endregion        
    }
}
