using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FastDelivery_Library;
using FastDelivery_Library.Modele;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using System.Collections.ObjectModel;

namespace FastDelivery_IHM
{

    public class MapView : Grid
    {
        Canvas carteUI;
        Canvas cheminUI;
        Canvas livraisonUI;
        double minX, minY, rX, rY;

        public SolidColorBrush colorMap { get; set; }

        public SolidColorBrush colorWay { get; set; }

        

        public MapView()
        {
            SizeChanged += MapView_SizeChanged;
            colorMap = new SolidColorBrush(Colors.Green);
            colorWay = new SolidColorBrush(Colors.Blue);
            carteUI = new Canvas();
            cheminUI = new Canvas();
            livraisonUI = new Canvas();
            Canvas.SetZIndex(carteUI, 0);
            Canvas.SetZIndex(cheminUI, 1);
            Canvas.SetZIndex(livraisonUI, 2);

            Children.Add(carteUI);
            Children.Add(cheminUI);
            Children.Add(livraisonUI);
        }

        public void LoadMap(Carte plan)
        {
            Children.Clear();
            DisplayMap(plan);
        }

        public void LoadDeliveries(DemandeDeLivraisons demandeLivraisons)
        {
            DisplayEntrepot(demandeLivraisons.entrepot);
            DisplayDeliveries(demandeLivraisons.livraisons);
        }


        public async void LoadWay(Tournee t)
        {
            foreach(var chemin in t.Hashchemin.Values)
            {
                DisplayWay(chemin.getTronconList());
                await Task.Delay(1000);
            }
        }


        private void MapView_SizeChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            //this.Display();
        }

        //private void Display()
        //{
        //    // On supprime tout ce qu'on avait affiché
        //    this.Children.Clear();
        //    rX = this.ActualWidth / (this.plan.Xmax - this.plan.Xmin);
        //    rY = this.ActualHeight / (this.plan.Ymax - this.plan.Ymin);

        //    if (planLoaded)
        //    {
        //        this.DisplayMap();
        //        // this.DisplayLivraison();
        //        if(livraisonLoaded)
        //        {
        //            this.DisplayLivraison();
        //        }
        //    }
        //}

        private void DisplayMap(Carte plan)
        {
            minX = plan.minX;
            minY = plan.minY;
            rX = this.ActualWidth / (double)(plan.maxX - plan.minX);
            rY = this.ActualHeight / (double)(plan.maxY - plan.minY);

            foreach (var troncon in plan.troncons)
            {
                Line line = new Line();

                line.Stroke = colorMap;

                line.SetValue(ToolTipService.ToolTipProperty, "hi");

                line.X1 = getX(troncon.Value.origine.x, minX, rX);
                line.Y1 = getY(troncon.Value.origine.y, minY, rY);
                line.X2 = getX(troncon.Value.destination.x, minX, rX);
                line.Y2 = getY(troncon.Value.destination.y, minY, rY);


                line.StrokeThickness = 3;
                carteUI.Children.Add(line);
            }

        }

        private void DisplayWay(List<Troncon> chemin)
        {
            foreach(var troncon in chemin)
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
