using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastDelivery_Library.Modele;

namespace FastDelivery_IHM.UndoRedo
{
    class AjouterLivraison : Actions
    {
        private DeliveryPop popup;
        private Map map;

        public AjouterLivraison(DeliveryPop popup, Map map)
        {
            this.popup = popup;
            this.map = map;
        }
        public object Do()
        {
        }

        public object Undo()
        {
            throw new NotImplementedException();
        }
    }
}
