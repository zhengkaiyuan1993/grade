using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace Grade
{
    public class EnglisgGradeModel
    {
        public string Year { get; set; }
        /// <summary>
        /// 学期
        /// </summary>
        public string Semester { get; set; }
        public string Name { get; set; }
        public string ID { get; set; }
        public string Date { get; set; }
        public string Grade { get; set; }
        public string ListenGrade { get; set; }
        public string ReadGrade { get; set; }
        public string WriteGrade { get; set; }
        public string AllGrade { get; set; }
    }
    /// <summary>
    /// EnglisgGradeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EnglisgGradeWindow : BaseWindow
    {
        private HttpVisiter visiter;
        internal EnglisgGradeWindow(List<EnglisgGradeModel> list, string name, HttpVisiter visiter)
        {
            InitializeComponent();
            this.visiter = visiter;
            this.Loaded += EnglisgGradeWindow_Loaded;
            this.Title += name;
            data.ItemsSource = list;
        }

        private void EnglisgGradeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetWaitingSize(200, 200);
            RefreshButton.Click += RefreshButton_Click;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
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
                data.ItemsSource = input;
            });
        }
    }
}
