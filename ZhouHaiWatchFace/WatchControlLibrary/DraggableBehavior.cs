using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using Point = System.Windows.Point;
using Brushes = System.Windows.Media.Brushes;
using Cursors = System.Windows.Input.Cursors;
using Panel = System.Windows.Controls.Panel;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Control = System.Windows.Controls.Control;
using System.Diagnostics;
using System.Xml.Linq;
using System.Windows.Controls.Primitives;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Cursor = System.Windows.Input.Cursor;

namespace WatchControlLibrary
{
    public class DraggableBehavior
    {
        private readonly FrameworkElement _target;
        public Border _border;
        private bool _isMouseDown;
        private Point _mouseDownPosition;
        private Point _mouseDownControlPosition;
        private Thumb _topLeft, _topRight, _bottomLeft, _bottomRight;
        private Thumb _left, _right, _top, _bottom;

        public Action<FrameworkElement> ShowSubline;//辅助线
        public Action HideSubLine;//隐藏辅助线

        //public static FrameworkElement oldTarget;

        public void AttachThumbs()
        {
            _topLeft = CreateThumb(Cursors.SizeNWSE, VerticalAlignment.Top, HorizontalAlignment.Left, new Thickness(-5, -5, 0, 0));
            _topRight = CreateThumb(Cursors.SizeNESW, VerticalAlignment.Top, HorizontalAlignment.Right, new Thickness(0, -5, -5, 0));
            _bottomLeft = CreateThumb(Cursors.SizeNESW, VerticalAlignment.Bottom, HorizontalAlignment.Left, new Thickness(-5, 0, 0, -5));
            _bottomRight = CreateThumb(Cursors.SizeNWSE, VerticalAlignment.Bottom, HorizontalAlignment.Right, new Thickness(0, 0, -5, -5));
            _left = CreateThumb(Cursors.SizeWE, VerticalAlignment.Center, HorizontalAlignment.Left, new Thickness(-5, 0, 0, 0));
            _right = CreateThumb(Cursors.SizeWE, VerticalAlignment.Center, HorizontalAlignment.Right, new Thickness(0, 0, -5, 0));
            _top = CreateThumb(Cursors.SizeNS, VerticalAlignment.Top, HorizontalAlignment.Center, new Thickness(0, -5, 0, 0));
            _bottom = CreateThumb(Cursors.SizeNS, VerticalAlignment.Bottom, HorizontalAlignment.Center, new Thickness(0, 0, 0, -5));

            _topLeft.DragDelta += (s, e) => ResizeTopLeft(e);
            _topRight.DragDelta += (s, e) => ResizeTopRight(e);
            _bottomLeft.DragDelta += (s, e) => ResizeBottomLeft(e);
            _bottomRight.DragDelta += (s, e) => ResizeBottomRight(e);
            _left.DragDelta += (s, e) => ResizeLeft(e);
            _right.DragDelta += (s, e) => ResizeRight(e);
            _top.DragDelta += (s, e) => ResizeTop(e);
            _bottom.DragDelta += (s, e) => ResizeBottom(e);
            _border.Child = new Grid
            {
                Children = { _topLeft, _topRight, _bottomLeft, _bottomRight, _left, _right, _top, _bottom }
            };
        }

        private Thumb CreateThumb(Cursor cursor, VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment, Thickness margin)
        {
            return new Thumb
            {
                Width = 6,
                Height = 6,
                Background = Brushes.White,
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(1),
                Cursor = cursor,
                VerticalAlignment = verticalAlignment,
                HorizontalAlignment = horizontalAlignment,
                Margin = margin
            };
        }

        private void ResizeTopLeft(DragDeltaEventArgs e)
        {
            double newWidth = _target.Width - e.HorizontalChange;
            double newHeight = _target.Height - e.VerticalChange;

            if (newWidth > 0 && newHeight > 0)
            {
                SetLeft(_target, GetLeft(_target) + e.HorizontalChange);
                SetTop(_target, GetTop(_target) + e.VerticalChange);
                _target.Width = newWidth;
                _target.Height = newHeight;
                UpdateBorderPosition();
            }
        }

        private void ResizeTopRight(DragDeltaEventArgs e)
        {
            double newWidth = _target.Width + e.HorizontalChange;
            double newHeight = _target.Height - e.VerticalChange;

            if (newWidth > 0 && newHeight > 0)
            {
                SetTop(_target, GetTop(_target) + e.VerticalChange);
                _target.Width = newWidth;
                _target.Height = newHeight;
                UpdateBorderPosition();
            }
        }

        private void ResizeBottomLeft(DragDeltaEventArgs e)
        {
            double newWidth = _target.Width - e.HorizontalChange;
            double newHeight = _target.Height + e.VerticalChange;

            if (newWidth > 0 && newHeight > 0)
            {
                SetLeft(_target, GetLeft(_target) + e.HorizontalChange);
                _target.Width = newWidth;
                _target.Height = newHeight;
                UpdateBorderPosition();
            }
        }

