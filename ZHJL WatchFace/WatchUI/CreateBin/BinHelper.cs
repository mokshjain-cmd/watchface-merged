using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WatchBasic;
using WatchBasic.Tool;
using WatchBasic.UIBasic;
using WatchBasic.WatchBin;
using WatchBasic.WatchBin.Model;
using WatchDB;
using WatchDB_DIY;
using WatchDBDIYJieLi;

namespace WatchUI.CreateBin
{
    public class BinHelper
    {

        public byte[] GetBinBytes(IEnumerable<LayerGroup> layerGroups, int langCount, WatchInfo? watchInfo, List<byte>? colorInfo = null)
        {
            List<byte> bytes = new List<byte>();
            MainIndex mainIndex = new MainIndex(null);//背景，
            bytes.AddRange(mainIndex.Value.GetBytes16());

            var kwh = watchInfo.WatchTypes.Where(i => i.WatchTypeName == "Kwh").FirstOrDefault();
            var kwhVal = GetBaseData("Kwh", langCount, kwh.Align, layerGroups);
            bytes.AddRange(kwhVal.Value.GetBytes32());

            var generalDate = watchInfo.WatchTypes.Where(i => i.WatchTypeName == "GeneralDate").FirstOrDefault();
            var generalDateVal = GetBaseData("GeneralDate", langCount, generalDate.Align, layerGroups);
            bytes.AddRange(generalDateVal.Value.GetBytes32());

            var step = watchInfo.WatchTypes.Where(i => i.WatchTypeName == "Step").FirstOrDefault();
            var stepVal = GetBaseData("Step", langCount, step.Align, layerGroups);
            bytes.AddRange(stepVal.Value.GetBytes32());
            var calorie = watchInfo.WatchTypes.Where(i => i.WatchTypeName == "Calorie").FirstOrDefault();
            var calorieVal = GetBaseData("Calorie", langCount, calorie.Align, layerGroups);
            bytes.AddRange(calorieVal.Value.GetBytes32());

            var heartRate = watchInfo.WatchTypes.Where(i => i.WatchTypeName == "HeartRate").FirstOrDefault();
            var heartRateVal = GetBaseData("HeartRate", langCount, heartRate.Align, layerGroups);
            CommonDefintion.IsHeart = heartRateVal.Value == 0 ? 0 : 1;
            bytes.AddRange(heartRateVal.Value.GetBytes32());

            var timeIndex = new TimeIndex(layerGroups, langCount, 0);
            bytes.AddRange(timeIndex.Value.GetBytes32());

            var timePointer = new TimePointerIndex(layerGroups, langCount, 0);
            bytes.AddRange(timePointer.Value.GetBytes32());


            BinImage binImage = new BinImage(7646);
            //if (color != null)
            //{
            //    binImage.Color = color;
            //}
            DetailIndex detail = new DetailIndex();
            if (CommonDefintion.Setting.ProjectName == "M65A")
            {
                detail = DIYSerializeTool.GetIndexM65A(watchInfo.WatchTypes.SelectMany(i => i.WatchGroups).ToList(), layerGroups, langCount, binImage);
            }
            else
            {
                detail = DIYSerializeTool.GetIndex(watchInfo.WatchTypes.SelectMany(i => i.WatchGroups).ToList(), layerGroups, langCount, binImage);
            }

            //var dx1s = DIYSerializeTool.GetDIYX1s(watchInfo.WatchTypes.SelectMany(i => i.WatchGroups).ToList(), layerGroups, langCount, binImage);
            //bytes.AddRange(SerializeTool.SerializeObject(dx1s));
            bytes.AddRange(detail.Serialize());
            bytes.AddRange(binImage.Bytes);

            if (colorInfo != null)//相册表盘信息
            {

                colorInfo.Add(Convert.ToByte(CommonDefintion.IsColor888 ? 1 : 0));
                colorInfo.AddRange(binImage.ColorAddrs.Count().GetBytes16(true));
                colorInfo.AddRange(Enumerable.Range(0, 17).Select(i => Convert.ToByte(0)).ToList());
                colorInfo.AddRange(binImage.ColorAddrs.Select(i => i.GetBytes32(true)).SelectMany(i => i).ToArray());

            }

            return bytes.ToArray();
        }



