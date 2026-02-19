using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBin.Model;

namespace WatchBin.BinAnalyze
{
    public class BinAnalyzeHelper
    {
        private static List<RecordBase> uids = new();

        public static void GetHeaderData(byte[] all)
        {
            var data = all[0..168];
            var magic_word = BitConverter.ToUInt32(data[0..4]);
            var version = data[4..24];
            var colorgroupcount = data[24];
            var themecount = data[28];
            var colorcount = data[29];
            var flags = data[30..31];
            var preview_img_data_addr = data[32..36];
            var addr = BitConverter.ToUInt32(preview_img_data_addr);
            Console.WriteLine($"preview_img_data_addr:{addr}");
            var reserved = data[36..40];  // 168 64+64 = 128
            var id = data[40..104];
            string sid = Encoding.UTF8.GetString(id);
            var name = data[104..168];
            string sname = Encoding.UTF8.GetString(name);
            int start = 168;
            for (int i = 0; i < themecount; i++)
            {
                GetTheme(all, start);
                start += 88;
            }
        }

        private static void GetTheme(byte[] all, int start)
        {
            //var data = all[168..256];
            var data = all[start..(start + 88)];
            var background = data[0..4];
            var preview_img_data_addr = data[4..8];
            var addr = BitConverter.ToInt32(preview_img_data_addr);
            if (addr > 0)
                GetRecordImage(all, addr);
            for (var i = 0; i < 10; i++)
            {
                var record_num = data.Skip(8 + i * 8).Take(4);
                var num = BitConverter.ToUInt32(record_num.ToArray());  // 0 3 7
                var address = data.Skip(12 + i * 8).Take(4);
                var addrs = BitConverter.ToUInt32(address.ToArray());
                if (num > 0)
                {
                    int ad = Convert.ToInt32(addrs);
                    GetRecord(all, ad, num);
                }
            }
        }

        private static void GetRecord(byte[] all, int start, uint count)
        {
            byte[] data;
            for (int i = 0; i < count; i++)
            {
                data = all[start..(start + 16)];
                var uid = data[0..4];
                var resid = BitConverter.ToUInt32(uid);
                var flags = data[4..8];
                var data_address = data[8..12];
                var addr = BitConverter.ToInt32(data_address);
                var data_length = data[12..16];
                var len = BitConverter.ToUInt32(data_length);
                uids.Add(new RecordBase
                {
                    Uid = resid,
                    Flags = BitConverter.ToUInt32(flags),
                    DataAddress = (uint)addr,
                    DataLength = len,
                });
                //var type = (resid >> 24) & 0xFF;
                var type = uid[3];
                switch (Enum.ToObject(typeof(Record_type_t), type))
                {
                    case Record_type_t.RECORD_TYPE_LAYOUT:
                        GetRecordLayout(all, addr);
                        break;
                    case Record_type_t.RECORD_TYPE_IMG:
                        GetRecordImage(all, addr);
                        break;
                    case Record_type_t.RECORD_TYPE_IMG_ARRAY:
                        GetRecordImageArray(all, addr);
                        break;
                    case Record_type_t.RECORD_TYPE_DATA:
                        GetRecordDataItem(all, addr);
                        break;
                    case Record_type_t.RECORD_TYPE_SLOT:
                        GetRecordSlot(all, addr);
                        break;
                    case Record_type_t.RECORD_TYPE_WIDGET:
                        GetRecordWidget(all, addr);
                        break;
                    default: break;
                }
                start += 16;
            }
        }

        private static void GetRecordSlot(byte[] all, int start)
        {
            var data = all[start..];
            var widget_count = data[0];
            var reserved = data[1];
            var flags = data[2..4];
            var res = (flags[1] & 0xFF) << 8 | flags[0];
            var haspos = res & 0x1;
            var reservedf = (res >> 1) & 0x4; //reserved for future edit combinations
            var slottype = (res >> 3) & 0xF;
            var poscount = (res >> 7) & 0xF;
            var reserved1 = (res >> 11) & 0xF;
            start += 4;
            if (slottype is 0 or 1 or 2)
            {
                for (int i = 0; i < widget_count; i++)
                {
                    var uid_widget = all[start..(start + 4)];
                    start += 4;
                }
            }
            else if (slottype is 3)
            {
                var widget_id = all[start..(start + 2)];
                start += 2;
                var reserveds = all[start..(start + 2)];
                start += 2;
                var parameters = all[start..(start + 4)];
                start += 4;
            }


        }

