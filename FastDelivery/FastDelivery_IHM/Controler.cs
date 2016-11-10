using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FastDelivery_Library.Modele;


using System.IO;

using Windows.UI.Xaml.Controls;
using FastDelivery_Library;
using FastDelivery_IHM.UndoRedo;
using Windows.UI.Xaml;

namespace FastDelivery_IHM
{
    /// <summary>
    /// Contrôleur de l'IHM
    /// </summary>
    public static class Controler
    {
        /// <summary>
        /// Carte contenant les données (les points et les tronçons)
        /// </summary>
        private static Carte carte { get; set; }

        /// <summary>
        /// Demande de livraison contenant les livraisons et l'entreport
        /// </summary>
        private static DemandeDeLivraisons demandeLivraisons { get; set; }

        /// <summary>
        /// Etat permettant de savoir ou on en est
        /// </summary>
        public static etat etatActuel { get; set; }

        /// <summary>
        /// Tournee qui contient la liste des livraisons orientés, l'entrepot et les chemins entre les différentes livraisons
        /// </summary>
        private static Tournee tournee;

        /// <summary>
        /// Stack d'action permettant le Undo Redo (pas implémenté dans cette version)
        /// </summary>
        private static Stack<Actions> undoStack = new Stack<Actions>();
        private static Stack<Actions> redoStack = new Stack<Actions>();


