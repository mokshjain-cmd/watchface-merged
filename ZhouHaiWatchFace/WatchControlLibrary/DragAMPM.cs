using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using Image = System.Windows.Controls.Image;
using WatchControlLibrary.Model;

namespace WatchControlLibrary
{
    public class DragAMPM : DragDataBase
    {
        static DragAMPM()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragAMPM), new FrameworkPropertyMetadata(typeof(DragAMPM)));
        }

        public DateTime? SetDateTime
        {
            get { return (DateTime?)GetValue(SetDateTimeProperty); }
            set { SetValue(SetDateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SetDateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SetDateTimeProperty =
            DependencyProperty.Register("SetDateTime", typeof(DateTime?), typeof(DragAMPM), new PropertyMetadata(default(DateTime?), ValueOnChanged));




        public string? AMSource
        {
            get { return (string?)GetValue(AMSourceProperty); }
            set { SetValue(AMSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AMSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AMSourceProperty =
            DependencyProperty.Register("AMSource", typeof(string), typeof(DragAMPM), new PropertyMetadata(string.Empty, ValueOnChanged));

        public string? PMSource
        {
            get { return (string?)GetValue(PMSourceProperty); }
            set { SetValue(PMSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PMSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PMSourceProperty =
            DependencyProperty.Register("PMSource", typeof(string), typeof(DragAMPM), new PropertyMetadata(string.Empty,ValueOnChanged));


        private static void ValueOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragAMPM)d;
                if (group != null)
                {
                    group.SetSize();
                    group.LoadImages();
                }
            }
        }

        public DragAMPM() : base()
        {

        }

        public override void SetSize()
        {
            if (!string.IsNullOrWhiteSpace(AMSource) && !string.IsNullOrWhiteSpace(PMSource))
            {

                var bitmap = new Bitmap(AMSource);
                this.Height = bitmap.Height;
                this.Width = bitmap.Width; ;
            }
        }

        public override void LoadImages()
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null &&!string.IsNullOrWhiteSpace(AMSource)&&!string.IsNullOrWhiteSpace(PMSource) && SetDateTime != null)
            {
                canvas.Children.Clear();
                var path = AMSource;
                if (SetDateTime.Value.Hour > 12) 
                {
                    path = PMSource;
                }

                if (!string.IsNullOrWhiteSpace(path)&&File.Exists(path))
                {
                    var image = new Image
                    {
                        Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(path), UriKind.RelativeOrAbsolute)),
                        Width = this.Width,
                        Height = this.Height,
                    };
                    canvas.Children.Add(image);

                }
            }
        }

       

    }

    public class DragBindAMPM :DragBindBase
    {
        private DateTime? setDateTime;
        private string? amSource;
        private string? pmSource;

        public DateTime? SetDateTime
        {
            get { return setDateTime; }
            set { SetProperty(ref setDateTime, value); }
        }

        public string? AMSource
        {
            get { return amSource; }
            set { SetProperty(ref amSource, value); }
        }

        public string? PMSource
        {
            get { return pmSource; }
            set { SetProperty(ref pmSource, value); }
        }

        public override IEnumerable<string?>? GetAllImages()
        {
            yield return AMSource;
            yield return PMSource;
                
        }

        public override DragDataBaseXml GetOutXml()
        {
            var outXml = new DragDataBaseXml();

            var array = new ImageArray
            {
                Name = OutXmlHelper.GetWatchElementName(),
                Images = GetAllImages().Select(x => new WatchImage
                {
                    Src = x,
                }).ToList(),
            };
            outXml!.ImageArrays!.Add(array);

            var dataItem = new DataItemImageValues
            {
                Name = OutXmlHelper.GetWatchElementName(),
                Source = DataItemTypeHelper.DataItemTypes[ItemName].ToString(),
                Ref = $"@{array.Name}",
            };
            outXml!.DataItemImageValues!.Add(dataItem);

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
