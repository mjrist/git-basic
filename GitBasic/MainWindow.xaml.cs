using GitBasic.Controls;
using System.Windows;
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
            AddPanelHotkeys(mainVM.CommandButtonVM.HotKeyHelper.Value);
            DataContext = mainVM;            
        }

        private void AddPanelHotkeys(HotKeyHelper hotKeyHelper)
        {
            hotKeyHelper.RegisterHotKey(new HotKey(Key.Up, ModifierKeys.Control, MoveSplitterUp));
            hotKeyHelper.RegisterHotKey(new HotKey(Key.Down, ModifierKeys.Control, MoveSplitterDown));
            hotKeyHelper.RegisterHotKey(new HotKey(Key.Left, ModifierKeys.Control, MoveSplitterLeft));
            hotKeyHelper.RegisterHotKey(new HotKey(Key.Right, ModifierKeys.Control, MoveSplitterRight));
        }

        private void MoveSplitterUp()
        {
            if (BottomRow.ActualHeight == 0)
            {
                TopRow.Height = new GridLength(0.5, GridUnitType.Star);
                BottomRow.Height = new GridLength(0.5, GridUnitType.Star);
            }
            else
            {
                TopRow.Height = new GridLength(0, GridUnitType.Star);
                BottomRow.Height = new GridLength(1, GridUnitType.Star);
            }
        }

        private void MoveSplitterDown()
        {
            if (TopRow.ActualHeight == 0)
            {
                TopRow.Height = new GridLength(0.5, GridUnitType.Star);
                BottomRow.Height = new GridLength(0.5, GridUnitType.Star);
            }
            else
            {
                TopRow.Height = new GridLength(1, GridUnitType.Star);
                BottomRow.Height = new GridLength(0, GridUnitType.Star);
            }
        }

        private void MoveSplitterLeft()
        {
            if (DiffViewer.RightColumn.ActualWidth == 0)
            {
                DiffViewer.LeftColumn.Width = new GridLength(0.5, GridUnitType.Star);
                DiffViewer.RightColumn.Width = new GridLength(0.5, GridUnitType.Star);
            }
            else
            {
                DiffViewer.LeftColumn.Width = new GridLength(0, GridUnitType.Star);
                DiffViewer.RightColumn.Width = new GridLength(1, GridUnitType.Star);
            }
        }

        private void MoveSplitterRight()
        {
            if (DiffViewer.LeftColumn.ActualWidth == 0)
            {
                DiffViewer.LeftColumn.Width = new GridLength(0.5, GridUnitType.Star);
                DiffViewer.RightColumn.Width = new GridLength(0.5, GridUnitType.Star);
            }
            else
            {
                DiffViewer.LeftColumn.Width = new GridLength(1, GridUnitType.Star);
                DiffViewer.RightColumn.Width = new GridLength(0, GridUnitType.Star);
            }
        }

        private void MainWindow_KeyUpOrDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                CommandButtonPanel.ShowButtonNumbers = true;
                PanelHotKeyIndicator.Visibility = Visibility.Visible;
            }
            else
            {
                CommandButtonPanel.ShowButtonNumbers = false;
                PanelHotKeyIndicator.Visibility = Visibility.Collapsed;
            }
        }
    }
}
