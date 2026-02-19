using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WatchBasic.Event;

namespace WatchJieLi.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ShowProgressBar>().Subscribe(arg =>
            {
                dialogHost.IsOpen = arg;
                if (dialogHost.IsOpen)
                {
                    dialogHost.DialogContent = new WatchUI.Views.ProgressBar();
                }
            });
            InitializeComponent();
            btnMax.Click += (sender, e) =>
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                }

            };
            btnMin.Click += (sender, e) =>
            {
                if (this.WindowState == WindowState.Minimized)
                {
                    this.WindowState = WindowState.Normal;
                }
                else
                {
                    this.WindowState = WindowState.Minimized;
                }

            };

            btnClose.Click += (sender, e) =>
            {
                this.Close();
                Application.Current.Shutdown(-1);

            };
            ColorZone.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
            ColorZone.MouseDoubleClick += (s, e) =>
            {
                if (this.WindowState == WindowState.Normal)
                {
                    this.WindowState = WindowState.Maximized;
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                }
            };
            //timeList.AddHandler(ListBoxItem.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.timeList_MouseLeftButtonDown), true);
            list.PreviewMouseWheel += (sender, e) =>
            {
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var listbox = (System.Windows.Controls.ListBox)sender;
                listbox.RaiseEvent(eventArg);
            };
            //typeList.AddHandler(ListBoxItem.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.timeList_MouseLeftButtonDown),true);
            list.PreviewMouseWheel += (sender, e) =>
            {
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var listbox = (System.Windows.Controls.ListBox)sender;
                listbox.RaiseEvent(eventArg);
            };
        }

        private void timeList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
        }
    }
}
