using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WatchBasic;
using WatchBasic.Language;

namespace ZHAPI
{
    public class WatchAPI
    {
        public readonly static string UpdateMainUrl = "https://test.wearheart.cn/zhkj/admin/infowear/diy/save";
        public readonly static string UpdatePointUrl = "https://test.wearheart.cn/zhkj/admin/infowear/diyHands/save";
        public readonly static string UpdateDetailUrl = "https://test.wearheart.cn/zhkj/admin/infowear/dataElementSet/save";
        public readonly static string SearchWatchIdUrl = "https://test.wearheart.cn/zhkj/admin/infowear/diy/diyInfo";
        public readonly static string CancelUploadUrl = "https://test.wearheart.cn/zhkj/admin/infowear/diy/delDiy";
        public readonly static string UpdateTimeUrl = "https://test.wearheart.cn/zhkj/admin/infowear/dialDiyNumber/save";
        public readonly static string UpdateTimeLocationUrl = "https://test.wearheart.cn/zhkj/admin/infowear/dialDiyNumberLocation/save";
        public readonly static string UpdatePointScaleUrl = "https://test.wearheart.cn/zhkj/admin/infowear/dialDiyCircle/save";


        public static void Updata_MainAsync(WatchMain main, string savePath)
        {
            var client = new RestClient(UpdateMainUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AlwaysMultipartFormData = true;
            AddPostForm(request, savePath, main);
            GetResult(client, request);

        }

        public static void Update_Point(WatchPointer pointer, string savePath)
        {
            var client = new RestClient(UpdatePointUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AlwaysMultipartFormData = true;
            AddPostForm(request, savePath, pointer);
            GetResult(client, request);
        }

        public static void Update_WatchPointScale(WatchPointScale scale, string savePath)
        {
            var client = new RestClient(UpdatePointScaleUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AlwaysMultipartFormData = true;
            AddPostForm(request, savePath, scale);
            GetResult(client, request);
        }

        public static string Update_Time(WatchTime time, string savePath)
        {
            var client = new RestClient(UpdateTimeUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AlwaysMultipartFormData = true;
            AddPostForm(request, savePath, time);
            return GetTimeResult(client, request);
        }

        public static void Update_TimeLocation(WatchTimeLocation timeLocation, string savePath)
        {
            var client = new RestClient(UpdateTimeLocationUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AlwaysMultipartFormData = true;
            AddPostForm(request, savePath, timeLocation);
            GetResult(client, request);
        }


        static void GetResult(RestClient client, RestRequest request)
        {
            var result = JsonConvert.DeserializeObject<Result>(client.Execute(request).Content);
            if (result.code != 0)
            {
                throw new Exception(result.msg);
            }
        }
        static string GetTimeResult(RestClient client, RestRequest request)
        {

            var result = JsonConvert.DeserializeObject<TimeResult>(client.Execute(request).Content);
            if (result.code != 0)
            {
                throw new Exception(result.msg);
            }
            return result.Id;
        }


        public static void Update_Detial(WatchDetail detail, string savePath)
        {
            var client = new RestClient(UpdateDetailUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AlwaysMultipartFormData = true;
            AddPostForm(request, savePath, detail);
            GetResult(client, request);

        }

        public static void AddPostForm(RestRequest formData, string savePath, object main)
        {
            var type = main.GetType();
            foreach (var p in type.GetProperties())
            {
                var obj = p.GetValue(main);
                var file = p.GetCustomAttribute<FileAttribute>();
                if (file != null)
                {
                    if (obj.GetType().Name.Contains("List"))
                    {
                        var list = obj as List<string>;
                        foreach (var l in list)
                        {
                            formData.AddFile(p.Name, savePath + l);
                        }
                    }
                    else
                    {
                        formData.AddFile(p.Name, savePath + obj?.ToString());
                    }

                }
                else
                {
                    formData.AddParameter(p.Name, obj?.ToString());
                }
            }
        }

        public static async Task CancelUploadAsync(string watchId, int deviceType)
        {
            var client = new RestClient(CancelUploadUrl);
            var request = new RestRequest() { Method = Method.Post };

            var json = new
            {
                dialCode = watchId,
                deviceType = deviceType
            };

            var jsonstr = JsonConvert.SerializeObject(json);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", jsonstr, ParameterType.RequestBody);
            var result = JsonConvert.DeserializeObject<Result>(client.Execute(request).Content);
            if (result.code != 0)
            {
                throw new Exception(result.msg);
            }

        }

        public static void UpdateWatch(JsonWatch watch, int deviceType, string savePath, string lang)
        {
            var main = GetWatchMain(watch, deviceType, lang);
            Updata_MainAsync(main, savePath);
            var sort = 0;
            foreach (var p in watch.pointers)
            {
                WatchPointer watchPointer = new WatchPointer()
                {
                    dialCode = watch.watchCode,
                    deviceType = deviceType,
                    sort = sort,
                    type = 1,
                    binName = Path.GetFileNameWithoutExtension(p.pointerDataPath),
                    binFile = p.pointerDataPath,
                    renderingsFile = p.pointerImgPath,
                    pointerPictureFile = p.pointerOverlayPath,
                };
                Update_Point(watchPointer, savePath);
                sort++;
            }

            foreach (var p in watch.time)
            {
                var watchTime = new WatchTime()
                {
                    dialCode = watch.watchCode,
                    deviceType = deviceType,
                    fontsName = p.fontsName,
                    binName = Path.GetFileNameWithoutExtension(p.timeDataPath),
                    binFile = p.timeDataPath,
                    renderingsFile = p?.locationInfos?.FirstOrDefault()?.timeImgPath,
                };
                var id = Update_Time(watchTime, savePath);
                var isfirst = false;
                p.locationInfos.ToList().ForEach(l =>
                {
                    bool defaultval = false;
                    if (!isfirst)
                    {
                        defaultval = true;
                        isfirst = true;
                    }
                    var location = new WatchTimeLocation
                    {
                        fDefault = defaultval,
                        fLocationName = l.locationName,
                        backimageFile = l.timeOverlayPath,
                        effectFile = l.timeImgPath,
                        fTextInfos = JsonConvert.SerializeObject(l.textInfos),
                        id = id,
                    };
                    Update_TimeLocation(location, savePath);
                });

            }
            sort = 0;
            foreach (var scale in watch.scaleImages)
            {
                WatchPointScale pointScale = new WatchPointScale
                {
                    fDeviceType = deviceType,
                    fCode = watch.watchCode,
                    dialFile = scale.overlayImgPath,
                    renderingsFile = scale.designsketchImgPath,
                    fSort = sort,
                };
                Update_WatchPointScale(pointScale, savePath);
            }

            foreach (var i in watch.complex.infos)
            {
                foreach (var c in i.detail)
                {
                    WatchDetail watchDetail = new WatchDetail()
                    {
                        dialCode = watch.watchCode,
                        deviceType = deviceType,
                        type = GetWatchType[i.location],
                        dataElementCode = ElementCode[c.typeName],
                        dataElementX = c.pointX.ToString(),
                        dataElementY = c.pointY.ToString(),
                        imgFile = c.picPath,
                        selected = c.isDefault ? 1 : 0,

                    };
                    Update_Detial(watchDetail, savePath);
                }
            }
        }

        public static IEnumerable<string> SearchWatch(string? deviceType)
        {
            var client = new RestClient($"{SearchWatchIdUrl}?deviceType={deviceType}");

            var request = new RestRequest() { Method = Method.Get };
            var res = client.Execute(request).Content;
            var result = JsonConvert.DeserializeObject<SearchRes>(res);
            if (result.code != 0)
            {
                return new List<string>();
                // throw new Exception(result.msg);
            }
            return result.list;

        }

        static WatchMain GetWatchMain(JsonWatch watch, int deviceType, string lang)
        {

            return new WatchMain
            {
                dialCode = watch.watchCode,
                dialName = watch.watchName,
                dialDesc = watch.watchDesc,
                deviceType = deviceType,
                deviceVersion = 1,
                sort = 0,
                backgroundBinCompressed = 0,
                pointerCompressed = 0,
                complexBinCompressed = 0,
                highLowMark = 0,
                defaultBackgroundImage = watch?.background?.backgroundImgPath,
                backgroundOverlay = watch?.overlayImgPaths?.ToList(),
                renderings = watch?.background?.designsketchImgPath,
                complicationsBin = watch?.complex?.path,
                deviceParsingRules = watch?.watchVersion,
                dpi = watch?.dpi,
                thumbnailDpi = watch?.thumbnailDpi,
                thumbnailOffset = watch?.thumbnailOffset,
                languageCode = LanguageFactory.GetDIYServiceLangCode(lang),
                dialType = watch.pointers.Count() > 0 ? 0 : 1,
            };

        }

        static Dictionary<string, int> GetWatchType => new Dictionary<string, int>
        {
            {"LeftUp",1 },
            {"MidUp",2 },
            {"RightUp",3 },
            {"LeftMid",4 },
            {"Mid",5 },
            {"RightMid",6 },
            {"LeftBottom",7 },
            {"MidBottom",8 },
            {"RightBottom",9 },
        };
        static Dictionary<string, string> ElementCode => new Dictionary<string, string>
        {
            {"Kwh","1" },
            {"GeneralDate","2" },
            {"Step","3" },
            {"Calorie","4" },
            {"HeartRate","5" },

        };




    }

}



