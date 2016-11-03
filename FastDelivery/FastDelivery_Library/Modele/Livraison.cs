using FastDelivery_Library.Modele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    /// <summary>
    /// Contient la durée, l'adresse d'une livraison. Si celle-ci est planifié contient la plage horaire
    /// </summary>
    public class Livraison : Lieu
    {
        public int duree;
        public string debutPlage, finPlage;
        public bool planifier = false;
        public Point adresse { get; set; }

        public string dateTest = "05/01/2009";
        public Livraison(Point adresse , int duree)
        {
            this.adresse = adresse;
            this.duree = duree;
        }
        
        public void SetPlage(string debut, string fin)
        {
            debutPlage = dateTest +debut;
            finPlage = dateTest +fin;
            planifier = true;
        }
    }
}
