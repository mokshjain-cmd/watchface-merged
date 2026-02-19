using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WatchBasic.Tool;
using WatchBasic.UIBasic;
using WatchDB;

namespace WatchBasic.UIBasic
{
    public static class LayerTool
    {
        public static void CheckLayerConsistency(IEnumerable<LayerGroup>? layerGroups)
        {
            if (layerGroups != null)
                foreach (var group in layerGroups)
                {
                    if (group.GroupCode != "辅助文件")
                    {
                        if (group.Layers?.GroupBy(i => new { i.Width, i.Height }).Count() > 1)
                        {
                            throw new Exception($"{group.GroupCode}图层尺寸不统一");
                        }
                    }

                }

        }

        public static int GetLayerIndex(string[] fileNames)
        {
            // Handle empty array (folders with no images)
            if (fileNames == null || fileNames.Length == 0)
            {
                return 0;
            }
            
            var last = fileNames.Last();
            if (fileNames.Length > 1)
            {
                var head = fileNames[0].ToArray().Last().ToString();
                if (NumTool.IsInt(head))
                {
                    return NumTool.IsInt(last) ? Convert.ToInt32(head + last) : 0;
                }
                return NumTool.IsInt(last) ? Convert.ToInt32(last) : 0;
            }
            else
            {
                return NumTool.IsInt(last) ? Convert.ToInt32(last) : 0;
            }
        }

        public static IEnumerable<LayerGroup> GetLayerGroups(this string path, bool notBitmapSource)
        {
            if (path == null)
            {
                return new List<LayerGroup>();
            }
            var source = Directory.GetDirectories(path!);
            var layerGroups = new List<LayerGroup>();

            foreach (var i in source)
            {
                var info = Path.GetFileNameWithoutExtension(i).Split('_');
                var IsDye = Directory.GetFiles(i).Where(f => f.Contains("NoDye")).Any();
                var group = new LayerGroup()
                {
                    GroupCode = info[0],
                    GroupName = info.Length > 1 ? $"{info[0]}_{info[1]}" : null,
                    ShowIndex = 0,
                    IsShow = true,
                };
                List<Layer> layers = new List<Layer>();
                foreach (var j in Directory.GetFiles(i).Where(f => f.EndsWith("bmp") || f.EndsWith("png")))
                {
                    var img = Image.FromFile(j);
                    var filenames = Path.GetFileNameWithoutExtension(j).Split('_');
                    var groupname = info.Length > 1 ? $"{info[0]}_{info[1]}" : null;
                    var layer = new Layer()
                    {
                        NotBitmapSource = notBitmapSource,
                        NotDye = IsDye,
                        LayerIndex = GetLayerIndex(filenames),
                        ImgPath = j,
                        Height = img == null ? 0 : img.Height,
                        Width = img == null ? 0 : img.Width,
                        Top = groupname?.Contains("背景") ?? false ? 0 : (info.Length != 5 ? 0 : Convert.ToInt32(info.Last())),
                        Left = groupname?.Contains("背景") ?? false ? 0 : (info.Length != 5 ? 0 : Convert.ToInt32(info[3]))
                    };
                    layers.Add(layer);
                    img?.Dispose();
                }
                group.Layers = layers.OrderBy(j => j.LayerIndex).ToList();
                layerGroups.Add(group);

            }
            var a = layerGroups.GroupBy(i => i.GroupCode).Where(i => i.Count() > 1);
            if (a.Any())
                throw new Exception($"{string.Join(",", a.Select(i => i.Key))}重复");
            return layerGroups;
            return (from i in source
                    let info = Path.GetFileNameWithoutExtension(i).Split('_')
                    let IsDye = Directory.GetFiles(i).Where(f => f.Contains("NoDye")).Any()
                    select new LayerGroup
                    {
                        GroupCode = info[0],
                        GroupName = info.Length > 1 ? $"{info[0]}_{info[1]}" : null,
                        Layers = (from j in Directory.GetFiles(i).Where(f => f.EndsWith("bmp") || f.EndsWith("png"))
                                  let img = Image.FromFile(j)
                                  let filenames = Path.GetFileNameWithoutExtension(j).Split('_')
                                  let groupname = info.Length > 1 ? $"{info[0]}_{info[1]}" : null
                                  select new Layer
                                  {
                                      NotBitmapSource = notBitmapSource,
                                      NotDye = IsDye,
                                      LayerIndex = GetLayerIndex(filenames),
                                      ImgPath = j,
                                      Height = img == null ? 0 : img.Height,
                                      Width = img == null ? 0 : img.Width,
                                      Top = groupname?.Contains("背景") ?? false ? 0 : (info.Length != 5 ? 0 : Convert.ToInt32(info.Last())),
                                      Left = groupname?.Contains("背景") ?? false ? 0 : (info.Length != 5 ? 0 : Convert.ToInt32(info[3]))
                                  }).OrderBy(j => j.LayerIndex).ToList(),
                        ShowIndex = 0,
                        IsShow = true,


                    }).ToList();
        }

