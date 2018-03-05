using System.Windows;

namespace Grade
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : BaseWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
            this.Loaded += LoginWindow_Loaded;
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            user.Focus();
            RefreshButton.Click += RefreshButton_Click;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            user.Text = "";
            pass.Password = "";
            user.Focus();
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(user.Text))
            {
                MessageBox.Show("用户名不能为空!", "警告");
                return;
            }
            if (string.IsNullOrWhiteSpace(pass.Password))
            {
                MessageBox.Show("密码不能为空!", "警告");
                return;
            }
            CheckCode code = new CheckCode(user.Text, pass.Password);
            code.Show();
            this.Close();
        }

        private void SetClick(object sender, RoutedEventArgs e)
        {
            SettingWindow sw = new SettingWindow();
            this.Visibility = Visibility.Collapsed;
            sw.ShowDialog();
            this.Visibility = Visibility.Visible;
        }
    }
}
