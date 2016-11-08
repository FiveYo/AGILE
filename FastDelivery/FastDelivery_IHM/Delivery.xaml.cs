using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Controls;

using FastDelivery_Library;
using Windows.UI.Xaml;
using System.ComponentModel;
using FastDelivery_Library.Modele;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace FastDelivery_IHM
{
    public sealed partial class Delivery : UserControl, INotifyPropertyChanged
    {
        public bool displayCheck { get; set; }
        public Lieu lieu { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public event RoutedEventHandler AddLivraison;

        public event RoutedEventHandler Select;

        public event RoutedEventHandler RemoveLivraison;

        public event RoutedEventHandler ChgPlage;

        public string description
        {
            get
            {
                if (lieu is Livraison)
                {
                    return String.Format("Adresse : ({0}, {1})\nDurée : {2}",
                        lieu.adresse.x, lieu.adresse.y, (lieu as Livraison).duree);
                }
                else 
                {
                    return String.Format("Entrepot\nAdresse : ({0}, {1})",
                        lieu.adresse.x, lieu.adresse.y);
                }
            }
        }

        public Delivery()
        {
            this.InitializeComponent();
            // Permet de binder correctement les propriétés (magie)
            (this.Content as FrameworkElement).DataContext = this;
        }


        public Delivery(Lieu liv)
        {
            this.InitializeComponent();
            // Permet de binder correctement les propriétés (magie)
            (this.Content as FrameworkElement).DataContext = this;


            lieu = liv;
            displayCheck = false;

            addBtn.Click += AddBtn_Click;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddLivraison?.Invoke(this, e);
        }

        public void toggleSplit()
        {
            displayCheck = !displayCheck;
            NotifyPropertyChanged("displayCheck");
        }

        internal void displayAddButton()
        {
            borderAdd.Visibility = Visibility.Visible;
            borderRm.Visibility = Visibility.Collapsed;
        }

        public void SetSelect(bool b)
        {
            select.IsChecked = b;
        }

        private void NotifyPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Select?.Invoke(this, e);
        }

        internal void displayRemoveButton()
        {
            borderRm.Visibility = Visibility.Visible;
            borderAdd.Visibility = Visibility.Collapsed;
        }

        internal void displayChgPlageButton()
        {
            borderRm.Visibility = Visibility.Visible;
            borderAdd.Visibility = Visibility.Collapsed;
        }

        private void rmBtn_Click(object sender, RoutedEventArgs e)
        {
            RemoveLivraison?.Invoke(this, e);
        }

        private void chgBtn_Click(object sender, RoutedEventArgs e)
        {
            ChgPlage?.Invoke(this, e);
        }
    }
}
