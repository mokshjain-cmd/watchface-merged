using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WatchBasic.Tool;
using WatchBasic.UIBasic;
using WatchBasic.WatchBin;
using WatchBasic.WatchBin.Model;
using WatchBasic.ZHPicRSA;
using WatchBasic.ZHPicRSA.Model;
using WatchDB;



namespace WatchUI.CreateBin
{
    public class DIYSerializeTool
    {

        //public static IEnumerable<DIYX1> GetDIYX1s(IEnumerable<WatchGroup>? watchGroups, IEnumerable<LayerGroup> layerGroups, int langCount, BinImage binImage)
        //{
        //    return new List<DIYX1>()
        //    {
        //        new DIYX1 { Key = 3 },
        //        GetDIYX1(watchGroups, "0201", layerGroups, langCount, 1, 4, binImage),
        //        GetDIYX1(watchGroups, "0202", layerGroups, langCount, 3, 5, binImage),
        //        GetDIYX1(watchGroups, "0202", layerGroups, langCount, 1, 6, binImage),
        //        GetDIYX1(watchGroups, "0202", layerGroups, langCount, 2, 7, binImage),
        //        GetDIYX1(watchGroups, "0203", layerGroups, langCount, 1, 8, binImage),
        //        GetDIYX1(watchGroups, "0205", layerGroups, langCount, 1, 9, binImage),
        //        GetDIYX1(watchGroups, "0204", layerGroups, langCount, 1, 10, binImage),
        //        new DIYX1() { Key = 11 },

        //       new DIYX1() { Key = 12 },
        //       GetDIYX1(watchGroups, "0301", layerGroups, langCount, 1, 13, binImage),
        //       GetDIYX1(watchGroups, "0303", layerGroups, langCount, 1, 14, binImage),
        //       GetDIYX1(watchGroups, "0304", layerGroups, langCount, 1, 15, binImage),
        //       GetDIYX1(watchGroups, "0501", layerGroups, langCount, 3, 16, binImage),
        //       GetDIYX1(watchGroups, "0501", layerGroups, langCount, 1, 17, binImage),
        //       GetDIYX1(watchGroups, "0501", layerGroups, langCount, 2, 18, binImage),


        //        GetDIYX1(watchGroups, "0502", layerGroups, langCount, 3, 19, binImage),
        //        GetDIYX1(watchGroups, "0502", layerGroups, langCount, 1, 20, binImage),
        //        GetDIYX1(watchGroups, "0502", layerGroups, langCount, 2, 21, binImage),

        //        new DIYX1 { Key = 22 },

        //        new DIYX1 { Key = 23 },
        //        GetDIYX1(watchGroups, "0701", layerGroups, langCount, 1, 24, binImage),
        //        GetDIYX1(watchGroups, "0702", layerGroups, langCount, 3, 25, binImage),
        //        GetDIYX1(watchGroups, "0702", layerGroups, langCount, 1, 26, binImage),
        //        GetDIYX1(watchGroups, "0702", layerGroups, langCount, 2, 27, binImage),
        //        GetDIYX1(watchGroups, "0705", layerGroups, langCount, 1, 28, binImage),
        //        GetDIYX1(watchGroups, "0703", layerGroups, langCount, 3, 29, binImage),
        //        GetDIYX1(watchGroups, "0703", layerGroups, langCount, 1, 30, binImage),
        //        GetDIYX1(watchGroups, "0703", layerGroups, langCount, 2, 31, binImage),
        //        GetDIYX1(watchGroups, "0710", layerGroups, langCount, 1, 32, binImage),
        //        new DIYX1 { Key = 33 },

        //        new DIYX1 { Key = 34 },
        //        GetDIYX1(watchGroups, "0901", layerGroups, langCount, 1, 35, binImage),
        //        GetDIYX1(watchGroups, "0902", layerGroups, langCount, 3, 36, binImage),
        //        GetDIYX1(watchGroups, "0902", layerGroups, langCount, 1, 37, binImage),
        //        GetDIYX1(watchGroups, "0902", layerGroups, langCount, 2, 38, binImage),


