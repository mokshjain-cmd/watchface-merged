using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WatchControlLibrary.Model;
using static System.Net.Mime.MediaTypeNames;
using Binding = System.Windows.Data.Binding;
using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using ListBox = System.Windows.Controls.ListBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using SelectionMode = System.Windows.Controls.SelectionMode;
using UserControl = System.Windows.Controls.UserControl;

namespace WatchControlLibrary
{
    /// <summary>
    /// DragItemList.xaml 的交互逻辑
    /// </summary>
    public partial class DragItemList : UserControl
    {
        public DragItemList()
        {
            InitializeComponent();
            list.SelectionMode = SelectionMode.Multiple;//多选
            var binding = new Binding("ItemsSource") { Source = this, Mode = BindingMode.TwoWay };
            list.SetBinding(ListBox.ItemsSourceProperty, binding);

            var selectbinding = new Binding("SelectItem") { Source = this, Mode = BindingMode.TwoWay };
            list.SetBinding(ListBox.SelectedItemProperty, selectbinding);

             // list.SelectionChanged += List_SelectionChanged;
            //var selectbindings = new Binding("SelectItems") { Source = this, Mode = BindingMode.TwoWay };
            //list.SetBinding(ListBox.SelectedItemsProperty, selectbindings);
            // ListBox.SelectedItemsProperty


        }

        private bool _isDown;
        private bool _isDragging;
        private int currentIdx;


        public DragBindBase SelectItem
        {
            get { return (DragBindBase)GetValue(SelectItemProperty); }
            set { SetValue(SelectItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectItemProperty =
            DependencyProperty.Register("SelectItem", typeof(DragBindBase), typeof(DragItemList), new PropertyMetadata(null));




        //public ObservableCollection<DragBindBase> SelectItems
        //{
        //    get { return (ObservableCollection<DragBindBase>)GetValue(SelectItemsProperty); }
        //    set { SetValue(SelectItemsProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for SelectItems.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty SelectItemsProperty =
        //    DependencyProperty.Register("SelectItems", typeof(ObservableCollection<DragBindBase>), typeof(DragItemList), new PropertyMetadata(null));



        public static readonly DependencyProperty ItemsSourceProperty =
           DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<DragBindBase>), typeof(DragItemList), new PropertyMetadata(null, OnItemSourceChanged));

        private static void OnItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DragItemList list)
            {

                if (e.NewValue is ObservableCollection<DragBindBase> bases)
                {
                    list.ItemsSource = bases;
                }
            }

        }

        //private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //   // 移除未选中的项
        //    foreach (DragBindBase item in e.RemovedItems)
        //    {
        //        SelectItems?.Remove(item);
        //    }

        //    // 添加选中的项
        //    foreach (DragBindBase item in e.AddedItems)
        //    {
        //        SelectItems?.Add(item);
        //    }
        //}
        public ObservableCollection<DragBindBase> ItemsSource
        {
            get { return (ObservableCollection<DragBindBase>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }


        private void TextBlock_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        private void TextBlock_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                FrameworkElement? droptarget = e.Source as FrameworkElement;
                if (droptarget != null)
                {
                    var binding = droptarget.DataContext as DragBindBase;
                    if (binding != null)
                    {
                        var idx = ItemsSource?.IndexOf(binding);
                        if (idx != -1 && idx != currentIdx)
                        {
                            var current = ItemsSource?[currentIdx];
                            if (current != null)
                            {
                                ItemsSource?.Remove(current);
                                ItemsSource?.Insert(idx ?? 0, current);
                            }
                        }

                    }
                }
                _isDown = false;
                _isDragging = false;

            }
        }

        private void TextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDown = true;
            var text = e.Source as FrameworkElement;
            if (text != null)
            {
                var current = text.DataContext as DragBindBase;
                if (current != null)
                {
                    currentIdx = ItemsSource?.IndexOf(current) ?? 0;
                }
            }

        }

        private void TextBlock_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDown = false;
            _isDragging = false;
        }

        private void TextBlock_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {
                if (_isDragging == false)
                {
                    _isDragging = true;
                    UIElement _dummyDragSource = new();
                    DragDrop.DoDragDrop(_dummyDragSource, new DataObject("UIElement", e.Source, true), DragDropEffects.Move);
                }
            }
        }

        private void TextBlock_DragOver(object sender, DragEventArgs e)
        {
           
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ItemsSource.Remove(SelectItem);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItems.Count == 0)
            {
                return;
            }
            var data = list.SelectedItems.Cast<DragBindBase>().ToList();

            var group = new DragBindWidget()
            {
                DragName = "组件" + (ItemsSource.Where(i => i.DragName.Contains("组件")).Count() + 1),
                SubItems = new ObservableCollection<DragBindBase>(data),
                //ElementType = BindMonitorType.Widget,
            };

            for (int i = 0; i < data.Count(); i++)
            {
                ItemsSource.Remove(data[i]);
            }
            ItemsSource.Add(group);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItems.Count == 0)
            {
                return;
            }
            var data = list.SelectedItems.Cast<DragBindBase>().FirstOrDefault();
            if (data != null && data is DragBindWidget bindWidget)
            {
                var items = bindWidget.SubItems?.ToList();
               
                ItemsSource.Remove(data);
                foreach (var item in items)
                {
                    ItemsSource.Add(item);
                }
            }

        }

        private void list_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void list_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (list.SelectedItem != null)
                {
                    list.SelectedItem = null;
                }
            }
        }
    }
}
