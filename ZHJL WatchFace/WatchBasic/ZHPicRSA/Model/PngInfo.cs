using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.Tool;

namespace WatchBasic.ZHPicRSA.Model
{
    public class PngInfo
    {
        public PngInfo()
        {
            Heads = new List<byte>();
            Image = new List<byte>();
        }
        public List<byte> Heads { get; set; }
        public List<byte> Image { get; set; }
        public int Size => Heads.Count();
    }

    /// <summary>
    /// 分段描述
    /// </summary>
    public class PngSegmentDescr
    {
        public PngSegmentDescr()
        {
            Colors = new List<Color>();
        }
        public int XStart { get; set; }
        public int Y { get; set; }
        public IList<Color> Colors { get; set; }

    }

    public class BitmapRow
    {
        public BitmapRow()
        {
            Colors = new List<Color>();
        }
        public List<Color> Colors { get; set; }
        public int XStart { get; set; }

        public int RowWidth { get; set; }

        public int YIndex { get; set; }

        public void FinishRow()
        {
            var list = Colors.Select((i, index) => new { index = index, color = i }).Where(i => i.color.A > 0).ToList();
            XStart = list?.Count == 0 ? 0 : list?.FirstOrDefault()?.index ?? 0;
            RowWidth = list?.Count == 0 ? 0 : (list?.LastOrDefault()?.index ?? 0) - XStart + 1;
            Colors = Colors.Skip(XStart).Take(RowWidth).ToList();
        }
    }

    public class PointInfo
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Addr { get; set; }

        public int Size { get; set; }

        public int PxCount { get; set; }

        public List<byte>? Heads { get; set; }

        public List<byte>? Image { get; set; }

        public List<byte>? RowData { get; set; }


        //public static PointInfo GetPointInfoBmp(Bitmap bitmap)
        //{
        //    PointInfo point = new PointInfo();
        //    List<BitmapRow> rows = GetBitmapRows(bitmap, false).ToList();
        //    List<byte> rowInfo = new List<byte>();
        //    List<byte> colorData = new List<byte>();
        //    int w = 0;
        //    foreach (var row in rows)
        //    {
        //        rowInfo.AddRange(((uint)row.XStart).GetBytes16());
        //        w += row.RowWidth;
        //        rowInfo.AddRange(((uint)w).GetBytes16());
        //        var color565s = row.Colors.Select(i => ImageUtils.GetBytesRGB565(i)).SelectMany(i => i).ToList();
        //        colorData.AddRange(color565s);
        //    }
        //    point.Size = 4 * (point.Height);
        //    point.RowData = rowInfo;
        //    point.Image = colorData;
        //    return point;
        //}

        static IEnumerable<BitmapRow> GetBitmapRows(Bitmap bitmap, bool png, ImagePoint? point = null)
        {
            List<BitmapRow> rows = new List<BitmapRow>();
            for (var i = 0; i < bitmap?.Height; i++)
            {
                BitmapRow row = new BitmapRow();
                row.YIndex = i;
                for (var j = 0; j < bitmap?.Width; j++)
                {
                    var color = bitmap.GetPixel(j, i);
                    row.Colors.Add(color);
                    if (png && color.A > 0)
                    {
                        point!.Top = Math.Min(point.Top, i);
                        point!.Bottom = Math.Max(point.Bottom, i);
                        point!.Left = Math.Min(point.Left, j);
                        point!.Right = Math.Max(point.Right, j);
                    }
                }
                row.FinishRow();
                rows.Add(row);
            }
               
            if (point!.Top== 0xffff) 
            {
                point!.Top = 0;
                point!.Bottom = bitmap.Height;
            }
            if(point!.Left== 0xffff) 
            {
                point!.Top = 0;
                point!.Right = bitmap.Width;
            }
            return rows;
        }

        public static PointInfo GetPointInfo(Bitmap bitmap, bool isARGB = false)
        {
            PointInfo point = new PointInfo();
            ImagePoint imagePoint = new ImagePoint
            {
                Top = 0xffff,
                Bottom = 0,
                Left = 0xffff,
                Right = 0,
            };
            List<BitmapRow> rows = GetBitmapRows(bitmap, true, imagePoint).ToList();
            List<byte> heads = new List<byte>();
            List<byte> rowInfo = new List<byte>();
            point.X = imagePoint.Left;
            point.Y = imagePoint.Top;
            point.Width = imagePoint.Width;
            point.Height = imagePoint.Height;

            heads.AddRange(point.Height.GetBytes16()); //行高
            heads.AddRange(0.GetBytes32());//地址
            heads.AddRange(point.X.GetBytes16());//x
            heads.AddRange(point.Y.GetBytes16()); //y
            heads.AddRange(point.Width.GetBytes16());//width


           
            List<byte> colorData = new List<byte>();
            int w = 0;
            rows = rows.Skip(point.Y).Take(point.Height).ToList();
            point.Size = 12 + 4 * (point.Height);
            point.PxCount = rows.SelectMany(i => i.Colors).Count();
            foreach (var row in rows)
            {
                rowInfo.AddRange(row.XStart.GetBytes16());
                w += row.RowWidth;
                rowInfo.AddRange(w.GetBytes16());
                if (!isARGB)
                {
                    colorData.AddRange(ImageUtils.GetPngBytesOnlyA(row.Colors));
                }
                else
                {
                    colorData.AddRange(ImageUtils.GetPngBytes(row.Colors,isARGB));
                }
            }
            point.Heads = heads;
            point.RowData = rowInfo;
            point.Image = colorData;
            return point;
        }

        public class ImagePoint
        {
            public int Top { get; set; }
            public int Left { get; set; }

            public int Bottom { get; set; }

            public int Right { get; set; }

            public int Width => Right - Left + 1;
            public int Height => Bottom - Top + 1;

        }


    }
}
