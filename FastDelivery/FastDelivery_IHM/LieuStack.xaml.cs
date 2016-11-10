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
using Windows.UI.Xaml.Media;
using Windows.UI;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace FastDelivery_IHM
{
    public sealed partial class LieuStack : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Classe correspondant à une livraison dans la liste déroulante de livraisons pour une tournée chargée.
        /// </summary>
        public bool displayCheck { get; set; }
        public Lieu lieu { get; set; }
        public string address;
        public string duree;

        public event PropertyChangedEventHandler PropertyChanged;
        public event RoutedEventHandler AddLivraison;
        public event RoutedEventHandler Select;
        public event RoutedEventHandler RemoveLivraison;
        public event RoutedEventHandler ChgPlage;

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

                if ((liv as Livraison).heureArrivee != DateTime.MinValue && (liv as Livraison).heureDepart != DateTime.MinValue)
                {
                    heureArriveeBox.Text = String.Format("{0:t} → {1:t}",
                        (liv as Livraison).heureArrivee, (liv as Livraison).heureDepart);
                }

                if ((liv as Livraison).planifier)
                {
                    plageHoraireBox.Text = String.Format("{0:t} à {1:t}",
                          (liv as Livraison).debutPlage, (liv as Livraison).finPlage);
                    if ((liv as Livraison).heureArrivee > (liv as Livraison).finPlage)
                    {
                        heureArriveeBox.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else if ((liv as Livraison).heureArrivee < (liv as Livraison).debutPlage)
                    {
                        heureArriveeBox.Foreground = new SolidColorBrush(Colors.Green);
                    }
                }

                (liv as Livraison).PropertyChanged += LieuStack_PropertyChanged;
            }
            else
            {
                Image img = (Image)Resources["entrepot"];
                typeBox.Source = img.Source;

                heureArriveeBox.Text = String.Format("{0:t}", (liv as Entrepot).heureDepart);
            }
            
            lieu = liv;

            addressBox.Text = String.Format("({0}, {1})", lieu.adresse.x, lieu.adresse.y);


            displayCheck = false;
        }

        private void LieuStack_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((lieu as Livraison).planifier)
            {
                plageHoraireBox.Text = String.Format("{0:t} à {1:t}",
                        (lieu as Livraison).debutPlage, (lieu as Livraison).finPlage);
                if ((lieu as Livraison).heureArrivee > (lieu as Livraison).finPlage)
                {
                    heureArriveeBox.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if ((lieu as Livraison).heureArrivee < (lieu as Livraison).debutPlage)
                {
                    heureArriveeBox.Foreground = new SolidColorBrush(Colors.Green);
                }
            }
            if ((lieu as Livraison).heureArrivee != DateTime.MinValue && (lieu as Livraison).heureDepart != DateTime.MinValue)
            {
                heureArriveeBox.Text = String.Format("{0:t} → {1:t}",
                    (lieu as Livraison).heureArrivee, (lieu as Livraison).heureDepart);
            }
        }

        public void SetSelect(bool b)
        {
            select.IsChecked = b;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Select?.Invoke(this, e);
        }
    }
}
