using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WatchBasic.UIBasic
{
    public class PreviewModel
    {
        public string? Title { get; set; }

        public string? FolderPath { get; set; }

        public string? WatchId { get; set; }

        public bool? IsUpload { get; set; }

        public string? Lang { get; set; }

        public Layer? Layer { get; set; }

        public BitmapSource? BitmapSource { get; set; }
    }
}
