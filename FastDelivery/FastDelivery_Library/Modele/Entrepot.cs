using FastDelivery_Library.Modele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library.Modele
{
    /// <summary>
    /// Contient l'adresse et l'heure de départ de l'entrepot, hérite de Lieu
    /// </summary>
    public class Entrepot : Lieu
    {
        /// <summary>
        /// id de l'entrepot
        /// </summary>
        public int id;
        /// <summary>
        /// Coordonnées de l'entrepôt
        /// </summary>
        public Point adresse { get; set; }
        /// <summary>
        /// Heure de départ de l'entrepot
        /// </summary>
        public DateTime heureDepart;
        /// <summary>
        /// Constructeur de la classe Entrepot
        /// </summary>
        /// <param name="id">id de l'entrepot</param>
        /// <param name="adresse">adresse de l'entrepot</param>
        /// <param name="heureDepart">heure de départ de l'entrepot</param>
        public Entrepot(int id, Point adresse, DateTime heureDepart)
        {
            this.id = id;
            this.adresse = adresse;
            this.heureDepart = heureDepart;
        }
    }
}
