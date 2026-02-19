using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WatchBasic.WatchBin.Model

{
    public class DetailIndex
    {
        [PropertyIndex(3)]
        public DIYX1? KWHBackground { get; set; }

        [PropertyIndex(4)]
        public DIYX1? KWHProgressbar { get; set; }

        [PropertyIndex(5)]
        public DIYX1? KWHText_CN { get; set; }

        [PropertyIndex(6)]
        public DIYX1? KWHText_EN { get; set; }

        [PropertyIndex(7)]
        public DIYX1? KWHText_CHT { get; set; }

        [PropertyIndex(8)]
        public DIYX1? KWHPicture { get; set; }

        [PropertyIndex(9)]
        public DIYX1? KWHNum { get; set; }

        [PropertyIndex(10)]
        public DIYX1? KWHPAH { get; set; }

        [PropertyIndex(11)]
        public DIYX1? BlueTooth { get; set; }

        [PropertyIndex(12)]
        public DIYX1? GenerateDateBackground { get; set; }

        [PropertyIndex(13)]
        public DIYX1? GenerateDateMonthNum { get; set; }

        [PropertyIndex(14)]
        public DIYX1? GenerateDateSeparator { get; set; }

        [PropertyIndex(15)]
        public DIYX1? GenerateDateDayNum { get; set; }


        [PropertyIndex(16)]
        public DIYX1? GenerateDateWeek_CN { get; set; }
        [PropertyIndex(17)]
        public DIYX1? GenerateDateWeek_EN { get; set; }
        [PropertyIndex(18)]
        public DIYX1? GenerateDateWeek_CHT { get; set; }

        [PropertyIndex(19)]
        public DIYX1? GenerateTimeSpan_CN { get; set; }

        [PropertyIndex(20)]
        public DIYX1? GenerateTimeSpan_EN { get; set; }

        [PropertyIndex(21)]
        public DIYX1? GenerateTimeSpan_CHT { get; set; }

        [PropertyIndex(22)]
        public DIYX1? R3 { get; set; }

        [PropertyIndex(23)]
        public DIYX1? StepBackground { get; set; }


        [PropertyIndex(24)]
        public DIYX1? StepProgressbar { get; set; }

        [PropertyIndex(25)]
        public DIYX1? StepText_CN { get; set; }

        [PropertyIndex(26)]
        public DIYX1? StepText_EN { get; set; }

        [PropertyIndex(27)]
        public DIYX1? StepText_CHT { get; set; }

        

        [PropertyIndex(28)]
        public DIYX1? StepNum { get; set; }

        [PropertyIndex(29)]
        public DIYX1? StepUnit_CN { get; set; }

        [PropertyIndex(30)]
        public DIYX1? StepUnit_EN { get; set; }

        [PropertyIndex(31)]
        public DIYX1? StepUnit_CHT { get; set; }


        [PropertyIndex(32)]
        public DIYX1? StepAnimation { get; set; }

        [PropertyIndex(33)]
        public DIYX1? R5 { get; set; }

        [PropertyIndex(34)]
        public DIYX1? CalorieBackground { get; set; }

        [PropertyIndex(35)]
        public DIYX1? CalorieProgressbar { get; set; }

        [PropertyIndex(36)]
        public DIYX1? CalorieText_CN { get; set; }

        [PropertyIndex(37)]
        public DIYX1? CalorieText_EN { get; set; }

        [PropertyIndex(38)]
        public DIYX1? CalorieText_CHT { get; set; }

        [PropertyIndex(39)]
        public DIYX1? CalorieNum { get; set; }

        [PropertyIndex(40)]
        public DIYX1? CalorieUnit_CN { get; set; }

        [PropertyIndex(41)]
        public DIYX1? CalorieUnit_EN { get; set; }

        [PropertyIndex(42)]
        public DIYX1? CalorieUnit_CHT { get; set; }

        

        [PropertyIndex(43)]
        public DIYX1? CalorieAnimation { get; set; }

        [PropertyIndex(44)]
        public DIYX1? R7 { get; set; }

        [PropertyIndex(45)]
        public DIYX1? HeartBackground { get; set; }

        [PropertyIndex(46)]
        public DIYX1? HeartText_CN { get; set; }

        [PropertyIndex(47)]
        public DIYX1? HeartText_EN { get; set; }

        [PropertyIndex(48)]
        public DIYX1? HeartText_CHT { get; set; }

       

        [PropertyIndex(49)]
        public DIYX1? HeartNum { get; set; }

        [PropertyIndex(50)]
        public DIYX1? HeartNull { get; set; }

        [PropertyIndex(51)]
        public DIYX1? HeartUnit_CN { get; set; }
        [PropertyIndex(52)]
        public DIYX1? HeartUnit_EN { get; set; }

        [PropertyIndex(53)]
        public DIYX1? HeartUnit_CHT { get; set; }

      

        [PropertyIndex(54)]
        public DIYX1? HeartAnimation { get; set; }

        [PropertyIndex(55)]
        public DIYX1? R9 { get; set; }

        
        [PropertyIndex(56)]
        public DIYX1? HourTens { get; set; }
        [PropertyIndex(57)]
        public DIYX1? HourUint { get; set; }
        [PropertyIndex(58)]
        public DIYX1? HourSplit { get; set; }
        [PropertyIndex(59)]
        public DIYX1? MinuteTens { get; set; }
        [PropertyIndex(60)]
        public DIYX1? MinuteUint { get; set; }
        [PropertyIndex(61)]
        public DIYX1? MinuteSplit { get; set; }
        [PropertyIndex(62)]
        public DIYX1? SecondTens { get; set; }
        [PropertyIndex(63)]
        public DIYX1? SecondUnit { get; set; }

        [PropertyIndex(64)]
        public DIYX1? R11 { get; set; }

        [PropertyIndex(65)]
        public DIYX1? R12 { get; set; }


        [PropertyIndex(66)]
        public DIYX1? HourPointer { get; set; }

        [PropertyIndex(67)]
        public DIYX1? MinutePointer { get; set; }

        [PropertyIndex(68)]
        public DIYX1? SecondPointer { get; set; }

        [PropertyIndex(69)]
        public DIYX1? R13 { get; set; }

        [PropertyIndex(70)]
        public DIYX1? R14 { get; set; }

        public byte[] Serialize()
        {
            return SerializeTool.SerializeObject(this);
        }
    }

    public class BackgroundIndex 
    {
        [PropertyIndex(1)]
        public DIYX1? Thumbnail { get; set; }
        [PropertyIndex(2)]
        public DIYX1? Background { get; set; }

        public byte[] Serialize()
        {
            return SerializeTool.SerializeObject(this);
        }
    }


}
