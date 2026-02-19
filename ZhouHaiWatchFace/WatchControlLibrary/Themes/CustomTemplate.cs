using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;

namespace WatchControlLibrary.Themes
{
    public partial class CustomTemplate : ResourceDictionary
    {
        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                // ListView拦截鼠标滚轮事件
                e.Handled = true;

                // 激发一个鼠标滚轮事件，冒泡给外层接收到
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var listbox = (System.Windows.Controls.ListBox)sender;
                listbox.RaiseEvent(eventArg);
            }
        }



        private bool _isDown;
        private bool _isDragging;
        //private int currentIdx;
        private ObservableCollection<string> ItemsSource;

        private object GetListBoxParent(object sender)
        {
            var a = VisualTreeHelper.GetParent(sender as FrameworkElement);
            if (a == null || a is System.Windows.Controls.ListBox) return a;
            else return GetListBoxParent(a);
            //return null;
        }

        private void KeyValue_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDown = true;
            var listbox = GetListBoxParent((sender as FrameworkElement));
            if (listbox != null)
            {
                var box = listbox as System.Windows.Controls.ListBox;
                ItemsSource = box.ItemsSource as ObservableCollection<string>;
                if (ItemsSource is null)
                {
                    ItemsSource = (box.Tag as DragBindKeyValue).ImageSource;
                }
                //var text = e.Source as FrameworkElement;
                //if (text != null)
                //{
                //    var current = text.DataContext as KeyValueData;
                //    if (current != null)
                //    {
                //        //var a = (sender as FrameworkElement).TemplatedParent;
                //        currentIdx = ItemsSource?.IndexOf(current) ?? 0;
                //    }
                //}
            }
        }

        private void KeyValue_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDown = false;
            _isDragging = false;
        }

        private void KeyValue_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isDown)
            {
                if (_isDragging == false && e.Source is not System.Windows.Controls.Button)
                {
                    _isDragging = true;
                    //UIElement _dummyDragSource = new();
                    //DragDrop.DoDragDrop(_dummyDragSource, new System.Windows.DataObject("UIElement", e.Source, true), DragDropEffects.Move);
                    DragDrop.DoDragDrop(sender as FrameworkElement, new System.Windows.DataObject("UIElement", sender, true), DragDropEffects.Move);

                }
            }
        }

        private void KeyValue_Drop(object sender, DragEventArgs e)
        {
            if (_isDragging && e.Data.GetDataPresent("UIElement"))
            {
                if (e.Data.GetData("UIElement", true) is StackPanel { DataContext: KeyValueData source } && source != null && 
                    sender is StackPanel { DataContext: KeyValueData target } && target != null)
                {
                    (source.Value, target.Value) = (target.Value, source.Value);  // 原数据ItemsSource换不换？
                    (ItemsSource[ItemsSource.IndexOf(source.Value)], ItemsSource[ItemsSource.IndexOf(target.Value)]) = (ItemsSource[ItemsSource.IndexOf(target.Value)], ItemsSource[ItemsSource.IndexOf(source.Value)]);
                }
                else if (e.Data.GetData("UIElement", true) is Grid { DataContext: string s1} && s1 != null &&
                    sender is Grid { DataContext: string s2 } && s2 != null)
                {
                    (ItemsSource[ItemsSource.IndexOf(s1)], ItemsSource[ItemsSource.IndexOf(s2)]) = (ItemsSource[ItemsSource.IndexOf(s2)], ItemsSource[ItemsSource.IndexOf(s1)]);
                }
                _isDown = false;
                _isDragging = false;
            }
        }
    }
}