        public static List<string> CheckLayerGroup(IEnumerable<LayerGroupCheck>? layerGroups, int languageCount)
        {
            List<string> errors = new List<string>();
            if (layerGroups != null)
            {

                //图层检查
                var groups = layerGroups.Where(i => i.GroupName?.Contains("进度条") ?? false);
                foreach (var g in groups)
                {
                    //g.Status = g.Left % 2 == 0;
                    //g.SizeStatus = g.Width % 2 == 0;
                    CheckStatus(g, 11);
                    //if (g?.LayerNum > 0 && g?.LayerNum != 11)
                    //{
                    //    if (g != null)
                    //    {
                    //        g.NumStatus = false;
                    //    }
                    //}
                }
                groups = layerGroups.Where(i => ((i.GroupName?.Contains("文字") ?? false) || (i.GroupName?.Contains("单位") ?? false)) && i.GroupCode != WeekCode(IsDiy) && i.GroupCode != TimeSpanCode(IsDiy));
                foreach (var g in groups)
                {

                    CheckStatus(g, languageCount);

                }

                groups = layerGroups.Where(i => i.GroupName?.Contains("数字") ?? false);
                foreach (var g in groups)
                {

                    CheckStatus(g, 10);

                }
                //List<string> infos = new List<string> { "分隔", "百分号", "普通", "无数据"};
                groups = layerGroups.Where(i => (i.GroupName?.Contains("分隔") ?? false) || (i.GroupName?.Contains("百分号") ?? false) || (i.GroupName?.Contains("普通") ?? false) || (i.GroupName?.Contains("无数据") ?? false));
                foreach (var g in groups)
                {
                    CheckStatus(g, 1);
                }
                var group = layerGroups.Where(i => i.GroupCode == WeekCode(IsDiy)).FirstOrDefault();
                CheckStatus(group, 7 * languageCount);

                group = layerGroups.Where(i => i.GroupCode == TimeSpanCode(IsDiy)).FirstOrDefault();
                CheckStatus(group, 2 * languageCount);

                var heart = layerGroups.Where(i => i.GroupCode == HeartNumCode(IsDiy)).FirstOrDefault();

                var nullHeart = layerGroups.Where(i => i.GroupCode == HeartNullCode(IsDiy)).FirstOrDefault();  // 单独判断心率无数据的尺寸
                if (nullHeart != null && heart != null)
                {
                    if (heart?.LayerNum > 0 && nullHeart?.Width != heart?.Width * 3)
                    {
                        nullHeart.HeartNullStatus = false;
                    }
                }

                //电量数字
                List<string> codes = KwdNumCodes(IsDiy);
                SetError(errors, codes, layerGroups);
                codes = StepNumCodes(IsDiy);
                SetError(errors, codes, layerGroups);
                codes = HeartRateNumCodes(IsDiy);
                SetError(errors, codes, layerGroups);
                codes = CalorieNumCodes(IsDiy);
                SetError(errors, codes, layerGroups);
            }
            return errors;
            //  return false; 
        }

