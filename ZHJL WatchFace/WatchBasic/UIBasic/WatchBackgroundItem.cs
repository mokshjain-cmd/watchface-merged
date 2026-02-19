using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.UIBasic
{
    public class WatchBackgroundItem : BindableBase
    {

        public WatchBackgroundItem(List<LayerGroup>? layers)
        {

            BgImage = layers?.Where(i => i.GroupCode == "0101").FirstOrDefault()?.ShowLayer;
            DesignSketch = layers?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("效果");
            CoordinateImage = layers?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("坐标");
            OverlayImage = layers?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("叠加");
            Thumbnail = layers?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("缩略");
            OverlayImages= layers?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayersByName("叠加");
            
            LayerGroups = layers;
        }

        public WatchBackgroundItem(List<LayerGroup>? layers, string path)
        {

            BgImage = layers?.Where(i => i.GroupCode == "0101").FirstOrDefault()?.ShowLayer;
            DesignSketch = layers?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("效果");
            CoordinateImage = layers?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("坐标");
            OverlayImage = layers?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("叠加");
            Thumbnail = layers?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("缩略");
            OverlayImages = layers?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayersByName("叠加");
            FontImages =$@"{path}\辅助文件".GetLayerGroups(false).SelectMany(i=>i.Layers);
            LayerGroups = layers;
        }

        public List<LayerGroup>? LayerGroups { get; set; }

        private Layer? thumbnail;

        public Layer? Thumbnail
        {
            get => thumbnail;
            set => SetProperty(ref thumbnail, value);
        }
        public void SetLanguage(int order)
        {
            var ratio = DesignSketch.Ratio;
            var designLayers = LayerGroups?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayersByName($"效果");
            var designSketch = designLayers[order];
            
            DesignSketch = designSketch;
            DesignSketch.Ratio = ratio;
            var thumbnailLayers = LayerGroups?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayersByName("缩略");
            Layer thumbnail = null;
            if (thumbnailLayers.Count-1< order) 
            {
                thumbnail = thumbnailLayers[thumbnailLayers.Count - 1];
            }
            else 
            {
                thumbnail = thumbnailLayers[order];
            }
           
            Thumbnail = thumbnail;
        }


        /// <summary>
        /// 效果图
        /// </summary>
        private Layer? designSketch;

        public Layer? DesignSketch
        {
            get => designSketch;
            set
            {
                SetProperty(ref designSketch, value);
            }
        }


        /// <summary>
        /// 背景图
        /// </summary>
        private Layer? bgImage;

        public Layer? BgImage
        {
            get => bgImage;
            set
            {
                SetProperty(ref bgImage, value);
            }
        }


        private IEnumerable<Layer>? overlayImages;

        public IEnumerable<Layer>? OverlayImages
        {
            get { return overlayImages; }
            set { overlayImages = value; }
        }


         private IEnumerable<Layer>? fontImages;

        public IEnumerable<Layer>? FontImages
        {
            get { return fontImages; }
            set { fontImages = value; }
        }





        /// <summary>
        /// 叠加图
        /// </summary>
        private Layer? overlayImage;

        public Layer? OverlayImage
        {
            get => overlayImage;
            set => SetProperty(ref overlayImage, value);
        }
        private Layer? coordinateImage;

        public Layer? CoordinateImage
        {
            get => coordinateImage;
            set => SetProperty(ref coordinateImage, value);
        }
    }
}
