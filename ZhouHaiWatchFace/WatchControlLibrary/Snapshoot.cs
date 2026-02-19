using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WatchControlLibrary
{
    public class Snapshoot : BindableBase
    {
        public Snapshoot()
        {

        }

        public static event Action<string>? DoShutterEvent;

        public static bool IsRevocation { get; set; }


        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var res = base.SetProperty(ref storage, value, propertyName);
            if (!IsRevocation)
                DoShutterEvent?.Invoke(propertyName);
            return res;
        }

    }


}
