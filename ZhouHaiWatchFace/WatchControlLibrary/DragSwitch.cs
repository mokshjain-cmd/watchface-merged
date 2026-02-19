using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using Image = System.Windows.Controls.Image;
using System.IO;
using WatchControlLibrary.Model;
using System.Xml.Linq;

namespace WatchControlLibrary
{
    public class DragSwitch : DragDataBase
    {
        static DragSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragSwitch), new FrameworkPropertyMetadata(typeof(DragSwitch)));
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DragSwitch), new PropertyMetadata(false, ValueOnChanged));

        public string? OpenSource
        {
            get { return (string?)GetValue(OpenSourceProperty); }
            set { SetValue(OpenSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenSourceProperty =
            DependencyProperty.Register("OpenSource", typeof(string), typeof(DragSwitch), new PropertyMetadata(string.Empty, ValueOnChanged));

        public string? CloseSource
        {
            get { return (string?)GetValue(PMSourceProperty); }
            set { SetValue(PMSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PMSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PMSourceProperty =
            DependencyProperty.Register("PMSource", typeof(string), typeof(DragSwitch), new PropertyMetadata(string.Empty, ValueOnChanged));


        private static void ValueOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragSwitch)d;
                if (group != null)
                {
                    group.SetSize();
                    group.LoadImages();
                }
            }
        }

        public DragSwitch() : base()
        {

        }

        public override void SetSize()
        {
            if (!string.IsNullOrWhiteSpace(OpenSource) && !string.IsNullOrWhiteSpace(CloseSource))
            {

                var bitmap = new Bitmap(OpenSource);
                this.Height = bitmap.Height;
                this.Width = bitmap.Width; ;
            }
        }

        public override void LoadImages()
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null && !string.IsNullOrWhiteSpace(OpenSource) && !string.IsNullOrWhiteSpace(CloseSource))
            {
                canvas.Children.Clear();
                var path = OpenSource;
                if (!IsOpen)
                {
                    path = CloseSource;
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

    public class DragBindSwitch : DragBindBase
    {
        private bool isOpen;
        private string? openSource;
        private string? closeSource;

        public bool IsOpen
        {
            get { return isOpen; }
            set { SetProperty(ref isOpen, value); }
        }

        public string? OpenSource
        {
            get { return openSource; }
            set { SetProperty(ref openSource, value); }
        }

        public string? CloseSource
        {
            get { return closeSource; }
            set { SetProperty(ref closeSource, value); }
        }

        public override IEnumerable<string?>? GetAllImages()
        {
            if(ItemName== "温度单位") 
            {
                yield return OpenSource;
                yield return CloseSource;
            }
            else 
            {
                yield return CloseSource;
                yield return OpenSource;
            }
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
            outXml.DataItemImageValues!.Add(dataItem);

            Layout layout = new Layout
            {
                Ref = $"@{dataItem.Name}",
                X = GetLeft(),
                Y = (int)this.Top,
            };
            outXml!.Layout = layout;
            return outXml;
        }
    }

}
