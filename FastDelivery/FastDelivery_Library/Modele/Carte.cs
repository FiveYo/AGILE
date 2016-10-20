using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library.Modele
{
    /// <summary>
    /// Contient la liste des tronçons et des points ainsi que des attributs décrivant les extremums des coordonnées
    /// </summary>
    public class Carte
    {
        public Dictionary<int, Point> points;
        public Dictionary<int, Troncon> troncons;
        public int minX, maxX, minY, maxY;
        public Carte(Dictionary<int,Point> dicoPoints, Dictionary<int, Troncon> dicoTroncons, int xmin, int xmax, int ymin, int ymax)
        {
            points = dicoPoints;
            troncons = dicoTroncons;
            minX = xmin;
            maxX = xmax;
            minY = ymin;
            maxY = ymax;
        }
    }
}
