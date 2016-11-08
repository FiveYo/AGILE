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

using FastDelivery_Library;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FastDelivery_IHM
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private Delivery selected;
        public MainPage()
        {
            this.InitializeComponent();
            this.navbar.IsPaneOpen = !navbar.IsPaneOpen;
            selected = null;
            // Enable Previous Button
            // SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            this.navbar.IsPaneOpen = !navbar.IsPaneOpen;
        }

        private async void loadMap_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();

            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.FileTypeFilter.Add(".xml");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                Stream streamFile = await file.OpenStreamForReadAsync();
                try
                {
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
            var picker = new Windows.Storage.Pickers.FileOpenPicker();


            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.FileTypeFilter.Add(".xml");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                Stream streamFile = await file.OpenStreamForReadAsync();
                try
                {
                    Tuple<List<Delivery>, Delivery> info = Controler.loadDeliveries(streamFile, mapCanvas);
                    this.navbar.IsPaneOpen = false;

                    listDeliveries.Children.Add(info.Item2);

                    info.Item2.Select += Livraison_Select;
                    info.Item2.AddLivraison += Livraison_AddLivraison;

                    foreach (var livraison in info.Item1)
                    {
                        listDeliveries.Children.Add(livraison);
                        livraison.Select += Livraison_Select;
                        livraison.AddLivraison += Livraison_AddLivraison;
                        livraison.RemoveLivraison += Livraison_RemoveLivraison;
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

        private async void getRoadMap_Click(object sender, RoutedEventArgs e)
        {
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

        private void Livraison_RemoveLivraison(object sender, RoutedEventArgs e)
        {
            Delivery d = sender as Delivery;
            Controler.RmLivTournee(d, mapCanvas);
            listDeliveries.Children.Remove(d);
        }

        private async void Livraison_AddLivraison(object sender, RoutedEventArgs e)
        {
            Delivery d = sender as Delivery;
            DeliveryPop popup = new DeliveryPop();
            await popup.ShowAsync();
            if(popup.continu)
            {
                Tuple<int, Delivery> toAdd = Controler.AddLivTournee(d.lieu, popup, mapCanvas);
                listDeliveries.Children.Insert(toAdd.Item1 != -1 ? toAdd.Item1 + 1 : 1, toAdd.Item2);
                toAdd.Item2.Select += Livraison_Select;
                toAdd.Item2.AddLivraison += Livraison_AddLivraison;
                toAdd.Item2.RemoveLivraison += Livraison_RemoveLivraison;
                toAdd.Item2.SetSelect(true);
            }
            
        }
        private void Livraison_Select(object sender, RoutedEventArgs e)
        {
            
            if((e.OriginalSource as ToggleButton).IsChecked == true)
            {
                selected = sender as Delivery;
                foreach (var delivery in listDeliveries.Children)
                {
                    if(delivery != sender)
                    {
                        (delivery as Delivery).SetSelect(false);
                    }
                }
            }
            else
            {
                selected = null;
            }
            
        }

        private async void loadCircuit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                feedBack.Text = "Chargement en cours de la tournée...";
                List<Delivery> deliveriesOrder = Controler.GetWay(mapCanvas);
                Delivery first = listDeliveries.Children.First() as Delivery;
                listDeliveries.Children.Clear();
                listDeliveries.Children.Add(first);
                foreach (var item in deliveriesOrder)
                {
                    listDeliveries.Children.Add(item);
                    item.AddLivraison += Livraison_AddLivraison;
                    item.Select += Livraison_Select;
                    item.RemoveLivraison += Livraison_RemoveLivraison;
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

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            // Gérer le cas ou on appuie sur + et il n'y a pas de tournée créé
            foreach (var item in listDeliveries.Children)
            {
                if (item is Delivery)
                {
                    ((Delivery)item).displayAddButton();
                }
            }
        }

        private void listDeliveries_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            int offset;

            if (e.Key == Windows.System.VirtualKey.Up) { offset = -1; }
            else if (e.Key == Windows.System.VirtualKey.Down) { offset = 1; }
            else { return; }

            if(selected != null)
            {
                int index = listDeliveries.Children.IndexOf(selected) + offset;
                if (index >= 0 && index < listDeliveries.Children.Count)
                {
                    Delivery d = listDeliveries.Children.ElementAt(index) as Delivery;
                    d.SetSelect(true);
                }
            }
            else if (listDeliveries.Children.Count > 0)
            {
                (listDeliveries.Children.First() as Delivery).SetSelect(true);
            }
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in listDeliveries.Children)
            {
                if (item is Delivery)
                {
                    ((Delivery)item).displayRemoveButton();
                }
            }
        }
    }
}