        private void ResizeBottomRight(DragDeltaEventArgs e)
        {
            double newWidth = _target.Width + e.HorizontalChange;
            double newHeight = _target.Height + e.VerticalChange;
            if (newWidth > 0 && newHeight > 0)
            {
                _target.Width = newWidth;
                _target.Height = newHeight;
                UpdateBorderPosition();
            }
        }

        private void ResizeLeft(DragDeltaEventArgs e)
        {
            double newWidth = _target.Width - e.HorizontalChange;

            if (newWidth > 0)
            {
                SetLeft(_target, GetLeft(_target) + e.HorizontalChange);
                _target.Width = newWidth;
                UpdateBorderPosition();
            }
        }

        private void ResizeRight(DragDeltaEventArgs e)
        {
            double newWidth = _target.Width + e.HorizontalChange;

            if (newWidth > 0)
            {
                _target.Width = newWidth;
                UpdateBorderPosition();
            }
        }

        private void ResizeTop(DragDeltaEventArgs e)
        {
            double newHeight = _target.Height - e.VerticalChange;

            if (newHeight > 0)
            {
                SetTop(_target, GetTop(_target) + e.VerticalChange);
                _target.Height = newHeight;

                UpdateBorderPosition();
            }
        }

        private void ResizeBottom(DragDeltaEventArgs e)
        {
            double newHeight = _target.Height + e.VerticalChange;

            if (newHeight > 0)
            {
                _target.Height = newHeight;
                UpdateBorderPosition();
            }
        }
        public static readonly DependencyProperty IsDraggableProperty = DependencyProperty.RegisterAttached(
            "IsDraggable", typeof(bool), typeof(DraggableBehavior), new PropertyMetadata(true, OnIsDraggableChanged));

        public static bool GetIsDraggable(UIElement element)
        {
            return (bool)element.GetValue(IsDraggableProperty);
        }
        public static void SetIsDraggable(UIElement element, bool value)
        {
            element.SetValue(IsDraggableProperty, value);
            if (element is DragNormalDateTime t)
            {
                // Debug.WriteLine(IsDraggableProperty);
            }
        }
        private static void OnIsDraggableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (d is FrameworkElement target)
            //{
            //    if ((bool)e.NewValue)
            //    {
            //        // Attach the behavior
            //        var behavior = new DraggableBehavior(target);

            //    }
            //    else
            //    {
            //        if (target.Tag is DraggableBehavior behavior)
            //        {
            //            behavior.Detach();

            //        }
            //    }
            //}
        }

        public DraggableBehavior(FrameworkElement target)
        {
            _target = target;
            var drag = target as IDraggable;
            _border = new Border
            {
                BorderBrush = Brushes.DeepSkyBlue,
                BorderThickness = new Thickness(1),
                Visibility = Visibility.Collapsed,
            };
            //SetLeft(_target, 0);//暂时取消
            //SetTop(_target, 0);
            // Attach();
        }

        public void Attach()
        {
            _target.PreviewMouseLeftButtonDown += MouseDown;
            _target.MouseMove += MouseMove;
            _target.PreviewMouseLeftButtonUp += MouseUp;
            _target.MouseEnter += MouseEnter;
            _target.MouseLeave += MouseLeave;
            _target.Loaded += TargetLoaded;
            _target.SizeChanged += _target_SizeChanged;
            _target.Cursor = Cursors.Hand;
            SetIsDraggable(_target, true);
            var parent = _target.Parent as Panel;
            if (parent != null && !parent.Children.Contains(_border))
            {
                parent.Children.Add(_border);
                Panel.SetZIndex(_border, 999);
            }
        }