        static string HeartNumCode(bool IsDiy)
        {
            if (IsDiy) return "0505";
            return "0804";
        }
        static string HeartNullCode(bool IsDiy)
        {
            if (IsDiy) return "0504";
            return "0803";
        }
        static string WeekCode(bool IsDiy)
        {
            if (IsDiy) return "0207";
            return "0501";
        }

        static string TimeSpanCode(bool IsDiy)
        {
            if (IsDiy) return "0208";
            return "0502";
        }

        public static string GetHeartRateNumCode(bool IsDiy)
        {
            if (IsDiy) return "0505";
            return "0804";
        }

        public static string GetHeartRateNullCode(bool IsDiy)
        {
            if (IsDiy) return "0504";
            return "0803";
        }

        static List<string> KwdNumCodes(bool IsDiy)
        {
            if (IsDiy)
            {
                return new List<string> { "0106", "0107", "0108" };
            }
            return new List<string> { "0205", "0206", "0207" };
        }

        static List<string> StepNumCodes(bool IsDiy)
        {
            if (IsDiy)
            {
                return new List<string> { "0304", "0305", "0306", "0307", "0308" };
            }
            return new List<string> { "0705", "0706", "0707", "0708", "0709" };
        }
        static List<string> HeartRateNumCodes(bool IsDiy)
        {
            if (IsDiy)
            {
                return new List<string> { "0505", "0506", "0507" };
            }
            return new List<string> { "0804", "0805", "0806" };
        }

        static List<string> CalorieNumCodes(bool IsDiy)
        {
            if (IsDiy)
            {
                return new List<string> { "0405", "0406", "0407", "0408" };
            }
            return new List<string> { "0905", "0906", "0907", "0908" };
        }

        static bool IsOnline(int numY, int numHeight, int unitY)
        {
            return Math.Abs(numY - unitY) < numHeight;
        }


        public static bool CheckUnit(List<LayerGroup> layerGroups, WatchInfo watchInfo) 
        {
            if (!CheckUnit(layerGroups, "0205", "0204", watchInfo)) 
            {
                throw new Exception("电量单位图片应放入文字文件夹");
            }
            if (!CheckUnit(layerGroups, "0705", "0703", watchInfo))
            {
                throw new Exception("步数单位图片应放入文字文件夹");
            }
            if (!CheckUnit(layerGroups, "0804", "0802", watchInfo))
            {
                throw new Exception("心率单位图片应放入文字文件夹");
            }
            if (!CheckUnit(layerGroups, "0905", "0903", watchInfo))
            {
                throw new Exception("卡路里单位图片应放入文字文件夹");
            }
            return true;
        }


        public static bool CheckUnit(List<LayerGroup> layerGroups,string numCode,string unitCode,WatchInfo watchInfo)
        {
            var unitDB= watchInfo.WatchTypes.SelectMany(i => i.WatchGroups).FirstOrDefault(i => i.GroupCode == unitCode);
            var unit = layerGroups.FirstOrDefault(i => i.GroupCode == unitCode);
            if (unitDB != null&&unit!=null&&unit.Layers.Count>0)
            {
                var num = layerGroups.FirstOrDefault(i => i.GroupCode == numCode);
                var numDB= watchInfo.WatchTypes.SelectMany(i => i.WatchGroups).FirstOrDefault(i => i.GroupCode == numCode);
                if (num != null && num.Layers.Count>0&& numDB!=null)
                {
                    return IsOnline(numDB.Top, num.ShowLayer.Height, unitDB.Top);
                }
            }
            return true;
        }


        static void CheckStatus(LayerGroupCheck? groupCheck, int num)
        {
            if (groupCheck != null)
            {

                if (groupCheck?.LayerNum > 0 && groupCheck?.LayerNum != num)
                {
                    groupCheck.NumStatus = false;
                }
            }
        }