        //        GetDIYX1(watchGroups, "0905", layerGroups, langCount, 1, 39, binImage),
        //        GetDIYX1(watchGroups, "0903", layerGroups, langCount, 3, 40, binImage),
        //        GetDIYX1(watchGroups, "0903", layerGroups, langCount, 1, 41, binImage),
        //        GetDIYX1(watchGroups, "0903", layerGroups, langCount, 2, 42, binImage),
        //        GetDIYX1(watchGroups, "0909", layerGroups, langCount, 1, 43, binImage),
        //        new DIYX1() { Key = 44 },

        //        new DIYX1 { Key = 45 },
        //        GetDIYX1(watchGroups, "0801", layerGroups, langCount, 3, 46, binImage),
        //        GetDIYX1(watchGroups, "0801", layerGroups, langCount, 1, 47, binImage),
        //        GetDIYX1(watchGroups, "0801", layerGroups, langCount, 2, 48, binImage),

        //        GetDIYX1(watchGroups, "0804", layerGroups, langCount, 1, 49, binImage),
        //        GetDIYX1(watchGroups, "0803", layerGroups, langCount, 1, 50, binImage),
        //        GetDIYX1(watchGroups, "0802", layerGroups, langCount, 3, 51, binImage),
        //        GetDIYX1(watchGroups, "0802", layerGroups, langCount, 1, 52, binImage),
        //        GetDIYX1(watchGroups, "0802", layerGroups, langCount, 2, 53, binImage),
        //        GetDIYX1(watchGroups, "0807", layerGroups, langCount, 1, 54, binImage),
        //        new DIYX1 { Key = 55 },

        //        GetDIYX1(watchGroups, "0601", layerGroups, langCount, 1, 56, binImage),
        //        GetDIYX1(watchGroups, "0602", layerGroups, langCount, 1, 57, binImage),
        //        GetDIYX1(watchGroups, "0603", layerGroups, langCount, 1, 58, binImage),
        //        GetDIYX1(watchGroups, "0604", layerGroups, langCount, 1, 59, binImage),
        //        GetDIYX1(watchGroups, "0605", layerGroups, langCount, 1, 60, binImage),
        //        GetDIYX1(watchGroups, "0606", layerGroups, langCount, 1, 61, binImage),
        //        GetDIYX1(watchGroups, "0607", layerGroups, langCount, 1, 62, binImage),
        //        GetDIYX1(watchGroups, "0608", layerGroups, langCount, 1, 63, binImage),
        //        new DIYX1 { Key = 64 },
        //        new DIYX1 { Key = 65 },
        //        GetDIYX1("1301", layerGroups, 66, binImage),
        //        GetDIYX1("1302", layerGroups, 67, binImage),
        //        GetDIYX1("1303", layerGroups, 68, binImage),
        //        new DIYX1 { Key = 69 },
        //        new DIYX1 { Key = 70 },

        //    };
        //}

        /// <summary>
        ///  M65A获取详细新增
        /// </summary>
        /// <param name="watchGroups"></param>
        /// <param name="layerGroups"></param>
        /// <param name="binImage"></param>
        /// <returns></returns>

