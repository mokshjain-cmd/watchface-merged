using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchControlLibrary.Model
{
    public class Watch : Snapshoot
    {
        public Watch()
        {

        }
        private int width;

        public int Width
        {
            get { return width; }
            set { SetProperty(ref width, value); }
        }

        private int height;

        public int Height
        {
            get { return height; }
            set { SetProperty(ref height, value); }
        }


        private MonitorItem? item;

        public MonitorItem? Item
        {
            get { return item; }
            set { SetProperty(ref item, value); }
        }



        private ObservableCollection<DragDataBase>? dragBindBases;

        public ObservableCollection<DragDataBase>? DragBindBases
        {
            get { return dragBindBases; }
            set { SetProperty(ref dragBindBases, value); }
        }

    }

    

    public class MonitorItem : Snapshoot
    {
        //  public delegate void NumChangedEventHandler(string numName, int value);

        public MonitorItem()
        {
            KwhNum = 0;
            StepNum = 0;
            HeartRateNum = 0;
            CalorieNum = 0;
            CurrentDateTime = DefaultTime;
            StrengthNum = 0;
            IsOpen = false;
        }

        public static DateTime DefaultTime=> System.Convert.ToDateTime($"2024-10-18 10:08:36");

       

        public event Action<string, int> NumChanged;
        public event Action<DateTime> TimeChanged;

        private int kwhNum;
        public int KwhNum
        {
            get { return kwhNum; }
            set { SetProperty(ref kwhNum, value); /*NumChange(nameof(KwhNum), value);*/ }
        }

        private int stepNum;

        public int StepNum
        {
            get { return stepNum; }
            set { SetProperty(ref stepNum, value); NumChange(nameof(StepNum), value); }
        }

        private void NumChange(string name, int value)
        {
            NumChanged?.Invoke(name, value);
        }

        private int heartRateNum;

        public int HeartRateNum
        {
            get { return heartRateNum; }
            set { SetProperty(ref heartRateNum, value); /*NumChange(nameof(HeartRateNum), value);*/ }
        }

        private int calorieNum;

        public int CalorieNum
        {
            get { return calorieNum; }
            set { SetProperty(ref calorieNum, value); NumChange(nameof(CalorieNum), value); }
        }

        private DateTime? currentDateTime;

        public DateTime? CurrentDateTime
        {
            get { return currentDateTime; }
            set { SetProperty(ref currentDateTime, value); TimeChanged?.Invoke(value.Value); }
        }

        private int strengthNum;

        public int StrengthNum
        {
            get { return strengthNum; }
            set { SetProperty(ref strengthNum, value); NumChange(nameof(StrengthNum), value); }
        }

        private bool isOpen;

        public bool IsOpen
        {
            get { return isOpen; }
            set { SetProperty(ref isOpen, value); }
        }
    }

    public class Monitor : Snapshoot
    {
        private BindMonitorType? monitorType;

        public BindMonitorType? MonitorType
        {
            get { return monitorType; }
            set { SetProperty(ref monitorType, value); }
        }
    }

    public enum BindMonitorType
    {

        WatchDateTime,
        Health,
        Tool,
    }





}
