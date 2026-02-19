using Emgu.CV.PhaseUnwrapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZHAPI
{
    public class JsonWatch
    {
        public string? id { get; set; }

        public string? watchCode { get; set; }
        public int watchVersion { get; set; } = 1;
        public string? shape { get; set; }
        public string? dpi { get; set; }

        public string? thumbnailDpi { get; set; }

        public string? thumbnailOffset { get; set; }

        public string? type { get; set; }

        public string? author { get; set; }

        public string? code { get; set; }

        public string? watchName { get; set; }

        public string? watchDesc { get; set; }

        public background? background { get; set; }

        public IEnumerable<string>? overlayImgPaths { get; set; }

        public IEnumerable<scaleImage>? scaleImages { get; set; }

        public complex? complex { get; set; }

        public IEnumerable<pointer>? pointers { get; set; }

        public IEnumerable<time>? time { get; set; }


    }


    public class scaleImage 
    {
       public string? designsketchImgPath { get; set; }

       public string? overlayImgPath { get; set; }
    }

    public class background
    {
        public string? md5Background { get; set; }
        public string? backgroundImgPath { get; set; }
        public string? md5thumbnail { get; set; }
        public string? thumbnailImgPath { get; set; }
        // public string? overlayImgPath { get; set; }
        public string? designsketchImgPath { get; set; }
    }




    public class complexInfo
    {
        public string? location { get; set; }

        public IEnumerable<InfoDetail>? detail { get; set; }
    }

    public class InfoDetail
    {
        public bool isDefault { get; set; }
        public string? typeName { get; set; }

        public int pointX { get; set; }

        public int pointY { get; set; }

        public string? picPath { get; set; }

    }

    public class complex
    {
        public bool isCompress { get; set; }
        public string? md5 { get; set; }
        public string? path { get; set; }

        public IEnumerable<complexInfo>? infos { get; set; }

    }

    public class pointer
    {
        public bool isCompress { get; set; }
        public string? md5 { get; set; }
        public string? pointerImgPath { get; set; }
        public string? pointerDataPath { get; set; }
        public string? pointerOverlayPath { get; set; }
    }

    public class time
    {
        public time()
        {
            locationInfos = new List<LocationInfo>();
        }
        /// <summary>
        /// 字体名
        /// </summary>
        public string? fontsName { get; set; }

        /// <summary>
        /// 是否压缩
        /// </summary>
        public bool isCompress { get; set; }
        public string? md5 { get; set; }

        /// <summary>
        /// bin文件地址
        /// </summary>
        public string? timeDataPath { get; set; }
        /// <summary>
        /// 位置信息
        /// </summary>
        public IEnumerable<LocationInfo>? locationInfos { get; set; }
    }

    public class LocationInfo
    {
        public LocationInfo()
        {
            textInfos = new List<TextInfo>();
        }
        /// <summary>
        /// 位置名
        /// </summary>
        public string? locationName { get; set; }
        /// <summary>
        /// 设计图地址
        /// </summary>
        public string? timeImgPath { get; set; }
        /// <summary>
        /// 叠加图
        /// </summary>
        public string? timeOverlayPath { get; set; }
        /// <summary>
        /// 字体信息
        /// </summary>
        public IEnumerable<TextInfo>? textInfos { get; set; }

        public bool? isDefault { get; set; }

    }

    public class TextInfo
    {
        /// <summary>
        /// bin文件颜色值写入地址
        /// </summary>
        public int? colorAddr { get; set; }

        /// <summary>
        /// bin文件X写入地址
        /// </summary>
        public int? leftAddr { get; set; }

        /// <summary>
        /// bin文件Y写入地址
        /// </summary>
        public int? topAddr { get; set; }

        /// <summary>
        /// bin文件X写入值
        /// </summary>
        public int? leftValue { get; set; }
        /// <summary>
        /// bin文件Y写入值
        /// </summary>
        public int? topValue { get; set; }

        public int ? isDisableAddr { get; set; }

        public int ? isDisableValue { get; set; }
    }


}
