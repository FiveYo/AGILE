
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    /// <summary>
    /// Contient ses coordonnées et les tronçons empruntable depuis ce point.
    /// </summary>
    public class Point
    {
        public int id, x, y;
        public List<Troncon> voisins;

        public Point(int ID , int X, int Y)
        {
            id = ID;
            x = X;
            y = Y;
            voisins = new List<Troncon>();
        }

        //Un voisin sera un objet Troncon
        public void AddVoisins(Troncon voisin)
        {
            voisins.Add(voisin);
        }
    }
}
