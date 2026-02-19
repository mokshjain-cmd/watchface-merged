using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WatchDB_DIY;
using WatchUI.CreateBin;
using WatchBasic.UIBasic;
using WatchDB;
using WatchBasic.Tool;
using System.Drawing;
using WatchBasic;
using WatchBasic.WatchBin;
using WatchBasic.WatchBin.Model;

namespace WatchJieLi.Common
{
    public class BinHelpers
    {

        public byte[] GetBinBytes(IEnumerable<LayerGroup> layerGroups, int langCount, WatchInfo? watchInfo, List<byte>? colorInfo,bool isAlbum)
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
            WatchBasic.WatchBin.Model.DetailIndex detail = new WatchBasic.WatchBin.Model.DetailIndex();
            detail = DIYSerializeTools.GetIndex(watchInfo.WatchTypes.SelectMany(i => i.WatchGroups).ToList(), layerGroups, langCount, binImage,isAlbum);

            bytes.AddRange(detail.Serialize());
            bytes.AddRange(binImage.Bytes);

            if (colorInfo != null)//相册表盘信息
            {
                colorInfo.Add(Convert.ToByte(CommonDefintion.IsColor888 ? 1 : 0));
                colorInfo.AddRange(binImage.ColorAddrs.Count().GetBytes16(true));
                colorInfo.Add(Convert.ToByte(1));//图片规则
                colorInfo.AddRange(Enumerable.Range(0, 16).Select(i => Convert.ToByte(0)).ToList());
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
            var background = DIYSerializeTools.GetBackground(watchInfo.WatchTypes.SelectMany(i => i.WatchGroups).ToList(), layerGroups, languageOrder, setting, binImage);
            bytes.AddRange(background.Serialize());
            bytes.AddRange(binImage.Bytes);
            return bytes.ToArray();
        }


        BaseData? GetBaseData(string watchType, int langCount, int align, IEnumerable<LayerGroup>? layerGroups,bool NotM65a=true)
        {
            switch (watchType)
            {
                case "Kwh": return new KWHIndex2(layerGroups, langCount, align);
                case "GeneralDate": return new GeneralDateIndex2(layerGroups, langCount, align);
                case "Step": return GetStep(layerGroups, langCount, align);
                case "Calorie": return GetCalorie(layerGroups, langCount, align);
                case "HeartRate": return new HeartRateIndex2(layerGroups, langCount, align);
                default: return null;
            }
        }

        HealthIndex2 GetStep(IEnumerable<LayerGroup>? layerGroups, int langCount, int align)
        {
            return new HealthIndexStep2(layerGroups, langCount, align);
        }
        HealthIndex2 GetCalorie(IEnumerable<LayerGroup>? layerGroups, int langCount, int align)
        {
            return new HealthIndexCalorie2(layerGroups, langCount, align);
        }

       


    }
}
