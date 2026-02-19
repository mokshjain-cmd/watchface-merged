using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using WatchControlLibrary.Model;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace WatchControlLibrary
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WatchControlLibrary"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WatchControlLibrary;assembly=WatchControlLibrary"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:DragWidget/>
    ///
    /// </summary>
    public class DragWidget : DragDataBase
    {
        static DragWidget()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragWidget), new FrameworkPropertyMetadata(typeof(DragWidget)));
        }
        public DragWidget() : base()
        {

        }



        public bool IsNew { get; set; }

        public ObservableCollection<DragDataBase>? Children
        {
            get { return (ObservableCollection<DragDataBase>)GetValue(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragDataBases.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.Register("Children", typeof(ObservableCollection<DragDataBase>), typeof(DragWidget), new PropertyMetadata(null, null));


        public bool? IsAutoLayout
        {
            get { return (bool?)GetValue(IsAutoLayoutProperty); }
            set { SetValue(IsAutoLayoutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAutoLayout.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAutoLayoutProperty =
            DependencyProperty.Register("IsAutoLayout", typeof(bool?), typeof(DragWidget), new PropertyMetadata(null, ValueChanged));



        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(DragWidget), new PropertyMetadata(Orientation.Horizontal, ValueChanged));




        public Direction Direction
        {
            get { return (Direction)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Direction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Direction), typeof(DragWidget), new PropertyMetadata(Direction.TopLeft, ValueChanged));




        public int Spacing
        {
            get { return (int)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Spacing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(int), typeof(DragWidget), new PropertyMetadata(0, ValueChanged));



        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DragWidget widget)
            {
                widget.LoadImages();
            }
        }

        public double GetHorizontal(double maxHeight, double currentY, DragDataBase element, Direction direction)
        {
            double height = Math.Max(element.Height, element.ActualHeight);
            double startY = 0;
            switch (direction)
            {
                case Direction.Center:
                    startY = (maxHeight - height) / 2;
                    break;
                case Direction.TopLeft:
                    startY = 0;
                    break;
                case Direction.Top:
                    startY = 0;
                    break;
                case Direction.TopRight:
                    startY = 0;
                    break;
                case Direction.Left:
                    startY = (maxHeight - height) / 2;
                    break;
                case Direction.Right:

                    startY = (maxHeight - height) / 2;
                    break;
                case Direction.BottomLeft:

                    startY = maxHeight - height;
                    break;
                case Direction.Bottom:

                    startY = maxHeight - height;
                    break;
                case Direction.BottomRight:

                    startY = maxHeight - height;
                    break;

            }

            return currentY + startY;


        }

        public double GetVertical(double maxWidth, double currentX, DragDataBase element, Direction direction)
        {
            double width = Math.Max(element.Width, element.ActualWidth);

            double startX = 0;

            switch (direction)
            {
                case Direction.Center:
                    startX = (maxWidth - width) / 2;

                    break;
                case Direction.TopLeft:
                    startX = 0;

                    break;
                case Direction.Top:
                    startX = (maxWidth - width) / 2;

                    break;
                case Direction.TopRight:
                    startX = maxWidth - width;

                    break;
                case Direction.Left:
                    startX = 0;

                    break;
                case Direction.Right:
                    startX = maxWidth - width;
                    break;
                case Direction.BottomLeft:
                    startX = 0;
                    break;
                case Direction.Bottom:
                    startX = (maxWidth - width) / 2;
                    break;
                case Direction.BottomRight:
                    startX = maxWidth - width;
                    break;
            }
            return currentX + startX;

        }

        public void AutoLayout(Canvas canvas, IEnumerable<DragDataBase>? children, Orientation orientation, Direction direction, int spacing)
        {
            // 计算每个元素的总宽度或高度
            double totalWidth = orientation == Orientation.Horizontal ? children?.Sum(x => x.Width) ?? 0 + spacing * (children.Count() - 1) : children.Max(x => x.Width);
            double totalHeight = orientation == Orientation.Vertical ? (children?.Sum(x => x.Height) ?? 0) + spacing * (children.Count() - 1) : children.Max(x => x.Height);
            var canvasWidth = Math.Max(this.Width, this.ActualWidth);
            var canvasHeight = Math.Max(this.Height, this.ActualHeight);
            var maxWdith = children?.Max(x => Math.Max(x.Width, x.ActualWidth)) ?? 0;
            var maxHeight = children?.Max(x => Math.Max(x.Height, x.ActualHeight)) ?? 0;

            // 根据方向调整起始点
            double startX = 0;
            double startY = 0;

            switch (direction)
            {
                case Direction.Center:
                    startX = (canvasWidth - totalWidth) / 2;
                    startY = (canvasHeight - totalHeight) / 2;
                    break;
                case Direction.TopLeft:
                    startX = 0;
                    startY = 0;
                    break;
                case Direction.Top:
                    startX = (canvasWidth - totalWidth) / 2;
                    startY = 0;
                    break;
                case Direction.TopRight:
                    startX = canvasWidth - totalWidth;
                    startY = 0;
                    break;
                case Direction.Left:
                    startX = 0;
                    startY = (canvasHeight - totalHeight) / 2;
                    break;
                case Direction.Right:
                    startX = canvasWidth - totalWidth;
                    startY = (canvasHeight - totalHeight) / 2;
                    break;
                case Direction.BottomLeft:
                    startX = 0;
                    startY = canvasHeight - totalHeight;
                    break;
                case Direction.Bottom:
                    startX = (canvasWidth - totalWidth) / 2;
                    startY = canvasHeight - totalHeight;
                    break;
                case Direction.BottomRight:
                    startX = canvasWidth - totalWidth;
                    startY = canvasHeight - totalHeight;
                    break;
            }

            // 自动布局
            double currentX = startX;
            double currentY = startY;
            if (children != null && children.Any())
                foreach (var element in children)
                {
                    canvas.Children.Add(element);


                    if (orientation == Orientation.Horizontal)
                    {
                        DraggableBehavior.SetLeft(element, currentX);
                        DraggableBehavior.SetTop(element, GetHorizontal(maxHeight, currentY, element, direction));
                    }
                    else
                    {
                        DraggableBehavior.SetLeft(element, GetVertical(maxWdith, currentX, element, direction));
                        DraggableBehavior.SetTop(element, currentY);
                    }

                    if (orientation == Orientation.Horizontal)
                    {
                        currentX += element.Width + spacing;
                    }
                    else
                    {
                        currentY += element.Height + spacing;
                    }
                    DraggableBehavior.SetIsDraggable(element, false);
                }
        }




        public void DefaultBorder()
        {
            var data = Children;
            CreateBoundingRectBorder((data as IEnumerable<FrameworkElement>).ToList());
        }

        public void NoramlLayout(Canvas canvas, IEnumerable<DragDataBase> children)
        {
            var left = DraggableBehavior.GetLeft(this);
            var top = DraggableBehavior.GetTop(this);
            foreach (var data in Children)
            {

                var tempLeft = DraggableBehavior.GetLeft(data);
                var tempTop = DraggableBehavior.GetTop(data);
                canvas.Children.Add(data);
                DraggableBehavior.SetLeft(data, IsNew ? tempLeft - left : tempLeft);
                DraggableBehavior.SetTop(data, IsNew ? tempTop - top : tempTop);
                DraggableBehavior.SetIsDraggable(data, DraggableBehavior.GetIsDraggable(this));
            }
            IsNew = false;
        }

        public override void LoadImages()
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null && Children != null)
            {
                canvas.Children.Clear();
                if ((IsAutoLayout ?? false))
                {
                    AutoLayout(canvas, Children, Orientation, this.Direction, Spacing);
                }
                else
                {
                    NoramlLayout(canvas, Children);
                }

            }
        }

        public override void SetSize()
        {
        }

        private void CreateBoundingRectBorder(List<FrameworkElement> elements)
        {
            if (elements == null || elements.Count == 0)
            {
                throw new ArgumentException("Elements list cannot be null or empty.");
            }
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (var element in elements)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                double right = left + element.Width;
                double bottom = top + element.Height;
                if (left < minX) minX = left;
                if (top < minY) minY = top;
                if (right > maxX) maxX = right;
                if (bottom > maxY) maxY = bottom;
            }
            Rect rect = new Rect(minX, minY, maxX - minX, maxY - minY);
            this.Width = rect.Width;
            this.Height = rect.Height;
            //this._draggableBehavior.
            DraggableBehavior.SetLeft(this, rect.Left);
            DraggableBehavior.SetTop(this, rect.Top);


        }


    }

    public class DragBindWidget : DragBindBase
    {
        public DragBindWidget()
        {
            IsAutoLayout = false;
            Orientation = Orientation.Horizontal;
            Direction = Direction.TopLeft;
        }
        public bool IsNew { get; set; }

        private bool? isAutoLayout;

        public bool? IsAutoLayout
        {
            get { return isAutoLayout; }
            set { SetProperty(ref isAutoLayout, value); }
        }

        private Orientation orientation;

        public Orientation Orientation
        {
            get { return orientation; }
            set
            {
                SetProperty(ref orientation, value);
            }
        }

        private int spacing;

        public int Spacing
        {
            get { return spacing; }
            set { SetProperty(ref spacing, value); }
        }

        private Direction direction;

        public Direction Direction
        {
            get { return direction; }
            set
            {
                SetProperty(ref direction, value);
            }
        }

        private KeyValuePair<string, string>? appData;

        public KeyValuePair<string, string>? AppData
        {
            get { return appData; }
            set { SetProperty(ref appData, value); }
        }

        private string? associatedData;

        public string? AssociatedData
        {
            get { return associatedData; }
            set { SetProperty(ref associatedData, value); }
        }

        private string? preview;

        public string? Preview
        {
            get { return preview; }
            set { SetProperty(ref preview, value); }
        }

        private string? editBox;

        public string? EditBox
        {
            get { return editBox; }
            set { SetProperty(ref editBox, value); }
        }

        public override IEnumerable<string?>? GetAllImages()
        {
            return null;
        }

        int level = 0;
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<Guid, string> keyValuePairs = new Dictionary<Guid, string>();
        public List<Tuple<string, string, string>> TempData = new List<Tuple<string, string, string>>();

        public void AddSubItems(DragDataBaseXml outXml, IEnumerable<DragBindBase> subItems)
        {
           
            foreach (var item in subItems!)
            {
                var xml = item.GetOutXml();
                keyValuePairs.Add(item.DragId!.Value, xml.Layout.Ref.TrimStart('@'));
                if (xml?.Images?.Any() ?? false)
                {
                    outXml!.Images!.AddRange(xml.Images);
                }
                if (xml?.ImageArrays?.Any() ?? false)
                {
                    outXml!.ImageArrays!.AddRange(xml.ImageArrays);
                }
                if (xml?.DataItemImageNumbers?.Any() ?? false)
                {
                    outXml!.DataItemImageNumbers!.AddRange(xml.DataItemImageNumbers);
                }
                if (xml?.DataItemImageValues?.Any() ?? false)
                {
                    outXml!.DataItemImageValues!.AddRange(xml.DataItemImageValues);
                }
                if (xml?.Widgets?.Any() ?? false)
                {
                    outXml!.Widgets!.AddRange(xml.Widgets);
                }
                //if (item.SubItems?.Any() ?? false)
                //    AddSubItems(outXml, item.SubItems);
            }
        }

        public void SetAlign(Widget widget, Direction direction)
        {
            if (direction == Direction.Center)
            {
                widget.JustifyContent = "center";
                widget.AlignItems = "center";
                widget.AlignContent = widget.AlignItems;
            }
            else if (direction == Direction.TopLeft)
            {
                widget.JustifyContent = "flex-start";
                widget.AlignItems = "flex-start";
                widget.AlignContent = widget.AlignItems;
            }
            else if (direction == Direction.Top)
            {
                widget.JustifyContent = widget.FlexDirection == "row" ? "center" : "flex-start";
                widget.AlignItems = widget.FlexDirection == "row" ? "flex-start" : "center";
                widget.AlignContent = widget.AlignItems;
            }
            else if (direction == Direction.TopRight)
            {
                widget.JustifyContent = widget.FlexDirection == "row" ? "flex-end" : "flex-start";
                widget.AlignItems = widget.FlexDirection == "row" ? "flex-start" : "flex-end";
                widget.AlignContent = widget.AlignItems;
            }
            else if (direction == Direction.Left)
            {
                widget.JustifyContent = widget.FlexDirection == "row" ? "flex-start" : "center";
                widget.AlignItems = widget.FlexDirection == "row" ? "center" : "flex-start";
                widget.AlignContent = widget.AlignItems;
            }
            else if (direction == Direction.Right)
            {
                widget.JustifyContent = widget.FlexDirection == "row" ? "flex-end" : "center";
                widget.AlignItems = widget.FlexDirection == "row" ? "center" : "flex-end";
                widget.AlignContent = widget.AlignItems;
            }
            else if (direction == Direction.BottomLeft)
            {
                widget.JustifyContent = widget.FlexDirection == "row" ? "flex-start" : "flex-end";
                widget.AlignItems = widget.FlexDirection == "row" ? "flex-end" : "flex-start";
                widget.AlignContent = widget.AlignItems;
            }
            else if (direction == Direction.Bottom)
            {
                widget.JustifyContent = widget.FlexDirection == "row" ? "center" : "flex-end";
                widget.AlignItems = widget.FlexDirection == "row" ? "flex-end" : "center";
                widget.AlignContent = widget.AlignItems;
            }
            else if (direction == Direction.BottomRight)
            {
                widget.JustifyContent = "flex-end";
                widget.AlignItems = "flex-end";
                widget.AlignContent = widget.AlignItems;
            }
        }

        //else if (align == Align.center)
        //   {
        //       Canvas.SetLeft(element, left - (element.Width / 2));

        //   }
        //   else
        //   {
        //       Canvas.SetLeft(element, left - element.Width);
        //   }



        public override DragDataBaseXml GetOutXml()
        {
            var outXml = new DragDataBaseXml();
            keyValuePairs = new Dictionary<Guid, string>();
            level = 0;
            AddSubItems(outXml, this.SubItems!);
            Widget widget = new Widget()
            {   
                DragId=this.DragId,
                Items = SubItems?.Select(x => new Model.Item
                {
                    Ref = @$"@{keyValuePairs[x.DragId!.Value]}",
                    X = (int)x.Left,
                    Y = (int)x.Top,

                }).ToList(),
                Height = (int)this.Height,
                Width = (int)this.Width,
                Name = OutXmlHelper.GetWatchElementName(),
                FlexDirection = (IsAutoLayout ?? false) ? (this.Orientation == Orientation.Horizontal ? "row" : "column") : string.Empty,
                JumpApp = AppData?.Value?.ToString(),
                WidgetName = AssociatedData?.ToString() ?? DragName,
                Gap = Spacing,
            };
          
            if (IsAutoLayout ?? false)
                SetAlign(widget, this.Direction);
            outXml.Widgets.Add(widget);
            outXml.Layout = new Layout
            {
                Ref = $"@{widget.Name}",
                X = GetLeft(),
                Y = (int)this.Top,
            };
            return outXml;
        }
    }

    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    public enum Direction
    {
        Center,     // 中心
        TopLeft,    // 左上
        Top,        // 上
        TopRight,   // 右上
        Left,       // 左
        Right,      // 右
        BottomLeft, // 左下
        Bottom,     // 下
        BottomRight // 右下
    }

}
