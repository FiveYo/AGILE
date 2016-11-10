using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using FastDelivery_Library;
using System.Threading;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FastDelivery_IHM
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DeliveryPop popup;
        const int TIMERESET = 10000;
        bool waitevent;

        private LieuStack selected;
        public MainPage()
        {
            this.InitializeComponent();
            this.navbar.IsPaneOpen = !navbar.IsPaneOpen;
            selected = null;
            mapCanvas.PointClicked += MapCanvas_PointClicked;

            waitevent = false;
        }

        private async void MapCanvas_PointClicked(object sender, EventMap e)
        {
            //Cette méthode ne peut être appelé que quand les livraisons sont chargés (géré dans Map)                
            popup = new DeliveryPop();
            popup.adresse = e.point;

            if (Controler.etatActuel == etat.livraisonCharge)
            {
                popup.SecondaryButtonText = "Créer";
            }
            await popup.ShowAsync();

            if (popup.continu)
            {
                if (Controler.etatActuel == etat.tourneeCalculee)
                {
                    waitevent = true;
                    feedBack.Text = "Séléctionner une livraison sur la carte s'il vous plait";
                }
                else
                {
                    Tuple<LieuStack, LieuMap> lieuUI = Controler.AddLivDemande(popup, mapCanvas);
                    lieuUI.Item1.Select += LieuStack_Selected;
                    lieuUI.Item2.Clicked += LieuMap_Clicked;
                    lieuUI.Item2.Modifier += LieuMap_Modifier;
                    lieuUI.Item2.Supprimer += LieuMap_Supprimer;
                    listDeliveries.Children.Add(lieuUI.Item1);
                }
            }
        }

        private async void LieuMap_Supprimer(object sender, RoutedEventArgs e)
        {
            if (waitevent)
            {
                if (await showCancel())
                    return;
            }
            LieuMap d = sender as LieuMap;

            List<LieuMap> lieuMap = Controler.RemoveLivraison(d.lieu, mapCanvas);

            if(lieuMap != null)
            {
                foreach (var lieu in lieuMap)
                {
                    lieu.Clicked += LieuMap_Clicked;
                    lieu.Modifier += LieuMap_Modifier;
                    lieu.Supprimer += LieuMap_Supprimer;
                }

                listDeliveries.Children.Remove(
                    listDeliveries.Children.Where((node) =>
                    {
                        if ((node as LieuStack).lieu == d.lieu)
                            return true;
                        return false;
                    }
                ).First());
            }
            else
            {
                feedBack.Text = "Vous ne pouvez pas avoir de tournée sans livraison";
            }
        }

        private async void LieuMap_Modifier(object sender, RoutedEventArgs e)
        {
            if (waitevent)
            {
                if (await showCancel())
                    return;
            }
            LieuMap d = sender as LieuMap;
            ChangePlagePop popup = new ChangePlagePop();
            await popup.ShowAsync();

            //conintu = validé
            if (popup.continu)
            {
                // Recupère les nouvelles plages
                DateTime debutPlage = new DateTime(popup.debutPlage.Ticks);
                DateTime finPlage = new DateTime(popup.finPlage.Ticks);
                Controler.ChangePlage(d.lieu, debutPlage, finPlage);
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            this.navbar.IsPaneOpen = !navbar.IsPaneOpen;
        }

        private async void loadMap_Click(object sender, RoutedEventArgs e)
        {
            if(waitevent)
            {
                if (await showCancel())
                    return;
            }
            var picker = new Windows.Storage.Pickers.FileOpenPicker();

            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.FileTypeFilter.Add(".xml");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                Stream streamFile = await file.OpenStreamForReadAsync();
                try
                {
                    listDeliveries.Children.Clear();
                    Controler.loadMap(streamFile, mapCanvas);
                    feedBack.Text = "Votre plan a été chargé avec succès. Vous pouvez dès maintenant charger une demande de livraison, ou un nouveau plan.";
                }
                catch (Exception_XML exc)
                {
                    feedBack.Text = "Échec du chargement de la carte : " + exc.Message;
                }
                catch
                {
                    feedBack.Text = "Une erreur est survenue lors du chargement de la carte. Veuillez réessayer.";
                }
                animFeedback.Begin();
            }
            else
            {
                feedBack.Text = "Échec du chargement : aucun fichier fourni.";
            }
            
        }
        
        private async void loadDeliveries_Click(object sender, RoutedEventArgs e)
        {
            if (waitevent)
            {
                if (await showCancel())
                    return;
            }
            var picker = new Windows.Storage.Pickers.FileOpenPicker();


            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.FileTypeFilter.Add(".xml");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                Stream streamFile = await file.OpenStreamForReadAsync();
                try
                {
                    Tuple<List<LieuStack>, List<LieuMap>> info = Controler.loadDeliveries(streamFile, mapCanvas);
                    this.navbar.IsPaneOpen = false;

                    listDeliveries.Children.Clear();

                    foreach (var lieu in info.Item1)
                    {
                        listDeliveries.Children.Add(lieu);
                        lieu.Select += LieuStack_Selected;
                    }

                    foreach (var lieu in info.Item2)
                    {
                        lieu.Clicked += LieuMap_Clicked;
                        lieu.Modifier += LieuMap_Modifier;
                        lieu.Supprimer += LieuMap_Supprimer;
                    }
                    feedBack.Text = "Votre demande de livraison a été chargée avec succès. Vous pouvez désormais calculer une tournée, ou charger un nouveau plan.";
                }
                catch (Exception_XML exc)
                {
                    feedBack.Text = "Échec du chargement de la tournée : " + exc.Message;
                }
                catch (Exception_Stream exc)
                {
                    feedBack.Text = exc.Message;
                }
                catch
                {
                    feedBack.Text = "Une erreur est survenue lors le chargement de la tournée. Veuillez réessayer.";
                }
                animFeedback.Begin();
            }
            else
            {
                feedBack.Text = "Échec du chargement : aucun fichier fourni.";
            }
        }

        private void LieuMap_Clicked(object sender, RoutedEventArgs e)
        {
            if(waitevent)
            {
                LieuMap lieuClicked = sender as LieuMap;
                waitevent = false;
                Tuple<int, LieuStack, LieuMap> toAdd = Controler.AddLivTournee(lieuClicked.lieu, popup, mapCanvas);
                toAdd.Item2.Select += LieuStack_Selected;
                toAdd.Item3.Clicked += LieuMap_Clicked;
                toAdd.Item3.Modifier += LieuMap_Modifier;
                toAdd.Item3.Supprimer += LieuMap_Supprimer;

                listDeliveries.Children.Insert(toAdd.Item1 + 2, toAdd.Item2);
                feedBack.Text = "La tournée a été calculée, vous pouvez la visualiser sur le plan. Vous pouvez également charger un nouveau plan.";
                return;
            }
            mapCanvas.SetSelect((sender as LieuMap).lieu);

            foreach (var lieuStack in listDeliveries.Children)
            {
                if ((lieuStack as LieuStack).lieu == (sender as LieuMap).lieu)
                {
                    (lieuStack as LieuStack).SetSelect(true);
                }
                else
                {
                    (lieuStack as LieuStack).SetSelect(false);
                }
            }
        }

        private async void getRoadMap_Click(object sender, RoutedEventArgs e)
        {
            if (waitevent)
            {
                if (await showCancel())
                    return;
            }
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain text", new List<string>() { ".txt" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            try
            {
                feedBack.Text = "Génération de la feuille de route";
                Controler.GetRoadMap(file);
                feedBack.Text = "Votre feuille de route à bien été générée.";
                animFeedback.Begin();
            }
            catch (TimeoutException)
            {
                feedBack.Text = "La feuille de route n'a pas été générée";
                var messageDialog = new MessageDialog("La feuille de route n'a pas été générée");
                await messageDialog.ShowAsync();
            }
        }
        
        private async void LieuStack_Selected(object sender, RoutedEventArgs e)
        {
            if (waitevent)
            {
                if (await showCancel())
                    return;
            }
            if ((e.OriginalSource as ToggleButton).IsChecked == true)
            {
                selected = sender as LieuStack;
                foreach (var LieuStack in listDeliveries.Children)
                {
                    if(LieuStack != sender)
                    {
                        (LieuStack as LieuStack).SetSelect(false);
                    }
                }

                mapCanvas.SetSelect(selected.lieu);
            }
            else
            {
                selected = null;
            }
            
        }

        private async void loadCircuit_Click(object sender, RoutedEventArgs e)
        {
            if (waitevent)
            {
                if (await showCancel())
                    return;
            }
            try
            {
                feedBack.Text = "Chargement en cours de la tournée...";
                List<LieuStack> deliveriesOrder = Controler.GetWay(mapCanvas);
                LieuStack first = listDeliveries.Children.First() as LieuStack;
                
                listDeliveries.Children.Clear();
                listDeliveries.Children.Add(first);
                foreach (var item in deliveriesOrder)
                {
                    listDeliveries.Children.Add(item);
                    item.Select += LieuStack_Selected;
                } 
                feedBack.Text = "La tournée a été calculée, vous pouvez la visualiser sur le plan. Vous pouvez également charger un nouveau plan.";
                animFeedback.Begin();
            }
            catch (Exception_Stream exc)
            {
                feedBack.Text = exc.Message;
            }
            catch (TimeoutException)
            {
                feedBack.Text = "Calcul de la tournée annulée. Sélectionnez une nouvelle carte et une nouvelle livraison";
                var messageDialog = new MessageDialog("L'opération a été annulée car le calcul demande un temps trop important (plus de 1 minute).");
                await messageDialog.ShowAsync();
            }
        }

        private async void listDeliveries_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (waitevent)
            {
                if (await showCancel())
                    return;
            }

            int offset;

            if (e.Key == Windows.System.VirtualKey.Up) { offset = -1; }
            else if (e.Key == Windows.System.VirtualKey.Down) { offset = 1; }
            else { return; }

            if(selected != null)
            {
                int index = listDeliveries.Children.IndexOf(selected) + offset;
                if (index >= 0 && index < listDeliveries.Children.Count)
                {
                    LieuStack d = listDeliveries.Children.ElementAt(index) as LieuStack;
                    d.SetSelect(true);
                }
            }
            else if (listDeliveries.Children.Count > 0)
            {
                (listDeliveries.Children.First() as LieuStack).SetSelect(true);
            }
        }

        private async Task<bool> showCancel()
        {
            MessageDialog m = new MessageDialog("Vous étiez en train de créer une nouvelle livraison, veuillez sélectionnez une livraison ou annuler");
            m.Commands.Add(new UICommand("Continuer") { Id = 0 });
            m.Commands.Add(new UICommand("Annuler") { Id = 1 });

            m.DefaultCommandIndex = 0;
            m.CancelCommandIndex = 1;

            var result = await m.ShowAsync();

            if(result.Id as int? == 1)
            {
                waitevent = false;
            }
            return waitevent;
        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            Controler.Undo();
        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            Controler.Redo();
        }
    }
}
