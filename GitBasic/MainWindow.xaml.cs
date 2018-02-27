using GitBasic.Controls;
using System.Windows.Input;

namespace GitBasic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {            
            InitializeComponent();
            CreateViewModel();
        }

        public void CreateViewModel()
        {
            MainVM mainVM = new MainVM();
            mainVM.CommandButtonVM.HotKeyHelper.Value = new HotKeyHelper(this);
            DataContext = mainVM;
        }

        private void MainWindow_KeyUpOrDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                CommandButtonPanel.ShowButtonNumbers = true;
            }
            else
            {
                CommandButtonPanel.ShowButtonNumbers = false;
            }
        }        
    }    
}
