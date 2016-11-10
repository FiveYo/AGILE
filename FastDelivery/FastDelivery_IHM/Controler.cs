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

        public static  string runtime { get; set; }

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

        public static void setRuntime(string run)
        {
            runtime = run;
        }

        public static List<Delivery> GetWay(Map mapCanvas)
        {
            string elapsedTime = "No runtime";
            List<Delivery> listOrder = new List<Delivery>();
            if (deliveriesLoaded && carteLoaded)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                tournee = Outils.creerTournee(demandeLivraisons, carte);
                sw.Stop();
                TimeSpan ts = sw.Elapsed;
                elapsedTime = "RunTime " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                setRuntime(elapsedTime);
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

        public static async void GetRoadMap(Windows.Storage.StorageFile file)
        {
            if (file != null)
            {
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                int i = 0;
                string intro = "Bonjour cher livreur,\r\nVous trouverez ci-après la liste des livraisons que vous devez effectuer et le parcours que vous allez emprunter.\r\nFastDelivery vous souhaite un bon voyage.\r\n \r\n";
                for(int j=0; j< tournee.livraisons.Count; j++)
                {
                    intro += "Livraison n°" +(++i)+" :\r\n   Coordonnées : (" + tournee.livraisons[j].adresse.x + ","+ tournee.livraisons[j].adresse.y + ") \r\n   Heure d'arrivée : " + tournee.livraisons[j].HeureDePassage + "\r\n   Heure de départ : " + tournee.livraisons[j].HeureDePassage + "\r\n   Itinéraire à suivre pour rejoindre cette livraison : ";
                    foreach(var troncon in tournee.Hashchemin[tournee.livraisons[j]].getTronconList())
                    {
                        intro += troncon.rue + ",";
                    }
                    intro = intro.Substring(0, intro.Length - 1);
                    if(j==0)
                    {
                        intro += ".\r\n   Depuis l'entrepot. \r\n \r\n";
                    }
                    else
                    {
                        intro += ".\r\n   Depuis l'adresse : (" + tournee.livraisons[j-1].adresse.x + "," + tournee.livraisons[j-1].adresse.y + ") \r\n \r\n";
                    }
                }

                await Windows.Storage.FileIO.WriteTextAsync(file, intro);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
            }
        }

        internal static void RmLivTournee(Delivery d, Map map)
        {
            Livraison l;
            if (d.lieu is Livraison)
            {
                
                tournee.DelLivraison(carte,d.lieu as Livraison);
                map.LoadWay(tournee);
            }
            
        }
    }
}