        private void _target_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateBorderPosition();
        }

        public void Detach()
        {
            SetIsDraggable(_target, false);
            _target.PreviewMouseLeftButtonDown -= MouseDown;
            _target.MouseMove -= MouseMove;
            _target.PreviewMouseLeftButtonUp -= MouseUp;
            _target.MouseEnter -= MouseEnter;
            _target.MouseLeave -= MouseLeave;
            _target.Loaded -= TargetLoaded;
            _target.SizeChanged -= _target_SizeChanged;
            _target.Cursor = Cursors.Arrow; // 恢复默认光标
        }

        private void TargetLoaded(object sender, RoutedEventArgs e)
        {
            // 强制进行布局和测量
            _target.UpdateLayout();
            UpdateBorderPosition();
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!GetIsDraggable(_target)) return;
            //if(_target != oldTarget)
            //{
            //    Panel.SetZIndex(_target, 3);
            //   if(oldTarget != null) Panel.SetZIndex(oldTarget, 1);
            //}
            _isMouseDown = true;
            //_mouseDownPosition = e.GetPosition(_target.Parent as UIElement);
            _mouseDownPosition = e.GetPosition(VisualTreeHelper.GetParent(_target) as UIElement);
            var left = double.IsNaN(GetLeft(_target)) ? 0 : GetLeft(_target);
            var top = double.IsNaN(GetTop(_target)) ? 0 : GetTop(_target);
            //var left = double.IsNaN(GetLeft(_target)) ? 0 : GetLeft(_target);
            //var top = double.IsNaN(GetTop(_target)) ? 0 : GetTop(_target);
            _mouseDownControlPosition = new Point(left, top);
            _target.CaptureMouse();
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown && GetIsDraggable(_target))
            {
                //var pos = e.GetPosition(_target.Parent as UIElement);
                var pos = e.GetPosition(VisualTreeHelper.GetParent(_target) as UIElement);
                var delta = pos - _mouseDownPosition;
                SetLeft(_target, (int)(_mouseDownControlPosition.X + delta.X));
                SetTop(_target, (int)(_mouseDownControlPosition.Y + delta.Y));
                UpdateBorderPosition();
                e.Handled = true;
                var drag = _target as IDraggable;
                if (drag?._draggableBehavior?.ShowSubline != null)
                    drag?._draggableBehavior?.ShowSubline(_target);
            }
          
        }


        private bool alwaysShow;
        public bool AlwaysShow
        {
            get { return alwaysShow; }

            set
            {
                alwaysShow = value;
                if (!alwaysShow)
                {
                    _border.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _border.Visibility = Visibility.Visible;
                }
            }
        }

        private void MouseEnter(object sender, MouseEventArgs e)
        {
            //if (!AlwaysShow) return;
            if (!GetIsDraggable(sender as FrameworkElement)) return;
            _border.Visibility = Visibility.Visible;
            UpdateBorderPosition();
            e.Handled = true;
        }

        private void MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isMouseDown && !AlwaysShow)
            {
                _border.Visibility = Visibility.Collapsed;
            }
            MyCanvas.DistancePath.Visibility = Visibility.Collapsed;
        }

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            _target.ReleaseMouseCapture();
            if (HideSubLine != null)
                HideSubLine();
        }

        public void UpdateBorderPosition()
        {
            if (_border != null)
            {
                Canvas.SetLeft(_border, Canvas.GetLeft(_target) - 2);
                Canvas.SetTop(_border, Canvas.GetTop(_target) - 2);
                _border.Width = _target.ActualWidth + 4;
                _border.Height = _target.ActualHeight + 4;
            }
        }

        public static readonly DependencyProperty TopProperty = DependencyProperty.RegisterAttached(
            "Top", typeof(double), typeof(DraggableBehavior), new PropertyMetadata(double.NaN, OnPositionChanged));

        public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached(
            "Left", typeof(double), typeof(DraggableBehavior), new PropertyMetadata(double.NaN, OnPositionChanged));



        public static Align GetAlign(DependencyObject? obj)
        {
            if (obj == null) return default(Align);
            return (Align)obj.GetValue(AlignProperty);
        }

        public static void SetAlign(DependencyObject? obj, Align value)
        {
            if (obj == null) return;
            obj.SetValue(AlignProperty, value);
        }

        // Using a DependencyProperty as the backing store for Align.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlignProperty =
            DependencyProperty.RegisterAttached("Align", typeof(Align), typeof(DraggableBehavior), new PropertyMetadata(Align.left, OnPositionChanged));

        public static double GetTop(FrameworkElement element)
        {
            return (double)element.GetValue(TopProperty);
        }

        public static void SetTop(FrameworkElement element, double? value)
        {
            element.SetValue(TopProperty, value);
        }

        public static double GetLeft(FrameworkElement element)
        {
            return (double)element.GetValue(LeftProperty);
        }

        public static void SetLeft(FrameworkElement element, double? value)
        {
            element.SetValue(LeftProperty, value);
        }

        static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var element = d as FrameworkElement;
            if (element != null)
            {
                if (e.Property == TopProperty)
                {
                    Canvas.SetTop(element, (double)e.NewValue);
                }
                else if (e.Property == LeftProperty)
                {
                    SetPostion(element, (double)e.NewValue);
                }
                else if (e.Property == AlignProperty)
                {
                    var left = double.IsNaN(GetLeft(element)) ? 0 : GetLeft(element);
                    SetPostion(element, left);
                }
                var drag = element as IDraggable;
                drag?._draggableBehavior?.UpdateBorderPosition();


            }
        }



        public static void SetPostion(FrameworkElement element)
        {
            var align = GetAlign(element);
            var left = GetLeft(element);
            if (align == Align.left)
            {
                Canvas.SetLeft(element, left);
            }
            else if (align == Align.center)
            {
                Canvas.SetLeft(element, left - (element.Width / 2));

            }
            else
            {
                Canvas.SetLeft(element, left - element.Width);
            }


        }

        public static void SetPostion(FrameworkElement element, double left)
        {
            var align = GetAlign(element);
            if (align == Align.left)
            {
                Canvas.SetLeft(element, left);
            }
            else if (align == Align.center)
            {
                Canvas.SetLeft(element, left - (Math.Max(element.ActualWidth, element.Width) / 2));
            }
            else
            {
                Canvas.SetLeft(element, left - (Math.Max(element.ActualWidth, element.Width)));
            }
        }





    }



}
