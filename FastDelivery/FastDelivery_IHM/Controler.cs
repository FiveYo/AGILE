using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DEBUG
using System.Diagnostics;
#endif

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
        private static bool deliveriesLoaded = false;
        private static DemandeDeLivraisons demandeLivraisons { get; set; }

        private static Tournee tournee;

        public static void loadMap(Stream file, Map map)
        {
            
            carte = Outils.ParserXml_Plan(file);
            map.LoadMap(carte);
            carteLoaded = true;
        }

        public static Tuple<List<Delivery>,Delivery> loadDeliveries(Stream streamFile, Map mapCanvas)
        {
            List<Delivery> livraisons = new List<Delivery>();
            if(carteLoaded)
            {   
                demandeLivraisons = Outils.ParserXml_Livraison(streamFile, carte.points);
                mapCanvas.LoadDeliveries(demandeLivraisons);

                foreach (var livraison in demandeLivraisons.livraisons.Values)
                {
                    livraisons.Add(new Delivery(livraison));
                }
                deliveriesLoaded = true;
            }
            else
            {
                throw new Exception_Stream("Load map before");
            }
            return new Tuple<List<Delivery>, Delivery>(livraisons, new Delivery(demandeLivraisons.entrepot));
        }

        public static Tuple<int, Delivery> AddLivTournee(Lieu lieu, DeliveryPop livraison, Map map)
        {
            Livraison toAdd = null;
            int index;

            if (lieu is Livraison)
            {
                index = tournee.livraisons.IndexOf(lieu as Livraison);
            }
            else
            {
                index = -1;
            }

            Point ptLiv;
            int idPt = 0;
            int dureeLiv = 0;

            int idLiv = 0;

            if (int.TryParse(livraison.idPointLiv, out idPt) && int.TryParse(livraison.dureeLiv, out dureeLiv))
            {
                if (carte.points.TryGetValue(idPt, out ptLiv))
                {
                    toAdd = new Livraison(
                        ptLiv, dureeLiv
                    );

                    int.TryParse(livraison.idLiv, out idLiv);

                    tournee.AddLivraison(carte, toAdd, index);

                    map.DisplayDelivery(toAdd);

                }

            }
            

            map.LoadWay(tournee);
            return new Tuple<int, Delivery>(index, new Delivery(toAdd));
        }

        public static List<Delivery> GetWay(Map mapCanvas)
        {
            List<Delivery> listOrder = new List<Delivery>();
            if (deliveriesLoaded && carteLoaded)
            {
                tournee = Outils.creerTournee(demandeLivraisons, carte);
                mapCanvas.LoadWay(tournee);
            }
            else
            {
                throw new Exception_Stream("Map not loaded or Deliveries not loaded please use your brain before this button");
            }

            foreach (var livraison in tournee.livraisons)
            {
                listOrder.Add(new Delivery(livraison));
            }
            return listOrder;
        }

        internal static void RmLivTournee(Delivery d, Map map)
        {

            map.LoadWay(tournee);
        }
    }
}