        public byte[] GetBackgroundBinBytes(IEnumerable<LayerGroup> layerGroups, int languageOrder, WatchInfo? watchInfo, WatchSetting setting)
        {
            List<byte> bytes = new List<byte>();
            BinImage binImage = new BinImage(224);
            //if (color != null)
            //{
            //    binImage.Color = color;
            //}
            var background = DIYSerializeTool.GetBackground(watchInfo.WatchTypes.SelectMany(i => i.WatchGroups).ToList(), layerGroups, languageOrder, setting, binImage);
            bytes.AddRange(background.Serialize());
            bytes.AddRange(binImage.Bytes);
            return bytes.ToArray();
        }


        BaseData? GetBaseData(string watchType, int langCount, int align, IEnumerable<LayerGroup>? layerGroups)
        {
            switch (watchType)
            {
                case "Kwh": return new KWHIndex(layerGroups, langCount, align);
                case "GeneralDate": return new GeneralDateIndex(layerGroups, langCount, align);
                case "Step": return GetStep(layerGroups, langCount, align);
                case "Calorie": return GetCalorie(layerGroups, langCount, align);
                case "HeartRate": return new HeartRateIndex(layerGroups, langCount, align);
                default: return null;
            }
        }

        HealthIndex GetStep(IEnumerable<LayerGroup>? layerGroups, int langCount, int align)
        {
            return new HealthIndexStep(layerGroups, langCount, align);
        }
        HealthIndex GetCalorie(IEnumerable<LayerGroup>? layerGroups, int langCount, int align)
        {
            return new HealthIndexCalorie(layerGroups, langCount, align);
        }

        #region diy

        public Tuple<byte[], List<int>> GetComplexBytes(WatchInfo_DIY? watchInfo, IEnumerable<LayerGroup>? layerGroups, int languageOrder)
        {
            List<byte> bytes = new List<byte>();
            var locations = (from i in Enumerable.Range(1, 9)
                             join t in watchInfo?.WatchLocations! on i equals LocationNum(t.LocationName) into temp
                             from p in temp.DefaultIfEmpty()
                             select new
                             {
                                 IsChecked = p != null ? 1 : 0,
                                 Count = p != null ? p?.LocationDetails?.Count : 0,
                                 ID = p != null ? LocationNum(p.LocationName) : 0,
                             }).ToList();

            locations?.ForEach(i =>
            {
                bytes.Add((byte)i.IsChecked);
                bytes.Add((byte)i.Count);
            });
            var num = locations?.Select(i => i.Count).Sum();
            BinImage binImage = new BinImage(9 * 2 + (num ?? 0) * 676);
            binImage.CurrentAddr += bytes.Count;
            var watchLocations = watchInfo?.WatchLocations.OrderBy(i => LocationNum(i.LocationName)).ToList();

            watchLocations.ForEach(info =>
            {
                foreach (var detail in info.LocationDetails)
                {
                    var ID = LocationNum(info.LocationName);//位置类型
                    bytes.Add((byte)ID);
                    var watchType = GetWatchType(watchInfo, detail.WatchTypeId);
                    var type = 0;
                    var align = 0;
                    if (watchType != null)
                    {
                        type = WatchTypeNum(watchType.WatchTypeName);
                        align = watchType.Align;
                    }
                    bytes.Add((byte)type);//数据类型
                    bytes.Add((byte)0);//是否可点击
                    bytes.Add((byte)align);
                    binImage.CurrentAddr += 4;
                    var diyX1s = GetDIYX1s(watchInfo, detail, layerGroups, languageOrder, binImage);
                  
                    bytes.AddRange(SerializeTool.SerializeObject(diyX1s));

                }
            });

            bytes.AddRange(binImage.Bytes);
            while (bytes.Count%4!=0) 
            {
              bytes.Add(0);
            }
            return Tuple.Create(bytes.ToArray(), binImage.ColorAddrs);
        }



