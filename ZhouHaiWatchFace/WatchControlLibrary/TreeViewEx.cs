using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using Brushes = System.Windows.Media.Brushes;
using TreeView = System.Windows.Controls.TreeView;
using Point = System.Windows.Point;

namespace WatchControlLibrary
{
    public class TreeViewEx : TreeView
    {
        static TreeViewEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewEx), new FrameworkPropertyMetadata(typeof(TreeViewEx)));
        }

        public TreeViewEx()
        {
            this.MouseUp += TreeViewEx_MouseUp;
        }

        private void TreeViewEx_MouseUp(object sender, MouseButtonEventArgs e)
        {


            Point mousePosition = e.GetPosition(this);
            // Perform a hit test to find the clicked item
            var hitTestResult = VisualTreeHelper.HitTest(this, mousePosition);
            if (hitTestResult != null)
            {
                // Find the TreeViewItem that was clicked
                var clickedItem = FindParent<TreeViewItem>(hitTestResult.VisualHit);
                if (clickedItem != null)
                {
                    var currnet = (DragBindBase)clickedItem.DataContext;

                    if (currnet != null)
                    {
                        SelectItem = currnet;
                        if (e.ChangedButton == MouseButton.Left)
                        {
                            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                            {
                                ////Handle multi-selection
                                if (SelectItems == null) SelectItems = new ObservableCollection<DragBindBase>();
                                if (currnet != null && !SelectItems.Contains(currnet))
                                {
                                    bool isAdd;
                                    if (!SelectItems.Any())
                                    {
                                        isAdd = true;
                                    }
                                    else
                                    {
                                       
                                        var items = this.ItemsSource.Cast<DragBindBase>().ToList();
                                        var first = SelectItems.FirstOrDefault();
                                        var level1 = FindLevel(items, first!, 0);
                                        var level2 = FindLevel(items, currnet!, 0);
                                        isAdd = level1 == level2;
                                    }
                                    if (isAdd)
                                    {
                                        SelectItems.Add(currnet);
                                    }
                                }
                            }
                            else
                            {
                                if (SelectItems != null)
                                    SelectItems.Clear();
                            }
                        }

                    }
                    var source = this.ItemsSource.Cast<DragBindBase>();
                    UpdateTreeViewItemColors((TreeViewEx)sender, source, SelectItems);

                }
            }


        }


         static int FindLevel(List<DragBindBase> dragBinds, DragBindBase item, int currentLevel)
        {
            foreach (var child in dragBinds)
            {
                if (item.DragId == child.DragId)
                {
                    return currentLevel;
                }
                else 
                {
                    if (child.SubItems != null&&child.SubItems.Any())
                    {
                        var level= FindLevel(new List<DragBindBase>(child.SubItems), item, currentLevel + 1);
                        if (level != -1)
                        {
                            return level;
                        }
                    }
                }

               
            }

            return -1; // If not found, return -1
        }


        private static T FindChild<T>(DependencyObject parent, object dataContext) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T && (child as TreeViewItem).DataContext == dataContext)
                {
                    return (T)child;
                }

                var childOfChild = FindChild<T>(child, dataContext);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }




        private static void UpdateTreeViewItemColors(DependencyObject parent, IEnumerable<DragBindBase> source, ObservableCollection<DragBindBase>? SelectItems)
        {
            if (source != null && source.Any())
                foreach (var item in source)
                {
                    var treeViewItem = FindChild<TreeViewItem>(parent, item);
                    if (treeViewItem != null && SelectItems != null)
                    {
                        var isSelected = SelectItems.Contains(item);
                        treeViewItem.Background = isSelected ? Brushes.AliceBlue : Brushes.Transparent; // Set color
                                                                                                        // treeViewItem.Foreground = isSelected ? Brushes.White : Brushes.Black; // Set foreground color
                    }
                    if (item.SubItems != null && item.SubItems.Any())
                    {
                        UpdateTreeViewItemColors(parent, item.SubItems, SelectItems);
                    }
                }
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            T parent = parentObject as T;
            return parent ?? FindParent<T>(parentObject);
        }




        public DragBindBase SelectItem
        {
            get { return (DragBindBase)GetValue(SelectItemProperty); }
            set { SetValue(SelectItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectItemProperty =
            DependencyProperty.Register("SelectItem", typeof(DragBindBase), typeof(TreeViewEx), new PropertyMetadata(null));




        public ObservableCollection<DragBindBase> SelectItems
        {
            get { return (ObservableCollection<DragBindBase>)GetValue(SelectItemsProperty); }
            set { SetValue(SelectItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectItemsProperty =
            DependencyProperty.Register("SelectItems", typeof(ObservableCollection<DragBindBase>), typeof(TreeViewEx), new PropertyMetadata(new ObservableCollection<DragBindBase>()));



    }
}
