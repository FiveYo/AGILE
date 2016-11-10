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

        
        /// <summary>
        /// méthode pour stopper le calcul si le temps de calcul devient trop grand
        /// </summary>
        /// <returns>Renvoie  true si chercheSolution() s'est terminee parce que la limite de temps avait ete atteinte, avant d'avoir pu explorer tout l'espace de recherche</returns>
        Boolean getTempsLimiteAtteint();

        /// <summary>
        /// Méthode principale de la classe qui cherchera automatiquement le meilleur coût via l'appel de la fonction brancAndBound
        /// </summary>
        /// <param name="tpsLimite" >"limite de temps pour la resolution"></param>
        /// <param name="nbSommets" >"nombre de sommet présent dans le graphe"></param>
        /// <param name="cout" >"cout[i][j] = duree pour aller de i a j, avec 0 <= i<nbSommets et 0 <= j<nbSommets "></param>
        /// <param name="duree" >" duree[i] = duree pour visiter le sommet i, avec 0 <= i<nbSommets"></param>
        /// <param name="demandeLiv" >"Objet regroupant les livraisons souhaitées"></param>
        void chercheSolution(TimeSpan tpsLimite, int nbSommets, int[,] cout, int[] duree, DemandeDeLivraisons demandeLiv);

        /// <summary>
        /// selectionne la meilleur solutio
        /// </summary>
        /// <param name="i" >"index"></param>
        /// <return>Renvoie le sommet visité en i-ème position dans la solution calculée par chercheSolution</return>
        int? getMeilleureSolution(int i);

        /// <summary>
        /// Selectionne le meilleur horaire
        /// </summary>
        /// <param name="i" >"index du meilleur horaire"></param>
        /// <returns>Renvoie l'horaire visité en i-ème position dans la solution calculée par chercheSolution</returns>
        DateTime? getMeilleurHoraire(int i);

        /// <summary>
        /// Selectionne le meilleur temps d'attente
        /// </summary>
        /// <param name="i" >"index du meilleur temps d'attente"></param>
        /// <returns>Renvoie le temps d'attenteen i-ème position dans la solution calculée par chercheSolution</returns>
        TimeSpan? getmeilleurtempsattente(int i);


    }
}