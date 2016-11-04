using FastDelivery_Library;
using FastDelivery_Library.Modele;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace FastDelivery_IHM
{
    public sealed partial class Map : UserControl
    {
        double minX, minY, rX, rY;

        public SolidColorBrush colorMap { get; set; }

        public SolidColorBrush colorWay { get; set; }

        public Map()
        {
            this.InitializeComponent();
            SizeChanged += MapView_SizeChanged;
            colorMap = new SolidColorBrush(Colors.Green);
            colorWay = new SolidColorBrush(Colors.Blue);
        }

        public void LoadMap(Carte plan)
        {
            carteUI.Children.Clear();
            DisplayMap(plan);
        }

        public void LoadDeliveries(DemandeDeLivraisons demandeLivraisons)
        {
            DisplayEntrepot(demandeLivraisons.entrepot);
            DisplayDeliveries(demandeLivraisons.livraisons);
        }

        public async void LoadWay(Tournee t)
        {
            foreach (var chemin in t.troncons.Values)
            {
                DisplayWay(chemin);
                await Task.Delay(1000);
            }
        }

        private void MapView_SizeChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //this.Display();
        }

        private void DisplayMap(Carte plan)
        {
            minX = plan.minX;
            minY = plan.minY;
            rX = map.ActualWidth / (plan.maxX - plan.minX);
            rY = map.ActualHeight / (plan.maxY - plan.minY);

            foreach (var troncon in plan.troncons)
            {
                Line line = new Line();

                line.Stroke = colorMap;

                line.SetValue(ToolTipService.ToolTipProperty, "hi");

                line.X1 = getX(troncon.Value.origine.x, minX, rX);
                line.Y1 = getY(troncon.Value.origine.y, minY, rY);
                line.X2 = getX(troncon.Value.destination.x, minX, rX);
                line.Y2 = getY(troncon.Value.destination.y, minY, rY);

                line.StrokeThickness = 1;
                carteUI.Children.Add(line);
            }

        }

        private void DisplayWay(List<Troncon> chemin)
        {
            foreach (var troncon in chemin)
            {
                Point first = troncon.origine;
                Point second = troncon.destination;

                Line line = new Line();

                line.Stroke = colorWay;

                line.X1 = getX(first.x, minX, rX);
                line.Y1 = getY(first.y, minY, rY);
                line.X2 = getX(second.x, minX, rX);
                line.Y2 = getY(second.y, minY, rY);

                line.StrokeThickness = 3;
                cheminUI.Children.Add(line);
            }
        }

        private async void DisplayDeliveries(Dictionary<int, Livraison> demandeLivraisons)
        {
            var source = new BitmapImage();

            var rass = RandomAccessStreamReference.CreateFromUri(new Uri(this.BaseUri, "/Assets/pointeur_livraison.png"));
            IRandomAccessStream stream = await rass.OpenReadAsync();

            IRandomAccessStream stream2 = stream.CloneStream();


            var decoder = await BitmapDecoder.CreateAsync(stream);
            var size = new BitmapSize { Width = decoder.PixelWidth, Height = decoder.PixelHeight };

            source.SetSource(stream2);

            var recenterY = -size.Height;
            var recenterX = -size.Width / 2;

            foreach (var point in demandeLivraisons)
            {
                Image intersection = new Image();
                intersection.Source = source;
                double top = getY(point.Value.adresse.y, minY, rY) + recenterY;
                double left = getX(point.Value.adresse.x, minX, rX) + recenterX;
                Canvas.SetTop(intersection, top);
                Canvas.SetLeft(intersection, left);
                livraisonUI.Children.Add(intersection);
            }
        }


        private async void DisplayEntrepot(Entrepot entrepot)
        {
            Point entrepotPt = entrepot.adresse;
            var source = new BitmapImage();

            var rass = RandomAccessStreamReference.CreateFromUri(new Uri(this.BaseUri, "/Assets/pointeur_entrepot.png"));
            IRandomAccessStream stream = await rass.OpenReadAsync();

            IRandomAccessStream stream2 = stream.CloneStream();


            var decoder = await BitmapDecoder.CreateAsync(stream);
            var size = new BitmapSize { Width = decoder.PixelWidth, Height = decoder.PixelHeight };

            source.SetSource(stream2);

            var recenterY = -size.Height;
            var recenterX = -size.Width / 2;
            Image intersection = new Image();
            intersection.Source = source;
            double top = getY(entrepotPt.y, minY, rY) + recenterY;
            double left = getX(entrepotPt.x, minX, rX) + recenterX;
            Canvas.SetTop(intersection, top);
            Canvas.SetLeft(intersection, left);
            livraisonUI.Children.Add(intersection);
        }

        private static double getX(double X, double minX, double rX)
        {
            return (X - minX) * rX;
        }

        private static double getY(double Y, double minY, double rY)
        {
            return (Y - minY) * rY;
        }
    }
}
