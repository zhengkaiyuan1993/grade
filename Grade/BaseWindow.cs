using System;
using System.Windows;
using System.Windows.Controls;

namespace Grade
{
    public class BaseWindow : Window
    {
        /// <summary>
        /// 等待控件
        /// </summary>
        private Control PART_CirclyWaiting = null;
        private Button PART_Refresh = null;
        private Button PART_Setting = null;
        protected Button RefreshButton
        {
            get
            {
                if (PART_Refresh != null)
                {
                    return PART_Refresh;
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
        }

        protected Button SettingButton
        {
            get
            {
                if (PART_Setting != null)
                {
                    return PART_Setting;
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            try
            {
                Button button = this.GetTemplateChild("PART_Close") as Button;
                if (button != null)
                {
                    button.Click += Close_Click;
                }
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
            }

            try
            {
                PART_CirclyWaiting = this.GetTemplateChild("PART_CirclyWaiting") as Control;
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
            }
            try
            {
                this.MouseLeftButtonDown += BaseView_MouseLeftButtonDown;
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
            }
            try
            {
                PART_Refresh = this.GetTemplateChild("PART_Refresh") as Button;
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
            }
            try
            {
                PART_Setting = this.GetTemplateChild("PART_Setting") as Button;
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
            }
        }
        /// <summary>
        /// 窗体移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseView_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {

                this.DragMove();
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
            }
        }
        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
            }
        }
        /// <summary>
        /// 获取文本资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual string GetStringResource(string key)
        {
            try
            {
                return this.Resources[key].ToString();
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
                return "";
            }
        }
        /// <summary>
        /// 显示新的主窗体
        /// </summary>
        /// <param name="win"></param>
        protected virtual void ShowView(Window win)
        {

            try
            {
                Application.Current.MainWindow = win;
                win.Show();
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
            }
        }
        /// <summary>
        /// 异步调用
        /// </summary>
        /// <param name="action"></param>
        public static void Invoke(Action action)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(action);
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
            }
        }
        /// <summary>
        /// 开始等待
        /// </summary>
        protected virtual void StartWaiting()
        {
            try
            {
                Invoke(() =>
                {
                    if (PART_CirclyWaiting != null)
                    {
                        PART_CirclyWaiting.Visibility = Visibility.Visible;
                    }
                });
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
            }
        }
        /// <summary>
        /// 结束等待
        /// </summary>
        protected virtual void StopWaiting()
        {
            try
            {
                Invoke(() =>
                {
                    if (PART_CirclyWaiting != null)
                    {
                        PART_CirclyWaiting.Visibility = Visibility.Collapsed;
                    }
                });
            }
            catch (Exception ex)
            {
                LogUtils.Write(ex);
            }
        }

        protected virtual void SetWaitingSize(double height, double width)
        {
            if (PART_CirclyWaiting != null)
            {
                PART_CirclyWaiting.Height = height;
                PART_CirclyWaiting.Width = width;
            }
        }
        protected virtual void SetWaitingMargin(Thickness margin)
        {
            if (PART_CirclyWaiting != null)
            {
                PART_CirclyWaiting.Margin = margin;
            }
        }
    }
}