        public static DetailIndex GetIndexM65A(IEnumerable<WatchGroup>? watchGroups, IEnumerable<LayerGroup> layerGroups, int langCount, BinImage binImage)
        {

            var detail = new DetailIndex()
            {
                KWHBackground = new DIYX1 { Key = 3 },
                KWHProgressbar = GetDIYX1(watchGroups, "0201", layerGroups, langCount, 1, 4, binImage),

                KWHText_CN = GetDIYX1(watchGroups, "0202", layerGroups, langCount, 3, 5, binImage),
                KWHText_EN = GetDIYX1(watchGroups, "0202", layerGroups, langCount, 1, 6, binImage),
                KWHText_CHT = GetDIYX1(watchGroups, "0202", layerGroups, langCount, 2, 7, binImage),

                KWHPicture = GetDIYX1(watchGroups, "0203", layerGroups, langCount, 1, 8, binImage),
                KWHNum = GetDIYX1(watchGroups, "0205", layerGroups, langCount, 1, 9, binImage),
                KWHPAH = GetDIYX1(watchGroups, "0204", layerGroups, langCount, 1, 10, binImage),
                BlueTooth = new DIYX1() { Key = 11 },

                GenerateDateBackground = new DIYX1() { Key = 12 },
                GenerateDateMonthNum = GetDIYX1(watchGroups, "0301", layerGroups, langCount, 1, 13, binImage),
                GenerateDateSeparator = GetDIYX1(watchGroups, "0303", layerGroups, langCount, 1, 14, binImage),
                GenerateDateDayNum = GetDIYX1(watchGroups, "0304", layerGroups, langCount, 1, 15, binImage),
                GenerateDateWeek_CN = GetDIYX1(watchGroups, "0501", layerGroups, langCount, 3, 16, binImage),
                GenerateDateWeek_EN = GetDIYX1(watchGroups, "0501", layerGroups, langCount, 1, 17, binImage),
                GenerateDateWeek_CHT = GetDIYX1(watchGroups, "0501", layerGroups, langCount, 2, 18, binImage),


                GenerateTimeSpan_CN = GetDIYX1(watchGroups, "0502", layerGroups, langCount, 3, 19, binImage),
                GenerateTimeSpan_EN = GetDIYX1(watchGroups, "0502", layerGroups, langCount, 1, 20, binImage),
                GenerateTimeSpan_CHT = GetDIYX1(watchGroups, "0502", layerGroups, langCount, 2, 21, binImage),

                R3 = new DIYX1 { Key = 22 },

                StepBackground = new DIYX1 { Key = 23 },
                StepProgressbar = GetDIYX1(watchGroups, "0701", layerGroups, langCount, 1, 24, binImage),
                StepText_CN = GetDIYX1(watchGroups, "0702", layerGroups, langCount, 3, 25, binImage),
                StepText_EN = GetDIYX1(watchGroups, "0702", layerGroups, langCount, 1, 26, binImage),
                StepText_CHT = GetDIYX1(watchGroups, "0702", layerGroups, langCount, 2, 27, binImage),



                StepNum = GetDIYX1(watchGroups, "0705", layerGroups, langCount, 1, 28, binImage),
                StepUnit_CN = GetDIYX1(watchGroups, "0703", layerGroups, langCount, 3, 29, binImage),
                StepUnit_EN = GetDIYX1(watchGroups, "0703", layerGroups, langCount, 1, 30, binImage),
                StepUnit_CHT = GetDIYX1(watchGroups, "0703", layerGroups, langCount, 2, 31, binImage),

                StepAnimation = GetDIYX1(watchGroups, "0710", layerGroups, langCount, 1, 32, binImage),
                R5 = new DIYX1 { Key = 33 },

                CalorieBackground = new DIYX1 { Key = 34 },
                CalorieProgressbar = GetDIYX1(watchGroups, "0901", layerGroups, langCount, 1, 35, binImage),
                CalorieText_CN = GetDIYX1(watchGroups, "0902", layerGroups, langCount, 3, 36, binImage),
                CalorieText_EN = GetDIYX1(watchGroups, "0902", layerGroups, langCount, 1, 37, binImage),
                CalorieText_CHT = GetDIYX1(watchGroups, "0902", layerGroups, langCount, 2, 38, binImage),


                CalorieNum = GetDIYX1(watchGroups, "0905", layerGroups, langCount, 1, 39, binImage),
                CalorieUnit_CN = GetDIYX1(watchGroups, "0903", layerGroups, langCount, 3, 40, binImage),
                CalorieUnit_EN = GetDIYX1(watchGroups, "0903", layerGroups, langCount, 1, 41, binImage),
                CalorieUnit_CHT = GetDIYX1(watchGroups, "0903", layerGroups, langCount, 2, 42, binImage),
                CalorieAnimation = GetDIYX1(watchGroups, "0909", layerGroups, langCount, 1, 43, binImage),
                R7 = new DIYX1() { Key = 44 },

                HeartBackground = new DIYX1 { Key = 45 },
                HeartText_CN = GetDIYX1(watchGroups, "0801", layerGroups, langCount, 3, 46, binImage),
                HeartText_EN = GetDIYX1(watchGroups, "0801", layerGroups, langCount, 1, 47, binImage),
                HeartText_CHT = GetDIYX1(watchGroups, "0801", layerGroups, langCount, 2, 48, binImage),

                HeartNum = GetDIYX1(watchGroups, "0804", layerGroups, langCount, 1, 49, binImage),
                HeartNull = GetDIYX1(watchGroups, "0803", layerGroups, langCount, 1, 50, binImage),
                HeartUnit_CN = GetDIYX1(watchGroups, "0802", layerGroups, langCount, 3, 51, binImage),
                HeartUnit_EN = GetDIYX1(watchGroups, "0802", layerGroups, langCount, 1, 52, binImage),
                HeartUnit_CHT = GetDIYX1(watchGroups, "0802", layerGroups, langCount, 2, 53, binImage),
                HeartAnimation = GetDIYX1(watchGroups, "0807", layerGroups, langCount, 1, 54, binImage),
                R9 = new DIYX1 { Key = 55 },

                HourTens = GetDIYX1(watchGroups, "0601", layerGroups, langCount, 1, 56, binImage),
                HourUint = GetDIYX1(watchGroups, "0602", layerGroups, langCount, 1, 57, binImage),
                HourSplit = GetDIYX1(watchGroups, "0603", layerGroups, langCount, 1, 58, binImage),
                MinuteTens = GetDIYX1(watchGroups, "0604", layerGroups, langCount, 1, 59, binImage),
                MinuteUint = GetDIYX1(watchGroups, "0605", layerGroups, langCount, 1, 60, binImage),
                MinuteSplit = GetDIYX1(watchGroups, "0606", layerGroups, langCount, 1, 61, binImage),
                SecondTens = GetDIYX1(watchGroups, "0607", layerGroups, langCount, 1, 62, binImage),
                SecondUnit = GetDIYX1(watchGroups, "0608", layerGroups, langCount, 1, 63, binImage),
                R11 = new DIYX1 { Key = 64 },
                R12 = new DIYX1 { Key = 65 },
                HourPointer = GetDIYX1("1301", layerGroups, 66, binImage),
                MinutePointer = GetDIYX1("1302", layerGroups, 67, binImage),
                SecondPointer = GetDIYX1("1303", layerGroups, 68, binImage),
                R13 = new DIYX1 { Key = 69 },
                R14 = new DIYX1 { Key = 70 },


            };
            return detail;
        }


