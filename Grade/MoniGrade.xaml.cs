using System;
using System.Windows;
using System.Windows.Controls;

namespace Grade
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MoniWindow : BaseWindow
    {
        public string Result { get; set; }
        public MoniWindow(string gra)
        {
            Result = gra;
            InitializeComponent();
            this.Loaded += LoginWindow_Loaded;
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            tb.Text = Result;
            tb.Tag = Result;
            tb.Text = tb.Tag.ToString();
            RefreshButton.Visibility = Visibility.Collapsed;
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            this.Result = tb.Text;
            this.DialogResult = true;
            this.Close();
        }
    }
}
