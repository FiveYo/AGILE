using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastDelivery_Library.Modele;

namespace FastDelivery_IHM.UndoRedo
{
    interface Actions
    {
        object Do();
        object Undo();
    }
}
