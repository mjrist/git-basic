using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GitBasic.Controls
{
    public class MetroWindow : Window
    {
        public MetroWindow()
        {            
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, this.OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, this.OnMaximizeWindow, this.OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, this.OnMinimizeWindow, this.OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, this.OnRestoreWindow, this.OnCanResizeWindow));            
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        #region DependencyProperties

        public Image TitleBarIcon
        {
            get { return (Image)GetValue(TitleBarIconProperty); }
            set { SetValue(TitleBarIconProperty, value); }
        }
        public static readonly DependencyProperty TitleBarIconProperty =
            DependencyProperty.Register("TitleBarIcon", typeof(Image), typeof(MetroWindow), new PropertyMetadata(default(Image)));

        public bool HasMinimizeButton
        {
            get { return (bool)GetValue(HasMinimizeButtonProperty); }
            set { SetValue(HasMinimizeButtonProperty, value); }
        }
        public static readonly DependencyProperty HasMinimizeButtonProperty =
            DependencyProperty.Register("HasMinimizeButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public bool HasMaximizeButton
        {
            get { return (bool)GetValue(HasMaximizeButtonProperty); }
            set { SetValue(HasMaximizeButtonProperty, value); }
        }
        public static readonly DependencyProperty HasMaximizeButtonProperty =
            DependencyProperty.Register("HasMaximizeButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public bool HasCloseButton
        {
            get { return (bool)GetValue(HasCloseButtonProperty); }
            set { SetValue(HasCloseButtonProperty, value); }
        }
        public static readonly DependencyProperty HasCloseButtonProperty =
            DependencyProperty.Register("HasCloseButton", typeof(bool), typeof(MetroWindow), new PropertyMetadata(true));

        public object TitleBarContent
        {
            get { return (object)GetValue(TitleBarContentProperty); }
            set { SetValue(TitleBarContentProperty, value); }
        }
        public static readonly DependencyProperty TitleBarContentProperty =
            DependencyProperty.Register("TitleBarContent", typeof(object), typeof(MetroWindow), new PropertyMetadata(null));
        
        #endregion
    }
}
