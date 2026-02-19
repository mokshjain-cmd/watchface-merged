using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;

namespace WatchUI.UI
{
    public class DIYGroupsModel
    {
        public DIYGroupsModel(string path, bool notBitmapSource = false)
        {
            Complex = new List<LayerGroup>();
            Pointers = new List<PointerImage>();
            Times = new List<TimeImage>();
            var backgroundGroups = path.GetLayerGroups(notBitmapSource).ToList();
            Background = new WatchBackgroundItem(backgroundGroups,path);

            var complexFolder = Directory.GetDirectories(path).Where(i => i.Contains("0201")).FirstOrDefault();
            if (complexFolder != null)
            {
                Complex = complexFolder.GetLayerGroups(notBitmapSource);
            }
            // 指针
            var pointerFolder = Directory.GetDirectories(path ?? String.Empty).FirstOrDefault(i => i.Contains("0302"));
            if (pointerFolder != null)
            {
                var points = (from i in Directory.GetDirectories(pointerFolder ?? String.Empty)
                              let groups = i.GetLayerGroups(notBitmapSource)
                              select new PointerImage(groups.ToList(), i)).ToList();
                Pointers = points.Where(i => i.IsVaild);
            }
            var timeFolder = Directory.GetDirectories(path ?? String.Empty).FirstOrDefault(i => i.Contains("0301"));
            if(timeFolder != null) 
            {
                var time = (from i in Directory.GetDirectories(timeFolder ?? String.Empty)
                            let groups = i.GetLayerGroups(notBitmapSource).Where(i=>i.Layers.Count()>0)
                            select new TimeImage(groups.ToList(), i)).ToList();
                Times = time.Where(i => i.IsVaild);
            }
        }
        public WatchBackgroundItem? Background { get; set; }

        public IEnumerable<LayerGroup>? Complex { get; set; }

        public IEnumerable<PointerImage>? Pointers { get; set; }

        public IEnumerable<TimeImage>? Times { get; set; }

    }
}
