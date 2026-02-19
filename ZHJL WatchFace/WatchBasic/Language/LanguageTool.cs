using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.Language
{
    public interface WatchLanguageService
    {
        Dictionary<string, int> Languages();

    }


    public class M65A : WatchLanguageService
    {
        public Dictionary<string, int> Languages()
        {
            return new Dictionary<string, int>
            {
                {"英文",1 },
                {"繁体",2 },
                {"中文",3 }
            };
        }
    }

    public class DIY : WatchLanguageService 
    {
        public Dictionary<string, int> Languages()
        {
            return new Dictionary<string, int>
            {
                {"英文",1 },
                {"中文",2 }
            };
        }
    }
    public class E27 : WatchLanguageService
    {
        public Dictionary<string, int> Languages()
        {
            return new Dictionary<string, int>
            {
                {"英文",1 },
                {"中文",2 },
                {"繁体",3 }
            };
        }
    }

    public class LanguageFactory
    {
        public LanguageFactory(string? name)
        {
            Service=GetLanguageService(name);
        }

        public  WatchLanguageService? Service { get; set; }

        public WatchLanguageService? GetLanguageService(string? name) 
        {
            switch (name)
            {
                case "M65A": return  new M65A();
                case "DIY":return new DIY();
                case "E27": return new E27();
                case "N019": return new E27();
                case "410*502":
                case "228*460":
                case "128*220":
                case "135*240":
                case "240*240":
                case "320*320":
                case "360*360":
                case "320*385":
                case "454*454":
                case "240*198":
                case "240*204":
                case "240*210":
                case "356*400":
                case "360*400":
                case "368*448":
                case "400*454":
                case "320*380": return new E27();
                default: return new E27();
            }
        }

         
        public  int GetLayerOrder(string? lang)
        {
            return Service.Languages()[lang] - 1;
        }

        public  string? GetIdCode(string? lang)
        {
            return Service?.Languages()[lang].ToString("00");
        }

        public static int GetDIYServiceLangCode(string lang) 
        {
            return lang switch
            {
                "中文"=>113,
                "英文"=>104,
                _ => 0,
            };
        }
       

       


    }



}
