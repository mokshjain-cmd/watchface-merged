using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WatchControlLibrary.Model;

namespace WatchControlLibrary
{
    public class AlbumBackground : DragBindBase
    {
        private string? backgroundSource;
        public string? BackgroundSource
        {
            get => backgroundSource;
            set
            {
                SetProperty(ref backgroundSource, value);
                RaisePropertyChanged(nameof(Background));
            }
        }

        //private System.Windows.Media.Brush? background = System.Windows.Media.Brushes.Black;
        //public  System.Windows.Media.Brush? Background { get => background; set => SetProperty(ref background, value); }
        [JsonIgnore]
        public System.Windows.Media.Brush? Background => BackgroundSource is null ? System.Windows.Media.Brushes.Black : new ImageBrush() { ImageSource = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(BackgroundSource), UriKind.RelativeOrAbsolute)) };

        public override IEnumerable<string?>? GetAllImages()
        {
            throw new NotImplementedException();
        }

        public override DragDataBaseXml GetOutXml()
        {
            throw new NotImplementedException();
        }
    }
}
