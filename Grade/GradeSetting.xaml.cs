using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System;

namespace Grade
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GradeSettingWindow : BaseWindow
    {
        public double G1 { get; set; }
        public double G2 { get; set; }
        public double G3 { get; set; }
        public double G4 { get; set; }
        public double G5 { get; set; }
        public ObservableCollection<KeyBool> GradeClassType { get; set; }
        public GradeSettingWindow(List<KeyBool> gradeClassType, double ng1, double ng2, double ng3, double ng4, double ng5)
        {
            InitializeComponent();
            GradeClassType = new ObservableCollection<KeyBool>(gradeClassType.Select(x => new KeyBool() { Key = x.Key, Value = x.Value }));
            G1 = ng1;
            G2 = ng2;
            G3 = ng3;
            G4 = ng4;
            G5 = ng5;
            this.Loaded += LoginWindow_Loaded;
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            list.ItemsSource = GradeClassType;
            g1.Text = G1.ToString();
            g1.Tag = G1.ToString();
            g1.TextChanged += CheckNumber;
            g2.Text = G2.ToString();
            g2.Tag = G2.ToString();
            g2.TextChanged += CheckNumber;
            g3.Text = G3.ToString();
            g3.Tag = G3.ToString();
            g3.TextChanged += CheckNumber;
            g4.Text = G4.ToString();
            g4.Tag = G4.ToString();
            g4.TextChanged += CheckNumber;
            g5.Text = G5.ToString();
            g5.Tag = G5.ToString();
            g5.TextChanged += CheckNumber;
            RefreshButton.Visibility = Visibility.Collapsed;
        }



        private void LoginClick(object sender, RoutedEventArgs e)
        {
            G1 = double.Parse(g1.Text);
            G2 = double.Parse(g2.Text);
            G3 = double.Parse(g3.Text);
            G4 = double.Parse(g4.Text);
            G5 = double.Parse(g5.Text);
            this.DialogResult = true;
            this.Close();
        }

        private void CheckNumber(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (double.TryParse(tb.Text, out double i))
            {
                tb.Tag = tb.Text;
            }
            else
            {
                tb.TextChanged -= CheckNumber;
                this.Dispatcher.BeginInvoke(new Action(delegate
                {
                    tb.Text = tb.Tag.ToString();
                }));
                tb.TextChanged += CheckNumber;
            }
        }
    }
}
