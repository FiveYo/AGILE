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
        public Point pointDepart, pointArrivee;
        public Dictionary<int, Troncon> listeTroncons;
        public Dictionary<int, Point> listePoints;
        public Graphe(Point depart, Point arrivee, List<object> hashList )
        {
            this.listePoints = (Dictionary<int, Point>)hashList[1]; ;
            this.listeTroncons = (Dictionary<int, Troncon>)hashList[0];
            this.pointDepart = depart;
            this.pointArrivee = arrivee;
            this.nombreSommets = listePoints.Count;
        }
    }
}
