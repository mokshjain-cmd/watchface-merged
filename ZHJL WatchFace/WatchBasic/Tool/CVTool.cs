using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.Tool
{
    public class CVTool
    {
        /// <summary>
        /// 图片旋转
        /// </summary>
        /// <param name="srcPic">原图片</param>
        /// <param name="angle">旋转的角度</param>
        /// <param name="scale">缩放比例</param>
        public static Bitmap ImgRotate(Image<Bgra, Byte> srcPic, double angle, double scale)
        {
            //旋转中心点
            PointF center = new PointF(srcPic.Cols / 2, srcPic.Rows / 2);
            Mat mapMat = new Mat();
            CvInvoke.GetRotationMatrix2D(center, angle, scale, mapMat);
            //CvInvoke.Imshow("mapMat", mapMat);  //发现显示只有标题栏，无图像
            Mat dstPic = new Mat();
            //  CvInvoke.WarpAffine(srcPic, dstPic, mapMat, new Size(srcPic.Cols, srcPic.Rows), borderMode: Emgu.CV.CvEnum.BorderType.Transparent);
            CvInvoke.WarpAffine(srcPic, dstPic, mapMat, new Size(srcPic.Cols, srcPic.Rows), Emgu.CV.CvEnum.Inter.Cubic, borderMode: Emgu.CV.CvEnum.BorderType.Constant);
            //var bitmap = dstPic.ToBitmap();
            //bitmap.Save($"{Math.Abs(angle)}.png");
            return dstPic.ToBitmap();
        }

        public static Image<Bgra, Byte> ImgInit(string fileName) 
        {
           var image= CvInvoke.Imread(fileName, Emgu.CV.CvEnum.ImreadModes.Unchanged);
           var input= image.ToImage<Bgra, byte>();
            return input;
            
        }

    }
}
