using FastDelivery_Library.Modele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library.Modele
{
    /// <summary>
    /// Contient la durée, l'adresse d'une livraison. Si celle-ci est planifié contient la plage horaire
    /// </summary>
    public class Livraison : Lieu
    {
        public int duree;

        public DateTime debutPlage { get; set; }
        public DateTime finPlage { get; set; }
        public bool planifier { get; set; }
        public Point adresse { get; set; }

        public DateTime HeureDePassage;
        public Livraison(Point adresse , int duree)
        {
            planifier = false;
            this.adresse = adresse;
            this.duree = duree;
        }
        
        public void SetPlage(DateTime debut, DateTime fin)
        {
            debutPlage = debut;
            finPlage = fin;
            planifier = true;
        }
    }
}