        /// <summary>
        /// Parse le fichier xml et charge la map associé sur l'IHM
        /// </summary>
        /// <param name="file">Stream du fichier à parser</param>
        /// <param name="map">Map (carte dans l'IHM)</param>
        public static void loadMap(Stream file, Map map)
        {
            if (etatActuel == etat.enCoursDeCalcul)
            {
                etatActuel = etat.enCoursDArret;
                Outils.StopTsp();
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

        /// <summary>
        /// Parse le fichier xml et charge les livraisons sur l'IHM
        /// </summary>
        /// <param name="streamFile">Stream du fichier à parser</param>
        /// <param name="mapCanvas">Map (carte dans l'IHM)</param>
        /// <returns></returns>
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

        /// <summary>
        /// Ajoute une livraison à la demande de livraison et met à jour l'IHM
        /// </summary>
        /// <param name="popup">PopUp contenant les informations de création de la livraison</param>
        /// <param name="map">Map (carte dans l'IHM)</param>
        /// <returns></returns>
        internal static Tuple<LieuStack, LieuMap> AddLivDemande(DeliveryPop popup, Map map)
        {
            if (etatActuel == etat.enCoursDeCalcul)
            {
                etatActuel = etat.enCoursDArret;
                Outils.StopTsp();
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


        /// <summary>
        /// Ajoute une livraison dans la tournée et l'affiche dans l'IHM
        /// </summary>
        /// <param name="lieu">Lieu précédent la nouvelle livraison</param>
        /// <param name="livraison">PopUp contenant les informations pour la nouvelle livraison</param>
        /// <param name="map">Map (carte dans l'IHM)</param>
        /// <returns></returns>
        public static Tuple<int, LieuStack, LieuMap> AddLivTournee(Lieu lieu, DeliveryPop livraison, Map map)
        {
            if (etatActuel == etat.enCoursDeCalcul)
            {
                etatActuel = etat.enCoursDArret;
                Outils.StopTsp();
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

        /// <summary>
        /// Calcul le chemin entre les différentes livraisons, lance une Task asynchrone (comme un thread mais en mieux)
        /// qui va mettre à jour l'IHM régulièrement
        /// </summary>
        /// <param name="mapCanvas">Map (carte dans l'IHM)</param>
        /// <param name="listDelivery">StackPanel à mettre à jour dans l'IHM</param>
        /// <param name="eventLieuStack">Handler de la méthode a appelé pour les évènements pour les items du StackPanel</param>
        public static void GetWay(Map mapCanvas, StackPanel listDelivery, Action<object, RoutedEventArgs> eventLieuStack)
        {
            Task chargeTsp = null;
            if (etatActuel == etat.enCoursDeCalcul)
            {
                etatActuel = etat.enCoursDArret;
                Outils.StopTsp();
            }
            if (etatActuel == etat.livraisonCharge || etatActuel == etat.tourneeCalculee)
            {
                try
                {
                    chargeTsp = Outils.startTsp(demandeLivraisons, carte);
                    tournee = null;
                    
                    loadWay(mapCanvas, listDelivery, eventLieuStack, chargeTsp);
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

        /// <summary>
        /// Charge le chemin sur l'IHM et met à jour l'ordre des livraisons dans la liste régulièrement
        /// </summary>
        /// <param name="mapCanvas">Map (carte dans l'IHM)</param>
        /// <param name="listDeliveries">StackPanel à mettre à jour dans l'IHM</param>
        /// <param name="eventLieuStack">Handler de la méthode a appelé pour les évènements pour les items du StackPanel</param>
        /// <param name="chargeTsp">Task permettant de savoir quand le TSP a fini de calculé</param>
        private async static void loadWay(Map mapCanvas, StackPanel listDeliveries, Action<object, RoutedEventArgs> eventLieuStack, Task chargeTsp)
        {
            Tournee tourneeTmp = null;
            await Task.Delay(1000);
            while(!chargeTsp.IsCompleted)
            {
                
                if (etatActuel == etat.enCoursDArret)
                {
                    return;
                }
                tourneeTmp = Outils.getResultActual(demandeLivraisons, carte);
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
                    if (tourneeTmp.CompareTo(tournee) != 0)
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
            if (etatActuel == etat.enCoursDArret)
                return;
            Tournee final = Outils.getResultActual(demandeLivraisons, carte);
            if(final != null)
            {
                tournee = final;

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
            etatActuel = etat.tourneeCalculee;
        }

        /// <summary>
        /// Permet de demander à l'utilisateur de sélectionner un dossier local où générer une feuille de route
        /// Génère ensuite la feuille de route dans le dossier spécifié, à l'aide des données de la tournée calculée
        /// </summary>
        /// <param name="file">Fichier où l'on va stocker la feuille de route</param>
        public static async void GetRoadMap(Windows.Storage.StorageFile file)
        {
            if (file != null && etatActuel == etat.tourneeCalculee)
            {
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                int i = 0;
                string intro = "Bonjour cher livreur,\r\nVous trouverez ci-après la liste des livraisons que vous devez effectuer et le parcours que vous allez emprunter.\r\nFastDelivery vous souhaite un bon voyage.\r\n \r\n";
                for (int j = 0; j < tournee.livraisons.Count; j++)
                {
                    intro += "Livraison n°" + (++i) + " :\r\n   Coordonnées : (" + tournee.livraisons[j].adresse.x + "," + tournee.livraisons[j].adresse.y + ") \r\n   Heure d'arrivée : " + tournee.livraisons[j].heureArrivee+ "\r\n   Heure de départ : " + tournee.livraisons[j].heureDepart + "\r\n   Itinéraire à suivre pour rejoindre cette livraison : ";
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
                Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
            }
        }

        /// <summary>
        /// Supprime une livraison de la tournée et/ou de la demande de livraison et actualise l'IHM
        /// </summary>
        /// <param name="lieu">Livraison à supprimer</param>
        /// <param name="map">Map (carte dans l'IHM)</param>
        /// <returns></returns>
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
                        int key = int.MaxValue;

                        demandeLivraisons.livraisons.Remove(
                            demandeLivraisons.livraisons.Where((node) =>
                            {
                                if (node.Value == lieu as Livraison)
                                {
                                    key = node.Key;
                                    return true;
                                }
                                else
                                    return false;
                            }).First().Key
                        );

                        Dictionary<int, Livraison> livtmp = new Dictionary<int, Livraison>();

                        foreach (var liv in demandeLivraisons.livraisons)
                        {
                            if (liv.Key > key)
                            {
                                livtmp.Add(liv.Key - 1, liv.Value);
                            }
                            else
                            {
                                livtmp.Add(liv.Key, liv.Value);
                            }
                        }

                        demandeLivraisons.livraisons = livtmp;


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
                        int key = int.MaxValue;

                        demandeLivraisons.livraisons.Remove(
                            demandeLivraisons.livraisons.Where((node) =>
                            {
                                if (node.Value == lieu as Livraison)
                                {
                                    key = node.Key;
                                    return true;
                                }
                                else
                                    return false;
                            }).First().Key
                        );

                        Dictionary<int, Livraison> livtmp = new Dictionary<int, Livraison>();

                        foreach (var liv in demandeLivraisons.livraisons)
                        {
                            if (liv.Key > key)
                            {
                                livtmp.Add(liv.Key - 1, liv.Value);
                            }
                            else
                            {
                                livtmp.Add(liv.Key, liv.Value);
                            }
                        }

                        demandeLivraisons.livraisons = livtmp;
                        l = map.LoadDeliveries(demandeLivraisons);
                    }
                }
            }
            return l;
        }

        /// <summary>
        /// Modifie les plages horaires de la livraison
        /// </summary>
        /// <param name="d">Livraison à modifier</param>
        /// <param name="debutPlage">nouvelle valeur pour début de plage</param>
        /// <param name="finPlage">nouvelle valeur pour fin</param>
        public static void ChangePlage(Lieu d, DateTime debutPlage, DateTime finPlage)
        {
            if(etatActuel == etat.tourneeCalculee)
            {
                Livraison livraisonNewPlage = d as Livraison;
                livraisonNewPlage.planifier = true;
                livraisonNewPlage.SetPlage(debutPlage, finPlage);
                tournee.ModifPlage(livraisonNewPlage, demandeLivraisons, carte);
            }
        }
    }

    /// <summary>
    /// Représente les différents états dans lequel on se trouve
    /// </summary>
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