        public byte[]? GetPointerBytes(PointerImage pointer, WatchPointer  point=null)
        {
            List<byte> bytes = new List<byte>();

            BinImage binImage = new BinImage(336);
            var pointerInfo = DIYSerializeTool_DIY.GetPointer(pointer.LayerGroups, binImage, point);
            bytes.AddRange(SerializeTool.SerializeObject(pointerInfo));
            bytes.AddRange(binImage.Bytes!);
            while(bytes.Count%4!=0) 
            {
                bytes.Add(0);
            }
            return bytes.ToArray();
        }

        public Tuple<byte[]?, IEnumerable<TimeLocationInfo>> GetTimeBytes(WatchInfo_DIY? watchInfo, TimeImage timeImage)
        {
            List<byte> bytes = new List<byte>();
            BinImage binImage = new BinImage(896);
            var timegroups = watchInfo?.WatchTimes?.Where(i => i.TimeCode == timeImage.TimeCode)?.FirstOrDefault()?.TimeGroups;

            var diytime = DIYSerializeTool_DIY.GetDIYTime(timegroups, timeImage.LayerGroups, 2, 1, binImage);
            bytes.AddRange(diytime.Serialize());
            bytes.AddRange(binImage.Bytes!);
            while (bytes.Count % 4 != 0) 
            {
               bytes.Add(0);
            }
            var infos = binImage.ColorAddrs.Select((i, index) => new TimeLocationInfo
            {
                ColorAddr = i,
                LeftAddr = binImage.LocationLefts[index],
                TopAddr= binImage.LocationTops[index],
                DisableAddr = binImage.DisableAddrs[index],
                GroupCode = binImage.GroupCodes[index],
            });
            return Tuple.Create(bytes.ToArray(), infos);
        }

        int LocationNum(string? location)
        {
            return location switch
            {
                "左上" => 1,
                "中上" => 2,
                "右上" => 3,
                "左中" => 4,
                "正中" => 5,
                "右中" => 6,
                "左下" => 7,
                "中下" => 8,
                "右下" => 9,
                _ => 0
            };
        }

        int WatchTypeNum(string typeName)
        {
            return typeName switch
            {
                "Kwh" => 1,
                "GeneralDate" => 2,
                "Step" => 3,
                "Calorie" => 4,
                "HeartRate" => 5,
                _ => 0
            };
        }

        WatchType? GetWatchType(WatchInfo_DIY watchInfo, int watchTypeId)

        {
            return watchInfo.WatchTypes.FirstOrDefault(i => i.ID == watchTypeId);

        }

        IEnumerable<DIYX1>? GetDIYX1s(WatchInfo_DIY watchInfo, LocationDetail location, IEnumerable<LayerGroup>? layerGroups, int languageOrder, BinImage binImage)
        {
            var watchType = GetWatchType(watchInfo, location.WatchTypeId);
            var watchGroups = watchInfo.WatchTypes.SelectMany(i => i.WatchGroups);

            return watchType.WatchTypeName switch
            {
                "Kwh" => GetKwh(watchGroups, location, layerGroups, languageOrder, binImage),
                "GeneralDate" => GetGeneralDate(watchGroups, location, layerGroups, languageOrder, binImage),
                "Step" => GetStep(watchGroups, location, layerGroups, languageOrder, binImage),
                "Calorie" => GetCalorie(watchGroups, location, layerGroups, languageOrder, binImage),
                "HeartRate" => GetHeartRate(watchGroups, location, layerGroups, languageOrder, binImage),
                _ => new List<DIYX1>()
            };

        }

