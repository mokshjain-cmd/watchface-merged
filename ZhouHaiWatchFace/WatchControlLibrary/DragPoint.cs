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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WatchControlLibrary.Model;
using System.IO;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using Newtonsoft.Json.Linq;
using Point = System.Windows.Point;
using System.Windows.Threading;
using System.Data.SqlTypes;
using UserControl = System.Windows.Controls.UserControl;
using Image = System.Windows.Controls.Image;

namespace WatchControlLibrary
{
    public class DragPoint : DragDataBase
    {
        static DragPoint()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragPoint), new FrameworkPropertyMetadata(typeof(DragPoint)));
        }


        public double OriginPointX
        {
            get { return (double)GetValue(OriginPointXProperty); }
            set { SetValue(OriginPointXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OriginPointX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OriginPointXProperty =
            DependencyProperty.Register("OriginPointX", typeof(double), typeof(DragPoint), new PropertyMetadata(
                default(double),
                ValueOnChanged
                ));


        public double OriginPointY
        {
            get { return (double)GetValue(OriginPointYProperty); }
            set { SetValue(OriginPointYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OriginPointX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OriginPointYProperty =
            DependencyProperty.Register("OriginPointY", typeof(double), typeof(DragPoint), new PropertyMetadata(
                default(double),
                ValueOnChanged
                ));





        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value",
                typeof(double),
                typeof(DragPoint),
                new PropertyMetadata(
                    default(double),
                    //OnValueChanged));
                    ValueOnChanged));



        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle",
                typeof(double),
                typeof(DragPoint),
                new FrameworkPropertyMetadata(
                    default(double),
                    FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender));



        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(DragPoint), new PropertyMetadata(
                default(double),
                ValueOnChanged
                ));



        public double EndAngle
        {
            get { return (double)GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }

        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(DragPoint), new PropertyMetadata(
                default(double),
                ValueOnChanged
                ));



        public string? Source
        {
            get { return (string?)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(DragPoint), new PropertyMetadata(string.Empty, ValueOnChanged));


        public DragPoint() : base()
        {

        }

        private static void ValueOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragPoint)d;
                if (group != null)
                {
                    group.SetSize();
                    group.LoadImages();
                }
            }
        }

        public override void SetSize()
        {
            if (!string.IsNullOrWhiteSpace(Source))
            {

                var bitmap = new Bitmap(Source);
                this.Height = bitmap.Height;
                this.Width = bitmap.Width; ;
            }
        }

        public void SetAngle()
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null && !string.IsNullOrWhiteSpace(Source) && File.Exists(Source))
            {
                var a = canvas.Children[0] as Image;
                (a.RenderTransform as RotateTransform).Angle = Angle;
                (_draggableBehavior._border.RenderTransform as RotateTransform).Angle = Angle;
            }
        }
        
        public override void LoadImages()
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null && !string.IsNullOrWhiteSpace(Source) && File.Exists(Source))
            {
                canvas.Children.Clear();

                Angle = Interpolate.Linear(new Point { X = 0, Y = 0 }, new Point { X = 360, Y = 360 }, Value);

                if (StartAngle > Angle)
                {
                    Angle = StartAngle;
                }
                if (EndAngle < Angle)
                {
                    Angle = EndAngle;
                }
                var image = new Image
                {
                    Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(Source), UriKind.RelativeOrAbsolute)),
                    Width = this.Width,
                    Height = this.Height,
                    RenderTransform = new RotateTransform
                    {
                        CenterX = OriginPointX,
                        CenterY = OriginPointY,
                        Angle = Angle,
                    },

                };
                _draggableBehavior._border.RenderTransform = new RotateTransform
                {
                    CenterX = OriginPointX + 2,
                    CenterY = OriginPointY + 2,
                    Angle = Angle,
                };


                canvas.Children.Add(image);

            }

            //if (StartAngle > Angle)
            //{
            //    Angle = StartAngle;
            //}
            //if (EndAngle < Angle)
            //{
            //    Angle = EndAngle;
            //}

        }

        public static class Interpolate
        {
            public static double Linear(double x1, double y1, double x2, double y2, double x)
            => y1 + (y2 - y1) / (x2 - x1) * (x - x1);
            public static double Linear(Point p1, Point p2, double x)
                    => Linear(p1.X, p1.Y, p2.X, p2.Y, x);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => TransformPatch(d);

        private static void TransformPatch(DependencyObject d)
        {
            if (d is DragPoint self)
            {
                //self.Angle = Interpolate.Linear(self.Minimum, self.Maximum, self.Value);
                self.Angle = Interpolate.Linear(new Point { X = 0, Y = 0 }, new Point { X = 360, Y = 360 }, self.Value);
                //var canvas = self.GetTemplateChild("PART_Canvas") as Canvas;
                //var a = canvas.Children[0] as Image;
                //(a.RenderTransform as RotateTransform).Angle = self.Angle;
                self.SetAngle();
                //self.LoadImages();
            }
        }

        private static void OnParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => RefreshAll(d);

        private static void RefreshAll(DependencyObject d)
        {
            TransformPatch(d);
        }

    }
    public class DragBindPoint : DragBindBase
    {
        private double startAngle = 0;
        private double endAngle = 360;
        private double originPointX;
        private double originPointY;
        private string? source;
        //private int maximum;
        private double value;
        private double angle;
        //private int minimum;       
        private ushort valueIndex = 0;

        public DragBindPoint(string? source)
        {
            if (source is not null)
            {
                var bitmap = new Bitmap(source);
                Source = source;
                OriginPointX = bitmap.Width * 1.0 / 2;
                OriginPointY = bitmap.Height * 1.0 / 2;
            }
        }

        public double StartAngle
        {
            get { return startAngle; }
            set { SetProperty(ref startAngle, value); }
        }

        public double EndAngle
        {
            get { return endAngle; }
            set { SetProperty(ref endAngle, value); }
        }

        public double OriginPointX
        {
            get { return originPointX; }
            set { SetProperty(ref originPointX, value); }
        }

        public double OriginPointY
        {
            get { return originPointY; }
            set { SetProperty(ref originPointY, value); }
        }

        public string? Source
        {
            get { return source; }
            set { SetProperty(ref source, value); }
        }

        public double Value
        {
            get { return value; }
            set { SetProperty(ref this.value, value); }
        }

        public double Angle
        {
            get { return angle; }
            set { SetProperty(ref angle, value); }
        }

        public ushort ValueIndex
        {
            get { return valueIndex; }
            set { SetProperty(ref valueIndex, value); EndAngle = value == 0 ? 720 : 360; }
        }

        //public int Maximum
        //{
        //    get { return maximum; }
        //    set { SetProperty(ref maximum, value); }
        //}

        //public int Minimum
        //{
        //    get { return minimum; }
        //    set { SetProperty(ref minimum, value); }
        //}

        public override IEnumerable<string?>? GetAllImages()
        {
            yield return Source;
        }



        public override DragDataBaseXml GetOutXml()
        {
            var outXml = new DragDataBaseXml();

            var image = new WatchImage
            {
                Name = OutXmlHelper.GetWatchElementName(),
                Src = Source,
                IsPointer = true,
                
            };
            outXml.Images!.Add(image);

            var dataItem = new DataItemPointer
            {
                Name = OutXmlHelper.GetWatchElementName(),
                //Source = DataItemTypeHelper.DataItemTypes[ItemName].ToString(),
                Source = ValueIndex == 0 ? DataItemTypeHelper.DataItemTypes["Hour"].ToString() : ValueIndex == 1 ? DataItemTypeHelper.DataItemTypes["Minute"].ToString() : DataItemTypeHelper.DataItemTypes["Second"].ToString(),
                Ref = $"@{image.Name}",
                AngleStart = 0,
                AngleRange = ValueIndex == 0 ? 720 : 360,
                ValueStart = 0,
                ValueRange = ValueIndex == 0 ? 24 : 60,
                PivotX = OriginPointX,
                PivotY = OriginPointY
            };
            outXml.DataItemPointers!.Add(dataItem);

            Layout layout = new Layout
            {
                Ref = $"@{dataItem.Name}",
                X = (int)this.Left,
                Y = (int)this.Top,
            };
            outXml!.Layout = layout;
            
            return outXml;

        }
    }
}