
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using WatchControlLibrary;
using WatchControlLibrary.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Path = System.IO.Path;



namespace WatchBin.Model
{
    public class BinHelper
    {
        static BinFileHeader SetHeader(WatchfaceOut watchXml)
        {
            var header = new BinFileHeader();
            var iDBytes = Encoding.UTF8.GetBytes(watchXml.Id!);
            Array.Resize(ref iDBytes, 64);
            header.Id = iDBytes;
            var nameBytes = Encoding.UTF8.GetBytes(watchXml.Name!);
            Array.Resize(ref nameBytes, 64);
            header.Name = nameBytes;
            header.Version = new Version();
            header.ThemeCount = (byte)(watchXml?.Themes?.Count ?? 0);
            var isAlbum = watchXml?.Themes?.Any(x => x.IsPhotoAlbumWatchface) ?? false;
            var isAod = watchXml?.Themes?.Any(x => x.Type == "AOD") ?? false;
            header.SetFlags(isAlbum, isAod);
            return header;
        }

        static List<RecordBase> GetRecordBases(Theme theme, Resources resources, int index, string styleName)
        {
            switch (index)
            {
                case 0:
                    return theme.Layout!
                              .Select((x, idx) => new RecordBase((byte)RecordType.Layout, true, (byte)0, (uint)idx, x!.Id!, styleName))
                              .ToList();

                case 2:
                    return resources.Images!.Where(x => !x.IsPreview)
                              .Select((x, idx) => new RecordBase((byte)RecordType.Img, true, (byte)0, (uint)idx, x!.Name!, styleName))
                              .ToList();

                case 3:
                    return resources.ImageArrays!
                              .Select((x, idx) => new RecordBase((byte)RecordType.ImgArray, true, (byte)0, (uint)idx, x!.Name!, styleName))
                              .ToList();
                case 4:
                    return resources.Sprites!
                              .Select((x, idx) => new RecordBase((byte)RecordType.Sprite, true, (byte)0, (uint)idx, x!.Name!, styleName))
                              .ToList();

                case 7:
                    return (resources.DataItemImageValues!.Select(x => x!.Name!)
                           .Concat(resources.DataItemImageNumbers!.Select(x => x.Name!))
                           .Concat(resources.DataItemImagePoints!.Select(x => x.Name!)))
                           .Select((x, idx) => new RecordBase((byte)RecordType.Data, true, (byte)0, (uint)idx, x, styleName)).ToList();

                case 8:
                    return resources.Slots!.Select((x, idx) => new RecordBase((byte)RecordType.Slot, true, (byte)0, (uint)idx, x!.Name!, styleName))
                              .ToList();

                case 9:
                    return resources.Widgets!
                              .Select((x, idx) => new RecordBase((byte)RecordType.Widget, true, (byte)0, (uint)idx, x!.Name!, styleName))
                              .ToList();

                default:
                    return new List<RecordBase>();
            }
        }

