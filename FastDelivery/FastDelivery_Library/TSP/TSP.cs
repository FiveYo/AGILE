using FastDelivery_Library.Modele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{




    public interface TSP
    {

        /**
         * @return true si chercheSolution() s'est terminee parce que la limite de temps avait ete atteinte, avant d'avoir pu explorer tout l'espace de recherche,
         */
        Boolean getTempsLimiteAtteint();

        /*
         * Cherche un circuit de duree minimale passant par chaque sommet (compris entre 0 et nbSommets-1)
         * @param tpsLimite : limite (en millisecondes) sur le temps d'execution de chercheSolution
         * @param nbSommets : nombre de sommets du graphe
         * @param cout : cout[i][j] = duree pour aller de i a j, avec 0 <= i < nbSommets et 0 <= j < nbSommets
         * @param duree : duree[i] = duree pour visiter le sommet i, avec 0 <= i < nbSommets
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tpsLimite" description"limite (en millisecondes) sur le temps d'execution de chercheSolution"></param>
        /// <param name="nbSommets" description="nombre de sommets du graphe"></param>
        /// <param name="cout" description="cout[i][j] = duree pour aller de i a j, avec 0 <= i < nbSommets et 0 <= j < nbSommets"></param>
        /// <param name="duree" description="duree[i] = duree pour visiter le sommet i, avec 0 <= i < nbSommets"></param>
        /// <param name="demandeLiv" description="Objet regroupant les livraisons souhaitée"></param>
        void chercheSolution(TimeSpan tpsLimite, int nbSommets, int[,] cout, int[] duree, DemandeDeLivraisons demandeLiv);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i" description="index"></param>
        /// <return>Renvoie le sommet visité en i-ème position dans la solution calculée par chercheSolution</return>
        int getMeilleureSolution(int i);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i" description="index du meilleur horaire"></param>
        /// <returns>Renvoie l'horaire visité en i-ème position dans la solution calculée par chercheSolution</returns>
        DateTime? getMeilleurHoraire(int i);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i" description="index du meilleur temps d'attente"></param>
        /// <returns>Renvoie le temps d'attenteen i-ème position dans la solution calculée par chercheSolution</returns>
        TimeSpan? getmeilleurtempsattente(int i)


    }
}