        public IEnumerable<DIYX1> GetKwh(IEnumerable<BaseGroup>? watchGroups, LocationDetail location, IEnumerable<LayerGroup>? layerGroups, int languageOrder, BinImage binImage)
        {
            List<DIYX1> dIYX1s = new List<DIYX1>();
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0101", location, layerGroups, languageOrder, binImage));//KWHBackground
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0102", location, layerGroups, languageOrder, binImage));//KWHProgressbar
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0104", location, layerGroups, languageOrder, binImage));//KWHText
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0103", location, layerGroups, languageOrder, binImage));//KWHPicture
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0105", location, layerGroups, languageOrder, binImage));//KWHPAH
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0106", location, layerGroups, languageOrder, binImage));//KWHNum
            return dIYX1s;
        }


        public IEnumerable<DIYX1> GetGeneralDate(IEnumerable<BaseGroup>? watchGroups, LocationDetail location, IEnumerable<LayerGroup>? layerGroups, int languageOrder, BinImage binImage)
        {
            List<DIYX1> dIYX1s = new List<DIYX1>();
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0201", location, layerGroups, languageOrder, binImage));//GenerateDateBackground
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0202", location, layerGroups, languageOrder, binImage));//GenerateDateMonthNum
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0204", location, layerGroups, languageOrder, binImage));//GenerateDateSeparator
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0205", location, layerGroups, languageOrder, binImage));//GenerateDateDayNum
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0207", location, layerGroups, languageOrder, binImage));//GenerateDateWeek
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0208", location, layerGroups, languageOrder, binImage));//GenerateTimeSpan
            return dIYX1s;
        }

        public IEnumerable<DIYX1> GetStep(IEnumerable<BaseGroup>? watchGroups, LocationDetail location, IEnumerable<LayerGroup>? layerGroups, int languageOrder, BinImage binImage)
        {
            List<DIYX1> dIYX1s = new List<DIYX1>();
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0301", location, layerGroups, languageOrder, binImage));//StepBackground
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0302", location, layerGroups, languageOrder, binImage));//StepProgressbar
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0303", location, layerGroups, languageOrder, binImage));//StepText
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0310", location, layerGroups, languageOrder, binImage));//StepUnit
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0304", location, layerGroups, languageOrder, binImage));//StepNum
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0309", location, layerGroups, languageOrder, binImage));//StepAnimation

            return dIYX1s;
        }

        public IEnumerable<DIYX1> GetCalorie(IEnumerable<BaseGroup>? watchGroups, LocationDetail location, IEnumerable<LayerGroup>? layerGroups, int languageOrder, BinImage binImage)
        {
            List<DIYX1> dIYX1s = new List<DIYX1>();
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0401", location, layerGroups, languageOrder, binImage));//CalorieBackground
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0402", location, layerGroups, languageOrder, binImage));//CalorieProgressbar
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0403", location, layerGroups, languageOrder, binImage));//CalorieText
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0404", location, layerGroups, languageOrder, binImage));//CalorieUnit
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0405", location, layerGroups, languageOrder, binImage));//CalorieNum
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0409", location, layerGroups, languageOrder, binImage));//CalorieAnimation
            return dIYX1s;
        }

        public IEnumerable<DIYX1> GetHeartRate(IEnumerable<BaseGroup>? watchGroups, LocationDetail location, IEnumerable<LayerGroup>? layerGroups, int languageOrder, BinImage binImage)
        {

            List<DIYX1> dIYX1s = new List<DIYX1>();
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0501", location, layerGroups, languageOrder, binImage));//HeartBackground
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0502", location, layerGroups, languageOrder, binImage));//HeartText
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0503", location, layerGroups, languageOrder, binImage));//HeartUnit
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0504", location, layerGroups, languageOrder, binImage));//HeartNull
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0505", location, layerGroups, languageOrder, binImage));//HeartNum
            dIYX1s.Add(DIYSerializeTool_DIY.GetDIYX1(watchGroups, "0508", location, layerGroups, languageOrder, binImage));//HeartAnimation
            return dIYX1s;
        }


        #endregion


    }
}
