using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FastDelivery_Library.Modele;

namespace FastDelivery_Library
{
    /// <summary>
    /// Classe permettant d'effectuer l'algorithme de dijkstra et de récupérer ainsi un graphe orienté complet
    /// </summary>
    public class DijkstraAlgorithm
    {
        /// <summary>
        /// Dictionnaire des points correspondant aux noeuds du graphe orienté
        /// </summary>
        private Dictionary<int, Point> noeuds;
        /// <summary>
        /// Liste des troncons correspondant aux arcs du graphe orienté
        /// </summary>
        private Dictionary<int, Troncon> Troncons;
        /// <summary>
        /// Liste des noeuds visités
        /// </summary>
        private HashSet<Point> settlednoeuds;
        /// <summary>
        /// Liste des noeuds pas encore visités
        /// </summary>
        private HashSet<Point> unSettlednoeuds;
        /// <summary>
        /// Dictionnaire regroupant tous les prédecesseurs d'un point
        /// </summary>
        private Dictionary<Point, Point> predecessors;
        /// <summary>
        /// Distance parcourue (coût)
        /// </summary>
        private Dictionary<Point, double> distance;
        /// <summary>
        /// Liste d'id servant à trouver une distance minimale
        /// </summary>
        private static List<int> idTest = new List<int>();

        /// <summary>
        /// Constructeur de la classe 
        /// </summary>
        /// <param name="carte">Objet de type Carte contenant l'ensemble des points et des troncons qui serviront à établir le graphe orienté complet</param>
        public DijkstraAlgorithm(Carte carte)
        {
            // create a copy of the array so that we can operate on this array
            this.noeuds = new Dictionary<int, Point>(carte.points);
            this.Troncons = new Dictionary<int, Troncon>(carte.troncons);
        }

        /// <summary>
        /// Méthode qui exécute l'algorithme de Dijkstra depuis un point de départ
        /// Les résultats sont ensuite accessibles par getters
        /// </summary>
        /// <param name="source">Point de dpart de l'algorithme de Dijkstra</param>
        public void execute(Point source)
        {
            settlednoeuds = new HashSet<Point>();
            unSettlednoeuds = new HashSet<Point>();
            distance = new Dictionary<Point, double>();
            predecessors = new Dictionary<Point, Point>();
            distance.Add(source, 0);
            unSettlednoeuds.Add(source);
            while (unSettlednoeuds.Count > 0)
            {
                Point node = getMinimum(unSettlednoeuds);
                settlednoeuds.Add(node);
                unSettlednoeuds.Remove(node);
                findMinimalDistances(node);
            }
        }

        /// <summary>
        /// Permet de détemriner la distance minimale entre un point et chacun de ses voisins
        /// </summary>
        /// <param name="node">Point dont on veut déterminer la distance minimale avec ses voisins</param>
        private void findMinimalDistances(Point node)
        {
            List<Point> adjacentnoeuds = getNeighbors(node);
            double test;
            foreach (Point target in adjacentnoeuds)
            {

                if (getShortestDistance(target) > getShortestDistance(node)
                                + getDistance(node, target))
                {
                    idTest.Add(target.id);
                    if (!distance.TryGetValue(target, out test))
                    {
                        distance.Add(target, getShortestDistance(node)
                                        + getDistance(node, target));
                        predecessors.Add(target, node);
                        unSettlednoeuds.Add(target);
                    }
                }
            }
        }

        /// <summary>
        /// Permet de déterminer la distance (coût) entre un point et un autre 
        /// </summary>
        /// <param name="node">Point d'origine</param>
        /// <param name="target">Point cible</param>
        /// <returns>le coût du troncon entre les deux points</returns>
        private double getDistance(Point node, Point target)
        {
            foreach (Troncon Troncon in Troncons.Values)
            {
                if ((Troncon.origine.id == node.id) && (Troncon.destination.id == target.id))
                {
                    return Troncon.cout;
                }
            }
            throw new Exception("Erreur calcul de la distance entre deux points");
        }

        /// <summary>
        /// Permet d'obtenir la liste de tous les voisins d'un point
        /// </summary>
        /// <param name="node">Point dont on veut connaître tous les voisins</param>
        /// <returns>La liste de tou sles voisins du point source</returns>
        private List<Point> getNeighbors(Point node)
        {
            List<Point> neighbors = new List<Point>();
            foreach (Troncon Troncon in Troncons.Values)
            {
                if ((Troncon.origine.id == node.id) && !isSettled(Troncon.destination))
                {
                    neighbors.Add(Troncon.destination);
                }
            }
            return neighbors;
        }

        /// <summary>
        /// Détermine le coût minimum parmis une liste de Points
        /// </summary>
        /// <param name="Pointes">Liste de points dont on veut connaître le coût minimal</param>
        /// <returns>la valeur minimale trouvée</returns>
        private Point getMinimum(HashSet<Point> Pointes)
        {
            Point minimum = null;
            foreach (Point Point in Pointes)
            {
                if (minimum == null)
                {
                    minimum = Point;
                }
                else
                {
                    if (getShortestDistance(Point) < getShortestDistance(minimum))
                    {
                        minimum = Point;
                    }
                }
            }
            return minimum;
        }

        /// <summary>
        /// Permet de détemriner si un point à déjà été visité
        /// </summary>
        /// <param name="Point">Point à étudier</param>
        /// <returns>true si le point à déjà été visité, false sinon</returns>
        private bool isSettled(Point Point)
        {
            return settlednoeuds.Contains(Point);
        }

        /// <summary>
        /// Détermine le coût minimal associé à un point dans le dictionnaire de Points et de distances
        /// </summary>
        /// <param name="destination">Point dont on veut connaître la valeur de la distance minimale</param>
        /// <returns>d la valeur de al distance minimale</returns>
        private double getShortestDistance(Point destination)
        {
            double d = 0;
            if (!distance.TryGetValue(destination, out d))
            {
                return int.MaxValue;
            }
            else
            {
                return d;
            }
        }

         /// <summary>
         /// Permet d'obtenir le chemin parcouru d'un point source à un point visé (plus court chemin entre deux points)
         /// </summary>
         /// <param name="target">point de destination</param>
         /// <returns>null si ce chemin n'existe pas, Liste ordonnée de Points s'il existe</returns>
        public LinkedList<Point> getPath(Point target)
        {
            LinkedList<Point> path = new LinkedList<Point>();
            Point step = target;
            Point tmp;
            // On vérifie si le chemin existe
            if (!predecessors.TryGetValue(step, out tmp))
            {
                return null;
            }
            path.AddLast(step);
            while (predecessors.TryGetValue(step, out step))
            {
                path.AddLast(step);
            }
            // On remet la liste dans le bon ordre
            path = new LinkedList<Point>(path.Reverse().ToList<Point>());
            return path;
        }

    }
}
