using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library.Modele
{
    public class Chemin
    {
        List<Troncon> tronconList = new List<Troncon>();

        public double cout;
        public Chemin(List<Troncon> troncons)
        {
            tronconList = troncons;
            calculCoutChemin();
        }
        public List<Troncon> getTronconList()
        {
            return tronconList;
        }
        public void setTronconList(List<Troncon> listTroncon)
        {
            this.tronconList = listTroncon;
        }
        public void calculCoutChemin()//le chemin doit être initialisé
        {
            double couttmp=0;
            foreach (Troncon troncon in tronconList)
            {
                couttmp += troncon.cout;
            }
            this.cout = couttmp;
        }
    }
}