        public static DetailIndex GetIndex(IEnumerable<WatchGroup>? watchGroups, IEnumerable<LayerGroup> layerGroups, int langCount, BinImage binImage)
        {

            var detail = new DetailIndex()
            {
                KWHBackground = new DIYX1 { Key = 3 },
                KWHProgressbar = GetDIYX1(watchGroups, "0201", layerGroups, langCount, 1, 4, binImage),

                KWHText_CN = GetDIYX1(watchGroups, "0202", layerGroups, langCount, 2, 5, binImage),
                KWHText_EN = GetDIYX1(watchGroups, "0202", layerGroups, langCount, 1, 6, binImage),
                KWHText_CHT = GetDIYX1(watchGroups, "0202", layerGroups, langCount, 3, 7, binImage),

                KWHPicture = GetDIYX1(watchGroups, "0203", layerGroups, langCount, 1, 8, binImage),
                KWHNum = GetDIYX1(watchGroups, "0205", layerGroups, langCount, 1, 9, binImage),
                KWHPAH = GetDIYX1(watchGroups, "0204", layerGroups, langCount, 1, 10, binImage),
                BlueTooth = GetDIYX1(watchGroups, "0102", layerGroups, langCount, 1, 11, binImage),

                GenerateDateBackground = new DIYX1() { Key = 12 },
                GenerateDateMonthNum = GetDIYX1(watchGroups, "0301", layerGroups, langCount, 1, 13, binImage),
                GenerateDateSeparator = GetDIYX1(watchGroups, "0303", layerGroups, langCount, 1, 14, binImage),
                GenerateDateDayNum = GetDIYX1(watchGroups, "0304", layerGroups, langCount, 1, 15, binImage),

                GenerateDateWeek_CN = GetDIYX1(watchGroups, "0501", layerGroups, langCount, 2, 16, binImage),
                GenerateDateWeek_EN = GetDIYX1(watchGroups, "0501", layerGroups, langCount, 1, 17, binImage),
                GenerateDateWeek_CHT = GetDIYX1(watchGroups, "0501", layerGroups, langCount, 3, 18, binImage),


                GenerateTimeSpan_CN = GetDIYX1(watchGroups, "0502", layerGroups, langCount, 2, 19, binImage),
                GenerateTimeSpan_EN = GetDIYX1(watchGroups, "0502", layerGroups, langCount, 1, 20, binImage),
                GenerateTimeSpan_CHT = GetDIYX1(watchGroups, "0502", layerGroups, langCount, 3, 21, binImage),

                R3 = new DIYX1 { Key = 22 },

                StepBackground = new DIYX1 { Key = 23 },
                StepProgressbar = GetDIYX1(watchGroups, "0701", layerGroups, langCount, 1, 24, binImage),
                StepText_CN = GetDIYX1(watchGroups, "0702", layerGroups, langCount, 2, 25, binImage),
                StepText_EN = GetDIYX1(watchGroups, "0702", layerGroups, langCount, 1, 26, binImage),
                StepText_CHT = GetDIYX1(watchGroups, "0702", layerGroups, langCount, 3, 27, binImage),



                StepNum = GetDIYX1(watchGroups, "0705", layerGroups, langCount, 1, 28, binImage),
                StepUnit_CN = GetDIYX1(watchGroups, "0703", layerGroups, langCount, 2, 29, binImage),
                StepUnit_EN = GetDIYX1(watchGroups, "0703", layerGroups, langCount, 1, 30, binImage),
                StepUnit_CHT = GetDIYX1(watchGroups, "0703", layerGroups, langCount, 3, 31, binImage),

                StepAnimation = GetDIYX1(watchGroups, "0710", layerGroups, langCount, 1, 32, binImage),
                R5 = new DIYX1 { Key = 33 },

                CalorieBackground = new DIYX1 { Key = 34 },
                CalorieProgressbar = GetDIYX1(watchGroups, "0901", layerGroups, langCount, 1, 35, binImage),
                CalorieText_CN = GetDIYX1(watchGroups, "0902", layerGroups, langCount, 2, 36, binImage),
                CalorieText_EN = GetDIYX1(watchGroups, "0902", layerGroups, langCount, 1, 37, binImage),
                CalorieText_CHT = GetDIYX1(watchGroups, "0902", layerGroups, langCount, 3, 38, binImage),


                CalorieNum = GetDIYX1(watchGroups, "0905", layerGroups, langCount, 1, 39, binImage),
                CalorieUnit_CN = GetDIYX1(watchGroups, "0903", layerGroups, langCount, 2, 40, binImage),
                CalorieUnit_EN = GetDIYX1(watchGroups, "0903", layerGroups, langCount, 1, 41, binImage),
                CalorieUnit_CHT = GetDIYX1(watchGroups, "0903", layerGroups, langCount, 3, 42, binImage),
                CalorieAnimation = GetDIYX1(watchGroups, "0909", layerGroups, langCount, 1, 43, binImage),
                R7 = new DIYX1() { Key = 44 },

                HeartBackground = new DIYX1 { Key = 45 },
                HeartText_CN = GetDIYX1(watchGroups, "0801", layerGroups, langCount, 2, 46, binImage),
                HeartText_EN = GetDIYX1(watchGroups, "0801", layerGroups, langCount, 1, 47, binImage),
                HeartText_CHT = GetDIYX1(watchGroups, "0801", layerGroups, langCount, 3, 48, binImage),

                HeartNum = GetDIYX1(watchGroups, "0804", layerGroups, langCount, 1, 49, binImage),
                HeartNull = GetDIYX1(watchGroups, "0803", layerGroups, langCount, 1, 50, binImage),
                HeartUnit_CN = GetDIYX1(watchGroups, "0802", layerGroups, langCount, 2, 51, binImage),
                HeartUnit_EN = GetDIYX1(watchGroups, "0802", layerGroups, langCount, 1, 52, binImage),
                HeartUnit_CHT = GetDIYX1(watchGroups, "0802", layerGroups, langCount, 3, 53, binImage),
                HeartAnimation = GetDIYX1(watchGroups, "0807", layerGroups, langCount, 1, 54, binImage),
                R9 = new DIYX1 { Key = 55 },

                HourTens = GetDIYX1(watchGroups, "0601", layerGroups, langCount, 1, 56, binImage),
                HourUint = GetDIYX1(watchGroups, "0602", layerGroups, langCount, 1, 57, binImage),
                HourSplit = GetDIYX1(watchGroups, "0603", layerGroups, langCount, 1, 58, binImage),
                MinuteTens = GetDIYX1(watchGroups, "0604", layerGroups, langCount, 1, 59, binImage),
                MinuteUint = GetDIYX1(watchGroups, "0605", layerGroups, langCount, 1, 60, binImage),
                MinuteSplit = GetDIYX1(watchGroups, "0606", layerGroups, langCount, 1, 61, binImage),
                SecondTens = GetDIYX1(watchGroups, "0607", layerGroups, langCount, 1, 62, binImage),
                SecondUnit = GetDIYX1(watchGroups, "0608", layerGroups, langCount, 1, 63, binImage),
                R11 = new DIYX1 { Key = 64 },
                R12 = new DIYX1 { Key = 65 },
                HourPointer = GetDIYX1("1301", layerGroups, 66, binImage),
                MinutePointer = GetDIYX1("1302", layerGroups, 67, binImage),
                SecondPointer = GetDIYX1("1303", layerGroups, 68, binImage),
                R13 = new DIYX1 { Key = 69 },
                R14 = new DIYX1 { Key = 70 },
            };
            return detail;
        }