        public static byte[] GetBin(WatchFace watchface)
        {

            OutXmlHelper.AllNames.Clear();
            WatchBinFile binFile = new WatchBinFile();
            var xmlWatch = watchface.GetWatchfaceOut();
            binFile.Header = SetHeader(xmlWatch);

            //Theme
            foreach (var style in watchface.WatchStyles!)
            {

                var resources = xmlWatch.ElementCache[style.StyleName!];
                var xmlTheme = xmlWatch.Themes!.FirstOrDefault(x => x.StyleName == style.StyleName);
                var theme = new BinTheme(style.StyleName!);
                if (xmlTheme != null)
                    //RECORD_TYPE_LAYOUT = 0x00, /* 0x00 */
                    theme.BinRecordsTable![0].RecordNum = (uint)(xmlTheme.Layout!.Count());
                //RECORD_TYPE_IMG, /* 0x02 */
                theme.BinRecordsTable![2].RecordNum = (uint)(resources.Images!.Where(x => !x.IsPreview)!.Count());
                //RECORD_TYPE_IMG_ARRAY, /* 0x03 */
                theme.BinRecordsTable![3].RecordNum = (uint)(resources.ImageArrays!.Count());
                //RECORD_TYPE_SPRITE, /* 0x04 */
                theme.BinRecordsTable![4].RecordNum = (uint)(resources.Sprites!.Count());
                //RECORD_TYPE_DATA, /* 0x07 */
                theme.BinRecordsTable![7].RecordNum = (uint)(resources.DataItemImageValues!.Count()
                                                           + resources.DataItemImageNumbers!.Count()
                                                           + resources.DataItemImagePoints!.Count());
                //RECORD_TYPE_SLOT, /* 0x08 */
                theme.BinRecordsTable![8].RecordNum = (uint)(resources.Slots!.Count());
                //RECORD_TYPE_WIDGET, /* 0x09 */
                theme.BinRecordsTable![9].RecordNum = (uint)(resources.Widgets!.Count());
                binFile.BinThemes!.Add(theme);
            }
            var lastAddr = (uint)binFile.CurrentSize;
            //RecordBase
            foreach (var theme in binFile.BinThemes!)
            {
                var resources = xmlWatch.ElementCache[theme.StyleName!];
                var xmlTheme = xmlWatch.Themes!.FirstOrDefault(x => x.StyleName == theme.StyleName);
                for (var i = 0; i < 10; i++)
                {

                    if (theme.BinRecordsTable![i].RecordNum > 0)
                    {
                        theme.BinRecordsTable![i].Address = (uint)binFile.CurrentSize;
                        lastAddr = theme.BinRecordsTable![i].Address;
                        List<RecordBase> recordBases = GetRecordBases(xmlTheme!, resources, i, theme.StyleName);
                        if (recordBases.Any())
                        {
                            binFile.RecordBases!.AddRange(recordBases);
                        }
                    }
                    else
                    {
                        theme.BinRecordsTable![i].Address = lastAddr;
                    }
                }
            }
            //them bg addr
            foreach (var theme in binFile.BinThemes!)
            {
                var xmlTheme = xmlWatch.Themes!.FirstOrDefault(x => x.StyleName == theme.StyleName);
                if (xmlTheme != null && !string.IsNullOrWhiteSpace(xmlTheme.Bg))
                {
                    theme.Background = BinCommonHelper.UidRelationTable[xmlTheme.Bg.Replace("@", "")];
                }
            }


            var layoutCount = 0;
            //RawData
            foreach (var record in binFile.RecordBases!)
            {
                var resources = xmlWatch.ElementCache[record.StyleName!];
                // var xml = xmlCache[record.StyleName];
                var type = record.GetRecordType();
                if (type == 0)
                {
                    var xmlTheme = xmlWatch.Themes!.FirstOrDefault(x => x.StyleName == record.StyleName);
                    var xmlLayout = xmlTheme!.Layout!.FirstOrDefault(x => x!.Id == record.Name);
                    if (xmlLayout == null)
                    {
                        throw new Exception("无对应的数据");
                    }
                    LayoutRecordData layout = LayoutRecordData.GetLayoutRecordData(xmlLayout);
                    var bytes = layout.GetBin();
                    record.DataAddress = (uint)binFile.CurrentSize;
                    record.DataLength = (uint)bytes.Length;
                    binFile.RawData.AddRange(bytes);
                    layoutCount++;
                    var theme = binFile.BinThemes.FirstOrDefault(x => x.StyleName == record.StyleName);
                    if (theme != null && layoutCount == theme!.BinRecordsTable![0].RecordNum) //添加预览图
                    {

                        if (watchface.WatchStyles.FirstOrDefault(x => x.StyleName == record.StyleName)?.ScreenType == WatchScreen.light) //亮屏需要加入预览图
                        {
                            try
                            {
                                var watchPreview = new WatchImage
                                {
                                    Name = record.StyleName,
                                    Src = $@"_preview\thumbnail_{record.StyleName}.png",
                                };
                                var recordImg = ImgRecordData.GetImgRecordData(watchPreview, watchface.FolderName, watchface.ColorBit);

                                if (binFile.Header.PreviewImgDataAddr == 0)
                                {
                                    binFile.Header.PreviewImgDataAddr = (uint)binFile.CurrentSize; //默认为第一个主题的预览图
                                }
                                theme = binFile.BinThemes.FirstOrDefault(x => x.StyleName == record.StyleName);
                                if (theme != null)
                                {
                                    theme.PreviewImgDataAddr = (uint)binFile.CurrentSize; //预览图地址
                                }
                                binFile.RawData.AddRange(recordImg.GetBin());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Preview image not found: {ex.Message} - Skipping preview generation.");
                            }

                        }
                        layoutCount = 0;

                    }



                }
                else if (type == 2)
                {
                    var img = resources.Images!.FirstOrDefault(x => x.Name == record.Name);
                    if (img != null)
                    {
                        img.Src = CommonHelper.OutPath(img.Src!, watchface.FolderName);
                        var recordImg = ImgRecordData.GetImgRecordData(img, watchface.FolderName,watchface.ColorBit);
                        var bytes = recordImg.GetBin();
                        var cache = binFile.GetBinCache(bytes);
                        record.DataAddress = (uint)cache.Addr!;
                        record.DataLength = (uint)cache.Length!;
                        //record.DataAddress = (uint)binFile.CurrentSize;
                        //record.DataLength = (uint)bytes.Length;
                        //binFile.RawData.AddRange(bytes);
                    }
                }
                else if (type == 3)
                {
                    var array = resources.ImageArrays!.FirstOrDefault(x => x.Name == record.Name);
                    if (array != null)
                    {
                        foreach (var item in array.Images!)
                        {
                            item.Src = $@"{CommonHelper.OutPath(item.Src!, watchface.FolderName).Replace("/", @"\")}";
                        }
                        var recordAarry = ImgArrayRecordData.GetImgArrayRecordData(watchface.FolderName, array,watchface.ColorBit);
                        var bytes = recordAarry.GetBin();
                        var cache = binFile.GetBinCache(bytes);
                        record.DataAddress = (uint)cache.Addr!;
                        record.DataLength = (uint)cache.Length!;
                        // binFile.RawData.AddRange(bytes);
                    }
                }
                else if (type == 4)
                {
                    record.DataAddress = (uint)binFile.CurrentSize;
                    var spriteXml = resources.Sprites!.FirstOrDefault(x => x.Name == record.Name);
                    if (spriteXml != null)
                    {
                        var sprite = SpriteRecordData.GetSpriteRecordData(spriteXml);
                        var bytes = sprite!.GetBin();
                        record.DataLength = (uint)bytes.Length;
                        binFile.RawData.AddRange(bytes);
                    }
                }
                else if (type == 7)
                {
                    var dataNum = resources.DataItemImageNumbers!.FirstOrDefault(x => x.Name == record.Name);
                    if (dataNum != null)
                    {
                        var dataval = DataValueRecordData.GetDataValueRecordData(dataNum, record);
                        record.DataAddress = (uint)binFile.CurrentSize;
                        var bytes = dataval.GetBin();
                        record.DataLength = (uint)bytes.Length;
                        binFile.RawData.AddRange(bytes);
                    }
                    var imgVal = resources.DataItemImageValues!.FirstOrDefault(x => x.Name == record.Name);
                    if (imgVal != null)
                    {
                        var dataval = DataValueRecordData.GetDataValueRecordData(imgVal, record);
                        record.DataAddress = (uint)binFile.CurrentSize;
                        var bytes = dataval.GetBin();
                        record.DataLength = (uint)bytes.Length;
                        binFile.RawData.AddRange(bytes);
                    }
                    var dataPoint = resources.DataItemImagePoints!.FirstOrDefault(x => x.Name == record.Name);
                    if (dataPoint != null)
                    {
                        var dataval = DataValueRecordData.GetDataValueRecordData(dataPoint, record);
                        record.DataAddress = (uint)binFile.CurrentSize;
                        var bytes = dataval.GetBin();
                        record.DataLength = (uint)bytes.Length;
                        binFile.RawData.AddRange(bytes);
                    }
                }
                else if (type == 8)
                {
                    record.DataAddress = (uint)binFile.CurrentSize;
                    var slot = resources.Slots!.FirstOrDefault(x => x.Name == record.Name);
                    if (slot != null)
                    {
                        var recordArray = SlotRecordData.GetSlotRecordData(slot);
                        var bytes = recordArray!.GetBin();
                        record.DataLength = (uint)bytes.Length;
                        binFile.RawData.AddRange(bytes);
                    }
                }
                else if (type == 9)
                {
                    record.DataAddress = (uint)binFile.CurrentSize;
                    var widget = resources.Widgets!.FirstOrDefault(x => x.Name == record.Name);

                    if (widget != null)
                    {
                        var widgetName = string.Empty;
                        if (!string.IsNullOrWhiteSpace(widget.WidgetName))
                        {
                            var tranlate = resources.Translations.FirstOrDefault(x => x.Name == widget.WidgetName.Replace("@", ""));
                            if (tranlate != null)
                            {
                                widgetName = tranlate.Items!.FirstOrDefault()!.Str;
                            }
                        }
                        var recordAarray = WidgetRecordData.GetWidgetRecordData(widget, widgetName);
                        var bytes = recordAarray.GetBin();
                        record.DataLength = (uint)bytes.Length;
                        binFile.RawData.AddRange(bytes);
                    }

                }
            }

            return binFile.GetBin();
        }


    }




}
