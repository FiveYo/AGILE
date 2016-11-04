using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using FastDelivery_Library;
using FastDelivery_Library.Modele;

using Windows.UI.Xaml.Controls;
using System.Diagnostics;

namespace FastDelivery_IHM
{
    public static class Controler
    {
        private static Carte carte { get; set; }
        private static bool carteLoaded = false;
        private static bool DeliveriesLoaded = false;
        private static DemandeDeLivraisons demandeLivraisons { get; set; }

        public static void loadMap(Stream file, MapView map)
        {
            
            carte = Outils.ParserXml_Plan(file);
            map.LoadMap(carte);
            carteLoaded = true;
        }

        public static void loadDeliveries(Stream streamFile, MapView mapCanvas, StackPanel list)
        {
            if(carteLoaded)
            {   
                demandeLivraisons = Outils.ParserXml_Livraison(streamFile, carte.points);
                mapCanvas.LoadDeliveries(demandeLivraisons);

                list.Children.Clear();
                foreach (var livraison in demandeLivraisons.livraisons.Values)
                {
                    list.Children.Add(
                        new Delivery(
                            livraison.adresse,
                            livraison.duree
                        )
                    );
                }
                DeliveriesLoaded = true;


            }
            else
            {
                throw new Exception_Stream("Load map before");
            }
            
            
            
        }

        public async static Task<string> GetWay(MapView mapCanvas)
        {
            string elapsedTime = "No runtime";
            if (DeliveriesLoaded && carteLoaded)
            {
                List<Point> l;
                Stopwatch sw = new Stopwatch();
                sw.Start(); 
                l = Outils.startTsp(demandeLivraisons, carte);
                sw.Stop();
                TimeSpan ts = sw.Elapsed;
                elapsedTime = "RunTime " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                DijkstraAlgorithm dijkstra = new DijkstraAlgorithm(carte);
                Point start = demandeLivraisons.entrepot.adresse;
                foreach (var point in l)
                {
                    if (point.id != start.id)
                    {
                        dijkstra.execute(start);
                        LinkedList<Point> result = dijkstra.getPath(point);
                        mapCanvas.LoadWay(result);
                        start = point;
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
            }
            else
            {
                throw new Exception_Stream("Map not loaded or Deliveries not loaded please use your brain before this button");
            }
            return elapsedTime;
        }
    }
}
