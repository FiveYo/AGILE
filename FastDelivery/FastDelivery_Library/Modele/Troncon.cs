using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library.Modele
{
    /// <summary>
    /// Contient la longueur, la vitesse, les points d'origine et de destination ainsi que le nom de la rue
    /// </summary>
    public class Troncon
    {
        // Paramètre de base du tronçon
        public int longueur, vitesse, id;
        //Points décrivant le sens et la direction du tronçon
        public Point destination, origine;
        //Nom de la rue 
        public string rue;

        //Cout calculé en seconde
        public double cout
        {
            get { return (double)longueur / (double)(vitesse / 3.6); }
        }

        public Troncon(Point destination, int longueur, Point origine, int vitesse, string nomRue, int ID )
        {
            this.destination = destination;
            this.origine = origine;
            this.longueur = longueur;
            this.vitesse = vitesse;
            rue = nomRue;
            id = ID;
        }
    }
}