        static void SetError(List<string> errors, List<string> codes, IEnumerable<LayerGroup> layerGroups)
        {
            var groups = layerGroups.Where(i => codes.Contains(i.GroupCode));

            if (groups.Select(i => i.LayerNum).Sum() > 0) //有值
            {
                if (groups.Count() < codes.Count)
                {
                    var groupCodes = codes.Except(groups.Select(i => i.GroupCode));
                    errors.Add($"{string.Join(",", groupCodes)} 图层无图片");
                }


            }
        }

        public static IEnumerable<string> AllowColorGroup()
        {
            if (IsDiy)
            {
                return new List<string>
                {

                    "0101","0102","0103","0104","0105","0106","0107","0108",
                    "0201","0202","0203","0204","0205","0206","0207","0208",
                    "0301","0302","0303","0304","0305","0306","0307","0308","0309",
                    "0401","0402","0403","0404","0405","0406","0407","0408","0409",
                    "0501","0502","0503","0504","0505","0506","0507","0508",
                    "3001","3002","3003","3004","3005","3006","3007","3008",

                };
            }

            return new List<string>
            {
              "0201","0202","0203","0204","0205","0206","0207","0102",
              "0301","0302","0303","0304","0305",
              "0501","0502",
              "0601","0602","0603","0604","0605","0606","0607","0608",
              "0701","0702","0703","0704","0705","0706","0707","0708","0709","0710",
              "0801","0802","0803","0804","0805","0806","0807",
              "0901","0905","0906","0907","0908","0902","0903","0909",
            };
        }
        static bool IsDiy = ConfigurationManager.AppSettings.AllKeys.Contains("DIY");
        public static IEnumerable<string> NoEditGroup()
        {
            if (IsDiy)
                return new List<string>() {
                    "0107", "0108",
                    "0203", "0206",
                    "0305", "0306", "0307", "0308",
                    "0406", "0407", "0408",
                    "0504", "0506", "0507",
                };

            return new List<string>() {
                   "0204","0206","0207",
                   "0302","0305",
                   "0706","0707","0708","0709",
                   "0803","0805","0806",
                   "0906","0907","0908"
                };
        }

        public static List<string> ComplexCodeHead()
        {
            return new List<string> { "02", "03", "06", "07", "08", "09" };
        }

        public static string[] GetLanguageCode()
        {
            if (IsDiy) return new string[] { "0104", "0207", "0208", "0303", "0403", "0404", "0502", "0503" };
            return new string[] { "0202", "0501", "0502", "0702", "0703", "0801", "0802", "0902", "0903", "1803", "1808", "1902", "2106", "2107", "2202", "2203" };
        }
        public static IEnumerable<AddrLayerCode>? GetAddrLayerCodes()
        {
            return new List<AddrLayerCode>
            {
              new AddrLayerCode{ GroupCode="0205",SubCodes=new List<string>{"0204" } },
              new AddrLayerCode{ GroupCode="0804",SubCodes=new List<string>{"0803" } },
              //new AddrLayerCode{ GroupCode="0301",SubCodes=new List<string>{"0302" } },
              //new AddrLayerCode{ GroupCode="0304",SubCodes=new List<string>{"0305" } },
              //new AddrLayerCode{ GroupCode="0705",SubCodes=new List<string>{"0706","0707","0708","0709" } },
              //new AddrLayerCode{ GroupCode="0804",SubCodes=new List<string>{"0805","0806" } },
              //new AddrLayerCode{ GroupCode="0905",SubCodes=new List<string>{"0906","0907","0908","0909" } },
            };
        }

        public static string GetParentCode(string groupCode)
        {
            var list = GetAddrLayerCodes();
            var layerCode = list!.Where(i => i.SubCodes!.Contains(groupCode)).FirstOrDefault();
            if (layerCode != null) return layerCode.GroupCode!;
            return String.Empty;
        }

