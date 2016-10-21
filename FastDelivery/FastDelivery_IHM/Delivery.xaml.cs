using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Controls;

using FastDelivery_Library;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace FastDelivery_IHM
{
    public sealed partial class Delivery : UserControl
    {
        public Delivery(Point pt, int duree)//TODO
        {
            this.InitializeComponent();
            this.adresse.Text = "Adresse : (" + pt.x.ToString() + ", " + pt.y.ToString() + ")\nDurée : " + duree.ToString() +")\nHeure de passage : " ;
        }

        public void toggleSplit()
        {
            //TODO
        }
    }
}
