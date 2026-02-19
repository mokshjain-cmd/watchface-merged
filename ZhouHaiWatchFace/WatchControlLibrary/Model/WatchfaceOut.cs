using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Prism.Mvvm;
using System.ComponentModel;
using System.Windows;
using System.Text.Json.Serialization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Markup;
namespace WatchControlLibrary.Model
{


    [XmlRoot("Watchface")]
    public class WatchfaceOut
    {
        public WatchfaceOut()
        {
            Resources = new Resources();
            Themes = new List<Theme>();
        }

        [XmlAttribute("name")]
        public string? Name { get; set; }

        //[XmlAttribute("cornerX")]
        //public double? CornerX { get; set; }

        //[XmlAttribute("cornerY")]
        //public double? CornerY { get; set; }

        [XmlAttribute("width")]
        public int Width { get; set; }

        [XmlAttribute("height")]
        public int Height { get; set; }

        [XmlAttribute("id")]
        public string? Id { get; set; }

        [XmlAttribute("SKU")]
        public bool Sku { get; set; }

        [XmlAttribute("compressMethod")]
        public string? CompressMethod { get; set; } = "None";

        [XmlAttribute("editable")]
        public bool Editable { get; set; } = false;

        [XmlAttribute("_deviceType")]
        public string? DeviceType { get; set; }

        [XmlElement("Resources")]
        public Resources? Resources { get; set; }

        [XmlElement("Theme")]
        public List<Theme>? Themes { get; set; }

        [XmlIgnore]
        public Dictionary<string, Resources> ElementCache = new Dictionary<string, Resources>();

        //[XmlElement("watch")]       
        //public WatchInfo? WatchInfo { get; set; }
    }

    public class WatchInfo
    {
        public WatchInfo()
        {
            HnTheme = new HnTheme();
        }
        public string? shape { get; set; }

        public string? name { get; set; }

        public string? deviceType { get; set; }

        public string? version { get; set; }

        public string? size { get; set; }

        public string? author { get; set; }

        public string? pkgName { get; set; }

        public string? imageFormat { get; set; } = "indexed8";

        public bool imageCompression { get; set; }

        public string? editorVersion { get; set; }

        public string? webVersionCreatedAt { get; set; }

        public string? editorVersionCreatedAt { get; set; }

        public string? webVersionUpdatedAt { get; set; }

        public string? editorVersionUpdatedAt { get; set; }

        public bool _recolorEnable { get; set; }
        [XmlElement]
        public HnTheme? HnTheme { get; set; }

    }

    public class HnTheme
    {
        public string? title { get; set; } /*= "Tree diagram";*/
        public string designer { get; set; } = "舟海";
        public string screen { get; set; } = "QXHD03";

        public string? briefInfo { get; set; }
        public string version { get; set; } = "qxzhzn.2.0.0";
        public string? extra { get; set; }
        [XmlIgnore]
        public string? extra_zh_title { get; set; }
        [XmlIgnore]
        public string? extra_zh_briefInfo { get; set; }
        [XmlIgnore]
        public string? extra_en_title { get; set; }
        [XmlIgnore]
        public string? extra_en_briefInfo { get; set; }

    }

    public class WatchLanguageInfo
    {
        public string? title { get; set; }

        public string? briefInfo { get; set; }
    }

    public class WatchInfoExtra
    {
        public WatchInfoExtra()
        {
            zh = new WatchLanguageInfo();
            en = new WatchLanguageInfo();
        }
        public WatchLanguageInfo? zh { get; set; }
        public WatchLanguageInfo? en { get; set; }
    }

    public class DragDataBaseXml
    {
        public DragDataBaseXml()
        {
            Images = new List<WatchImage>();
            ImageArrays = new List<ImageArray>();
            DataItemImageNumbers = new List<DataItemImageNumber>();
            DataItemPointers = new List<DataItemPointer>();
            DataItemImageValues = new List<DataItemImageValues>();
            Widgets = new List<Widget>();
            Slots = new List<Slot>();
            Translations = new List<Translation>();
            Sprites = new List<Sprite>();

        }
        public List<DataItemPointer>? DataItemPointers { get; set; }
        public List<WatchImage>? Images { get; set; }
        public List<ImageArray>? ImageArrays { get; set; }
        public List<DataItemImageNumber>? DataItemImageNumbers { get; set; }

        public List<DataItemImageValues>? DataItemImageValues { get; set; }
        public List<Sprite>? Sprites { get; set; }
        public List<Widget>? Widgets { get; set; }
        public List<Slot>? Slots { get; set; }