        public static BackgroundIndex GetBackground(IEnumerable<WatchGroup>? watchGroups, IEnumerable<LayerGroup> layerGroups, int languageOrder,WatchSetting  Setting, BinImage binImage)
        {

            var layers = layerGroups?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayersByName($"缩略");
            var thumbnail = layers.Skip(languageOrder).Take(1).FirstOrDefault();
            if (thumbnail == null)
            {
                MessageBox.Show("缩略图文件不存在或命名错误,导出终止");
                throw new Exception("缩略图文件不存在或命名错误");
            }
            
            thumbnail.Left = Setting.ThumbnailX;
            thumbnail.Top = Setting.ThumbnailY;

            var bgimage = layerGroups?.Where(i => i.GroupCode!.Contains("0101")).FirstOrDefault()?.ShowLayer;
            return new BackgroundIndex
            {
                Thumbnail = GetDIYX1ByBackground("缩略", thumbnail, 1, binImage),
                Background = GetDIYX1ByBackground("背景", bgimage, 2, binImage),
            };
        }

        

        static DIYX1 GetDIYX1(IEnumerable<BaseGroup>? watchGroups, string groupCode, IEnumerable<LayerGroup>? layerGroups, int languageCount, int languageOrder, int key, BinImage binImage,bool IsDiy=false)
        {
            var dataFormat = new DIYX1();
            dataFormat.Key = key;
            if (watchGroups == null) return dataFormat;

            var groups =DIYX1BinTool.GetLayerGroups(layerGroups, groupCode);
            if (groups == null || groups.Count <= 0) return dataFormat;

            var group = groups.FirstOrDefault();
            if (group == null || group.LayerNum <= 0) return dataFormat;

            var location = watchGroups?.Where(i => i.GroupCode == groupCode).FirstOrDefault();
            if (location == null) return dataFormat;

            if (languageCount < languageOrder) return dataFormat;

            dataFormat = new DIYX1
            {
                X = location == null ? 0 : location.Left,
                Y = location == null ? 0 : location.Top,
                Height = group?.Height ?? 0,
                Width = group?.Width ?? 0,
                Key = key,

            };

            var parentCode = LayerTool.GetParentCode(groupCode);
            if (!string.IsNullOrWhiteSpace(parentCode) && binImage.ValuePairs.ContainsKey(parentCode))
            {
                 var details = binImage.ValuePairs[parentCode];
                //dataFormat.Addrs = details.Addrs;
                //dataFormat.HeadSizes = details.HeadSizes;
                //dataFormat.Num = details.Num;
                dataFormat.ColorValue = details.ColorValue;
                //dataFormat.PicCompress = dataFormat.PicCompress;
               // return dataFormat;
            }

            if (location != null && !string.IsNullOrEmpty(location?.ColorDesc) && (group?.ShowLayer?.IsPng ?? false))
            {
                var color = ColorTranslator.FromHtml(location?.ColorDesc);

                dataFormat.ColorValue = (color.B << 16) | (color.G << 8) | color.R;
                // ImageUtils.RGB888ToRGB565(color.R, color.G, color.B);
                //dataFormat.ColorValue = color.;
                binImage.ColorAddrs.Add((key - 3) * 112 + 107 + 30 + 100);
            }
            foreach (var g in groups)
            {
                if (LayerTool.GetLanguageCode().Contains(groupCode)) //需切换语言
                {
                    var count = g.LayerNum;
                    g.Layers = g.Layers?.Skip((languageOrder - 1) * count).Take(count).ToList();

                }
            }
            dataFormat.Num = group?.Layers == null ? 0 : group.Layers.Count;
            var imageType = DIYX1BinTool.GetImageType(groups.FirstOrDefault()?.Layers);
            var layers = groups.FirstOrDefault()?.Layers?.ToList();
            if (imageType == ".BMP")
            {
                dataFormat.PicCompress = PicCompress.bmp1;
                DIYX1BinTool.SetBmp(dataFormat, groupCode, layers, binImage);
            }
            else if (imageType == ".PNG")
            {
                if (group?.Layers?.FirstOrDefault()?.NotDye ?? false) //不需要填色
                {
                    dataFormat.PicCompress = PicCompress.png1;
                }
                else
                {
                    dataFormat.PicCompress = PicCompress.png2;//只要透明度

                }

               
                DIYX1BinTool.SetPng(dataFormat, groupCode, layers, binImage, false);
            }
            if (LayerTool.IsParentCode(groupCode))
            {
                binImage.ValuePairs.Add(groupCode, new BinImageDetail
                {
                    Addrs = dataFormat.Addrs,
                    HeadSizes = dataFormat.HeadSizes,
                    Num = dataFormat.Num,
                    ColorValue = dataFormat.ColorValue,
                    picCompress = dataFormat.PicCompress

                });
            }
            return dataFormat;
        }

