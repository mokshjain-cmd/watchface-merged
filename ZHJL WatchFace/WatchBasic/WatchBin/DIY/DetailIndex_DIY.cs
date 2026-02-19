using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.WatchBin.Model;

namespace WatchBasic.WatchBin.DIY
{
    public class DetailIndex_DIY
    {
        [PropertyIndex(1)]
        public DIYX1? KWHBackground { get; set; }

        [PropertyIndex(2)]
        public DIYX1? KWHProgressbar { get; set; }

        [PropertyIndex(3)]
        public DIYX1? KWHText { get; set; }

        [PropertyIndex(4)]
        public DIYX1? KWHPicture { get; set; }

        [PropertyIndex(5)]
        public DIYX1? KWHPAH { get; set; }

        [PropertyIndex(6)]
        public DIYX1? KWHNum { get; set; }

        [PropertyIndex(7)]
        public DIYX1? GenerateDateBackground { get; set; }

        [PropertyIndex(8)]
        public DIYX1? GenerateDateMonthNum { get; set; }

        [PropertyIndex(9)]
        public DIYX1? GenerateDateSeparator { get; set; }

        [PropertyIndex(10)]
        public DIYX1? GenerateDateDayNum { get; set; }

        [PropertyIndex(11)]
        public DIYX1? GenerateDateWeek { get; set; }

        [PropertyIndex(12)]
        public DIYX1? GenerateTimeSpan { get; set; }

        [PropertyIndex(13)]
        public DIYX1? StepBackground { get; set; }

        [PropertyIndex(14)]
        public DIYX1? StepProgressbar { get; set; }

        [PropertyIndex(15)]
        public DIYX1? StepText { get; set; }

        [PropertyIndex(16)]
        public DIYX1? StepUnit { get; set; }

        [PropertyIndex(17)]
        public DIYX1? StepNum { get; set; }

        [PropertyIndex(18)]
        public DIYX1? StepAnimation { get; set; }

        [PropertyIndex(19)]
        public DIYX1? CalorieBackground { get; set; }

        [PropertyIndex(20)]
        public DIYX1? CalorieProgressbar { get; set; }

        [PropertyIndex(21)]
        public DIYX1? CalorieText { get; set; }

        [PropertyIndex(22)]
        public DIYX1? CalorieUnit { get; set; }

        [PropertyIndex(23)]
        public DIYX1? CalorieNum { get; set; }

        [PropertyIndex(24)]
        public DIYX1? CalorieAnimation { get; set; }

        [PropertyIndex(25)]
        public DIYX1? HeartBackground { get; set; }

        [PropertyIndex(26)]
        public DIYX1? HeartText { get; set; }

        [PropertyIndex(27)]
        public DIYX1? HeartUnit { get; set; }

        [PropertyIndex(28)]
        public DIYX1? HeartNull { get; set; }


        [PropertyIndex(29)]
        public DIYX1? HeartNum { get; set; }

        [PropertyIndex(30)]
        public DIYX1? HeartAnimation { get; set; }

        //public byte[] Serialize()
        //{
        //    return Tool.SerializeObject(this);
        //}

    }
}
