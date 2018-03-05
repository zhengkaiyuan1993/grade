using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Grade
{
    public class KeyBool
    {
        public string Key { get; set; }
        public bool Value { get; set; }
    }
    public class KeyDouble :INotifyPropertyChanged
    {
        public string key;
        public string Key
        {
            get { return key; }
            set
            {
                key = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Key"));
                }
            }
        }
        public double val;
        public double Value
        {
            get { return val; }
            set
            {
                val = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    public partial class GradeWindow : BaseWindow
    {
        private const string TITLE = "成绩查询-平均分{0:.00}-基点{1:.00}";
        private HttpVisiter visiter;
        private List<KeyDouble> gradeKeyValues = null;
        private List<KeyBool> gradeClassType = null;
        private ObservableCollection<GradeModel> grades = null;
        internal GradeWindow(List<GradeModel> list, string name, HttpVisiter visiter)
        {
            InitializeComponent();
            this.visiter = visiter;
            this.Loaded += EnglisgGradeWindow_Loaded;
            this.Title += name;
            grades = new ObservableCollection<GradeModel>(list);
            data.ItemsSource = grades;
            gradeKeyValues = new List<KeyDouble>();
            gradeKeyValues.Add(new KeyDouble() { Key = "优", Value = 95 });
            gradeKeyValues.Add(new KeyDouble() { Key = "良", Value = 85 });
            gradeKeyValues.Add(new KeyDouble() { Key = "中", Value = 75 });
            gradeKeyValues.Add(new KeyDouble() { Key = "及格", Value = 65 });
            gradeKeyValues.Add(new KeyDouble() { Key = "不及格", Value = 0 });
            gradeClassType = list.Select(x => x.ClassNature).Distinct().Select(y=>new KeyBool()
            {
                Key = y,
                Value = CheckDefaultType(y)
            }).ToList();
            Cal();
        }

        private void Cal()
        {
            double sumCredit = 0;
            double sumGrade = 0;
            double sumPoint = 0;
            foreach (var t in grades)
            {
                if (IsCal(t.ClassNature) && (!t.Cut))
                {
                    var cre = GetGradeOnce(t.Credit);
                    sumCredit += cre;
                    var maxGrade = (Max(GetGradeOnce(t.Grade), GetGradeOnce(t.ExaminationGrade), GetGradeOnce(t.ReworkGrade)));
                    sumGrade += (maxGrade * cre);
                    sumPoint += ((maxGrade / 10.0d) - 5.0d) * cre;
                }
            }
            this.Title = string.Format(TITLE, sumGrade / sumCredit, sumPoint / sumCredit);
        }

        private bool IsCal(string type)
        {
            var val = gradeClassType.FirstOrDefault(x => string.Equals(x.Key, type.Trim()));
            if (val == null)
            {
                return false;
            }
            else
            {
                return val.Value;
            }
        }

        private double GetGradeOnce(string s)
        {
            string str = s.Trim();
            if (double.TryParse(str, out double res))
            {
                return res;
            }
            else
            {
                var val = gradeKeyValues.FirstOrDefault(x => string.Equals(x.Key, str));
                if (val == null)
                {
                    return 0;
                }
                else
                {
                    return val.Value;
                }
            }
        }

        private double Max(double x1, double x2, double x3)
        {
            double res = 0;
            if (x1 > res)
            {
                res = x1;
            }
            if (x2 > res)
            {
                res = x2;
            }
            if (x3 > res)
            {
                res = x3;
            }
            return res;
        }

        private bool CheckDefaultType(string key)
        {
            switch (key)
            {
                case "必修课":
                    return true;
                case "实践性教学":
                    return true;
                case "专业方向课":
                    return true;
                default:
                    return false;
            }
        }

        private void EnglisgGradeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetWaitingSize(200, 200);
            RefreshButton.Click += RefreshButton_Click;
            SettingButton.Visibility = Visibility.Visible;
            SettingButton.Click += SettingButton_Click;
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            GradeSettingWindow set = new GradeSettingWindow(gradeClassType, gradeKeyValues[0].Value, gradeKeyValues[1].Value, gradeKeyValues[2].Value, gradeKeyValues[3].Value, gradeKeyValues[4].Value);
            if (set.ShowDialog() == true)
            {
                gradeKeyValues[0].Value = set.G1;
                gradeKeyValues[1].Value = set.G2;
                gradeKeyValues[2].Value = set.G3;
                gradeKeyValues[3].Value = set.G4;
                gradeKeyValues[4].Value = set.G5;
                gradeClassType = set.GradeClassType.ToList();
                Cal();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(Click3, null);
        }
        private void Click3(object state)
        {
            StartWaiting();
            grades = new ObservableCollection<GradeModel>(visiter.GetGrade());
            StopWaiting();
            Invoke(() =>
            {
                data.ItemsSource = grades;
                Cal();
            });
        }

        private void CutClick(object sender, RoutedEventArgs e)
        {
            GradeModel model = ((FrameworkElement)sender).Tag as GradeModel;
            model.Cut = true;
            Cal();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            GradeModel model = ((FrameworkElement)sender).Tag as GradeModel;
            model.Cut = false;
            Cal();
        }

        private void MoniClick(object sender, RoutedEventArgs e)
        {
            GradeModel model = ((FrameworkElement)sender).Tag as GradeModel;
            MoniWindow moni = new MoniWindow(model.Grade);
            if (moni.ShowDialog() == true)
            {
                model.Grade = moni.Result;
                if (double.TryParse(moni.Result, out double g))
                {
                    model.GradePoint = (g / 10.0d - 5.0d).ToString();
                }
                else
                {
                    var val = gradeKeyValues.FirstOrDefault(x => string.Equals(x.Key, moni.Result));
                    if (val == null)
                    {
                        model.GradePoint = "0";
                    }
                    else
                    {
                        model.GradePoint = (val.Value / 10.0d - 5.0d).ToString();
                    }
                }
                model.ExaminationGrade = "";
                model.ReworkGrade = "";
                Cal();
            }
        }
    }
}
