using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library.UndoRedo
{
    interface Actions
    {
        object Do(Lieu lieu, DeliveryPop livraison, Map map);
        object Undo();
    }
}
