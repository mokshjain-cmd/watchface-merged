using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchControlLibrary
{
    public interface IDraggable
    {
        DraggableBehavior _draggableBehavior { get; set; }

        string? DragName {  get; set; }

        Guid? DragId { get; set; }

    }
}
