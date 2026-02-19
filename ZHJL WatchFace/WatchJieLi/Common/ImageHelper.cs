using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZHAPI;

namespace WatchJieLi.Common
{
    public class ImageHelper
    {
        public DllInvoke dll { get; set; }

        public ImageHelper(string Path)
        {
           
            dll = new DllInvoke(Path);
        }


        [DllImport(@"img64\\image.dll", EntryPoint = "image_compress", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr image_compress2(IntPtr info);


        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        delegate IntPtr _image_compress(IntPtr info);
        public IntPtr image_compress(IntPtr info)
        {
            return ((_image_compress)dll.Invoke("image_compress", typeof(_image_compress)))(info);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct image_info
    {
        public int width;
        public int height;
        public int format;
        public IntPtr buf;
        public int len;
        //public image_info(int width, int height, int format, IntPtr buf, int len)
        //{
        //    this.width = width;
        //    this.height = height;
        //    this.format = format;
        //    this.buf = buf;
        //    this.len=len;
        //}
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct image_buf
    {
        public int len;
        public IntPtr buf;
        //public image_buf(int len,IntPtr buf) 
        //{
        //    this.len = len;
        //    this.buf=buf;
        //}
    }

    public class DllInvoke
    {
        [DllImport("kernel32.dll")]
        private extern static IntPtr LoadLibrary(String path);
        [DllImport("kernel32.dll")]
        private extern static IntPtr GetProcAddress(IntPtr lib, String funcName);
        [DllImport("kernel32.dll")]
        private extern static bool FreeLibrary(IntPtr lib);
        public IntPtr hLib;
        public DllInvoke(String DLLPath)
        {
            hLib = LoadLibrary(DLLPath);
        }
        //释放
        ~DllInvoke()
        {
            FreeLibrary(hLib);
        }
        //将要执行的函数转换为委托
        public Delegate Invoke(String APIName, Type t)
        {
            IntPtr api = GetProcAddress(hLib, APIName);
            // Marshal.(api.ToString());
            return (Delegate)Marshal.GetDelegateForFunctionPointer(api, t);
        }
    }
}
