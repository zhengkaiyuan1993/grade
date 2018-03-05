using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Grade
{
    /// <summary>
    /// CheckCode.xaml 的交互逻辑
    /// </summary>
    public partial class CheckCode : BaseWindow
    {
        private string str_user = null;
        private string str_pass = null;
        private HttpVisiter visiter = null;
        public CheckCode(string user, string pass)
        {
            InitializeComponent();
            this.str_user = user;
            this.str_pass = pass;
            this.Loaded += CheckCode_Loaded;

        }

        private void CheckCode_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetWaitingMargin(new Thickness(20, 20, 20, 70));
            this.SetWaitingSize(100,100);
            RefreshButton.Click += RefreshButton_Click;
            ThreadPool.QueueUserWorkItem(LoadCheckCode, null);
            user.Focus();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(LoadCheckCode, null);
            user.Focus();
        }

        private void LoadCheckCode(object state)
        {
            this.StartWaiting();
            visiter = new HttpVisiter(Constant.Url);
            var ms = visiter.GetCheckCode();
            Invoke(() =>
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                image.Source = bi;
            });
            this.StopWaiting();
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(Login, user.Text);
            submit.IsEnabled = false;
        }

        private void Login(object state)
        {
            this.StartWaiting();
            string code = state.ToString();
            bool flag = visiter.Login(str_user, str_pass, code);
            this.StopWaiting();
            if (flag)
            {
                Invoke(() =>
                {
                    SelectWindow s = new SelectWindow(visiter);
                    s.Show();
                    this.Close();
                });
            }
            else
            {
                ThreadPool.QueueUserWorkItem(LoadCheckCode, null);
                Invoke(() =>
                {
                    MessageBox.Show("登录失败!", "警告");
                    user.Focus();
                    submit.IsEnabled = true;
                });
            }
        }
    }
}
