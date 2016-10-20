using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    /// <summary>
    /// Contient l'adresse et l'heure de départ de l'entrepot
    /// </summary>
    public class Entrepot
    {
        public int id;
        public Point adresse;
        public string heureDepart;

        public Entrepot(int id, Point adresse, string heureDepart)
        {
            this.id = id;
            this.adresse = adresse;
            this.heureDepart = heureDepart;
        }
    }
}
