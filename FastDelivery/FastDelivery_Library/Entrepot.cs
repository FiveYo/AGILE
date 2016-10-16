using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    public class Entrepot
    {
        int Id;
        Point Adresse;
        string HeureDepart;
        public Entrepot(int id, Point adresse, string HeureDep)
        {
            this.Id = id;
            this.Adresse = adresse;
            this.HeureDepart = HeureDep;
        }
    }
}
