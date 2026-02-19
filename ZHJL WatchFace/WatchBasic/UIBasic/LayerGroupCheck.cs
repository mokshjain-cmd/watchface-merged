using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.UIBasic
{
    public class LayerGroupCheck : LayerGroup
    {
        //  //g.Status = g.Left % 2 == 0;
        //g.SizeStatus = g.Width % 2 == 0;
        //public bool Status => NumStatus && SizeStatus;

        
        public bool Status=> Left % 2 == 0;


        private bool numStatus;

        public bool NumStatus
        {
            get { return numStatus; }
            set
            {
                SetProperty(ref numStatus, value);
                //RaisePropertyChanged(nameof(Status));
            }
        }

       

        public bool SizeStatus=> Width % 2 == 0&& HeartNullStatus;

        public bool HeartNullStatus { get; set; } = true;


    }

    public class LayerGroupLocation: LayerGroupCheck 
    {
        private string?  locationName;

        public string?  LocationName
        {
            get { return locationName; }
            set { SetProperty(ref locationName,value); }
        }

    }
}