        /// <summary>
        /// 用于背景
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="key"></param>
        /// <param name="binImage"></param>
        /// <returns></returns>
        static DIYX1 GetDIYX1ByBackground(string layerName, Layer? layer, int key, BinImage binImage)
        {
            var dataFormat = new DIYX1();
            dataFormat.Key = key;
            dataFormat = new DIYX1
            {
                X = layer.Left,
                Y = layer.Top,
                Height = layer?.Height ?? 0,
                Width = layer?.Width ?? 0,
                Key = key,

            };
            // dataFormat.HeadSize = dataFormat.Width * dataFormat.Height * 2;

            dataFormat.Num = 1;
            var imageType = DIYX1BinTool.GetImageType(layer);
            List<Layer> layers = new List<Layer>();
            layers.Add(layer!);
            var isThumbnail = key == 1;
            if (imageType == ".BMP")
            {
                dataFormat.PicCompress = PicCompress.bmp1;
                DIYX1BinTool.SetBmp(dataFormat, layerName, layers, binImage, isThumbnail);
            }
            else if (imageType == ".PNG")
            {
                dataFormat.PicCompress = PicCompress.png1;
                DIYX1BinTool.SetPng(dataFormat, layerName, layers, binImage, false,isThumbnail);
            }

            return dataFormat;
        }



