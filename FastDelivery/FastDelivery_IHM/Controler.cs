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
        private static int countCheck = 0;
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

        public static void loadDeliveries(Stream streamFile, Map mapCanvas, StackPanel list)
        {
            Delivery tmp;
            if(carteLoaded)
            {   
                demandeLivraisons = Outils.ParserXml_Livraison(streamFile, carte.points);
                mapCanvas.LoadDeliveries(demandeLivraisons);

                list.Children.Clear();
                foreach (var livraison in demandeLivraisons.livraisons.Values)
                {
                    tmp = new Delivery(livraison);
                    tmp.Checked += Checkbox_Checked;
                    tmp.AddLivraison += Delivery_AddLivraison;
                    list.Children.Add(tmp);
                }
                deliveriesLoaded = true;
            }
            else
            {
                throw new Exception_Stream("Load map before");
            }
        }

        private async static void Delivery_AddLivraison(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            int index;
            Livraison livraison = (sender as Delivery).livraison;
            if (tournee == null)
                return;

            index = tournee.livraisons.IndexOf(livraison);

            DeliveryPop popup = new DeliveryPop();

            await popup.ShowAsync();

            //Switch en fonction du bouton appuyer + boucle while avec gestion erreur

            Point ptLiv;
            int idPt = 0;
            int dureeLiv = 0;

            int idLiv = 0;

            if (int.TryParse(popup.idPointLiv, out idPt) && int.TryParse(popup.dureeLiv, out dureeLiv))
            {
                if (carte.points.TryGetValue(idPt, out ptLiv))
                {
                    Livraison toAdd = new Livraison(
                        ptLiv, dureeLiv
                    );

                    int.TryParse(popup.idLiv, out idLiv);
                    // Tu as l'ID de la liv (je sais pas il sert a quoi mais bon) dans idLiv
                    // Milllllyyyyyy c'est ici
                    // tournee.AddLiv
                    //

                    // t'enerve pas si ça marche pas sur la map, il faut la réactualiser et j'arrive pas à le faire proprement
                    
                }
            }
        }

        private static void Checkbox_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if((e.OriginalSource as CheckBox).IsChecked == true)
            {
                countCheck++;
            }
            else
            {
                countCheck--;
            }
        }

        public static void GetWay(Map mapCanvas)
        {
            if (deliveriesLoaded && carteLoaded)
            {
                tournee = Outils.creerTournee(demandeLivraisons, carte);
                mapCanvas.LoadWay(tournee);
            }
            else
            {
                throw new Exception_Stream("Map not loaded or Deliveries not loaded please use your brain before this button");
            }
        }
    }
}
