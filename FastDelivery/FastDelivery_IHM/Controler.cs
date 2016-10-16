using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using FastDelivery_Library;

namespace FastDelivery_IHM
{
    public static class Controler
    {
        private static Plan plan { get; set; }
        private static StructPlan structPlan { get; set; }
        private static bool planLoaded = false;
        private static Dictionary<int, Livraison> demandeLivraisons { get; set; }

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

        public static void loadDeliveries(Stream streamFile, MapView mapCanvas)
        {
            if(planLoaded)
            {
                demandeLivraisons = Outils.ParserXml_Livraison(streamFile, structPlan.HashPoint);
                mapCanvas.LoadDeliveries(demandeLivraisons);
            }
            else
            {
                throw new Exception("Load map before");
            }
        }

        public static void GetWay(MapView mapCanvas)
        {
            //TODO
            // Peut etre faire une boucle si tu as une liste de liste de point
            //mapCanvas.LoadDeliveries();
        }
    }
}
