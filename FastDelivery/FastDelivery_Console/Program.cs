using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastDelivery_Library;
namespace FastDelivery_Console

{
    class Program
    {
        static void Main(string[] args)
        {
            Point dep = new Point(1, 10, 10);
            Point arr = new Point(2, 20, 20);
            Troncon tr1 = new Troncon(dep, 20, arr, 10, "toto", 1);
            Troncon tr2 = new Troncon(arr, 21, dep, 20, "toto", 2);
            List<object> listObj = new List<object>();
            Dictionary<int, Point> dico = new Dictionary<int, Point>();
            Dictionary<int, Troncon> dico2 = new Dictionary<int, Troncon>();
            dico.Add(0, dep);
            dico.Add(1, arr);
            dico2.Add(0, tr1);
            dico2.Add(1, tr2);
            listObj.Add(dico2);
            listObj.Add(dico);
            Graphe gr = new Graphe(dep, arr, listObj);
            Dijkstra dij = new Dijkstra(gr);
            //double testDistance = dij.getDistance(dep, arr); WORKS
            //List<Point> testNeighbors = dij.getNeighbors(dep); WORKS
            //double cesar = dij.findMinimalDistances(dep); Compile et exécute mais manque des données pour savoir si ça marche
            int i = 0;
        }
    }
}
