
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library.Modele
{
    /// <summary>
    /// Contient ses coordonnées et les tronçons empruntable depuis ce point.
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Paramètre de base du point récupéré dans le XML
        /// </summary>
        public int id { get; set; }
        public int x {get; set;}
        public int y {get; set;}

        /// <summary>
        /// List de troncon repertoriant les troncon voisins au point
        /// </summary>
        public List<Troncon> voisins { get; private set; }

        public Point(int ID , int X, int Y)
        {
            id = ID;
            x = X;
            y = Y;
            voisins = new List<Troncon>();
        }

        /// <summary>
        /// Un voisin sera un objet Troncon
        /// </summary>
        /// <param name="voisin"></param>
        public void AddVoisins(Troncon voisin)
        {
            voisins.Add(voisin);
        }

        public override string ToString()
        {
            return String.Format("Point {0} : ({1}, {2})", id, x, y);
        }
    }
}
