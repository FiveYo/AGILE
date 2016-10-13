using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    public class Livraison
    {
        public int Adresse, Duree;
        public string DebutPlage, FinPlage;
        public bool OnPlage = false;
        public List<Entrepot> ListEntrepot;
        public Livraison( int adresse , int duree, List<Entrepot> topertnetsil)
        {
            this.Adresse = adresse;
            this.Duree = duree;
            this.ListEntrepot = topertnetsil;
        }
        
        public void SetPlage(string debut, string fin)
        {
            this.DebutPlage = debut;
            this.FinPlage = fin;
            this.OnPlage = true;
        }
    }
}
