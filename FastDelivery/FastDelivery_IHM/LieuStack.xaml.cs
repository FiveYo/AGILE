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
    public sealed partial class LieuStack : UserControl, INotifyPropertyChanged
    {
        public bool displayCheck { get; set; }
        public Lieu lieu { get; set; }

        public string address;
        public string duree;

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

        public LieuStack()
        {
            this.InitializeComponent();
        }


        public LieuStack(Lieu liv)
        {
            this.InitializeComponent();
            if (liv is Livraison)
            {
                dureeBox.Text = (liv as Livraison).duree.ToString();
                Image img = (Image)Resources["livraison"];
                typeBox.Source = img.Source;
                if ((liv as Livraison).planifier)
                {
                    if ((liv as Livraison).HeureDePassage != null)
                    {
                        heureArriveeBox.Text = String.Format("{0:t}",
                            (liv as Livraison).HeureDePassage);
                    }
                    plageHoraireBox.Text = String.Format("{0:t} à {1:t}",
                          (liv as Livraison).debutPlage, (liv as Livraison).finPlage);
                }
                
            }
            else
            {
                Image img = (Image)Resources["entrepot"];
                typeBox.Source = img.Source;
            }
            
            lieu = liv;

            addressBox.Text = String.Format("({0}, {1})", lieu.adresse.x, lieu.adresse.y);


            displayCheck = false;
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
