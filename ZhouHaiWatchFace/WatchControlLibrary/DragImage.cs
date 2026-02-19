using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using CommonServiceLocator;

using Microsoft.Extensions.DependencyInjection;
using WatchControlLibrary.Model;
using Binding = System.Windows.Data.Binding;
using Image = System.Windows.Controls.Image;

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
    ///     <MyNamespace:DragImage/>
    ///
    /// </summary>
    public class DragImage : DragDataBase
    {
        static DragImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragImage), new FrameworkPropertyMetadata(typeof(DragImage)));
        }



        public string?  Source
        {
            get { return (string?)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(DragImage), new PropertyMetadata(string.Empty, ValueOnChanged));

        private static void ValueOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragImage)d;
                if (group != null)
                {
                    group.SetSize();
                    group.LoadImages();
                }
            }
        }

        //public DragImage(IRatioService ratioService) : base()
        //{
        //    _ratioService = ratioService;

        //    // 访问 ratio 值
        //    var height = _ratioService.CurrentRatio.ratio_height;
        //    var width = _ratioService.CurrentRatio.ratio_width;
        //}

        public DragImage() : base()
        {

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
        
        public override void LoadImages()
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null && !string.IsNullOrWhiteSpace(Source)&&File.Exists(Source))
            {
                canvas.Children.Clear();
                var image = new Image
                {
                    Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(Source), UriKind.RelativeOrAbsolute)),
                    Width = this.Width, 
                    Height = this.Height,
                };
                canvas.Children.Add(image);
            }
        }
    }

    public class DragBindImage : DragBindBase
    {
        private string? source;
        //private readonly IRatioService _ratioService;

        //public DragBindImage(IRatioService ratioService) 
        //{
        //    _ratioService = ratioService;

        //    // 访问 ratio 值
        //    var height = _ratioService.CurrentRatio.ratio_height;
        //    var width = _ratioService.CurrentRatio.ratio_width;
        //}

        public string? Source
        {
            get { return source; }
            set { SetProperty(ref source, value); }
        }

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
            };
            outXml.Images!.Add(image);
            Layout layout = new Layout
            {
                Ref = $"@{image.Name}",
                X = (int)this.Left,
                Y = (int)this.Top,
            };
            outXml!.Layout = layout;
            return outXml;
        }
    }
    
}
