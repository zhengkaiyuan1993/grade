using System.Threading;
using System.Windows;

namespace Grade
{
    /// <summary>
    /// SelectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectWindow : BaseWindow
    {
        private HttpVisiter visiter;
        internal SelectWindow(HttpVisiter visiter)
        {
            this.visiter = visiter;
            InitializeComponent();
            this.Loaded += SelectWindow_Loaded;
        }

        private void SelectWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetWaitingSize(150, 150);
            RefreshButton.Visibility = Visibility.Collapsed;
        }

        private void Click0(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(Click0, null);
        }

        private void Click0(object state)
        {
            StartWaiting();
            var input = visiter.SearchExam();
            Invoke(() =>
            {
                StopWaiting();
                if (input == null)
                {
                    MessageBox.Show("获取考试时间信息失败!", "警告");
                }
                else
                {
                    ExamWindow en = new ExamWindow(input, visiter.GetName(), false, visiter);
                    this.Visibility = Visibility.Collapsed;
                    en.ShowDialog();
                    this.Visibility = Visibility.Visible;
                }
            });
        }

        private void Click1(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(Click1, null);
        }

        private void Click1(object state)
        {
            StartWaiting();
            var input = visiter.SearchReExam();
            StopWaiting();
            Invoke(() =>
            {
                if (input == null)
                {
                    MessageBox.Show("获取二考时间信息失败!", "警告");
                }
                else
                {
                    ExamWindow en = new ExamWindow(input, visiter.GetName(), true, visiter);
                    this.Visibility = Visibility.Collapsed;
                    en.ShowDialog();
                    this.Visibility = Visibility.Visible;
                }
            });
        }

        private void Click2(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(Click2, null);
        }

        private void Click2(object state)
        {
            StartWaiting();
            var input = visiter.GetGrade();
            StopWaiting();
            Invoke(() =>
            {
                if (input == null)
                {
                    MessageBox.Show("获取考试成绩信息失败!", "警告");
                }
                else
                {
                    GradeWindow en = new GradeWindow(input, visiter.GetName(), visiter);
                    this.Visibility = Visibility.Collapsed;
                    en.ShowDialog();
                    this.Visibility = Visibility.Visible;
                }
            });
        }

        private void Click3(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(Click3, null);
        }

        private void Click3(object state)
        {
            StartWaiting();
            var input = visiter.GetEndlishGrade();
            StopWaiting();
            Invoke(() =>
            {
                if (input == null)
                {
                    MessageBox.Show("获取等级考试成绩信息失败!", "警告");
                }
                else
                {
                    EnglisgGradeWindow en = new EnglisgGradeWindow(input, visiter.GetName(), visiter);
                    this.Visibility = Visibility.Collapsed;
                    en.ShowDialog();
                    this.Visibility = Visibility.Visible;
                }
            });
        }
    }
}
