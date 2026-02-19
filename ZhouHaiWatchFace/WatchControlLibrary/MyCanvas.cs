using Mapster;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using WatchControlLibrary.Model;
using static System.Windows.Forms.LinkLabel;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace WatchControlLibrary
{
    public class MyCanvas : Canvas
    {

        public static Path DistancePath { get; set; }
        public static TextBlock DistanceX { get; set; }
        public static TextBlock DistanceY { get; set; }
        static MyCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MyCanvas), new FrameworkPropertyMetadata(typeof(MyCanvas)));
        }
        public MyCanvas()
        {
            this.MouseDown += MyCanvas_MouseDown;
            DistancePath ??= new Path
            {
                Stroke = System.Windows.Media.Brushes.LightGreen,
            };
            DistanceX ??= new TextBlock
            {
                Foreground = System.Windows.Media.Brushes.LightPink,
            };
            DistanceY ??= new TextBlock
            {
                Foreground = System.Windows.Media.Brushes.LightPink,
            };
        }

        private void ItemsSource_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            RefreshCanvas();

        }

        public void RefreshCanvas()
        {
            Children.Clear();

            SelectElement = null;
            foreach (var item in ItemsSource)
            {
                AddItem(item);
            }
        }


        private void SelectedElements_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.SelectedElements == null || (!this.SelectedElements.Any())) return;
            var dragNames = SelectedElements.Select(x => x.DragId);

            foreach (var item in Children.Cast<FrameworkElement>().Where(x => x is IDraggable).Select(x => (IDraggable)x))
            {
                if (dragNames.Contains(item.DragId))
                {
                    item._draggableBehavior.AlwaysShow = true;
                }
                else
                {
                    item._draggableBehavior.AlwaysShow = false;
                }
            }
        }

        private void MyCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UIElement elementWithFocus = VisualTreeHelper.GetParent(sender as UIElement) as UIElement;
            TraversalRequest request = new TraversalRequest(FocusNavigationDirection.First);
            if (elementWithFocus != null)
            {
                var a = elementWithFocus.MoveFocus(request);
            }
            Point clickPoint = e.GetPosition(this);
            FrameworkElement? clickedElement = GetElementAtPoint(clickPoint);
            //Point clickPoint = e.GetPosition(this);
            //FrameworkElement? clickedElement = GetElementAtPoint(clickPoint);

            if (clickedElement != null)
            {
                var drag = clickedElement as IDraggable;
                var bind = ItemsSource.Cast<DragBindBase>().FirstOrDefault(x => x.DragId == drag?.DragId);
                SelectElement = bind;
                //DraggableBehavior.oldTarget = clickedElement;
            }
            else
            {
                SelectElement = null;
            }

            SelectedElements = new ObservableCollection<DragBindBase>();
        }

        public FrameworkElement? GetElementAtPoint(Point point)
        {
            //return this.Children.Cast<FrameworkElement>().Where(x => x is IDraggable).FirstOrDefault(x => IsClicked(point, x));
            return this.Children.Cast<FrameworkElement>().Where(x => x is IDraggable && IsClicked(point, x)).OrderBy(x => GetZIndex(x)).LastOrDefault();
        }

        bool IsClicked(Point point, FrameworkElement element)
        {
            var left = Canvas.GetLeft(element);
            var top = Canvas.GetTop(element);
            var right = left + element.Width;
            var bottom = top + element.Height;
            return point.X >= left && point.X <= right && point.Y >= top && point.Y <= bottom;
        }


        public MonitorItem MonitorItem
        {
            get { return (MonitorItem)GetValue(MonitorItemProperty); }
            set { SetValue(MonitorItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MonitorItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MonitorItemProperty =
            DependencyProperty.Register("MonitorItem", typeof(MonitorItem), typeof(MyCanvas), new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty ItemsSourceProperty =
       DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<DragBindBase>), typeof(MyCanvas), new PropertyMetadata(null, OnItemsSourceChanged));

        public ObservableCollection<DragBindBase> ItemsSource
        {
            get { return (ObservableCollection<DragBindBase>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public DragBindBase? SelectElement
        {
            get { return (DragBindBase?)GetValue(SelectElementProperty); }
            set { SetValue(SelectElementProperty, value); }
        }

        public static readonly DependencyProperty SelectElementProperty =
            DependencyProperty.Register("SelectElement", typeof(DragBindBase), typeof(MyCanvas), new PropertyMetadata(null, SelectElementOnChaged));

        public ObservableCollection<DragBindBase> SelectedElements
        {
            get { return (ObservableCollection<DragBindBase>)GetValue(SelectedElementsProperty); }
            set { SetValue(SelectedElementsProperty, value); }
        }



        public static readonly DependencyProperty SelectedElementsProperty =
            DependencyProperty.Register("SelectedElements", typeof(ObservableCollection<DragBindBase>), typeof(MyCanvas), new PropertyMetadata(new ObservableCollection<DragBindBase>(), SelectedElementsChanaged));

        private static void SelectedElementsChanaged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MyCanvas canvas)
            {
                if (canvas.SelectedElements != null)
                {
                    if (e.OldValue is ObservableCollection<DragBindBase> oldCollection)
                    {
                        oldCollection.CollectionChanged -= canvas.SelectedElements_CollectionChanged;
                    }
                    if (e.NewValue is ObservableCollection<DragBindBase> newCollection)
                    {
                        newCollection.CollectionChanged += canvas.SelectedElements_CollectionChanged;
                    }

                }
            }

        }
        static void SetSelectStatus(IEnumerable<DragDataBase> dragDataBases, Guid? dragId)
        {
            foreach (var item in dragDataBases)
            {
                if (item.DragId == dragId)
                {
                    item._draggableBehavior.AlwaysShow = true;
                    //SetZIndex(item, 3);
                }
                else
                {
                    item._draggableBehavior.AlwaysShow = false;
                    //SetZIndex(item, 1);
                }
                if (item is DragWidget widget)
                {
                    if (widget.Children != null && widget.Children.Any())
                        SetSelectStatus(widget.Children, dragId);
                }
                else if (item is DragSlot dragSlot)
                {
                    if (dragSlot.Widgets != null && dragSlot.Widgets.Any())
                        SetSelectStatus(dragSlot.Widgets, dragId);
                }
            }
        }

        private static void SelectElementOnChaged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MyCanvas canvas)
            {
                if (e.NewValue is DragBindBase bind)
                {
                    var dragDataBases = canvas.Children.Cast<FrameworkElement>().Where(x => x is DragDataBase).Select(x => (DragDataBase)x);
                    SetSelectStatus(dragDataBases, bind.DragId);
                }
                else
                {
                    var dragDataBases = canvas.Children.Cast<FrameworkElement>().Where(x => x is DragDataBase).Select(x => (DragDataBase)x);
                    SetSelectStatus(dragDataBases, Guid.NewGuid());
                }
            }
        }

        static void RemoveChangedMenthod(ObservableCollection<DragBindBase> oldCollection, MyCanvas canvas)
        {
            oldCollection.CollectionChanged -= canvas.ItemsSource_CollectionChanged;
            foreach (var item in oldCollection)
            {
                if (item.SubItems != null && item.SubItems.Any())
                {
                    RemoveChangedMenthod(item.SubItems, canvas);
                }
            }
        }
        static void AddChangedMenthod(ObservableCollection<DragBindBase> oldCollection, MyCanvas canvas)
        {
            oldCollection.CollectionChanged += canvas.ItemsSource_CollectionChanged;
            foreach (var item in oldCollection)
            {
                if (item.SubItems != null && item.SubItems.Any())
                {
                    AddChangedMenthod(item.SubItems, canvas);
                }
            }
        }
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MyCanvas canvas)
            {
                if (canvas.ItemsSource != null)
                {
                    if (e.OldValue is ObservableCollection<DragBindBase> oldCollection)
                    {
                        RemoveChangedMenthod(oldCollection, canvas);
                        // oldCollection.CollectionChanged -= canvas.ItemsSource_CollectionChanged;

                    }

                    if (e.NewValue is ObservableCollection<DragBindBase> newCollection)
                    {
                        AddChangedMenthod(newCollection, canvas);
                        //newCollection.CollectionChanged += canvas.ItemsSource_CollectionChanged;
                    }
                    canvas.Children.Clear();
                    canvas.SelectElement = null;
                    foreach (var item in canvas.ItemsSource)
                    {
                        canvas.AddItem(item);
                    }
                }
                else
                    canvas.Children.Clear();
            }
        }


        DragDataBase GetDragDataBase(DragBindBase bindBase)
        {
            DragDataBase control = bindBase switch
            {
                DragBindAMPM => bindBase.Adapt<DragAMPM>(),
                DragBindImage => bindBase.Adapt<DragImage>(),
                DragBindNums => bindBase.Adapt<DragNums>(),
                DragBindDouble => bindBase.Adapt<DragDouble>(),
                DragBindWeek => bindBase.Adapt<DragWeek>(),
                DragBindProgress => bindBase.Adapt<DragProgress>(),
                DragBindSwitch => bindBase.Adapt<DragSwitch>(),
                DragBindWidget => bindBase.Adapt<DragWidget>(),
                DragBindNormalDateTime => bindBase.Adapt<DragNormalDateTime>(),
                DragBindSingleDigit => bindBase.Adapt<DragSingleDigit>(),
                DragBindMonthDay => bindBase.Adapt<DragMonthDay>(),
                DragBindKeyValue => bindBase.Adapt<DragKeyValue>(),
                DragBindPoint => bindBase.Adapt<DragPoint>(),
                DragBindSlot => bindBase.Adapt<DragSlot>(),
                DragBindAnimFrame => bindBase.Adapt<DragAnimFrame>(),
                _ => throw new NotImplementedException(),
            };
            BindingHelper.AutoBindDependencyProperties(bindBase, control);
            if (MonitorItem != null && bindBase.ElementType == BindMonitorType.WatchDateTime)
            {
                BindingHelper.MonitorValueBind(MonitorItem, bindBase);
            }
            control.SetSize();
            if ((bindBase.SubItems?.Any() ?? false) && bindBase is DragBindWidget bindWidget && control is DragWidget widget)
            {
                foreach (var item in bindBase.SubItems)
                {
                    var child = GetDragDataBase(item);
                    if (widget.Children == null)
                    {
                        widget.Children = new ObservableCollection<DragDataBase>();
                    }
                    widget.Children.Add(child);
                }
                if (bindWidget.IsNew)
                {
                    widget.DefaultBorder();
                    widget.IsNew = true;
                    bindWidget.IsNew = false;
                }
                if (widget.Children != null)
                    foreach (var child in widget.Children)
                    {
                        SetGroupValueChanged(child, widget.LoadImages);
                    }
                widget._draggableBehavior.AttachThumbs();

            }

            return control;
        }

        void SetGroupValueChanged(DragDataBase dragDataBase, Action LoadImages)
        {
            if (dragDataBase is not DragWidget widget)
            {
                dragDataBase.GroupValueChanged = LoadImages;
            }
            else
            {
                if (widget.Children != null)
                {
                    foreach (var item in widget.Children)
                    {
                        SetGroupValueChanged(item, widget.LoadImages);
                    }
                }
            }
        }

        public void AddItem(DragBindBase bindBase)
        {
            if (bindBase != null)
            {
                var control = GetDragDataBase(bindBase);

                if (bindBase is DragBindSlot bindSlot && bindSlot.SubItems != null)
                {
                    if (control is DragSlot slot && bindSlot.SubItems.Any())
                    {
                        slot.Widgets = new ObservableCollection<DragDataBase>(bindSlot.SubItems.Select(x => GetDragDataBase(x)));
                        slot.SetSize();
                    }
                }
                if (control != null)
                {
                    this.Children.Add(control);
                    control._draggableBehavior.ShowSubline += this.ShowSubLine;
                    control._draggableBehavior.HideSubLine += this.HideSubLine;
                    //control.MouseMove += MouseMove;
                    //control.MouseDown += Child_MouseDown;
                    //control.MouseUp += Child_MouseUp;

                    //居中
                    if (Canvas.GetLeft(control) is double.NaN)
                    {
                        bindBase.Left = (this.Width - control.Width) / 2;
                    }
                    if (Canvas.GetTop(control) is double.NaN)
                    {
                        bindBase.Top = (this.Height - control.Height) / 2;
                    }

                }

            }
        }

        bool isMousePress = false;
        FrameworkElement? clickedElement = null;
        double offsetX = 0;
        double offsetY = 0;
        //辅助虚线
        Line LeftLine = new Line();
        Line TopLine = new Line();


        Point clickPoint;
        Point _mouseDownLT;
        Point _mouseDownCenter;
        Point _mouseDownRB;
        // 获取可对齐的点
        List<Point> Points = new List<Point>();
        private void ClearDashedLine()
        {
            LeftLine.Visibility = Visibility.Hidden;
            TopLine.Visibility = Visibility.Hidden;
        }


        public SolidColorBrush GetBrush()
        {
            var background = Children.Cast<FrameworkElement>().Where(x => x is IDraggable).FirstOrDefault(x => x.Width == this.Width && x.Height == this.Height);//查找背景图
            if (background != null && background is DragImage dragImage)
            {
                Dictionary<int, int> colorCount = new Dictionary<int, int>();
                using (var bitmap = new System.Drawing.Bitmap(dragImage.Source)) 
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            var pixelColor = bitmap.GetPixel(x, y);
                            int colorKey = pixelColor.ToArgb();

                            if (colorCount.ContainsKey(colorKey))
                            {
                                colorCount[colorKey]++;
                            }
                            else
                            {
                                colorCount[colorKey] = 1;
                            }
                        }


                    }
                    var colorKV = colorCount.MaxBy(x => x.Value);
                    var color = System.Drawing.Color.FromArgb(colorKV.Key);

                    return new SolidColorBrush(Color.FromRgb((byte)(255 - color.R), (byte)(255 - color.G), (byte)(255 - color.B)));
                }
                
              
                // new System.Drawing.Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
            }

            return new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
        }


        public void ShowSubLine(FrameworkElement element)
        {
           
            Children.Remove(TopLine);
            Children.Remove(LeftLine);
            var currentLeft = (int)Canvas.GetLeft(element);
            var currentTop = (int)Canvas.GetTop(element);
            var currentRight = (int)(Canvas.GetLeft(element) + element.Width);
            var currentBottom = (int)(Canvas.GetTop(element) + element.Height);
            var currentCenterX = (int)(Canvas.GetLeft(element) + element.Width / 2);
            var currentCenterY = (int)(Canvas.GetTop(element) + element.Height / 2);
            var childrenPoints = Children.Cast<FrameworkElement>().Where(x => x is IDraggable).Where(x => ((IDraggable)x).DragId != ((IDraggable)element).DragId)
                .Select(x => new
                {
                    left = (int)Canvas.GetLeft(x),
                    top = (int)Canvas.GetTop(x),
                    right = (int)(Canvas.GetLeft(x) + x.Width),
                    bottom = (int)(Canvas.GetTop(x) + x.Height),
                    centerX = (int)(Canvas.GetLeft(x) + x.Width / 2),
                    centerY = (int)(Canvas.GetTop(x) + x.Height / 2),
                });

            var verticals = childrenPoints.Select(x => new int[] { x.left, x.centerX, x.right }).SelectMany(x => x).ToList();
            verticals.AddRange(new int[] { 0, (int)(this.Width / 2), (int)this.Width });
            var horizontals = childrenPoints.Select(x => new int[] { x.top, x.centerY, x.bottom }).SelectMany(x => x).ToList();
            horizontals.AddRange(new int[] { 0, (int)(this.Height / 2), (int)this.Height });
            var coordinate = verticals.Zip(horizontals).ToArray();
            var currentVerticals = new int[] { currentLeft, currentCenterX, currentRight };
            var currentHorizontals = new int[] { currentTop, currentCenterY, currentBottom };
            var currentcoordinate = currentVerticals.Zip(currentHorizontals).ToArray();
            for (var i = 0; i < currentcoordinate.Length; i++)
            {
                for (var j = 0; j < coordinate.Length; j++)
                {
                    if (currentcoordinate[i].First == coordinate[j].First)
                    {

                        var maxDistance = coordinate.Where(x => x.First == currentcoordinate[i].First).MaxBy(x => Math.Abs(x.Second - currentcoordinate[i].Second));

                        TopLine.X1 = currentcoordinate[i].First;
                        TopLine.X2 = currentcoordinate[i].First;
                        TopLine.Y2 = currentcoordinate[i].Second;
                        TopLine.Y1 = maxDistance!.Second;
                        TopLine.Visibility = Visibility.Visible;
                        TopLine.Stroke = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF)); ;
                        TopLine.StrokeThickness = 1;
                        TopLine.StrokeDashArray = new DoubleCollection() { 2, 2 };
                        if (!Children.Contains(TopLine))
                        {
                            Children.Add(TopLine);
                        }
                    }

                    if (currentcoordinate[i].Second == coordinate[j].Second)
                    {
                        var maxDistance = coordinate.Where(x => x.Second == currentcoordinate[i].Second).MaxBy(x => Math.Abs(x.First - currentcoordinate[i].First));

                        LeftLine.X1 = currentcoordinate[i].First;
                        LeftLine.X2 = maxDistance.First;
                        LeftLine.Y1 = currentcoordinate[i].Second;
                        LeftLine.Y2 = currentcoordinate[i].Second;
                        LeftLine.Visibility = Visibility.Visible;
                        LeftLine.Stroke = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF)); ;
                        LeftLine.StrokeThickness = 1;
                        LeftLine.StrokeDashArray = new DoubleCollection() { 2, 2 };
                        if (!Children.Contains(LeftLine))
                        {
                            Children.Add(LeftLine);

                        }
                    }


                }
            }

        }

        public void HideSubLine()
        {
            LeftLine.Visibility = Visibility.Hidden;
            TopLine.Visibility = Visibility.Hidden;
        }

        private new void MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (isMousePress)
            {
                var pos = e.GetPosition(this);
                var delta = pos - clickPoint;
                //拖动控件的左上角点
                int nowLTPositionX = (int)(_mouseDownLT.X + delta.X);
                int nowLTPositionY = (int)(_mouseDownLT.Y + delta.Y);
                //中心点
                int nowCenterPositionX = (int)(_mouseDownCenter.X + delta.X);
                int nowCenterPositionY = (int)(_mouseDownCenter.Y + delta.Y);
                //右下角点
                int nowRBPositionX = (int)(_mouseDownRB.X + delta.X);
                int nowRBPositionY = (int)(_mouseDownRB.Y + delta.Y);

                int CenterWidth = (int)(this.Width / 2);
                int CenterHeight = (int)(this.Height / 2);

                if (LeftLine.IsVisible)
                {
                    if (Math.Abs(offsetX - delta.X) > 2)
                    {
                        SetLeft(clickedElement, nowLTPositionX);
                        LeftLine.Visibility = Visibility.Hidden;
                        SelectElement.Left = nowLTPositionX;
                    }
                }
                else { SetLeft(clickedElement, nowLTPositionX); SelectElement.Left = nowLTPositionX; }

                if (TopLine.IsVisible)
                {
                    if (Math.Abs(offsetY - delta.Y) > 2)
                    {
                        SetTop(clickedElement, nowLTPositionY);
                        TopLine.Visibility = Visibility.Hidden;
                        SelectElement.Top = nowLTPositionY;
                    }
                }
                else { SetTop(clickedElement, nowLTPositionY); SelectElement.Top = nowLTPositionY; }
                //竖线
                foreach (var point in Points)
                {
                    //判断Canvas中心
                    if (CenterWidth - nowCenterPositionX == 0 || CenterWidth - nowLTPositionX == 0 || CenterWidth - nowRBPositionX == 0)
                    {
                        LeftLine.Visibility = Visibility.Visible;
                        LeftLine.X1 = CenterWidth;
                        LeftLine.Y1 = 0;
                        LeftLine.X2 = CenterWidth;
                        LeftLine.Y2 = (int)Height;
                        offsetY = delta.Y;
                        SetLeft(clickedElement, nowLTPositionX); SelectElement.Left = nowLTPositionX;
                        return;
                    }
                    if ((int)point.X - nowLTPositionX == 0)
                    {
                        drawLine1((int)point.Y, nowLTPositionX, nowLTPositionY);
                        offsetX = delta.X;
                        SetLeft(clickedElement, nowLTPositionX); SelectElement.Left = nowLTPositionX;
                        return;
                    }
                    else if ((int)point.X - nowCenterPositionX == 0)
                    {
                        drawLine1((int)point.Y, nowCenterPositionX, nowCenterPositionY);
                        offsetX = delta.X;
                        SetLeft(clickedElement, nowLTPositionX); SelectElement.Left = nowLTPositionX;
                        return;
                    }
                    else if ((int)point.X - nowRBPositionX == 0)
                    {
                        drawLine1((int)point.Y, nowRBPositionX, nowRBPositionY);
                        offsetX = delta.X;
                        SetLeft(clickedElement, nowLTPositionX); SelectElement.Left = nowLTPositionX;
                        return;
                    }
                }
                //横线
                foreach (var point in Points)
                {
                    if (CenterHeight - nowCenterPositionY == 0 || CenterHeight - nowLTPositionY == 0 || CenterHeight - nowRBPositionY == 0)
                    {
                        TopLine.Visibility = Visibility.Visible;
                        TopLine.X1 = 0;
                        TopLine.Y1 = CenterHeight;
                        TopLine.X2 = (int)Width;
                        TopLine.Y2 = CenterHeight;
                        offsetY = delta.Y;
                        SetTop(clickedElement, nowLTPositionY); SelectElement.Top = nowLTPositionY;
                        return;
                    }
                    if ((int)point.Y - nowLTPositionY == 0)
                    {
                        drawLine2((int)point.X, nowLTPositionX, nowLTPositionY);
                        offsetY = delta.Y;
                        SetTop(clickedElement, nowLTPositionY); SelectElement.Top = nowLTPositionY;
                        return;
                    }
                    else if ((int)point.Y - nowCenterPositionY == 0)
                    {
                        drawLine2((int)point.X, nowCenterPositionX, nowCenterPositionY);
                        offsetY = delta.Y;
                        SetTop(clickedElement, nowLTPositionY); SelectElement.Top = nowLTPositionY;
                        return;
                    }
                    else if ((int)point.Y - nowRBPositionY == 0)
                    {
                        drawLine2((int)point.X, nowRBPositionX, nowRBPositionY);
                        offsetY = delta.Y;
                        SetTop(clickedElement, nowLTPositionY); SelectElement.Top = nowLTPositionY;
                        return;
                    }
                }
                e.Handled = true;
            }
        }

        private void Child_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMousePress = true;
            clickPoint = e.GetPosition(this);
            clickedElement = GetElementAtPoint(clickPoint);
            if (clickedElement != null)
            {
                var left = double.IsNaN(GetLeft(clickedElement)) ? 0 : GetLeft(clickedElement);
                var top = double.IsNaN(GetTop(clickedElement)) ? 0 : GetTop(clickedElement);
                _mouseDownLT = new Point(left, top);
                _mouseDownCenter = new Point(left + clickedElement.ActualWidth / 2, top + clickedElement.ActualHeight / 2);
                _mouseDownRB = new Point(left + clickedElement.ActualWidth, top + clickedElement.ActualHeight);
                setDrawablePoints();
                var drag = clickedElement as IDraggable;
                var bind = ItemsSource.Cast<DragBindBase>().FirstOrDefault(x => x.DragName == drag?.DragName);
                SelectElement = bind;
                if (!Children.Contains(LeftLine))
                {
                    // 创建线条
                    LeftLine.Stroke = Brushes.White; // 设置线条颜色
                    LeftLine.StrokeDashArray = new DoubleCollection() { 2, 2 };
                    Children.Add(LeftLine);
                }
                if (!Children.Contains(TopLine))
                {
                    TopLine.Stroke = Brushes.White;
                    TopLine.StrokeDashArray = new DoubleCollection() { 2, 2 };
                    Children.Add(TopLine);
                }

            }
            else
            {
                SelectElement = null;
            }

            SelectedElements = new ObservableCollection<DragBindBase>();
        }

        private void Child_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isMousePress = false;
            clickedElement = null;
            offsetX = 0;
            offsetY = 0;
            ClearDashedLine();
        }

        private void drawLine1(double pointY, double positionX, double positionY)
        {
            LeftLine.Visibility = Visibility.Visible;
            LeftLine.X1 = positionX; // 设置起始点的X坐标
            LeftLine.Y1 = positionY; // 设置起始点的Y坐标
            LeftLine.X2 = positionX; // 设置结束点的X坐标
            LeftLine.Y2 = pointY; // 设置结束点的Y坐标
        }
        private void drawLine2(double pointX, double positionX, double positionY)
        {
            TopLine.Visibility = Visibility.Visible;
            TopLine.X1 = pointX;
            TopLine.Y1 = positionY;
            TopLine.X2 = positionX;
            TopLine.Y2 = positionY;
        }
        private void setDrawablePoints()
        {
            Point oldP = _mouseDownLT;
            foreach (FrameworkElement item in Children)
            {
                if (clickedElement != item)
                {
                    Point p = new Point(GetLeft(item), GetTop(item));
                    // 获取控件的宽度和高度
                    double width = item.ActualWidth;
                    double height = item.ActualHeight;
                    if (oldP == null)
                    {
                        Points.Add(p);
                        oldP = p;
                        //添加控件的中点和右下角点
                        Points.Add(new Point(p.X + width / 2, p.Y + height / 2));
                        Points.Add(new Point(p.X + width, p.Y + height));
                    }
                    else
                    {
                        if (Math.Abs(oldP.X - p.X) > 5 && Math.Abs(oldP.Y - p.X) > 5 && Math.Abs(_mouseDownLT.X - p.X) > 5 && Math.Abs(_mouseDownLT.Y - p.X) > 5)
                        {
                            Points.Add(p);
                            oldP = p;
                            Points.Add(new Point(p.X + width / 2, p.Y + height / 2));
                            Points.Add(new Point(p.X + width, p.Y + height));
                        }
                    }
                }
            }

        }

    }

}


