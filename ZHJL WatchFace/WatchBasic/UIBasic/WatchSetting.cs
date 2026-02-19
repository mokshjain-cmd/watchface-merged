using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.UIBasic
{
    public class WatchSetting: BindableBase
    {
        private string? projectName;

        public string? ProjectName
        {
            get => projectName;
            set => SetProperty(ref projectName,value);
        }


        private int width;

        public int Width
        {
            get { return width; }
            set { SetProperty(ref width, value); }
        }

        private int height;

        public int Height
        {
            get { return height; }
            set { SetProperty(ref height,value); }
        }

        private int thumbnailWidth;

        public int ThumbnailWidth
        {
            get { return thumbnailWidth; }
            set { SetProperty(ref thumbnailWidth,value); }
        }

        private int thumbnailHeight;

        public int ThumbnailHeight
        {
            get { return thumbnailHeight; }
            set { SetProperty(ref thumbnailHeight,value); }
        }


        private int thumbnailX;

        public int ThumbnailX
        {
            get { return thumbnailX; }
            set { SetProperty(ref thumbnailX,value); }
        }


        private int thumbnailY;

        public int ThumbnailY
        {
            get { return thumbnailY; }
            set { SetProperty(ref thumbnailY, value); }
        }
        
        private int maxValue=1500;

        public int MaxValue
        {
            get { return maxValue; }
            set { SetProperty(ref maxValue, value); }
        }


    }
}
