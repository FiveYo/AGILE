﻿using FastDelivery_Library.Modele;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace FastDelivery_IHM
{
    /// <summary>
    /// Permet de représenter un Lieu sur la carte et de gérer les animations
    /// </summary>
    public sealed partial class LieuMap : UserControl
    {
        public event RoutedEventHandler Clicked;
        public event RoutedEventHandler Supprimer;
        public event RoutedEventHandler Modifier;

        private bool _isChecked;

        public bool isChecked
        {
            get
            {
                return _isChecked;
            }
        }

        public Lieu lieu { get; set; }
        public LieuMap()
        {
            this.InitializeComponent();
        }

        public LieuMap(Lieu l)
        {
            this.InitializeComponent();

            lieu = l;

            image.Tapped += Image_Tapped;

            image.RightTapped += Image_RightTapped;

            if (lieu is Livraison)
            {
                Image img = (Image)Resources["livraison"];
                image.Source = img.Source;
            }
            else
            {
                Image img = (Image)Resources["entrepot"];
                image.Source = img.Source;
            }
        }

        private void Image_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (lieu is Livraison)
            {
                MenuFlyout mf = new MenuFlyout();
                MenuFlyoutItem mfi = new MenuFlyoutItem();
                mfi.Text = "Supprimer";
                mfi.Click += SupprimmerClick;

                mf.Items.Add(mfi);


                if (Controler.etatActuel == etat.tourneeCalculee)
                {
                    MenuFlyoutItem mfi2 = new MenuFlyoutItem();
                    mfi2.Text = "Modifier plage horaire";
                    mfi2.Click += ModifierClick;

                    mf.Items.Add(mfi2);
                }

                mf.ShowAt(image);
            }
        }

        private void ModifierClick(object sender, RoutedEventArgs e)
        {
            Modifier?.Invoke(this, e);
        }

        private void SupprimmerClick(object sender, RoutedEventArgs e)
        {
            Supprimer?.Invoke(this, e);
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Clicked?.Invoke(this, e);
        }

        public void SetSelect(bool b)
        {
            if (b)
            {
                if (b != _isChecked)
                {
                    animationPointeur.Begin();
                    if (lieu is Livraison)
                    {
                        Image img = (Image)Resources["livraisonShadow"];
                        image.Source = img.Source;
                    }
                    else
                    {
                        Image img = (Image)Resources["entrepotShadow"];
                        image.Source = img.Source;
                    }
                }
            }
            else
            {
                if (b != _isChecked)
                {
                    if (lieu is Livraison)
                    {
                        Image img = (Image)Resources["livraison"];
                        image.Source = img.Source;
                    }
                    else
                    {
                        Image img = (Image)Resources["entrepot"];
                        image.Source = img.Source;
                    }
                }
            }

            _isChecked = b;
        }
    }
}
