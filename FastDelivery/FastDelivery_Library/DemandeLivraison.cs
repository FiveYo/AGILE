using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    public class DemandeLivraison
    {
        public Dictionary<int, Livraison> HashLiv;

        public DemandeLivraison(Dictionary<int,Livraison> hashliv)
        {
            this.HashLiv = hashliv;
        }

    }
}
