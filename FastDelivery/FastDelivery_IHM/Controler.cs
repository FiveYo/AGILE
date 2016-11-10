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
using System.Collections.ObjectModel;
using Windows.UI.Xaml;

namespace FastDelivery_IHM
{
    public static class Controler
    {
        private static Task chargeTsp;
        private static Carte carte { get; set; }
        private static DemandeDeLivraisons demandeLivraisons { get; set; }

        public static etat etatActuel { get; set; }
        private static Tournee tournee;
        private static Stack<Actions> undoStack = new Stack<Actions>();
        private static Stack<Actions> redoStack = new Stack<Actions>();

        public static void loadMap(Stream file, Map map)
        {
            if (etatActuel == etat.enCoursDeCalcul)
            {
                Outils.StopTsp();
                etatActuel = etat.enCoursDArret;
            }
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
            if (etatActuel == etat.enCoursDeCalcul)
            {
                Outils.StopTsp();
                etatActuel = etat.enCoursDArret;
            }
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
            if (etatActuel == etat.enCoursDeCalcul)
            {
                Outils.StopTsp();
                etatActuel = etat.enCoursDArret;
            }
            int id = demandeLivraisons.livraisons.Keys.Max() + 1;
            Livraison liv = new Livraison(popup.adresse, popup.duree);
            if(popup.planifier)
            {
                liv.SetPlage(popup.startDate, popup.endDate);
            }
            demandeLivraisons.livraisons.Add(id, liv);

            LieuMap lieuMap = map.AddDelivery(liv);
            LieuStack lieuStack = new LieuStack(liv);

            return new Tuple<LieuStack, LieuMap>(lieuStack, lieuMap);
        }

