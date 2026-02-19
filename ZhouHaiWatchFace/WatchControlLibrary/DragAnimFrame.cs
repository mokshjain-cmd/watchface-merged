using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using WatchControlLibrary.Model;
using System.IO;
using System.Windows.Media;

namespace WatchControlLibrary
{
    public class DragAnimFrame : DragImageSource
    {

        static DragAnimFrame()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragAnimFrame), new FrameworkPropertyMetadata(typeof(DragAnimFrame)));
        }

        public DragAnimFrame() : base()
        {

        }

        public override void SetSize()
        {
            if (ImageSource?.Any() ?? false)
            {
                var imgPath = ImageSource.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(imgPath))
                {
                    var bitmap = new Bitmap(imgPath);
                    this.Height = bitmap.Height;
                    this.Width = bitmap.Width;
                }
            }
        }

        public bool IsRepeat
        {
            get { return (bool)GetValue(IsRepeatProperty); }
            set { SetValue(IsRepeatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRepeat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRepeatProperty =
            DependencyProperty.Register("IsRepeat", typeof(bool), typeof(DragAnimFrame), new PropertyMetadata(false));

        public int? RepeatCount
        {
            get { return (int?)GetValue(RepeatCountProperty); }
            set { SetValue(RepeatCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RepeatCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RepeatCountProperty =
            DependencyProperty.Register("RepeatCount", typeof(int?), typeof(DragAnimFrame), new PropertyMetadata(0));

        public int? FrameRate
        {
            get { return (int?)GetValue(FrameRateProperty); }
            set { SetValue(FrameRateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FrameRate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrameRateProperty =
            DependencyProperty.Register("FrameRate", typeof(int?), typeof(DragAnimFrame), new PropertyMetadata(0));



        public bool IsPlay
        {
            get { return (bool)GetValue(IsPlayProperty); }
            set { SetValue(IsPlayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPlay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPlayProperty =
            DependencyProperty.Register("IsPlay", typeof(bool), typeof(DragAnimFrame), new PropertyMetadata(false, DoPlay));

        private static async void DoPlay(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var animFrame = (DragAnimFrame)d;
                if (animFrame != null)
                {
                    if (animFrame.IsPlay)
                    {
                        await animFrame.Play();
                        animFrame.IsPlay = false;

                    }

                }
            }
        }

        void ShowIndex(int index)
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null)
            {
                canvas.Children.Clear();
                if ((ImageSource?.Any() ?? false))
                {
                    var imgPath = ImageSource?.Skip(index).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(imgPath) && File.Exists(imgPath))
                    {
                        var image = new System.Windows.Controls.Image
                        {
                            Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(imgPath), UriKind.RelativeOrAbsolute)),
                            Width = this.Width,
                            Height = this.Height,
                        };
                        canvas.Children.Add(image);
                    }
                }
            }
        }

        public async Task Play()
        {
            if (ImageSource!=null&&(FrameRate ?? 0) > 0)
            {

                var count = ImageSource.Count;
                var interval = 1000 / FrameRate;
                var repeatCount = RepeatCount ?? 0;
                while (repeatCount > 0 || IsRepeat)
                {
                    for (int i = 0; i < count; i++)
                    {
                        await Task.Run(() =>
                         {
                             System.Windows.Application.Current.Dispatcher.Invoke( () =>
                             {
                                 ShowIndex(i);
                              
                             });
                             Thread.Sleep(interval!.Value);
                             

                         });

                    }
                    repeatCount--;
                }
                LoadImages();




            }
        }




        public override void LoadImages()
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null)
            {
                canvas.Children.Clear();
                if ((ImageSource?.Any() ?? false))
                {
                    var imgPath = ImageSource?.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(imgPath) && File.Exists(imgPath))
                    {
                        var image = new System.Windows.Controls.Image
                        {
                            Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(imgPath), UriKind.RelativeOrAbsolute)),
                            Width = this.Width,
                            Height = this.Height,
                        };
                        canvas.Children.Add(image);
                    }
                }
            }
        }

        public override void OnFolderSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragAnimFrame)d;
                if (group != null && !string.IsNullOrWhiteSpace(e.NewValue?.ToString()))
                {
                    group.SetSize();
                    group.LoadImages();
                }
            }
        }
    }

    public class DragBindAnimFrame : DragBindBase
    {
        private ObservableCollection<string>? imageSource;
        public ObservableCollection<string>? ImageSource
        {
            get { return imageSource; }
            set { SetProperty(ref imageSource, value); }
        }


        public override IEnumerable<string?>? GetAllImages()
        {
            if (ImageSource != null && ImageSource.Any())
            {
                foreach (var image in ImageSource)
                {
                    yield return image;
                }
            }

        }

        private int? repeatCount;

        public int? RepeatCount
        {
            get { return repeatCount; }
            set { SetProperty(ref repeatCount, value); }
        }

        private int? frameRate;

        public int? FrameRate
        {
            get { return frameRate; }
            set { SetProperty(ref frameRate, value); }
        }

        private bool isRepeat;

        public bool IsRepeat
        {
            get { return isRepeat; }
            set { SetProperty(ref isRepeat, value); }
        }

        private bool isPlay;

        public bool IsPlay
        {
            get { return isPlay; }
            set { SetProperty(ref isPlay, value); }
        }

        public override DragDataBaseXml GetOutXml()
        {
            var outXml = new DragDataBaseXml();
            var array = new ImageArray
            {
                Name = OutXmlHelper.GetWatchElementName(),
                Images = this.ImageSource?.Select(x => new WatchImage
                {
                    Src = x,
                }).ToList(),
            };
            outXml!.ImageArrays!.Add(array);
            var dataItem = new Sprite
            {
                Name = OutXmlHelper.GetWatchElementName(),
                Ref = $"@{array.Name}",
                Interval = (int)(1000/FrameRate),
                RepeatCount = IsRepeat?0:(int)RepeatCount,
                ImageCount=ImageSource?.Count(),
            };
            outXml!.Sprites.Add(dataItem);
            outXml!.Layout = new Layout
            {
                Ref = "@" + dataItem.Name,
                X = GetLeft(),
                Y = (int)Top,
            };
            return outXml;
        }
    }
}
