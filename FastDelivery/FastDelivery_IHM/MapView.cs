using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FastDelivery_Library;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace FastDelivery_IHM
{

    public class MapView : Canvas
    {
        bool planLoaded;
        double minX, minY, rX, rY;

        public MapView()
        {
            this.SizeChanged += MapView_SizeChanged;
            planLoaded = false;
        }

        public void LoadMap(StructPlan plan)
        {
            Children.Clear();
            planLoaded = true;
            DisplayMap(plan);
        }

        public void LoadDeliveries(Dictionary<int, Livraison> demandeLivraisons)
        {
            DisplayEntrepot(demandeLivraisons);
            DisplayDeliveries(demandeLivraisons);
        }


        public void LoadWay(LinkedList<Point> chemin)
        {
            DisplayWay(chemin);
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

        private void DisplayMap(StructPlan plan)
        {
            minX = plan.Xmin;
            minY = plan.Ymin;
            rX = this.ActualWidth / (double)(plan.Xmax - plan.Xmin);
            rY = this.ActualHeight / (double)(plan.Ymax - plan.Ymin);

            foreach (var troncon in plan.HashTroncon)
            {
                Line line = new Line();

                line.Stroke = new SolidColorBrush(Colors.Green);

                line.X1 = getX(troncon.Value.Origin.x, minX, rX);
                line.Y1 = getY(troncon.Value.Origin.y, minY, rY);
                line.X2 = getX(troncon.Value.Destination.x, minX, rX);
                line.Y2 = getY(troncon.Value.Destination.y, minY, rY);

                line.StrokeThickness = 1;
                this.Children.Add(line);
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

            var recenterY = - size.Height;
            var recenterX = - size.Width / 2;

            foreach (var point in demandeLivraisons)
            {
                Image intersection = new Image();
                intersection.Source = source;
                double top = getY(point.Value.Adresse.y, minY, rY) + recenterY;
                double left = getX(point.Value.Adresse.x, minX, rX) + recenterX;
                Canvas.SetTop(intersection, top);
                Canvas.SetLeft(intersection, left);
                this.Children.Add(intersection);
            }
        }

        private void DisplayWay(LinkedList<Point> chemin)
        {
            Point first = chemin.First.Value;
            foreach(var point in chemin.Skip(1))
            {
                Point second = point;

                Line line = new Line();

                line.Stroke = new SolidColorBrush(Colors.Blue);

                line.X1 = getX(first.x, minX, rX);
                line.Y1 = getY(first.y, minY, rY);
                line.X2 = getX(second.x, minX, rX);
                line.Y2 = getY(second.y, minY, rY);

                line.StrokeThickness = 3;
                this.Children.Add(line);

                first = second;
            }
        }

        private async void DisplayEntrepot(Dictionary<int, Livraison> demandeLivraisons)
        {
            //Point entrepot = demandeLivraisons.First().Value.Entrepot.adresse;
            //var source = new BitmapImage();

            //var rass = RandomAccessStreamReference.CreateFromUri(new Uri(this.BaseUri, "/Assets/pointeur_entrepot.png"));
            //IRandomAccessStream stream = await rass.OpenReadAsync();

            //IRandomAccessStream stream2 = stream.CloneStream();


            //var decoder = await BitmapDecoder.CreateAsync(stream);
            //var size = new BitmapSize { Width = decoder.PixelWidth, Height = decoder.PixelHeight };

            //source.SetSource(stream2);

            //var recenterY = -size.Height;
            //var recenterX = -size.Width / 2;
            //Image intersection = new Image();
            //intersection.Source = source;
            //double top = getY(entrepot.y, minY, rY) + recenterY;
            //double left = getX(entrepot.x, minX, rX) + recenterX;
            //Canvas.SetTop(intersection, top);
            //Canvas.SetLeft(intersection, left);
            //this.Children.Add(intersection);
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
