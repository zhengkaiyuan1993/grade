using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Grade
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : BaseWindow
    {
        public SettingWindow()
        {
            InitializeComponent();
            this.Loaded += SettingWindow_Loaded;
        }

        private void SettingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            url.Text = Constant.Url;
            RefreshButton.Click += RefreshButton_Click;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            url.Text = Constant.Url;
        }

        private void SetClick(object sender, RoutedEventArgs e)
        {
            Constant.Url = url.Text;
            this.Close();
        }
    }
}
