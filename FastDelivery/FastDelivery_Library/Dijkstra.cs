using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FastDelivery_Library.Modele;

namespace FastDelivery_Library
{
    public class DijkstraAlgorithm
    {
        private Dictionary<int, Point> noeuds;
        private Dictionary<int, Troncon> Troncons;
        private HashSet<Point> settlednoeuds;
        private HashSet<Point> unSettlednoeuds;
        private Dictionary<Point, Point> predecessors;
        private Dictionary<Point, double> distance;
        private static List<int> idTest = new List<int>();

        public DijkstraAlgorithm(Carte carte)
        {
            // create a copy of the array so that we can operate on this array
            this.noeuds = new Dictionary<int, Point>(carte.points);
            this.Troncons = new Dictionary<int, Troncon>(carte.troncons);
        }

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

        private double getDistance(Point node, Point target)
        {
            // Utiliser de préférences les voisins des Points plutôt que de 
            // parcourir toute la liste des troncons
            foreach (Troncon Troncon in Troncons.Values)
            {
                if ((Troncon.origine.id == node.id) && (Troncon.destination.id == target.id))
                {
                    return Troncon.cout;
                }
            }
            throw new Exception("Erreur calcul de la distance entre deux points");
        }

        private List<Point> getNeighbors(Point node)
        {
            // Idem
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

        private bool isSettled(Point Point)
        {
            return settlednoeuds.Contains(Point);
        }

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

        /*
         * This method returns the path from the source to the selected target and
         * NULL if no path exists
         */
        public LinkedList<Point> getPath(Point target)
        {
            LinkedList<Point> path = new LinkedList<Point>();
            Point step = target;
            Point tmp;
            // check if a path exists
            if (!predecessors.TryGetValue(step, out tmp))
            {
                return null;
            }
            path.AddLast(step);
            while (predecessors.TryGetValue(step, out step))
            {
                path.AddLast(step);
            }
            // Put it into the correct order
            path = new LinkedList<Point>(path.Reverse().ToList<Point>());
            return path;
        }

    }
}
