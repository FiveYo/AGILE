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
using FastDelivery_IHM.UndoRedo;

namespace FastDelivery_IHM
{
    public static class Controler
    {
        private static Carte carte { get; set; }
        private static DemandeDeLivraisons demandeLivraisons { get; set; }

        public static etat etatActuel { get; set; }
        private static Tournee tournee;
        private static Stack<Actions> undoStack = new Stack<Actions>();
        private static Stack<Actions> redoStack = new Stack<Actions>();

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

        internal static Tuple<LieuStack, LieuMap> AddLivDemande(DeliveryPop popup, Map map)
        {
            AjouterLivraison addLivraison = new AjouterLivraison(popup, map, demandeLivraisons);
            undoStack.Push(addLivraison);
            return (addLivraison.Do() as Tuple<LieuStack, LieuMap>);
        }

        public static Tuple<int, LieuStack, LieuMap> AddLivTournee(Lieu lieu, DeliveryPop livraison, Map map)
        {
            if (etatActuel == etat.tourneeCalculee)
            {
                AjouterTournee addTournee = new AjouterTournee(lieu, livraison, map, tournee, carte, demandeLivraisons);
                undoStack.Push(addTournee);
                return (addTournee.Do() as Tuple<int, LieuStack, LieuMap>);
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

        internal static List<LieuMap> RemoveLivraison(Lieu lieu, Map map)
        {
            if (demandeLivraisons.livraisons.Count > 1 && (lieu is Livraison))
            {
                if  (etatActuel == etat.tourneeCalculee)
                {
                    // Remove tournée
                }
                else // etatActuel == etat.livraisonCharge
                {
                    // Remove livraison
                }
            }
            return new List<LieuMap>();
        }

        public static void ChangePlage(Lieu d, DateTime debutPlage, DateTime finPlage)
        {
            if(etatActuel == etat.tourneeCalculee)
            {
                Livraison livraisonNewPlage = d as Livraison;
                livraisonNewPlage.SetPlage(debutPlage, finPlage);
                tournee.ModifPlage(livraisonNewPlage, demandeLivraisons, carte);
            }
        }

        public static void Undo()
        {
            if (undoStack.Count > 0)
            {
                Actions toUndo = undoStack.Pop();
                redoStack.Push(toUndo);
                toUndo.Undo();
            }
        }

        public static void Redo()
        {
            if (redoStack.Count > 0)
            {
                Actions toRedo = redoStack.Pop();
                undoStack.Push(toRedo);
                toRedo.Do();
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
