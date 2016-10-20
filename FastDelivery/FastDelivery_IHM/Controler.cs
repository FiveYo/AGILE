using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using FastDelivery_Library;
using FastDelivery_Library.Modele;

using Windows.UI.Xaml.Controls;

namespace FastDelivery_IHM
{
    public static class Controler
    {
        private static Carte carte { get; set; }
        private static bool carteLoaded = false;
        private static DemandeDeLivraisons demandeLivraisons { get; set; }

        public static void loadMap(Stream file, MapView map)
        {
            try
            {
                carte = Outils.ParserXml_Plan(file);
                map.LoadMap(carte);
                carteLoaded = true;
            }
            catch (Exception)
            {
                throw;
            }
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
                
            }
            else
            {
                throw new Exception("Load map before");
            }
            
            
            
        }

        public async static void GetWay(MapView mapCanvas)
        {
            List<Point> l = Outils.startTsp(demandeLivraisons, carte);
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
    }
}
