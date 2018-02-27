using System;
using System.Windows;
using System.Windows.Controls;

namespace GitBasic.Controls
{
    /// <summary>
    /// Interaction logic for CommandButtonPanel.xaml
    /// </summary>
    public partial class CommandButtonPanel : UserControl
    {
        public CommandButtonPanel()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            Button1Action();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Button2Action();
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            Button3Action();
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            Button4Action();
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            Button5Action();
        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            Button6Action();
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            Button7Action();
        }

        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            Button8Action();
        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            Button9Action();
        }

        private void Button10_Click(object sender, RoutedEventArgs e)
        {
            Button10Action();
        }

        #region Dependency Properties

        public bool ShowButtonNumbers
        {
            get { return (bool)GetValue(ShowButtonNumbersProperty); }
            set { SetValue(ShowButtonNumbersProperty, value); }
        }
        public static readonly DependencyProperty ShowButtonNumbersProperty =
            DependencyProperty.Register("ShowButtonNumbers", typeof(bool), typeof(CommandButtonPanel), new PropertyMetadata(false));

        public string Button1Text
        {
            get { return (string)GetValue(Button1TextProperty); }
            set { SetValue(Button1TextProperty, value); }
        }
        public static readonly DependencyProperty Button1TextProperty =
            DependencyProperty.Register("Button1Text", typeof(string), typeof(CommandButtonPanel), new PropertyMetadata(string.Empty));

        public string Button2Text
        {
            get { return (string)GetValue(Button2TextProperty); }
            set { SetValue(Button2TextProperty, value); }
        }
        public static readonly DependencyProperty Button2TextProperty =
            DependencyProperty.Register("Button2Text", typeof(string), typeof(CommandButtonPanel), new PropertyMetadata(string.Empty));

        public string Button3Text
        {
            get { return (string)GetValue(Button3TextProperty); }
            set { SetValue(Button3TextProperty, value); }
        }
        public static readonly DependencyProperty Button3TextProperty =
            DependencyProperty.Register("Button3Text", typeof(string), typeof(CommandButtonPanel), new PropertyMetadata(string.Empty));

        public string Button4Text
        {
            get { return (string)GetValue(Button4TextProperty); }
            set { SetValue(Button4TextProperty, value); }
        }
        public static readonly DependencyProperty Button4TextProperty =
            DependencyProperty.Register("Button4Text", typeof(string), typeof(CommandButtonPanel), new PropertyMetadata(string.Empty));

        public string Button5Text
        {
            get { return (string)GetValue(Button5TextProperty); }
            set { SetValue(Button5TextProperty, value); }
        }
        public static readonly DependencyProperty Button5TextProperty =
            DependencyProperty.Register("Button5Text", typeof(string), typeof(CommandButtonPanel), new PropertyMetadata(string.Empty));

        public string Button6Text
        {
            get { return (string)GetValue(Button6TextProperty); }
            set { SetValue(Button6TextProperty, value); }
        }
        public static readonly DependencyProperty Button6TextProperty =
            DependencyProperty.Register("Button6Text", typeof(string), typeof(CommandButtonPanel), new PropertyMetadata(string.Empty));

        public string Button7Text
        {
            get { return (string)GetValue(Button7TextProperty); }
            set { SetValue(Button7TextProperty, value); }
        }
        public static readonly DependencyProperty Button7TextProperty =
            DependencyProperty.Register("Button7Text", typeof(string), typeof(CommandButtonPanel), new PropertyMetadata(string.Empty));

        public string Button8Text
        {
            get { return (string)GetValue(Button8TextProperty); }
            set { SetValue(Button8TextProperty, value); }
        }
        public static readonly DependencyProperty Button8TextProperty =
            DependencyProperty.Register("Button8Text", typeof(string), typeof(CommandButtonPanel), new PropertyMetadata(string.Empty));

        public string Button9Text
        {
            get { return (string)GetValue(Button9TextProperty); }
            set { SetValue(Button9TextProperty, value); }
        }
        public static readonly DependencyProperty Button9TextProperty =
            DependencyProperty.Register("Button9Text", typeof(string), typeof(CommandButtonPanel), new PropertyMetadata(string.Empty));

        public string Button10Text
        {
            get { return (string)GetValue(Button10TextProperty); }
            set { SetValue(Button10TextProperty, value); }
        }
        public static readonly DependencyProperty Button10TextProperty =
            DependencyProperty.Register("Button10Text", typeof(string), typeof(CommandButtonPanel), new PropertyMetadata(string.Empty));
        
        public Action Button1Action
        {
            get { return (Action)GetValue(Button1ActionProperty); }
            set { SetValue(Button1ActionProperty, value); }
        }
        public static readonly DependencyProperty Button1ActionProperty =
            DependencyProperty.Register("Button1Action", typeof(Action), typeof(CommandButtonPanel), new PropertyMetadata(new Action(() => { })));

        public Action Button2Action
        {
            get { return (Action)GetValue(Button2ActionProperty); }
            set { SetValue(Button2ActionProperty, value); }
        }
        public static readonly DependencyProperty Button2ActionProperty =
            DependencyProperty.Register("Button2Action", typeof(Action), typeof(CommandButtonPanel), new PropertyMetadata(new Action(() => { })));

        public Action Button3Action
        {
            get { return (Action)GetValue(Button3ActionProperty); }
            set { SetValue(Button3ActionProperty, value); }
        }
        public static readonly DependencyProperty Button3ActionProperty =
            DependencyProperty.Register("Button3Action", typeof(Action), typeof(CommandButtonPanel), new PropertyMetadata(new Action(() => { })));

        public Action Button4Action
        {
            get { return (Action)GetValue(Button4ActionProperty); }
            set { SetValue(Button4ActionProperty, value); }
        }
        public static readonly DependencyProperty Button4ActionProperty =
            DependencyProperty.Register("Button4Action", typeof(Action), typeof(CommandButtonPanel), new PropertyMetadata(new Action(() => { })));

        public Action Button5Action
        {
            get { return (Action)GetValue(Button5ActionProperty); }
            set { SetValue(Button5ActionProperty, value); }
        }
        public static readonly DependencyProperty Button5ActionProperty =
            DependencyProperty.Register("Button5Action", typeof(Action), typeof(CommandButtonPanel), new PropertyMetadata(new Action(() => { })));

        public Action Button6Action
        {
            get { return (Action)GetValue(Button6ActionProperty); }
            set { SetValue(Button6ActionProperty, value); }
        }
        public static readonly DependencyProperty Button6ActionProperty =
            DependencyProperty.Register("Button6Action", typeof(Action), typeof(CommandButtonPanel), new PropertyMetadata(new Action(() => { })));

        public Action Button7Action
        {
            get { return (Action)GetValue(Button7ActionProperty); }
            set { SetValue(Button7ActionProperty, value); }
        }
        public static readonly DependencyProperty Button7ActionProperty =
            DependencyProperty.Register("Button7Action", typeof(Action), typeof(CommandButtonPanel), new PropertyMetadata(new Action(() => { })));

        public Action Button8Action
        {
            get { return (Action)GetValue(Button8ActionProperty); }
            set { SetValue(Button8ActionProperty, value); }
        }
        public static readonly DependencyProperty Button8ActionProperty =
            DependencyProperty.Register("Button8Action", typeof(Action), typeof(CommandButtonPanel), new PropertyMetadata(new Action(() => { })));

        public Action Button9Action
        {
            get { return (Action)GetValue(Button9ActionProperty); }
            set { SetValue(Button9ActionProperty, value); }
        }
        public static readonly DependencyProperty Button9ActionProperty =
            DependencyProperty.Register("Button9Action", typeof(Action), typeof(CommandButtonPanel), new PropertyMetadata(new Action(() => { })));

        public Action Button10Action
        {
            get { return (Action)GetValue(Button10ActionProperty); }
            set { SetValue(Button10ActionProperty, value); }
        }
        public static readonly DependencyProperty Button10ActionProperty =
            DependencyProperty.Register("Button10Action", typeof(Action), typeof(CommandButtonPanel), new PropertyMetadata(new Action(() => { })));

        #endregion
    }
}
