using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDB
{
    /// <summary>
    /// 表盘信息
    /// </summary>
    public class WatchInfo
    {
        public WatchInfo()
        {

            WatchTypes = new List<WatchType>();

        }
        public int ID { get; set; }

        /// <summary>
        ///表盘名称 
        /// </summary>
        public string? WatchName { get; set; }

        /// <summary>
        /// 表盘Code（唯一）
        /// </summary>
        public string? WatchCode { get; set; }

        public string? Shape => WatchCode?.Split('_')?.FirstOrDefault();
        public string? Dpi => WatchCode?.Split('_')[1];
        public string? Type => WatchCode?.Split('_')[2];

        /// <summary>
        /// 系列
        /// </summary>
        public string? Series => WatchCode?.Split('_')[3].Split('#').FirstOrDefault();

        public string? SeriesType => WatchCode?.Split('_')[3].Split('#').LastOrDefault();

        public string? Author => WatchCode?.Split('_')[4];
        public string? Code => WatchCode?.Split('_')[5];

        public string? SubCode => WatchCode?.Split('_')[6];

        public virtual ICollection<WatchType>? WatchTypes { get; set; }

    }


    [NotMapped]
    public class BaseGroup
    {
        public int ID { get; set; }

        public string? GroupCode { get; set; }

        public int Left { get; set; }

        public int Top { get; set; }

        public string? ColorDesc { get; set; }

    }


    //电量、心率、通用日期、步数、卡路里
    public class WatchType
    {
        public WatchType()
        {
            WatchGroups = new List<WatchGroup>();
        }
        public int ID { get; set; }
        public string? WatchTypeName { get; set; }

        public int Align { get; set; }
        public ICollection<WatchGroup>? WatchGroups { get; set; }
        public virtual WatchInfo? WatchInfo { get; set; }

    }

    //对应图片文件夹
    public class WatchGroup : BaseGroup
    {
        public int WatchTypeId { get; set; }
        public virtual WatchType? WatchType { get; set; }
    }




}