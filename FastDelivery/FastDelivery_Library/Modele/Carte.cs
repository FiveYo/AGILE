using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library.Modele
{
    /// <summary>
    /// Contient la liste des tronçons et des points du plan ainsi que des attributs décrivant les extremums des coordonnées
    /// </summary>
    public class Carte
    {
        public Dictionary<int, Point> points;
        public Dictionary<int, Troncon> troncons;
        public int minX, maxX, minY, maxY;
        /// <summary>
        /// Constructeur de la classe Carte
        /// </summary>
        /// <param name="dicoPoints">Dictionnaire de points avec leru id en clé</param>
        /// <param name="dicoTroncons">dictionnaire de troncons avec leur id en clé</param>
        /// <param name="xmin">valeur minimale de x</param>
        /// <param name="xmax">valeur maximale de x</param>
        /// <param name="ymin">valeur minimale de y</param>
        /// <param name="ymax">valeur maximale de y</param>
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
