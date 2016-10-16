using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    public class Plan
    {
        public Dictionary<int,Point> ListPoint;
        public Dictionary<int,Troncon> ListTroncon;
        public Tournee tournee;
        public DemandeLivraison livAsk;

        public Plan(Dictionary<int, Point> HashPoint, Dictionary<int, Troncon> HashTroncon,Tournee tourneet, DemandeLivraison demande)
        {
            this.ListPoint = HashPoint;
            this.ListTroncon = HashTroncon;
            this.tournee = tourneet;
            this.livAsk = demande;
        }
    }
}