        /// <summary>
        /// 指针专用
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="layerGroups"></param>
        /// <param name="key"></param>
        /// <param name="binImage"></param>
        /// <returns></returns>
        static DIYX1 GetDIYX1(string groupCode, IEnumerable<LayerGroup>? layerGroups, int key, BinImage binImage)
        {
            var dataFormat = new DIYX1();
            dataFormat.Key = key;

            var groups = DIYX1BinTool.GetLayerGroups(layerGroups, groupCode);
            if (groups == null || groups.Count <= 0) return dataFormat;

            var group = groups.FirstOrDefault();
            if (group == null || group.LayerNum <= 0) return dataFormat;
            dataFormat = new DIYX1
            {
                Height = group?.Height ?? 0,
                Width = group?.Width ?? 0,
                Key = key,
                Code = groupCode,
            };

            dataFormat.Num = 16;
            var imageType = DIYX1BinTool.GetImageType(groups.FirstOrDefault()?.Layers);
            var layers = groups.FirstOrDefault()?.Layers?.ToList();

            if (imageType == ".BMP")
            {
                dataFormat.PicCompress = PicCompress.bmp1;
                DIYX1BinTool.SetBmp(dataFormat, groupCode, layers, binImage);
            }
            else if (imageType == ".PNG")
            {
                // var isGeneratePic = layers.Count==1;

                dataFormat.PicCompress = PicCompress.png1;
                DIYX1BinTool.SetPng(dataFormat, groupCode, layers, binImage, true);
            }
            return dataFormat;

        }


       



    }

}
