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

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FastDelivery_IHM
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            MapView map = this.mapCanvas;
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.FileTypeFilter.Add(".xml");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                Stream streamFile = await file.OpenStreamForReadAsync();
                StructPlan xmlData = Outils.ParserXml_Plan(streamFile);
                map.LoadMap(xmlData);
                //Dictionary<int, FastDelivery_Library.Point> points = xmlData.HashPoint;
                //Dictionary<int, Troncon> troncons = xmlData.HashTroncon;
                

                //foreach (var point in points)
                //{
                //    TextBlock intersection = new TextBlock();
                //    intersection.Text = point.Value.id.ToString() + " : (" + point.Value.x.ToString() + "," + point.Value.y.ToString() + ")";
                //    Canvas.SetTop(intersection, point.Value.y);
                //    Canvas.SetLeft(intersection, point.Value.x);
                //    map.Children.Add(intersection);
                //}
            }
            else
            {
                
            }
        }
    }
}