        public static Tuple<int, LieuStack, LieuMap> AddLivTournee(Lieu lieu, DeliveryPop livraison, Map map)
        {
            if (etatActuel == etat.enCoursDeCalcul)
            {
                Outils.StopTsp();
                etatActuel = etat.enCoursDArret;
            }
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

        public static void GetWay(Map mapCanvas, StackPanel listDelivery, Action<object, RoutedEventArgs> eventLieuStack)
        {
            if (etatActuel == etat.enCoursDeCalcul)
            {
                Outils.StopTsp();
                etatActuel = etat.enCoursDArret;
            }
            if (etatActuel == etat.livraisonCharge || etatActuel == etat.tourneeCalculee)
            {
                try
                {
                    chargeTsp = Outils.startTsp(demandeLivraisons, carte);
                    tournee = null;
                    loadWay(mapCanvas, listDelivery, eventLieuStack);
                    etatActuel = etat.enCoursDeCalcul;
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                throw new Exception_Stream("Veuillez charger une carte et une tournée en premier");
            }
        }

        private async static void loadWay(Map mapCanvas, StackPanel listDeliveries, Action<object, RoutedEventArgs> eventLieuStack)
        {
            while(!chargeTsp.IsCompleted)
            {
                if(etatActuel == etat.enCoursDArret)
                {
                    chargeTsp.Wait();
                    return;
                }
                Tournee tourneeTmp = await Outils.getResultActual(demandeLivraisons, carte);
                if (tourneeTmp != null)
                {
                    if (tournee == null)
                    {
                        tournee = tourneeTmp;

                        mapCanvas.LoadWay(tournee);

                        LieuStack first = listDeliveries.Children.First() as LieuStack;

                        listDeliveries.Children.Clear();
                        listDeliveries.Children.Add(first);
                        List<LieuStack> listOrder = new List<LieuStack>();
                        foreach (var livraison in tournee.livraisons)
                        {
                            listOrder.Add(new LieuStack(livraison));
                        }

                        foreach (var item in listOrder)
                        {
                            listDeliveries.Children.Add(item);
                            item.Select += eventLieuStack.Invoke;
                        }

                    }
                    if (!tourneeTmp.livraisons.SequenceEqual(tournee.livraisons))
                    {
                        tournee = tourneeTmp;
                        mapCanvas.LoadWay(tournee);

                        LieuStack first = listDeliveries.Children.First() as LieuStack;

                        listDeliveries.Children.Clear();
                        listDeliveries.Children.Add(first);
                        List<LieuStack> listOrder = new List<LieuStack>();
                        foreach (var livraison in tournee.livraisons)
                        {
                            listOrder.Add(new LieuStack(livraison));
                        }

                        foreach (var item in listOrder)
                        {
                            listDeliveries.Children.Add(item);
                            item.Select += eventLieuStack.Invoke;
                        }
                    }
                }
                tournee = tourneeTmp;
                await Task.Delay(2000);
            }
            etatActuel = etat.tourneeCalculee;
        }

        public static async void GetRoadMap(Windows.Storage.StorageFile file)
        {
            if (file != null && etatActuel == etat.tourneeCalculee)
            {
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                int i = 0;
                string intro = "Bonjour cher livreur,\r\nVous trouverez ci-après la liste des livraisons que vous devez effectuer et le parcours que vous allez emprunter.\r\nFastDelivery vous souhaite un bon voyage.\r\n \r\n";
                for (int j = 0; j < tournee.livraisons.Count; j++)
                {
                    intro += "Livraison n°" + (++i) + " :\r\n   Coordonnées : (" + tournee.livraisons[j].adresse.x + "," + tournee.livraisons[j].adresse.y + ") \r\n   Heure d'arrivée : " + tournee.livraisons[j].HeureDePassage + "\r\n   Heure de départ : " + tournee.livraisons[j].HeureDePassage + "\r\n   Itinéraire à suivre pour rejoindre cette livraison : ";
                    foreach (var troncon in tournee.Hashchemin[tournee.livraisons[j]].getTronconList())
                    {
                        intro += troncon.rue + ",";
                    }
                    intro = intro.Substring(0, intro.Length - 1);
                    if (j == 0)
                    {
                        intro += ".\r\n   Depuis l'entrepot. \r\n \r\n";
                    }
                    else
                    {
                        intro += ".\r\n   Depuis l'adresse : (" + tournee.livraisons[j - 1].adresse.x + "," + tournee.livraisons[j - 1].adresse.y + ") \r\n \r\n";
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

        internal static List<LieuMap> RemoveLivraison(Lieu lieu, Map map)
        {
            if (etatActuel == etat.enCoursDeCalcul)
            {
                Outils.StopTsp();
                etatActuel = etat.enCoursDArret;
            }
            List<LieuMap> l = null;
            if (demandeLivraisons.livraisons.Count > 1)
            {
                if (etatActuel == etat.tourneeCalculee)
                {
                    if (lieu is Livraison)
                    {
                        demandeLivraisons.livraisons.Remove(
                            demandeLivraisons.livraisons.Where((node) =>
                            {
                                if (node.Value == lieu as Livraison)
                                    return true;
                                else
                                    return false;
                            }).First().Key
                        );
                        tournee.DelLivraison(carte, lieu as Livraison);
                        l = map.LoadDeliveries(demandeLivraisons);
                        map.LoadWay(tournee);
                    }
                }
                else
                {
                    //etatActuel == etat.livraisonCharge
                    if (lieu is Livraison)
                    {
                        demandeLivraisons.livraisons.Remove(
                            demandeLivraisons.livraisons.Where((node) =>
                            {
                                if (node.Value == lieu as Livraison)
                                    return true;
                                else
                                    return false;
                            }).First().Key
                        );
                        l = map.LoadDeliveries(demandeLivraisons);
                    }
                }
            }
            return l;
        }

        public static void ChangePlage(Lieu d, DateTime debutPlage, DateTime finPlage)
        {
            if(etatActuel == etat.tourneeCalculee)
            {
                Livraison livraisonNewPlage = d as Livraison;
                livraisonNewPlage.SetPlage(debutPlage, finPlage);
                //tournee.ModifPlage(livraisonNewPlage, demandeLivraisons, carte);
            }
        }
    }

    public enum etat
    {
        intial,
        carteCharge,
        livraisonCharge,
        enCoursDeCalcul,
        tourneeCalculee,
        enCoursDArret
    }
}
