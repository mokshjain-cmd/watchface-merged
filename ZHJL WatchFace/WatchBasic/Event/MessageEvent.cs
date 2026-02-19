using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;


namespace WatchBasic.Event
{

    public class ShowProgressBar : PubSubEvent<bool> { }
    public class RefreshEvent : PubSubEvent<RefreshEventInfo>
    {
    }

    public class RefreshEventInfo
    {
        public string? GroupCode { get; set; }
        public string? ColorDesc { get; set; }

        public bool? IsDiy { get; set; }

        public string?[] ColorDescs { get; set; }
    }

    /// <summary>
    /// 百分号
    /// </summary>
    public class RefreshPAH
    {
        public IEnumerable<LayerGroup>? LayerGroups { get; set; }

    }
    public class RefreshPAHEvent : PubSubEvent<RefreshPAH>
    {

    }

    /// <summary>
    /// 心率刷新
    /// </summary>
    public class HeartRateRefreshEvent : PubSubEvent
    {

    }


    //图层位置同步
    public class WatchItemLayerPointEvent : PubSubEvent<WatchItemLayerPointasync>
    {

    }

    //图层位置同步
    public class WatchItemLayerPointasync
    {

        public string? PointerCode { get; set; } // 指针更新用
        public string? GroupCode { get; set; }

        public int Left { get; set; }

        public int Top { get; set; }

        public string? ColorDesc { get; set; }
        public string?[] ColorDescs { get; set; }
    }

    public class WatchGroupasync
    {
        public string? GroupCode { get; set; }
        public int Left { get; set; }

        public int Top { get; set; }

    }

    public class WatchGroupEvent : PubSubEvent<WatchGroupasync>
    {

    }

    public class WatchGroupColorDesc
    {
        public string? GroupCode { get; set; }
        public string? ColorDesc { get; set; }
    }

    public class WatchGroupColorDescEvent : PubSubEvent<WatchGroupColorDesc>
    {

    }


    public class WatchTypeAlignEvent : PubSubEvent<WatchTypeAlignasync>
    {
    }

    /// <summary>
    /// 对齐方式同步
    /// </summary>
    public class WatchTypeAlignasync
    {
        public string? WatchType { get; set; }
        public int Align { get; set; }

    }

    public class LocationItemEvent : PubSubEvent<LocationItemasync>
    {

    }

    public class LocationItemasync
    {
        public string? LocationName { get; set; }

        public bool IsChecked { get; set; }

    }


    public class TimeGroupEvent : PubSubEvent<TimeGroupasync>
    {

    }

    public class TimeGroupasync
    {
        public string? TimeCode { get; set; }
        public string? GroupCode { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public string? ColorDesc { get; set; }
        public string?[] ColorDescs { get; set; }
    }

    public class WatchTypeLocationGroupEvent : PubSubEvent<WatchTypeLocationGroupAsync>
    {

    }

    public class WatchTypeLocationGroupAsync
    {

        public LayerGroup? LayerGroup { get; set; }
    }


    public class WacthTypeLocationGroupsEvent : PubSubEvent<WacthTypeLocationGroupsAsync>
    {

    }

    public class WacthTypeLocationGroupsAsync 
    {
       public string? WatchType { get; set; }
       public IEnumerable<LayerGroup>? LayerGroups { get; set; }
    }

    public class SetWatchGroupColorArray
    {
        public string? GroupCode { get; set; }
        public string?[]? ColorDescs { get; set; }
    }
    public class SetWatchGroupColorArrayEvent : PubSubEvent<SetWatchGroupColorArray>
    {

    }
}
