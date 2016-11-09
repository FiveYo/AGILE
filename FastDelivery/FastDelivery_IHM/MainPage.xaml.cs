﻿using System;
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

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FastDelivery_IHM
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private LieuStack selected;
        public MainPage()
        {
            this.InitializeComponent();
            this.navbar.IsPaneOpen = !navbar.IsPaneOpen;
            selected = null;
            mapCanvas.PointClicked += MapCanvas_PointClicked;
        }

        private async void MapCanvas_PointClicked(object sender, EventMap e)
        {
            DeliveryPop popup = new DeliveryPop();
            await popup.ShowAsync();
            if (popup.continu)
            {
                // null doit être remplacer par la livraison séléctionné
                Tuple<int, LieuStack> toAdd = Controler.AddLivTournee(null, popup, mapCanvas);
                listDeliveries.Children.Insert(toAdd.Item1 != -1 ? toAdd.Item1 + 1 : 1, toAdd.Item2);
                toAdd.Item2.Select += LieuStack_Selected;
                toAdd.Item2.AddLivraison += LieuStack_AddLiv;
                toAdd.Item2.RemoveLivraison += LieuStack_RmLiv;
                toAdd.Item2.SetSelect(true);
            }
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
                    Tuple<List<LieuStack>, List<LieuMap>> info = Controler.loadDeliveries(streamFile, mapCanvas);
                    this.navbar.IsPaneOpen = false;

                    foreach (var lieu in info.Item1)
                    {
                        listDeliveries.Children.Add(lieu);
                        lieu.Select += LieuStack_Selected;
                        lieu.AddLivraison += LieuStack_AddLiv;
                        lieu.RemoveLivraison += LieuStack_RmLiv;
                        lieu.ChgPlage += Livraison_ChangePlage;
                    }

                    foreach (var lieu in info.Item2)
                    {
                        lieu.Checked += LieuMap_Checked;
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

        private void LieuMap_Checked(object sender, RoutedEventArgs e)
        {
            mapCanvas.SetSelect((sender as LieuMap).lieu);

            foreach (var lieuStack in listDeliveries.Children)
            {
                if ((lieuStack as LieuStack).lieu != (sender as LieuMap).lieu)
                {
                    (lieuStack as LieuStack).SetSelect(false);
                }
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

        private void LieuStack_RmLiv(object sender, RoutedEventArgs e)
        {
            LieuStack d = sender as LieuStack;
            Controler.RmLivTournee(d, mapCanvas);
            listDeliveries.Children.Remove(d);

        }

        private async void Livraison_ChangePlage(object sender, RoutedEventArgs e)
        {
            LieuStack d = sender as LieuStack;
            ChangePlagePop popup = new ChangePlagePop();
            await popup.ShowAsync();

            //conintu = validé
            if (popup.continu)
            {
                // Recupère les nouvelles plages
                String debutPlage = (popup.debutPlage).ToString();
                String finPlage = (popup.finPlage).ToString();            
                Controler.ChangePlage(d.lieu,debutPlage,finPlage);
            }

        }

        private async void LieuStack_AddLiv(object sender, RoutedEventArgs e)
        {
            LieuStack d = sender as LieuStack;
            DeliveryPop popup = new DeliveryPop();
            await popup.ShowAsync();
            if(popup.continu)
            {
                Tuple<int, LieuStack> toAdd = Controler.AddLivTournee(d.lieu, popup, mapCanvas);
                listDeliveries.Children.Insert(toAdd.Item1 != -1 ? toAdd.Item1 + 1 : 1, toAdd.Item2);
                toAdd.Item2.Select += LieuStack_Selected;
                toAdd.Item2.AddLivraison += LieuStack_AddLiv;
                toAdd.Item2.RemoveLivraison += LieuStack_RmLiv;
                toAdd.Item2.ChgPlage += Livraison_ChangePlage;
                toAdd.Item2.SetSelect(true);
            }
            
        }
        private void LieuStack_Selected(object sender, RoutedEventArgs e)
        {
            
            if((e.OriginalSource as ToggleButton).IsChecked == true)
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
                    item.AddLivraison += LieuStack_AddLiv;
                    item.Select += LieuStack_Selected;
                    item.RemoveLivraison += LieuStack_RmLiv;
                    item.ChgPlage += Livraison_ChangePlage;
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
                if (item is LieuStack)
                {
                    ((LieuStack)item).displayAddButton();
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
                    LieuStack d = listDeliveries.Children.ElementAt(index) as LieuStack;
                    d.SetSelect(true);
                }
            }
            else if (listDeliveries.Children.Count > 0)
            {
                (listDeliveries.Children.First() as LieuStack).SetSelect(true);
            }
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in listDeliveries.Children)
            {
                if (item is LieuStack)
                {
                    ((LieuStack)item).displayRemoveButton();
                }
            }
        }

        private void chgPlageButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in listDeliveries.Children)
            {
                if (item is LieuStack)
                {
                    ((LieuStack)item).displayChgPlageButton();
                }
            }
        }
    }
}
