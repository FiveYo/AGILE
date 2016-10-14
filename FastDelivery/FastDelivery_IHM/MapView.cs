using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FastDelivery_Library;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;

namespace FastDelivery_IHM
{

    class MapView : Canvas
    {
        public MapView()
        {
            this.Loaded += MapView_Loaded;
        }

        private async void MapView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var dialog = new MessageDialog("heelo", "title");
            await dialog.ShowAsync();
        }
    }
}