        public Layout? Layout { get; set; }

        public List<Translation> Translations { get; set; }

        
    }

    public class Resources
    {
        public Resources()
        {
            Images = new List<WatchImage>();
            ImageArrays = new List<ImageArray>();
            DataItemImageValues = new List<DataItemImageValues>();
            DataItemImageNumbers = new List<DataItemImageNumber>();
            DataItemImagePoints = new List<DataItemPointer>();
            Translations = new List<Translation>();
            Widgets = new List<Widget>();
            Slots = new List<Slot>();
            Sprites= new List<Sprite>();
        }
        [XmlElement("Translation")]
        public List<Translation> Translations { get; set; }

        [XmlElement("Image")]
        public List<WatchImage>? Images { get; set; }

        [XmlElement("ImageArray")]
        public List<ImageArray>? ImageArrays { get; set; }

        [XmlElement("DataItemImageNumber")]
        public List<DataItemImageNumber>? DataItemImageNumbers { get; set; }

        [XmlElement("DataItemImageValues")]
        public List<DataItemImageValues>? DataItemImageValues { get; set; }

        [XmlElement("DataItemPointer")]
        public List<DataItemPointer>? DataItemImagePoints { get; set; }

        [XmlElement("Widget")]
        public List<Widget>? Widgets { get; set; }

        [XmlElement("Slot")]
        public List<Slot>? Slots { get; set; }

        [XmlElement("Sprite")]
        public List<Sprite>? Sprites { get; set; }


    }

