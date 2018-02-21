using GitBasic.Controls;

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
            mainVM.HotKeyHelper = new HotKeyHelper(this);
            DataContext = mainVM;
        }
    }    
}
