using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FastDelivery_Library.Modele;

#if DEBUG
using System.Diagnostics;
#endif

using System.IO;

using Windows.UI.Xaml.Controls;
using FastDelivery_Library;

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
            }
            catch
            {
                throw;
            }
            etatActuel = etat.carteCharge;
        }

        public static Tuple<List<LieuStack>,List<LieuMap>> loadDeliveries(Stream streamFile, Map mapCanvas)
        {
            List<LieuStack> lieuStack = new List<LieuStack>();
            List<LieuMap> lieuMap;
            if(etatActuel >= etat.carteCharge)
            {   
                try
                {
                    demandeLivraisons = Outils.ParserXml_Livraison(streamFile, carte.points);

                    lieuMap = mapCanvas.LoadDeliveries(demandeLivraisons);
                    lieuStack.Add(new LieuStack(demandeLivraisons.entrepot));
                    foreach (var livraison in demandeLivraisons.livraisons.Values)
                    {
                        lieuStack.Add(new LieuStack(livraison));
                    }
                }
                catch
                {
                    throw;
                }
                etatActuel = etat.livraisonCharge;
            }
            else
            {
                throw new Exception_Stream("Veuillez charger une carte en premier");
            }
            return new Tuple<List<LieuStack>, List<LieuMap>>(lieuStack, lieuMap);
        }

        public static Tuple<int, LieuStack> AddLivTournee(Lieu lieu, DeliveryPop livraison, Map map)
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

                        int.TryParse(livraison.idPointLiv, out idLiv);

                        tournee.AddLivraison(carte, toAdd, index);

                        map.AddDelivery(toAdd);

                    }

                }


                map.LoadWay(tournee);
                return new Tuple<int, LieuStack>(index, new LieuStack(toAdd));
            }
            else
            {
                throw new Exception_Stream("Une tournée doit préalablement être calculée");
            }
        }

        public static List<LieuStack> GetWay(Map mapCanvas)
        {
            List<LieuStack> listOrder = new List<LieuStack>();
            if (etatActuel == etat.livraisonCharge || etatActuel == etat.tourneeCalculee)
            {
                try
                {
                    tournee = Outils.creerTournee(demandeLivraisons, carte);
                    mapCanvas.LoadWay(tournee);
                }
                catch
                {
                    throw;
                }
                etatActuel = etat.tourneeCalculee;
            }
            else
            {
                throw new Exception_Stream("Veuillez charger une carte et une tournée en premier");
            }

            foreach (var livraison in tournee.livraisons)
            {
                listOrder.Add(new LieuStack(livraison));
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

        internal static void RmLivTournee(LieuStack d, Map map)
        {
            if (etatActuel == etat.tourneeCalculee)
            {
                if (d.lieu is Livraison)
                {

                    tournee.DelLivraison(carte, d.lieu as Livraison);
                    map.LoadWay(tournee);
                    //map.ReloadLieuStack(tournee);
                }
            }
        }

        public static void ChangePlage(Lieu d, String debutPlage, String finPlage)
        {
            foreach (var livraison in tournee.livraisons)
            {
            }

            Livraison livraisonNewPlage = d as Livraison;
            livraisonNewPlage.SetPlage(debutPlage, finPlage);

            foreach (var livraison in tournee.livraisons)
            {
            }

            tournee.ModifPlage(livraisonNewPlage, demandeLivraisons, carte);

            foreach (var livraison in tournee.livraisons)
            {
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
