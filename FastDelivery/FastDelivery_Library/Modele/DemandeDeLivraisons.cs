using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library.Modele
{
    /// <summary>
    /// Contient la liste les livraisons et l'entrepot
    /// </summary>
    public class DemandeDeLivraisons
    {
        public Dictionary<int, Livraison> livraisons;
        public Entrepot entrepot;

        public DemandeDeLivraisons(Dictionary<int,Livraison> livraisons, Entrepot entrepot)
        {
            this.livraisons = livraisons;
            this.entrepot = entrepot;
        }

    }
}
