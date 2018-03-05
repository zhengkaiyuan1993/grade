using System.ComponentModel;

namespace Grade
{
    internal class GradeModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 学年
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// 学期
        /// </summary>
        public string Semester { get; set; }
        /// <summary>
        /// 课程代码
        /// </summary>
        public string ClassCode { get; set; }
        /// <summary>
        /// 课程名称
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 课程性质
        /// </summary>
        public string ClassNature { get; set; }
        /// <summary>
        /// 课程归属
        /// </summary>
        public string ClassOwnership { get; set; }
        /// <summary>
        /// 学分
        /// </summary>
        public string Credit { get; set; }

        private string gradePoint;
        /// <summary>
        /// 绩点
        /// </summary>
        public string GradePoint
        {
            get { return gradePoint; }
            set
            {
                gradePoint = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("GradePoint"));
                }
            }
        }
        private string grade;
        /// <summary>
        /// 成绩
        /// </summary>
        public string Grade
        {
            get { return grade; }
            set
            {
                grade = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Grade"));
                }
            }
        }
        /// <summary>
        /// 辅修标记
        /// </summary>
        public string MinorFlag { get; set; }
        private string examinationGrade;
        /// <summary>
        /// 补考成绩
        /// </summary>
        public string ExaminationGrade
        {
            get { return examinationGrade; }
            set
            {
                examinationGrade = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ExaminationGrade"));
                }
            }
        }
        private string reworkGrade;
        /// <summary>
        /// 重修成绩
        /// </summary>
        public string ReworkGrade
        {
            get { return reworkGrade; }
            set
            {
                reworkGrade = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ReworkGrade"));
                }
            }
        }
        /// <summary>
        /// 学院名称
        /// </summary>
        public string CollegeName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 重修标记
        /// </summary>
        public string ReworkFlag { get; set; }
        /// <summary>
        /// 课程英文名称
        /// </summary>
        public string ClassEnglishName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool cut;
        public bool Cut
        {
            get { return cut; }
            set
            {
                cut = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Cut"));
                }
            }
        }
    }
}