        private static void EzipImageData(byte[] all, int start, int len)
        {
            var len_orig = all[start..(start + 4)];
            //var len = BitConverter.ToInt32(len_orig);
            var align_size = all[(start + 4)..(start + 8)];  // 0
            var align_s = BitConverter.ToInt32(align_size);
            var imgalldata = all[start..(start + len)];
            var imgdata = imgalldata[(8 + align_s)..];
        }

        private static void GetRecordImage(byte[] all, int start)
        {
            var data = all[start..(start + 24)];
            var fmt = data[0];
            var flags = data[1];
            var recolor = flags & 0x1;
            var compression = (flags >> 2) & 0x7;
            var reserved = data[2..4];
            var img_width = data[4..6];
            var w = (img_width[1] & 0xFF) << 8 | img_width[0];
            var img_height = data[6..8];
            var h = (img_height[1] & 0xFF) << 8 | img_height[0];
            var total_len = data[8..12];
            var lenth = BitConverter.ToInt32(total_len);
            //ezip  // 非指针图片数据格式
            EzipImageData(all, start + 12, lenth);
            //var len_orig = data[12..16]; 
            //var align_size = data[16..20];  // 0
            //var align_s = BitConverter.ToInt32(align_size);
            //var imgalldata = all[(start + 12)..(start + 12 + lenth)];
            //var imgdata = imgalldata[(8 + align_s)..];

        }
        private static void GetPointImage(byte[] all, int start)
        {
            //指针图片数据大小： 格式头+像素*3
            var data = all[start..(start + 24)];
            var fmt = data[0];
            var flags = data[1];
            var recolor = flags & 0x1;
            var compression = (flags >> 2) & 0x7;
            var reserved = data[2..4];
            var img_width = data[4..6];
            var w = (img_width[1] & 0xFF) << 8 | img_width[0];
            var img_height = data[6..8];
            var h = (img_height[1] & 0xFF) << 8 | img_height[0];
            var total_len = data[8..12];
            var lenth = BitConverter.ToInt32(total_len);

            var fmt_header = data[12..20];
            Bitmap bitmap = new Bitmap(w, h);
            int index = start + 20;
            int temp1;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    //temp1 = (all[index] & 0xFF) << 8 | all[index+1];
                    temp1 = (all[index + 1] & 0xFF) << 8 | all[index];
                    int a = all[index + 2];
                    int r = ((temp1 >> 11) & 0x1F) << 3;
                    int g = ((temp1 >> 5) & 0x3F) << 2;
                    int b = (temp1 & 0x1F) << 3;
                    bitmap.SetPixel(j, i, Color.FromArgb(a, r, g, b));
                    index += 3;
                }
            }
            bitmap.Save("point.png");
            bitmap.Dispose();
        }

        private static void GetRecordWidget(byte[] all, int start)
        {
            var data = all[start..(start + 44)];
            var name = data[0..32];
            var bg = data[32..36];
            var uid_preview_img = data[36..40];
            var record_count = data[40];
            var group_type = data[41];
            var flags = data[42..44];
            var resflag = BitConverter.ToUInt16(flags);
            var usebg = resflag & 0x1;  //1
            var jumpApp = resflag & 0x2;  //2
            var editimg = (resflag >> 2) & 0x1;
            var hasicon = resflag & 0x8;   //4
            var appid = (resflag >> 4) & 0xFF;
            var hassize = (resflag >> 12) & 0x1;  // 12
            var haspara = (resflag >> 13) & 0x1;
            var auto = (resflag >> 14) & 0x1;
            var b15 = (resflag >> 15) & 0x1;
            start += 44;
            uint x, y, uid;
            for (var i = 1; i <= record_count; i++)
            {
                var temp = all[start..(start + 8)];
                x = (uint)((temp[1] & 0xFF) << 8 | temp[0]);
                y = (uint)((temp[3] & 0xFF) << 8 | temp[2]);
                uid = BitConverter.ToUInt32(temp.AsSpan()[4..8]);
                start += 8;
            }
            /* var optional_data = all[start..(start + 4)];*///flags.bit2, flags.bit3, flags.bit12, flags.bit13, flags.bit14 flags.bit15
            //if (jumpApp == 1) { byte[] app = all[start..(start + 4)]; start += 4; }
            if (editimg == 1) { byte[] edit = all[start..(start + 4)]; start += 4; }
            if (hasicon == 1) { byte[] edit = all[start..(start + 4)]; start += 4; }
            if (hassize == 1) { byte[] size = all[start..(start + 4)]; start += 4; }
            if (haspara == 1) { byte[] para = all[start..(start + 4)]; start += 4; }
            if (auto == 1) { byte[] layout = all[start..(start + 4)]; start += 4; }
            if (b15 == 1) { byte[] opaflags = all[start..(start + 4)]; start += 4; }

        }

        private static void GetRecordLayout(byte[] all, int start)
        {
            var data = all[start..(start + 16)];
            var res_uid = data[0..4];
            var x = data[4..6];
            var res_x = (x[1] & 0xFF) << 8 | x[0];
            var y = data[6..8];
            var res_y = (y[1] & 0xFF) << 8 | y[0];
            var paramter = data[8..12];
            var reserved1 = data[12..14];
            var reserved = data[14..16];
        }

        private static void GetRecordImageArray(byte[] all, int start)
        {
            var data = all[start..(start + 12)];
            var fmt = data[0];
            var imgnumber = data[1];
            var flags = data[2..4];
            var width = data[4..6];
            var w = (width[1] & 0xFF) << 8 | width[0];
            var height = data[6..8];
            var h = (height[1] & 0xFF) << 8 | height[0];
            var datalen = data[8..12];
            var len = BitConverter.ToInt32(datalen);
            int[] lens = new int[imgnumber];
            start += 12;
            for (int i = 0; i < imgnumber; i++)
            {
                lens[i] = BitConverter.ToInt32(all.AsSpan()[start..(start + 4)]);
                start += 4;
            }
            for (int i = 0; i < imgnumber; i++)
            {
                EzipImageData(all, start, lens[i]);
                start += lens[i];
            }
        }

        private static void GetRecordDataItem(byte[] all, int start)
        {
            var data = all[start..(start + 8)];
            var source_id = data[0..2];
            var source_idhex = (source_id[1] & 0xFF) << 8 | source_id[0];
            var digits = data[2];
            var totaldigit = digits & 0x0F;  // 总位数
            //var decimaldigit = (digits & 0xFF) >> 4;  // 小数位数
            var decimaldigit = (digits >> 4) & 0x0F;  // 小数位数
            var flags = data[3];
            var alignmode = flags & 0x3;
            var leadingzero = (flags >> 2) & 0x1;
            var trailingzero = (flags >> 3) & 0x1;
            var style = (flags >> 4) & 0xF;
            var flags1 = data[4..6];
            var res = (flags1[1] & 0xFF) << 8 | flags1[0];
            var supportrecolor = res & 0x1;
            var renderrule = (res >> 1) & 0x7F;
            var paramter = data[6..8];
            var p = (paramter[1] & 0xFF) << 8 | paramter[0];
            switch (Enum.ToObject(typeof(DataStyle), style))
            {
                case DataStyle.DATA_STYLE_IMAGE_NUMBER:
                    GetItemImageNumber(all, start + 8);
                    break;
                case DataStyle.DATA_STYLE_POINTER:
                    GetItemImagePoint(all, start + 8);
                    break;
                case DataStyle.DATA_STYLE_IMAGE_VALUES:
                    GetItemImageValues(all, start + 8);
                    break;
                // 下面暂时没用
                case DataStyle.DATA_STYLE_IMG_OPACITY:
                    GetItemImageOpacity(all, start + 8);
                    break;
                case DataStyle.DATA_STYLE_ARC_PROGRESS_BAR:
                    GetItemArcProgress(all, start + 8);
                    break;
                case DataStyle.DATA_STYLE_TEXT:
                    GetItemText(all, start + 8);
                    break;
                case DataStyle.DATA_STYLE_LINE_PROGRESS_BAR:
                    GetItemLineProgress(all, start + 8);
                    break;
                default:
                    break;
            }
        }

        private static void GetItemLineProgress(byte[] all, int start)
        {
            var data = all[start..(start + 44)];
            var uid_fg_img = data[0..4];
            var uid_bg_img = data[4..8];
            var flags = data[8..12];
            var value_start = data[12..16];
            var value_range = data[16..20];
            var start_x = data[20..22];
            var start_y = data[22..24];
            var end_x = data[24..26];
            var end_y = data[26..28];
            var reserved = data[28..30];
            var bar_width = data[30..32];
            //指示器
            var uid_indicator_img = data[32..36];
            var off_x = data[36..38];
            var off_y = data[38..40];
            var reserved1 = data[40..44];
        }

        private static void GetItemArcProgress(byte[] all, int start)
        {
            var data = all[start..(start + 44)];
            var uid_fg_img = data[0..4];
            var uid_bg_img = data[4..8];
            var flags = data[8..12];
            var value_start = data[12..16];
            var value_range = data[16..20];
            var center_x = data[20..22];
            var center_y = data[22..24];
            var angle_start = data[24..26];
            var angle_range = data[26..28];
            var bar_radius = data[28..30];
            var bar_width = data[30..32];
            //指示器
            var uid_indicator_img = data[32..36];
            var radius = data[36..38];
            var reserved = data[38..40];
            var reserved2 = data[40..44];

        }

        private static void GetItemImageOpacity(byte[] all, int start)
        {
            var data = all[start..(start + 20)];
            var uid_fg_img = data[0..4];
            var uid_bg_img = data[4..8];
            var start_value = data[8..12];
            var end_value = data[12..16];
            var start_opa = data[16];
            var end_opa = data[17];
            var reserved = data[18..20];
        }

        private static void GetItemImagePoint(byte[] all, int start)
        {
            var data = all[start..(start + 24)];
            var uid_img = data[0..4];
            var id = BitConverter.ToUInt32(uid_img);
            var value_start = data[4..8];
            int v_s = BitConverter.ToInt32(value_start);
            var value_range = data[8..12];
            int v_r = BitConverter.ToInt32(value_range);
            var center_x = data[12..14];
            double x = (center_x[1] & 0xFF) << 8 | center_x[0];
            //double x = BitConverter.ToDouble(center_x);
            var center_y = data[14..16];
            double y = (center_y[1] & 0xFF) << 8 | center_y[0];
            //double y = BitConverter.ToDouble(center_y);
            var angle_start = data[16..18];
            int a_s = (angle_start[1] & 0xFF) << 8 | angle_start[0];
            var angle_range = data[18..20];
            int a_r = (angle_range[1] & 0xFF) << 8 | angle_range[0];
            var flag = data[20..24];
            var res = BitConverter.ToUInt32(flag);
            var b2 = (res >> 2) & 0x1;
            var b3 = (res >> 3) & 0x1;
            var point = uids.FirstOrDefault(r => r.Uid == id);
            if (point is not null)
            {
                GetPointImage(all, (int)point.DataAddress);
            }
        }

        private static void GetItemImageValues(byte[] all, int start)
        {
            var data = all[start..(start + 8)];
            var uidImageArray = data[0..4];
            var id = BitConverter.ToUInt32(uidImageArray);
            var record = uids.FirstOrDefault(r => r.Uid == id);
            //var len = GetRecordImageArray(all, record.DataAddress);
            var len = all[record.DataAddress + 1];
            var reserved = data[4];
            var flags = data[5];
            var has_position_table = flags & 0x1;
            var has_value_table = (flags >> 1) & 0x1;
            var has_invalid_value_image = (flags >> 2) & 0x1;
            var rotation = data[6..8];
            //后续长度根据id获取图组中的图片数量参数决定
            if (has_position_table is 1 && has_value_table is 1)
            {
                //pos_value_table
            }
            else if (has_position_table is 1)
            {
                //pos_table
            }
            else
            {
                //value_table
                var index = start + 8 + 4 * len;
                var value = all[index..(index + 4)];
            }
        }

        private static void GetItemText(byte[] all, int start)
        {
            // 待验证
            var data = all[start..(start + 12)];
            var color = data[0..4];
            var fontsize = data[4];

        }

        private static void GetItemImageNumber(byte[] all, int start)
        {
            var data = all[start..(start + 12)];
            var uidImageArray = data[0..4];
            var id = BitConverter.ToUInt32(uidImageArray);
            var decimal_offset_x = data[4];
            var space = data[5];
            var rotation = data[6..8];
            var uid_unit_icon = data[8..12];
            var unit = BitConverter.ToUInt32(uid_unit_icon);
        }
    }
}
