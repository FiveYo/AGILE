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
        private DemandeDeLivraisons demandeLivraisons;
        private Livraison liv;

        public AjouterLivraison(DeliveryPop popup, Map map, DemandeDeLivraisons demandeLivraisons)
        {
            this.popup = popup;
            this.map = map;
            this.demandeLivraisons = demandeLivraisons;
        }
        public object Do()
        {
            int id = demandeLivraisons.livraisons.Keys.Max() + 1;
            liv = new Livraison(popup.adresse, popup.duree);
            if (popup.planifier)
            {
                liv.SetPlage(popup.startDate, popup.endDate);
            }
            demandeLivraisons.livraisons.Add(id, liv);

            LieuMap lieuMap = map.AddDelivery(liv);
            LieuStack lieuStack = new LieuStack(liv);

            return new Tuple<LieuStack, LieuMap>(lieuStack, lieuMap);
        }

        public object Undo()
        {
            List<LieuMap> l = null;
            demandeLivraisons.livraisons.Remove(
                            demandeLivraisons.livraisons.Where((node) =>
                            {
                                if (node.Value == liv as Livraison)
                                    return true;
                                else
                                    return false;
                            }).First().Key
                        );
            l = map.LoadDeliveries(demandeLivraisons);
            return l;
        }
    }
}
