﻿using System;
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
        private static DemandeDeLivraisons demandeLivraisons { get; set; }

        private static etat etatActuel { get; set; }
        private static Tournee tournee;

        public static void loadMap(Stream file, Map map)
        {
            try
            {
                carte = Outils.ParserXml_Plan(file);
                map.LoadMap(carte);
                etatActuel = etat.carteCharge;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Tuple<List<Delivery>,Delivery> loadDeliveries(Stream streamFile, Map mapCanvas)
        {
            List<Delivery> livraisons = new List<Delivery>();
            if(etatActuel >= etat.carteCharge)
            {   
                demandeLivraisons = Outils.ParserXml_Livraison(streamFile, carte.points);
                mapCanvas.LoadDeliveries(demandeLivraisons);

                foreach (var livraison in demandeLivraisons.livraisons.Values)
                {
                    livraisons.Add(new Delivery(livraison));
                }

                etatActuel = etat.livraisonCharge;
            }
            else
            {
                throw new Exception_Stream("Load map before");
            }
            return new Tuple<List<Delivery>, Delivery>(livraisons, new Delivery(demandeLivraisons.entrepot));
        }

        public static Tuple<int, Delivery> AddLivTournee(Lieu lieu, DeliveryPop livraison, Map map)
        {
            if (etatActuel == etat.tourneeCalculee)
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
            else
            {
                throw new Exception("toto");
            }
        }

        public static List<Delivery> GetWay(Map mapCanvas)
        {
            List<Delivery> listOrder = new List<Delivery>();
            if (etatActuel == etat.livraisonCharge)
            {
                tournee = Outils.creerTournee(demandeLivraisons, carte);
                mapCanvas.LoadWay(tournee);
                etatActuel = etat.tourneeCalculee;
            }
            else
            {
                throw new Exception_Stream("Map not loaded or Deliveries not loaded , load one before this button");
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
                // write to file


                //System.IO.MemoryStream ms = new System.IO.MemoryStream();
                //iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 30f, 30f, 30f, 30f);
                //iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                //doc.Open();
                //doc.Add(new iTextSharp.text.Chunk("hello world"));
                //doc.Close();
                //byte[] Result = ms.ToArray();



                await Windows.Storage.FileIO.WriteTextAsync(file, "bonjour \r\n salut");
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
            }
        }

        internal static void RmLivTournee(Delivery d, Map map)
        {
            if (etatActuel == etat.tourneeCalculee)
            {
                if (d.lieu is Livraison)
                {

                    tournee.DelLivraison(carte, d.lieu as Livraison);
                    map.LoadWay(tournee);
                    //map.ReloadDelivery(tournee);
                }
            }
        }

        public static Tuple<int, Delivery> ChangePlage(Lieu lieu, ChangePlagePop livraison, Map map)
        {
            if (etatActuel == etat.tourneeCalculee)
            {
                Livraison toAdd = null;
                /*int index;

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


                map.LoadWay(tournee);*/
                return new Tuple<int, Delivery>(0, new Delivery(toAdd));
            }
            else
            {
                throw new Exception("toto");
            }
        }
    }

    public enum etat
    {
        intial,
        carteCharge,
        livraisonCharge,
        tourneeCalculee
    }
}
