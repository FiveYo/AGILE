using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    public class Graphe
    {
        public int nombreSommets;
        public Dictionary<int, Troncon> listeTroncons;
        public Dictionary<int, Point> listePoints;
        public Graphe(StructPlan hashList)
        {
            this.listePoints = hashList.HashPoint;
            this.listeTroncons = hashList.HashTroncon;
            this.nombreSommets = listePoints.Count;
        }
    }
}
