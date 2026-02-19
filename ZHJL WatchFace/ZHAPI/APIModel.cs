using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZHAPI
{

    /// <summary>
    /// 主表盘
    /// </summary>
    public class WatchMain
    {
        public string? dialCode { get; set; }
        public string? dialName { get; set; }
        public string? dialDesc { get; set; }
        public long deviceType { get; set; }
        public int deviceVersion { get; set; } = 1;
        public int sort { get; set; } = 0;
        public int backgroundBinCompressed { get; set; }
        public int pointerCompressed { get; set; }
        public int complexBinCompressed { get; set; }
        public int highLowMark { get; set; }
        [File]
        public string? defaultBackgroundImage { get; set; }

        [File]
        public List<string>? backgroundOverlay { get; set; }

        [File]
        public string? complicationsBin { get; set; }

        [File]
        public string? renderings { get; set; }

        public int? deviceParsingRules { get; set; }
        public string? dpi { get; set; }
        public string? thumbnailDpi { get; set; }
        public string? thumbnailOffset { get; set; }

        public int languageCode { get; set; }

        public int dialType { get; set; }
    }

    /// <summary>
    /// 指针文件
    /// </summary>
    public class WatchPointer
    {
        public string? dialCode { get; set; }
        public int deviceType { get; set; }
        public int type { get; set; }
        public string? binName { get; set; }
        [File]
        public string? binFile { get; set; }
        [File]
        public string? renderingsFile { get; set; }
        public int sort { get; set; } = 0;
        public string? pointerCoordinatesX { get; set; } = "0";
        public string? pointerCoordinatesY { get; set; } = "0";

        [File]
        public string? pointerPictureFile { get; set; }

    }

    public class WatchPointScale
    {
        public string? fCode { get; set; }
        public long? fDeviceType { get; set; }
        public int fType { get; set; }
        [File]
        public string? dialFile { get; set; }
        [File]
        public string? renderingsFile { get; set; }
        public int fSort { get; set; }
    }

    public class WatchTime
    {
        public string? dialCode { get; set; }
        public int? deviceType { get; set; }

        public string? fontsName { get; set; }
        /// <summary>
        /// bin文件名
        /// </summary>
        public string? binName { get; set; }
        /// <summary>
        /// bin文件
        /// </summary>
        [File]
        public string? binFile { get; set; }
        /// <summary>
        /// 效果图文件
        /// </summary>
        [File]
        public string? renderingsFile { get; set; }
    }



    public class WatchTimeLocation
    {
        /// <summary>
        /// 位置名称
        /// </summary>
        public string? fLocationName { get; set; }

        public string? fTextInfos { get; set; }

        public bool? fDefault { get; set; }
        [File]
        public string? backimageFile { get; set; }

        [File]
        public string? effectFile { get; set; }

        public string? id { get; set; }
    }

    public class WatchDetail
    {
        public string? dialCode { get; set; }
        public long deviceType { get; set; }
        public int type { get; set; }
        public string? dataElementCode { get; set; }
        public string? dataElementX { get; set; }
        public string? dataElementY { get; set; }
        [File]
        public string? imgFile { get; set; }

        public int selected { get; set; }

    }

    public class FileAttribute : Attribute
    {

    }



    public class Result
    {
        public int code { get; set; }
        public string? msg { get; set; }
    }

    public class TimeResult : Result
    {
        public string Id { get; set; }
    }

    public class SearchRes : Result
    {
        public IEnumerable<string>? list { get; set; }
    }


}