    public class WatchImage
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("src")]
        public string? Src { get; set; }

        [XmlAttribute("compressMethod")]
        public string? CompressMethod { get; set; } = "None";

        [XmlAttribute("format")]
        public string? Format { get; set; } = "indexed8";

        [XmlIgnore]
        /// <summary>
        /// 是否为指针
        /// </summary>
        public bool IsPointer { get; set; } = false;
        [XmlIgnore]
        /// <summary>
        /// 是否为预览图
        /// </summary>
        public bool IsPreview { get; set; } = false;
    }

    public class ImageArray
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("compressMethod")]
        public string? CompressMethod { get; set; } = "None";

        [XmlAttribute("format")]
        public string? Format { get; set; } = "indexed8";

        [XmlElement("Image")]
        public List<WatchImage>? Images { get; set; }
    }

    public class DataItemImageValues
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("source")]
        public string? Source { get; set; }

        [XmlAttribute("ref")]
        public string? Ref { get; set; }

        [XmlAttribute("rotation")]
        public int Rotation { get; set; }

        [XmlAttribute("supportRecolor")]
        public bool SupportRecolor { get; set; }

        [XmlElement("Param")]
        public List<Param>? Params { get; set; }
    }

    public class DataItemPointer
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("source")]
        public string? Source { get; set; }

        [XmlAttribute("ref")]
        public string? Ref { get; set; }

        [XmlAttribute("supportRecolor")]
        public bool SupportRecolor { get; set; }

        [XmlAttribute("pivotX")]
        public double PivotX { get; set; }

        [XmlAttribute("pivotY")]
        public double PivotY { get; set; }

        [XmlAttribute("renderRule")]
        public string? RenderRule { get; set; } = "alwaysShow";

        [XmlAttribute("parameter")]
        public int Parameter { get; set; } = 1000;

        [XmlAttribute("angleStart")]
        public int AngleStart { get; set; }

        [XmlAttribute("angleRange")]
        public int AngleRange { get; set; }

        [XmlAttribute("valueStart")]
        public int ValueStart { get; set; }

        [XmlAttribute("valueRange")]
        public int ValueRange { get; set; }


    }

    public class DataItemImageNumber : DataItemImageValues
    {


        [XmlAttribute("unitIcon")]
        public string? UnitIcon { get; set; }


        [XmlAttribute("leadingZero")]
        public bool LeadingZero { get; set; }

        [XmlAttribute("decimalOffsetX")]
        public int DecimalOffsetX { get; set; }

        [XmlAttribute("trailingZero")]
        public bool TrailingZero { get; set; }

        [XmlAttribute("totalDigits")]
        public int TotalDigits { get; set; }

        [XmlAttribute("decimalDigits")]
        public int DecimalDigits { get; set; }

        [XmlAttribute("parameter")]
        public int Parameter { get; set; } = 1000;

        [XmlAttribute("renderRule")]
        public string? RenderRule { get; set; } = "alwaysShow";

        [XmlAttribute("align")]
        public string? Align { get; set; } = "left";

        [XmlAttribute("space")]
        public int Space { get; set; }
    }

    public class Sprite
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("ref")]
        public string Ref { get; set; }

        [XmlAttribute("interval")]
        public int Interval { get; set; }

        [XmlAttribute("repeatCount")]
        public int RepeatCount { get; set; }
        [XmlIgnore]
        public int? ImageCount { get; set; }
    }

    public class Widget
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("flex_direction")]
        public string? FlexDirection { get; set; }

        [XmlAttribute("justify_content")]
        public string? JustifyContent { get; set; }

        [XmlAttribute("align_items")]
        public string? AlignItems { get; set; }
        [XmlAttribute("align_content")]
        public string? AlignContent { get; set; }

        [XmlAttribute("jumpApp")]
        public string? JumpApp { get; set; }


        [XmlAttribute("widgetName")]
        public string? WidgetName { get; set; }

        [XmlAttribute("gap")]
        public int Gap { get; set; }

        [XmlAttribute("w")]
        public int Width { get; set; }

        [XmlAttribute("h")]
        public int Height { get; set; }

        [XmlAttribute("editBox")]
        public string? EditBox { get; set; }

        [XmlAttribute("preview")]
        public string? Preview { get; set; }

        [XmlAttribute("groupType")]
        public string? GroupType { get; set; } = "general";

        [XmlElement("Item")]
        public List<Item>? Items { get; set; }

        [XmlIgnore]
        public Guid? DragId { get; set; }
    }

    public class Slot
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("type")]
        public string? Type { get; set; }

        [XmlAttribute("movable")]
        public bool Movable { get; set; }

        [XmlElement("Item")]
        public List<Item>? Items { get; set; } = new List<Item>();

        [XmlElement("Position")]
        public List<Position>? Positions { get; set; } = new List<Position>();
    }
    public class Position
    {
        [XmlAttribute("x")]
        public int X { get; set; }

        [XmlAttribute("y")]
        public int Y { get; set; }

        [XmlAttribute("_curSlotPosition")]
        public bool CurSlotPosition { get; set; }
    }

    public class Item
    {
        public Item()
        {
            Id = OutXmlHelper.GetWatchElementName();
        }
        private int x;
        private int y;
        [XmlAttribute("_id")]
        public string? Id { get; set; }

        [XmlAttribute("ref")]
        public string? Ref { get; set; }

        [XmlAttribute("x")]
        public int X
        {
            get => x;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(X), "坐标X不能为负数");
                x = value;
            }
        }

        [XmlAttribute("y")]
        public int Y
        {
            get => y;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Y), "坐标Y不能为负数");
                y = value;
            }
        }
    }

    public class Theme
    {
        public Theme()
        {
            Layout = new List<Layout>();
            // Id = OutXmlHelper.GetWatchElementName();
        }

        //[XmlAttribute("_id")]
        //public string? Id { get; set; }

        [XmlAttribute("type")]
        public string? Type { get; set; }

        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("bgColor")]
        public string? BgColor { get; set; } = "#000000";

        [XmlAttribute("isPhotoAlbumWatchface")]
        public bool IsPhotoAlbumWatchface { get; set; }

        [XmlAttribute("preview")]
        public string? Preview { get; set; }

        [XmlElement("Layout")]
        public List<Layout>? Layout { get; set; }

        [XmlAttribute("bg")]
        public string? Bg { get; set; }

        [XmlIgnore]
        public string? StyleName {  get; set; }
    }

    public class Layout
    {
        public Layout()
        {
            Id = OutXmlHelper.GetWatchElementName();
        }
        private int x;
        private int y;
        [XmlAttribute("_id")]
        public string? Id { get; set; }

        [XmlAttribute("ref")]
        public string? Ref { get; set; }

        [XmlAttribute("x")]
        public int X
        {
            get => x;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(X), "坐标X不能为负数");
                x = value;
            }
        }

        [XmlAttribute("y")]
        public int Y
        {
            get => y;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Y), "坐标Y不能为负数");
                y = value;
            }
        }



    }

    public class Param
    {
        [XmlAttribute("value")]
        public int Value { get; set; }

        [XmlAttribute("_label")]
        public string? Label { get; set; }
    }

    [XmlRoot("Translation")]
    public class Translation
    {
        public Translation()
        {
            Items = new List<TranslationItem>();
        }
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlElement("Item")]
        public List<TranslationItem>? Items { get; set; }
    }

    public class TranslationItem
    {
        [XmlAttribute("language")]
        public string? Language { get; set; }

        [XmlAttribute("str")]
        public string? Str { get; set; }
    }

}
