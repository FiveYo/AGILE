using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    public class Troncon
    {
        public int Destination, Length, Origin, Speed;
        public string StreetName;
        public Troncon(int dest, int longueur, int origine, int vitesse, string nomRue )
        {
            this.Destination = dest;
            this.Origin = origine;
            this.Length = longueur;
            this.Speed = vitesse;
            this.StreetName = nomRue;
        } 
    }
}