        public static bool IsParentCode(string groupCode)
        {
            var list = GetAddrLayerCodes();
            return list!.Select(i => i.GroupCode).Contains(groupCode);
        }

        public static List<string> LocationNames => new List<string>
        {
          "左上","中上","右上",
          "左中","正中","右中",
          "左下","中下","右下"
        };

        public static IEnumerable<LayerGroup> CopyLayerGroups(this IEnumerable<LayerGroup> groups)
        {
            return groups.Select(i => new LayerGroup
            {
                GroupCode = i.GroupCode,
                GroupName = i.GroupName,
                IsEnable = i.IsEnable,
                IsShow = i.IsShow,
                ShowIndex = i.ShowIndex,
                LanguageCount = i.LanguageCount,
                LongShow = i.LongShow,
                LanguagePage = i.LanguagePage,
                Layers = i.Layers?.Select(CopyLayerNoPath).ToList()
            }).ToList();
        }

        public static Layer CopyLayerNoPath(Layer l)
        {
            var layer = new Layer
            {
                Height = l.Height,
                Width = l.Width,
                Id = l.Id,
                //ImgPath = l.ImgPath,
                LayerIndex = l.LayerIndex,
                Angle = l.Angle,
                BitmapSource = l.BitmapSource,
                // LayerColor = l.LayerColor,
                AbsoluteLeft = l.AbsoluteLeft,
                AbsoluteTop = l.AbsoluteTop,
                Left = l.Left - l.AbsoluteLeft,
                Top = l.Top - l.AbsoluteTop,
                Ratio = l.Ratio,
                NotDye = l.NotDye,
                ColorDesc = (l.IsPng ?? false) && (!l.NotDye) ? l.ColorDesc : String.Empty,
            };
            layer.ImgPath = l.ImgPath;
            return layer;
        }

        public static IEnumerable<LayerGroupCheck> CopyLayerGroupChecks(this IEnumerable<LayerGroup> groups)
        {
            return groups.Select(i => new LayerGroupCheck
            {
                GroupCode = i.GroupCode,
                GroupName = i.GroupName,
                IsEnable = i.IsEnable,
                IsShow = true,
                ShowIndex = i.ShowIndex,
                NumStatus = true,
                Layers = i.Layers?.Select(CopyLayerNoPath).ToList()
            }).ToList();
        }

        public static IEnumerable<LayerGroupLocation> CopyLayerGroupLocations(this IEnumerable<LayerGroup> groups)
        {
            return groups.Select(i => new LayerGroupLocation
            {
                GroupCode = i.GroupCode,
                GroupName = i.GroupName,
                IsEnable = i.IsEnable,
                IsShow = true,
                ShowIndex = i.ShowIndex,
                NumStatus = true,
                Layers = i.Layers?.Select(CopyLayerNoPath).ToList()
            }).ToList();
        }

        public static List<LayerGroup> GetSimpleLayerGroups(string path)
        {
            var source = Directory.GetDirectories(path);
            return (from i in source
                    let info = Path.GetFileNameWithoutExtension(i).Split('_')
                    where info[0].Contains("辅助文件")
                    select new LayerGroup
                    {
                        Layers = (from j in Directory.GetFiles(i).Where(f => f.Contains("效果"))

                                  select new Layer
                                  {
                                      ImgPath = j,
                                  }).OrderBy(j => j.LayerIndex).ToList(),
                    }).ToList();
        }

        public static Layer? GetBatchLayerGroups(string path)
        {

            return (from j in Directory.GetFiles(path).Where(f => f.Contains("designsketch"))
                    select new Layer
                    {
                        ImgPath = j,
                    }).FirstOrDefault();
        }
    }

    public class AddrLayerCode
    {
        public AddrLayerCode()
        {
            SubCodes = new List<string>();
        }
        public string? GroupCode { get; set; }

        public IEnumerable<string>? SubCodes { get; set; }
    }

}
