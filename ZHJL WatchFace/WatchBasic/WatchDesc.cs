using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic
{
    /// <summary>
    /// 表盘描述信息
    /// </summary>
    public class WatchDesc
    {
        public string? Language { get; set; }

        public string? WatchName { get; set; }
        public string? Description { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string? WatchCategory { get; set; }
    }
}
