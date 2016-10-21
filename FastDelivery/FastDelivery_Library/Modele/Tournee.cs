using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    /// <summary>
    /// Contient la liste des livraisons ordonnées et des tronçons ainsi que l'entrepot
    /// </summary>
    public class Tournee
    {
        public Entrepot entrepot;

        // Les livraisons sont ordonnées par ordre de passage
        public List<Livraison> livraisons;
        public List<Troncon> troncons;

        public DateTime heureDepart;
        public DateTime heureArrivee;
        

        public Tournee(Entrepot entrepot, List<Livraison> livraisons, List<Troncon> troncons)
        {
            this.entrepot = entrepot;
            this.livraisons = livraisons;
            this.troncons = troncons;
        }
    }
}
