using FastDelivery_Library.Modele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_IHM.UndoRedo
{
    class AjouterTournee : Actions
    {
        object Actions.Do(Lieu lieu, DeliveryPop livraison, Map map)
        {

            return Controler.AddLivTournee(lieu, livraison, map);

        }

        object Actions.Undo()
        {
            throw new NotImplementedException();
        }
    }
}
