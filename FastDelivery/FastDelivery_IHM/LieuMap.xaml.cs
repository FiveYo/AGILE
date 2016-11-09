using FastDelivery_Library;
using FastDelivery_Library.Modele;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace FastDelivery_IHM
{
    public sealed partial class LieuMap : UserControl
    {
        public event RoutedEventHandler Checked;

        private bool _isChecked;

        public bool isChecked {
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

            if(lieu is Livraison)
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

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            EventMap em = new EventMap();
            em.lieu = lieu;
            Checked?.Invoke(this, e);
        }

        public void SetSelect(bool b)
        {
            if (b)
            {
                if (b != _isChecked)
                {
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
