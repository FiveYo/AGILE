using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    public class Tournee
    {
        public List<Troncon> ListTroncon;
        public List<Livraison> ListLivraison;

        public Tournee(List<Troncon> TroncList,List<Livraison> LivraisonList)
        {
            this.ListTroncon = TroncList;
            this.ListLivraison = LivraisonList;
        }
    }
}
