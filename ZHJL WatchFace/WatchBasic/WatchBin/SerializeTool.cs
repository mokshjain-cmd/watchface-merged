using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.Tool;
using WatchBasic.WatchBin.Model;

namespace WatchBasic.WatchBin
{
    public class SerializeTool
    {
        public static int? ArrayboolConvertByte(bool[]? array)
        {
            return array?.Select(Convert.ToUInt16).Aggregate(0, (acc, x) => (acc << 1) | x);
        }
         
        public static byte[] SerializeObject(IEnumerable<DIYX1> dIYXes) 
        {
            List<byte> bytes = new List<byte>();
            
            bytes.AddRange(GetBytes(dIYXes));
            return bytes.ToArray();
        }

        static byte[]GetBytes(IEnumerable<object> list) 
        {
            List<byte> bytes = new List<byte>();
            list.ToList().ForEach(dx1 =>
            {
                try
                {
                    if (dx1 != null)
                    {
                        var tmp = (DIYX1)dx1;
                         var datas2 = (from type in dx1.GetType().GetProperties().Where(i => i.Name != "Addrs" && i.Name != "HeadSizes" && i.Name != "Code")
                                      let index = type.GetCustomAttributes(true).Where(i => i.GetType() == typeof(PropertyIndex)).FirstOrDefault() as PropertyIndex
                                      let ByteNum = type.GetCustomAttributes(true).Where(i => i.GetType() == typeof(ByteNum)).FirstOrDefault() as ByteNum
                                      select new SerializeDesc
                                      {
                                          ByteNum = ByteNum,
                                          Index = index.Index,
                                          Value = (int)type.GetValue(dx1)
                                      }).OrderBy(j => j.Index).ToList();
                        var dx1Obj = dx1 as DIYX1;
                        if (dx1Obj != null)
                        {
                            if (dx1Obj.Addrs.Count != 12)
                            {
                                dx1Obj.Addrs.AddRange(Enumerable.Range(0, 12 - dx1Obj.Addrs.Count).Select(i => 0));
                            }
                            if (dx1Obj.HeadSizes.Count != 12)
                            {
                                dx1Obj.HeadSizes.AddRange(Enumerable.Range(0, 12 - dx1Obj.HeadSizes.Count).Select(i => 0));
                            }
                            var dxAddrs = dx1Obj.Addrs.Select(i => new SerializeDesc { ByteNum = new ByteNum(4), Value = i }).ToList();
                            var dxHeadSizes = dx1Obj.HeadSizes.Select(i => new SerializeDesc { ByteNum = new ByteNum(4), Value = i }).ToList();
                            var addr = datas2.Where(i => i.Index == 8).FirstOrDefault();
                            var index = datas2.IndexOf(addr);
                            datas2.Remove(addr);
                            var headsize = datas2.Where(i => i.Index == 9).FirstOrDefault();
                            datas2.Remove(headsize);
                            for (var i = 0; i < dxAddrs.Count(); i++)
                            {
                                datas2.Insert(index, dxAddrs[i]);
                                index++;
                                datas2.Insert(index, dxHeadSizes[i]);
                                index++;
                            }
                        }
                        var binData = datas2.Select(i => GetBytes(i.ByteNum, Convert.ToInt32(i.Value))).SelectMany(i => i).ToArray();

                        bytes.AddRange(binData.ToArray());
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
              

            });
            return bytes.ToArray();
        }


        public static byte[] SerializeObject(object obj)
        {
            List<byte> bytes = new List<byte>();
            var type = obj.GetType();

            var list = (from t in type.GetProperties()
                        let index = t.GetCustomAttributes(true).Where(i => i.GetType() == typeof(PropertyIndex)).FirstOrDefault() as PropertyIndex
                        select new
                        {

                            index = index.Index,
                            val = t.GetValue(obj)
                        }).OrderBy(i => i.index).Select(i => i.val).ToList();

            bytes.AddRange(GetBytes(list));
            return bytes.ToArray();


        }
        static byte[] GetBytes(ByteNum num, int val)
        {
           
            return val.GetBytes(num.Num);
        }
    }

    public class SerializeDesc
    {
        public int Index { get; set; }
        public ByteNum ByteNum { get; set; }

        public int Value { get; set; }
    }
}
