using System.Collections.Generic;

namespace FastDelivery_Library.Modele
{
    /// <summary>
    /// Classe contenant la liste des troncons d'un chemin, et le coût total de ces troncons
    /// </summary>
    public class Chemin
    {
        /// <summary>
        /// Liste des troncons composant le Chemin
        /// </summary>
        List<Troncon> tronconList = new List<Troncon>();
        /// <summary>
        /// Coût total du chemin
        /// </summary>
        public double cout;
        /// <summary>
        /// Constructeur de la classe Chemin
        /// </summary>
        /// <param name="troncons">Liste des troncons composant le Chemin</param>
        public Chemin(List<Troncon> troncons)
        {
            tronconList = troncons;
            calculCoutChemin();
        }
        public List<Troncon> getTronconList()
        {
            return tronconList;
        }
        public void setTronconList(List<Troncon> listTroncon)
        {
            this.tronconList = listTroncon;
        }
        /// <summary>
        /// Permet de détemriner le coût total du Chemin
        /// </summary>
        public void calculCoutChemin()
        {
            double couttmp = 0;
            foreach (Troncon troncon in tronconList)
            {
                couttmp += troncon.cout;
            }
            this.cout = couttmp;
        }
    }
}
