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

    class MapView : Canvas
    {
        StructPlan plan;
        bool planLoaded;
        bool livraisonLoaded;
        double rX, rY, minX, minY;

        public MapView()
        {
            this.SizeChanged += MapView_SizeChanged;
            planLoaded = false;
            livraisonLoaded = false;
        }

        public void LoadMap(StructPlan plan)
        {
            this.plan = plan;
            planLoaded = true;
            minX = plan.Xmin;
            minY = plan.Ymin;
            this.Display();
        }

        private void MapView_SizeChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Display();
        }

        private void Display()
        {
            // On supprime tout ce qu'on avait affiché
            this.Children.Clear();
            rX = this.ActualWidth / (this.plan.Xmax - this.plan.Xmin);
            rY = this.ActualHeight / (this.plan.Ymax - this.plan.Ymin);

            if (planLoaded)
            {
                this.DisplayMap();
                this.DisplayLivraison();
                if(livraisonLoaded)
                {
                    this.DisplayLivraison();
                }
            }
        }

        private void DisplayMap()
        {
            foreach (var troncon in this.plan.HashTroncon)
            {
                Line line = new Line();

                line.Stroke = new SolidColorBrush(Colors.Green);

                line.X1 = getX(troncon.Value.Origin.x);
                line.Y1 = getY(troncon.Value.Origin.y);
                line.X2 = getX(troncon.Value.Destination.x);
                line.Y2 = getY(troncon.Value.Destination.y);

                line.StrokeThickness = 2;
                this.Children.Add(line);
            }
        }

        private async void DisplayLivraison()
        {
            if (this.planLoaded)
            {
                var source = new BitmapImage();

                var rass = RandomAccessStreamReference.CreateFromUri(new Uri(this.BaseUri, "/Assets/pointeur.png"));
                IRandomAccessStream stream = await rass.OpenReadAsync();

                IRandomAccessStream stream2 = stream.CloneStream();


                var decoder = await BitmapDecoder.CreateAsync(stream);
                var size = new BitmapSize { Width = decoder.PixelWidth, Height = decoder.PixelHeight };
                
                source.SetSource(stream2);

                var recenterY = - size.Height;
                var recenterX = - size.Width / 2;

                foreach (var point in this.plan.HashPoint)
                {
                    Image intersection = new Image();
                    intersection.Source = source;
                    double top = getY(point.Value.y) + recenterY;
                    double left = getX(point.Value.x) + recenterX;
                    Canvas.SetTop(intersection, top);
                    Canvas.SetLeft(intersection, left);
                    this.Children.Add(intersection);
                }
            }
        }

        private double getX(double X)
        {
            return (X - this.minX) * this.rX;
        }

        private double getY(double Y)
        {
            return (Y - this.minY) * this.rY;
        }
    }
}
