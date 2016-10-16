using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    public class Livraison
    {
        public int  Duree;
        public string DebutPlage, FinPlage;
        public bool OnPlage = false;
        public Point Adresse;
        public Livraison(Point adresse , int duree)
        {
            this.Adresse = adresse;
            this.Duree = duree;
        }
        
        public void SetPlage(string debut, string fin)
        {
            this.DebutPlage = debut;
            this.FinPlage = fin;
            this.OnPlage = true;
        }
    }
}
