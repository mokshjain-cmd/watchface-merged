using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.ZHPicRSA
{
    /// <summary>
    /// 安卓风格化代码
    /// </summary>
    public class ChangeColorUtils
    {

        //  private static final String TAG = ChangeColorUtils.class.getSimpleName();


        public static int[] myRgbToHsv(int input_R, int input_G, int input_B)
        {

            //        MyLog.i(TAG, "数据转换1 = input_R=" + input_R + "  input_G=" + input_G + "  B=" + B);

            int[] result = new int[3];

            double H = 0;
            double S = 0;
            double V = 0;

            int max = Math.Max(input_R, Math.Max(input_G, input_B));
            int min = Math.Min(input_R, Math.Min(input_G, input_B));

            //           MyLog.i(TAG,"数据转换 = max = " + max);
            //           MyLog.i(TAG,"数据转换 = min = " + min);

            V = max;

            double x1 = (max - min);

            //           MyLog.i(TAG,"数据转换 = x1 = " + x1);

            if (max != 0)
            {
                S = x1 * 360 / max;
            }

            double RX = input_G - input_B;
            double GX = input_B - input_R;
            double BX = input_R - input_G;

            //           MyLog.i(TAG,"数据转换 = RX = " + RX);
            //           MyLog.i(TAG,"数据转换 = GX = " + GX);
            //           MyLog.i(TAG,"数据转换 = BX = " + BX);

            if (input_R == max)
            {
                //               MyLog.i(TAG,"数据转换 = input_R = max");
                if (RX != 0 && x1 != 0)
                {
                    //                   MyLog.i(TAG,"数据转换 = Rmax!=0");
                    double rxv1 = RX / x1 * 60;
                    //                   MyLog.i(TAG,"数据转换 = rxv1=" + rxv1);
                    H = rxv1;
                }
                else
                {
                    //                   MyLog.i(TAG,"数据转换 = Rmax==0");
                }
            }
            else if (input_G == max)
            {
                //               MyLog.i(TAG,"数据转换 = input_G = max");
                if (GX != 0 && x1 != 0)
                {
                    //                   MyLog.i(TAG,"数据转换 = Gmax!=0");
                    double gxv1 = GX / x1 * 60;
                    //                   MyLog.i(TAG,"数据转换 = gxv1=" + gxv1);
                    H = 120 + gxv1;
                }
                else
                {
                    //                   MyLog.i(TAG,"数据转换 = Gmax==0");
                    H = 120;
                }
            }
            else if (input_B == max)
            {
                //               MyLog.i(TAG,"数据转换 = B = max");
                if (BX != 0 && x1 != 0)
                {
                    //                   MyLog.i(TAG,"数据转换 = Bmax!=0");
                    double bxv1 = BX / x1 * 60;
                    //                   MyLog.i(TAG,"数据转换 = bxv1=" + bxv1);
                    H = 240 + bxv1;
                }
                else
                {
                    H = 240;
                    //                   MyLog.i(TAG,"数据转换 = Bmax==0");
                }
            }


            if (H < 0)
            {
                H = H + 360;
            }

            //           MyLog.i(TAG,"数据转换x1 = H/360 = " + (H / 360));
            //           MyLog.i(TAG,"数据转换x1 = S/360 = " + (S / 360));
            //           MyLog.i(TAG,"数据转换x1 = V/255 = " + (V / 255));

            //        result[0] = (int) H;
            //        result[1] = (int) S;
            //        result[2] = (int) V;

            result[0] = (int)(H);
            result[1] = (int)(S / 360 * 100);
            result[2] = (int)(V / 255 * 100);

            return result;

        }

        public static int[] myHsvToRgb(int inputH, int inputS, int inputV)
        {

            int[] result = new int[3];

            double H = inputH;
            double S = inputS;
            double V = inputV;

            //        MyLog.i(TAG, "数据转换2 = H=" + H + "  S=" + S + "  V=" + V);

            double R = 0;
            double G = 0;
            double B = 0;

            V = V * 255 / 100;


            if (H >= 360)
            {
                H = 359;
            }


            if (S == 0)
            {
                R = G = B = V;
            }
            else
            {
                H = H / 60;

                //            MyLog.i(TAG, "数据转换x2 = H =" + H);

                int index = (int)Math.Floor(H);
                //            MyLog.i(TAG, "数据转换x2 = index =" + index);

                double XF = H - index;
                //            MyLog.i(TAG, "数据转换x2 = XF =" + XF);

                double XP = V * (100 - S);
                double XQ = V * (100 - S * XF);
                double XT = V * (100 - S * (1 - XF));

                //            MyLog.i(TAG, "数据转换x2 1 = XP =" + XP);
                //            MyLog.i(TAG, "数据转换x2 1 = XQ =" + XQ);
                //            MyLog.i(TAG, "数据转换x2 1 = XT =" + XT);

                XP = XP / 100;
                XQ = XQ / 100;
                XT = XT / 100;

                //            MyLog.i(TAG, "数据转换x2 2 = XP =" + XP);
                //            MyLog.i(TAG, "数据转换x2 2 = XQ =" + XQ);
                //            MyLog.i(TAG, "数据转换x2 2 = XT =" + XT);


                switch (index)
                {
                    case 0:
                        R = V;
                        G = XT;
                        B = XP;
                        break;
                    case 1:
                        R = XQ;
                        G = V;
                        B = XP;
                        break;
                    case 2:
                        R = XP;
                        G = V;
                        B = XT;
                        break;
                    case 3:
                        R = XP;
                        G = XQ;
                        B = V;
                        break;
                    case 4:
                        R = XT;
                        G = XP;
                        B = V;
                        break;
                    //                case 5:
                    //                    R = V;
                    //                    G = XP;
                    //                    B = XQ;
                    //                    break;

                    default:
                        R = V;
                        G = XP;
                        B = XQ;
                        break;

                }


            }

            //        MyLog.i(TAG, "数据转换2 2 = R=" + R + "  G=" + G + "  B=" + B);


            result[0] = (int)R;
            result[1] = (int)G;
            result[2] = (int)B;

            return result;

        }


        public static int[] mychangeColor(int input_R, int input_G, int input_B, int inputH)
        {

            int[] result = new int[3];

            double H = 0;
            double S = 0;
            double V = 0;

            int max = Math.Max(input_R, Math.Max(input_G, input_B));
            int min = Math.Min(input_R, Math.Min(input_G, input_B));


            V = max;

            double x1 = (max - min);


            if (max != 0)
            {
                S = x1 * 360 / max;
            }

            double RX = input_G - input_B;
            double GX = input_B - input_R;
            double BX = input_R - input_G;


            if (input_R == max)
            {
                if (RX != 0 && x1 != 0)
                {
                    double rxv1 = RX / x1 * 60;
                    H = rxv1;
                }
                else
                {
                }
            }
            else if (input_G == max)
            {
                if (GX != 0 && x1 != 0)
                {
                    double gxv1 = GX / x1 * 60;
                    H = 120 + gxv1;
                }
                else
                {
                    H = 120;
                }
            }
            else if (input_B == max)
            {
                if (BX != 0 && x1 != 0)
                {
                    double bxv1 = BX / x1 * 60;
                    H = 240 + bxv1;
                }
                else
                {
                    H = 240;
                }
            }


            if (H < 0)
            {
                H = H + 360;
            }


            H = (int)(H);
            S = (int)(S / 360 * 100);
            V = (int)(V / 255 * 100);

            H = Math.Abs((inputH - H));

            //        MyLog.i(TAG, "数据转换2 = H=" + H + "  S=" + S + "  V=" + V);

            double R = 0;
            double G = 0;
            double B = 0;

            V = V * 255 / 100;


            if (H >= 360)
            {
                H = 359;
            }


            if (S == 0)
            {
                R = G = B = V;
            }
            else
            {
                H = H / 60;

                //            MyLog.i(TAG, "数据转换x2 = H =" + H);

                int index = (int)Math.Floor(H);
                //            MyLog.i(TAG, "数据转换x2 = index =" + index);

                double XF = H - index;
                //            MyLog.i(TAG, "数据转换x2 = XF =" + XF);

                double XP = V * (100 - S);
                double XQ = V * (100 - S * XF);
                double XT = V * (100 - S * (1 - XF));

                //            MyLog.i(TAG, "数据转换x2 1 = XP =" + XP);
                //            MyLog.i(TAG, "数据转换x2 1 = XQ =" + XQ);
                //            MyLog.i(TAG, "数据转换x2 1 = XT =" + XT);

                XP = XP / 100;
                XQ = XQ / 100;
                XT = XT / 100;

                //            MyLog.i(TAG, "数据转换x2 2 = XP =" + XP);
                //            MyLog.i(TAG, "数据转换x2 2 = XQ =" + XQ);
                //            MyLog.i(TAG, "数据转换x2 2 = XT =" + XT);


                switch (index)
                {
                    case 0:
                        R = V;
                        G = XT;
                        B = XP;
                        break;
                    case 1:
                        R = XQ;
                        G = V;
                        B = XP;
                        break;
                    case 2:
                        R = XP;
                        G = V;
                        B = XT;
                        break;
                    case 3:
                        R = XP;
                        G = XQ;
                        B = V;
                        break;
                    case 4:
                        R = XT;
                        G = XP;
                        B = V;
                        break;
                    //                case 5:
                    //                    R = V;
                    //                    G = XP;
                    //                    B = XQ;
                    //                    break;

                    default:
                        R = V;
                        G = XP;
                        B = XQ;
                        break;

                }


            }

            //        MyLog.i(TAG, "数据转换2 2 = R=" + R + "  G=" + G + "  B=" + B);


            result[0] = (int)R;
            result[1] = (int)G;
            result[2] = (int)B;

            return result;
        }


        public static Bitmap myChangeBitmap24(Bitmap? inpuBitmap, int inputH)
        {

            int width = inpuBitmap.Width;
            int height = inpuBitmap.Height;

            // Bitmap bitmap = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888);
            var bitmap = new Bitmap(width, height);
            Color color = default(Color);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {

                    var input_color = inpuBitmap.GetPixel(i, j);

                    int inputA = input_color.A;
                    int inputR = input_color.R;
                    int inputG = input_color.G;
                    int inputB = input_color.B;
                    //                MyLog.i(TAG, "数据转换======================================= i = " + i + " j = " + j);

                    //                int[] hsv = myRgbToHsv(inputR, inputG, inputB);
                    //                int H = hsv[0];
                    //                int S = hsv[1];
                    //                int V = hsv[2];
                    //                H = Math.abs((inputH - H));
                    ////                H = inputH;
                    //                int[] rgb = myHsvToRgb(H, S, V);

                    int[] rgb = mychangeColor(inputR, inputG, inputB, inputH);

                    color = Color.FromArgb(inputA, rgb[0], rgb[1], rgb[2]);
                    bitmap.SetPixel(i, j, color);
                }
            }
            return bitmap;
        }
    }
}
