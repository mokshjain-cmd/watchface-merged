using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.Tool;

namespace WatchBasic.ZHPicRSA.Model
{
    public class BmpInfo
    {
        public BmpInfo()
        {
            BmpRows = new List<BmpRow>();
            Heads = new List<byte>();
            Images = new List<byte>();
        }
        public List<BmpRow> BmpRows { get; set; }

        public List<byte> Heads { get; set; }

        public List<byte> Images { get; set; }

        public void Finished()
        {
            var num = 0;
            foreach (var i in BmpRows)
            {
                num += i.BmpCompresses!.Count();
                if (CommonDefintion.IsColor888) 
                {
                    Heads.AddRange(num.GetBytes32(true));
                }
                else 
                {
                    Heads.AddRange(num.GetBytes16(true));
                }
               
                var data = i.BmpCompresses!.Select(b => b.CompressBytes).SelectMany(b => b);
                Images.AddRange(data);
            }
        }

    }
}
