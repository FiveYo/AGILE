using FastDelivery_Library;
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
        private Lieu lieu;
        private DeliveryPop livraison;
        private Map map;
        private Tournee tournee;
        private Carte carte;
        private DemandeDeLivraisons demandeLivraisons;

        public AjouterTournee(Lieu lieu, DeliveryPop livraison, Map map, Tournee tournee, Carte carte, DemandeDeLivraisons demandeLivraisons)
        {
            this.lieu = lieu;
            this.livraison = livraison;
            this.map = map;
            this.tournee = tournee;
            this.carte = carte;
            this.demandeLivraisons = demandeLivraisons;
        }
        object Actions.Do()
        {
            LieuMap lieuMap;
            Livraison toAdd = null;
            int index;

            if (lieu is Livraison)
            {
                index = tournee.livraisons.IndexOf(lieu as Livraison);
            }
            else
            {
                index = -1;
            }

            toAdd = new Livraison(
                livraison.adresse, livraison.duree
            );

            tournee.AddLivraison(carte, toAdd, index);
            int id = demandeLivraisons.livraisons.Keys.Max() + 1;
            demandeLivraisons.livraisons.Add(id, toAdd);
            lieuMap = map.AddDelivery(toAdd);
            map.LoadWay(tournee);

            return new Tuple<int, LieuStack, LieuMap>(index, new LieuStack(toAdd), lieuMap);
        }

        object Actions.Undo()
        {
            throw new NotImplementedException();
        }
    }
}
