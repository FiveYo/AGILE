using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using FastDelivery_Library;
using Windows.UI.Xaml.Controls;

namespace FastDelivery_IHM
{
    public static class Controler
    {
        private static Plan plan { get; set; }
        private static StructPlan structPlan { get; set; }
        private static bool planLoaded = false;
        private static StructLivraison structLivraison { get; set; }

        public static void loadMap(Stream file, MapView map)
        {
            try
            {
                structPlan = Outils.ParserXml_Plan(file);
                map.LoadMap(structPlan);
                planLoaded = true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void loadDeliveries(Stream streamFile, MapView mapCanvas, StackPanel list)
        {
            if(planLoaded)
            {
                structLivraison = Outils.ParserXml_Livraison(streamFile, structPlan.HashPoint);
                mapCanvas.LoadDeliveries(structLivraison.HashLivraison);

                list.Children.Clear();
                foreach (var livraison in structLivraison.HashLivraison.Values)
                {
                    list.Children.Add(
                        new Delivery(
                            livraison.Adresse,
                            livraison.Duree
                        )
                    );
                }
                
            }
            else
            {
                throw new Exception("Load map before");
            }
            
            
            
        }

        public static void GetWay(MapView mapCanvas)
        {

            List<Point> l = Outils.startTsp(structLivraison, structPlan);
            Graphe G = new Graphe(structPlan);
            DijkstraAlgorithm dijkstra = new DijkstraAlgorithm(G);

            //Point p1 = structLivraison.HashLivraison.ElementAt(5).Value.Adresse;
            //dijkstra.execute(p1);
            //Point p = structLivraison.HashLivraison.ElementAt(2).Value.Adresse;
            //LinkedList<Point> result = dijkstra.getPath(p);

            //mapCanvas.LoadWay(result);
            Point start = structLivraison.entrepot.Adresse;
            foreach (var point in l)
            {
                if (point.id != start.id)
                {
                    dijkstra.execute(start);
                    LinkedList<Point> result = dijkstra.getPath(point);
                    mapCanvas.LoadWay(result);
                    start = point;
                }
            }
        }
    }
}
