using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    public class Troncon
    {
        public int Length,Speed,Id;
        public Point Destination, Origin;
        public string StreetName;
        public Troncon(Point dest, int longueur, Point origine, int vitesse, string nomRue, int ID )
        {
            this.Destination = dest;
            this.Origin = origine;
            this.Length = longueur;
            this.Speed = vitesse;
            this.StreetName = nomRue;
            this.Id = ID;
        } 

        public double calculPoids()
        {
            double poids = Length / Speed;
            return poids;
        }
    }
}
