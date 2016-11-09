using FastDelivery_Library.Modele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library.Modele
{
    /// <summary>
    /// Contient l'adresse et l'heure de départ de l'entrepot
    /// </summary>
    public class Entrepot : Lieu
    {
        public int id;
        public Point adresse { get; set; }
        public DateTime heureDepart;

        public Entrepot(int id, Point adresse, DateTime heureDepart)
        {
            this.id = id;
            this.adresse = adresse;
            this.heureDepart = heureDepart;
        }
    }
}
