using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO;
using WatchControlLibrary.Model;
using Prism.Mvvm;

namespace WatchControlLibrary
{
    public class DragKeyValue : DragImageSource
    {
        static DragKeyValue()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragKeyValue), new FrameworkPropertyMetadata(typeof(DragKeyValue)));
        }

        public DragKeyValue() : base()
        {

        }


        public int CurrentNum
        {
            get { return (int)GetValue(CurrentNumProperty); }
            set { SetValue(CurrentNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentNumProperty =
            DependencyProperty.Register("CurrentNum", typeof(int), typeof(DragKeyValue), new PropertyMetadata(0, ValueChanged));


        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragKeyValue)d;
                if (group != null)
                {
                    group.SetSize();
                    group.LoadImages();
                }
            }
        }


        public override void LoadImages()
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null)
            {
                canvas.Children.Clear();
                if (ImageSource?.Any() ?? false)
                {
                    var imgPath = ImageSource?.Skip(CurrentNum).FirstOrDefault();
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

        public override void OnFolderSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragKeyValue)d;
                if (group != null && !string.IsNullOrWhiteSpace(e.NewValue?.ToString()))
                {

                    group.SetSize();
                    group.LoadImages();
                }
            }
        }



    }

    public class DragBindKeyValue : DragBindBase
    {
        private int maxNum;
        private int currentNum;
        private ObservableCollection<string>? imageSource;

        public IDictionary<int, string> KeyValues { get; set; }
        public DragBindKeyValue()
        {
            KeyValues = new Dictionary<int, string>();
        }

        public DragBindKeyValue(IDictionary<int, string> keyValues, IEnumerable<string> path)
        {
            KeyValues = keyValues;
            this.MaxNum = keyValues.Count - 1;
            this.MinNum = 0;
            if (path.Count() < keyValues.Count)
            {
                var temp = path.ToList();
                temp.AddRange(Enumerable.Range(0, keyValues.Count() - path.Count()).Select(s => ""));
                ImageSource = new ObservableCollection<string>(temp);
            }
            else
                ImageSource = new ObservableCollection<string>(path);

            ValueDataImages = new ObservableCollection<KeyValueData>(KeyValues.Select((x, index) => new KeyValueData { Key = x.Value, Value = ImageSource[index] }));
        }

        public KeyValuePair<int, string>? KeyValue
        {
            get
            {
                if (KeyValues.Any() && CurrentNum <= KeyValues.Count() - 1)
                {
                    return KeyValues.ToArray()[CurrentNum];
                }

                return null;
            }
        }

        public ObservableCollection<KeyValueData>? ValueDataImages { get; set; }
        //public IDictionary<string, string?>? ValuesImages
        //{
        //    get
        //    {
        //        if (ImageSource != null && ImageSource.Any())
        //            return  (from k in KeyValues.Select((i, idx) => new { idx = idx, pair = i })
        //                    join i in ImageSource.Select((i, idx) => new { idx = idx, image = i })
        //                    on k.idx equals i.idx into temp
        //                    from t in temp.DefaultIfEmpty()
        //                    select new 
        //                    {
        //                        key = k.pair.Value,
        //                        value = t?.image,

        //                    }).ToDictionary(x => x.key, x => x.value ?? null);
        //    //});
        //        return null;
        //    }
        //}

        public int CurrentNum
        {
            get { return currentNum; }
            set
            {
                SetProperty(ref currentNum, value);
                RaisePropertyChanged(nameof(KeyValue));
            }
        }

        public ObservableCollection<string>? ImageSource
        {
            get { return imageSource; }
            set
            {
                SetProperty(ref imageSource, value);
                //RaisePropertyChanged(nameof(ValuesImages));
            }
        }

        public override IEnumerable<string?>? GetAllImages()
        {
            if (ImageSource != null && ImageSource.Any())
            {
                foreach (var item in ImageSource)
                {
                    yield return item;
                }
            }
        }

        ImageArray GetImageArrayByWeather()
        {
            var imageArray = from groupInfo in
                       (from k in this.KeyValues.Select((kv, index) => new { kv = kv, idx = index })
                        join m in this.ImageSource!.Select((img, index) => new { img = img, idx = index }) on k.idx equals m.idx
                        select new
                        {
                            img = m.img,
                            groupId = k.kv.Key
                        })
                             join w in StaticData.Weathers on groupInfo.groupId equals w.GroupId into temp2
                             from t in temp2.DefaultIfEmpty()
                             select groupInfo.img;
            return new ImageArray
            {
                Name = OutXmlHelper.GetWatchElementName(),
                Images = imageArray?.Select(x => new WatchImage
                {
                    Src = x,
                }).ToList(),
            };
        }

        public override DragDataBaseXml GetOutXml()
        {
            var outXml = new DragDataBaseXml();
            //ItemName 天气
            ImageArray? array = default;
            if (ItemName == "天气")
            {
                array = GetImageArrayByWeather();
            }
            else
            {
                array = new ImageArray
                {
                    Name = OutXmlHelper.GetWatchElementName(),
                    Images = this.ImageSource?.Select(x => new WatchImage
                    {
                        Src = x,
                    }).ToList(),
                };
            }

            outXml!.ImageArrays!.Add(array);
            var dataItem = new DataItemImageValues
            {
                Name = OutXmlHelper.GetWatchElementName(),
                Source = DataItemTypeHelper.DataItemTypes[ItemName].ToString(),
                Ref = $"@{array.Name}",

            };
            if (ItemName == "天气")
            {
                dataItem.Params = StaticData.Weathers.Select(x => new Param
                {
                    Value = x.Id,
                    Label = x.Content_CN

                }).ToList();
            }
            else
            {
                dataItem.Params = KeyValues.Select(x => new Param
                {
                    Value = x.Key,
                    Label = x.Value,
                }).ToList();
            }
            outXml!.DataItemImageValues!.Add(dataItem);
            outXml!.Layout = new Layout
            {
                Ref = "@" + dataItem.Name,
                X = GetLeft(),
                Y = (int)Top,
            };
            return outXml;
        }
    }

    public class KeyValueData : Snapshoot
    {
        private string? key;
        public string? Key
        {
            get => key;
            set => SetProperty(ref key, value);
        }

        private string? _value;
        public string? Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }
}
