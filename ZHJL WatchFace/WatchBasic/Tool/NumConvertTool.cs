using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.Tool
{
    public static class NumConvertTool
    {

        public static byte[] GetBytes16(this uint value, bool forward = false)
        {
            if (forward)
            {
                return BitConverter.GetBytes(value).Reverse().Skip(2).Take(2).ToArray();
            }
            return BitConverter.GetBytes(value).Take(2).ToArray();
        }

        public static byte[] GetBytes16(this int value, bool forward = false)
        {
            if (forward)
            {
                return BitConverter.GetBytes(value).Reverse().Skip(2).Take(2).ToArray();
            }
            return BitConverter.GetBytes(value).Take(2).ToArray();
        }

        public static byte[] GetBytes(this uint value, int take, bool forward = false)
        {
            if (forward)
            {
                var bytes = BitConverter.GetBytes(value).Reverse();
                var skip = bytes.Count() - take;
                var temp = BitConverter.GetBytes(value).Reverse().ToList();
                return BitConverter.GetBytes(value).Reverse().Skip(skip).Take(take).ToArray();
            }
            return BitConverter.GetBytes(value).Take(take).ToArray();
        }

        public static byte[] GetBytes(this int value, int take, bool forward = false)
        {
            if (forward)
            {
                var bytes = BitConverter.GetBytes(value).Reverse();
                var skip = bytes.Count() - take;
                return BitConverter.GetBytes(value).Reverse().Skip(skip).Take(take).ToArray();
            }
            return BitConverter.GetBytes(value).Take(take).ToArray();
        }


        public static byte[] GetBytes32(this uint value, bool forward = false)
        {
            if (forward)
            {
                return BitConverter.GetBytes(value).Reverse().ToArray();
            }
            return BitConverter.GetBytes(value).ToArray();
        }

        public static byte[] GetBytes32(this int value, bool forward = false)
        {
            if (forward)
            {
                return BitConverter.GetBytes(value).Reverse().ToArray();
            }
            return BitConverter.GetBytes(value).ToArray();
        }

        public static UInt16 GetInt16(this byte[] bytes,bool forward=false) 
        {
            if (forward) 
            {
                bytes = bytes.Reverse().ToArray();
            }
            return BitConverter.ToUInt16(bytes);
        }

        public static UInt16 GetInt16(this IEnumerable<byte> bytes, bool forward = false) 
        {
            if (forward)
            {
                bytes = bytes.Reverse().ToArray();
            }
            if (bytes.Count() == 1) 
            {
                return (ushort)bytes.ToArray()[0];
            }
            return BitConverter.ToUInt16(bytes.ToArray());
        }

        public static UInt32 GetInt32(this IEnumerable<byte> bytes, bool forward = false)
        {
            if (forward)
            {
                bytes = bytes.Reverse().ToArray();
            }
            return BitConverter.ToUInt32(bytes.ToArray());
        }

        public static UInt32 GetInt32(this byte[] bytes, bool forward = false)
        {
            if (forward)
            {
                bytes = bytes.Reverse().ToArray();
            }
            return BitConverter.ToUInt32(bytes);
        }

    }
}
