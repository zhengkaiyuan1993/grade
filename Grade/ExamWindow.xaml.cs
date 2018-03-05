using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    public class ExamModel
    {
        public string Number { get; set; }
        public string ClassName { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Area { get; set; }
        public string Type { get; set; }
        public string Site { get; set; }
        public string School { get; set; }
    }
    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class ExamInfo
    {
        public string ViewState { get; set; }
        public List<KeyValue> Year { get; set; }
        public List<KeyValue> Semester { get; set; }
    }
    /// <summary>
    /// ExamWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExamWindow : BaseWindow
    {
        private ExamInfo info;
        private HttpVisiter visiter;
        private bool re;
        internal ExamWindow(ExamInfo info, string name, bool re, HttpVisiter visiter)
        {
            InitializeComponent();
            this.Loaded += ExamWindow_Loaded;
            this.info = info;
            this.visiter = visiter;
            this.re = re;
            yer.ItemsSource = info.Year;
            yer.SelectedValue = info.Year[0].Key;
            sem.ItemsSource = info.Semester;
            sem.SelectedValue = info.Semester[0].Key;
            yer.SelectionChanged += SelectionChanged;
            sem.SelectionChanged += SelectionChanged;
            if (re)
            {
                this.Title = "二考时间查询-" + name;
            }
            else
            {
                this.Title = "考试时间查询-" + name;
            }
        }

        private void ExamWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetWaitingSize(200, 200);
            RefreshButton.Click += RefreshButton_Click;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            ThreadPool.QueueUserWorkItem(RefreshContext, new Tuple<string, string>(yer.SelectedValue.ToString(), sem.SelectedValue.ToString()));
        }

        private void RefreshContext(object state)
        {
            Tuple<string, string> tuple = (Tuple<string, string>)state;
            StartWaiting();
            IEnumerable ie = null;
            if (re)
            {
                ie = visiter.SearchReExamDetail(info.ViewState, tuple.Item1, tuple.Item2);
            }
            else
            {
                ie = visiter.SearchExamDetail(info.ViewState, tuple.Item1, tuple.Item2);
            }
            StopWaiting();
            Invoke(() => 
            {
                data.ItemsSource = ie;
            });
        }
    }
}
