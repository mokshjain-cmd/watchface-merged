using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDB;
using WatchDB_DIY;

namespace WatchDBDIYJieLi
{
    public class WatchInfo_DIYJieLi : WatchInfo_DIY
    {
        public WatchInfo_DIYJieLi()
        {

            //WatchTypes = new List<WatchType>();
            //WatchLocations = new List<WatchLocation>();
            //WatchTimes = new List<WatchTime>();
            WatchPointers = new List<WatchPointer>();
        }

        //public virtual ICollection<WatchTime>? WatchTimes { get; set; }
        public virtual ICollection<WatchPointer>? WatchPointers { get; set; }

        //public virtual ICollection<WatchLocation>? WatchLocations { get; set; }
    }

    //public class WatchLocation
    //{
    //    public WatchLocation()
    //    {
    //        LocationDetails = new List<LocationDetail>();
    //    }
    //    public int ID { get; set; }

    //    public string? LocationName { get; set; }

    //    //偏移坐标Left
    //    public int Left { get; set; }

    //    //偏移坐标Top
    //    public int Top { get; set; }

    //    public ICollection<LocationDetail>? LocationDetails { get; set; }

    //    public virtual WatchInfo_DIYJieLi? WatchInfo { get; set; }

    //}

    //public class LocationDetail
    //{
    //    public int ID { get; set; }

    //    public int WatchTypeId { get; set; }

    //    public int IsDefalut { get; set; }
    //    public virtual WatchLocation? WatchLocation { get; set; }


    //}


    //public class WatchTime
    //{
    //    public int ID { get; set; }

    //    public string? TimeCode { get; set; }
    //    public ICollection<TimeGroup>? TimeGroups { get; set; }
    //    public virtual WatchInfo_DIYJieLi? WatchInfo { get; set; }
    //}

    //public class TimeGroup : BaseGroup
    //{
    //    public virtual WatchTime? WatchTime { get; set; }
    //}

    public class WatchPointer
    {
        public int ID { get; set; }

        public string? PointerCode { get; set; }
        public ICollection<PointerGroup>? PointerGroups { get; set; }
        public virtual WatchInfo_DIYJieLi? WatchInfo { get; set; }
    }

    public class PointerGroup : BaseGroup
    {
        public virtual WatchPointer? WatchPointer { get; set; }
    }
}
