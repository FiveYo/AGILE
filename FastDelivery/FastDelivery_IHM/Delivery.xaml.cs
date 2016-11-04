using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Controls;

using FastDelivery_Library;
using Windows.UI.Xaml;
using System.ComponentModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace FastDelivery_IHM
{
    public sealed partial class Delivery : UserControl, INotifyPropertyChanged
    {
        public bool displayCheck { get; set; }
        public Livraison livraison { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public event RoutedEventHandler Checked;

        public event RoutedEventHandler AddLivraison;

        public string description
        {
            get
            {
                return String.Format("Adresse : ({0}, {1})\nDurée : {2}",
                    livraison.adresse.x, livraison.adresse.y, livraison.duree);
            }
        }

        public Delivery()
        {
            this.InitializeComponent();
            // Permet de binder correctement les propriétés (magie)
            (this.Content as FrameworkElement).DataContext = this;

            checkbox.Checked += Checkbox_Checked;
        }


        public Delivery(Livraison liv)
        {
            this.InitializeComponent();
            // Permet de binder correctement les propriétés (magie)
            (this.Content as FrameworkElement).DataContext = this;


            livraison = liv;
            displayCheck = false;

            checkbox.Checked += Checkbox_Checked;

            addBtn.Click += AddBtn_Click;
        }

        private void Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            Checked?.Invoke(this, e);
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

        public void toggleAddButton()
        {
            add.Visibility = add.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }





        private void NotifyPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
