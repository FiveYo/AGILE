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

namespace FastDelivery_IHM
{

    public class MapView : Canvas
    {
        const int TAILLE_POINTS = 8;
        const int TAILLE_POINTS_FAKE = 16;
        List<ItemsControl> calques;
        ItemsControl carteUI;
        ItemsControl cheminUI;
        double minX, minY, rX, rY;

        public MapView()
        {
            SizeChanged += MapView_SizeChanged;
            calques = new List<ItemsControl>();
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
            foreach(var chemin in t.troncons.Values)
            {
                DisplayWay(chemin);
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
                Line lineToAim = new Line();
                line.StrokeThickness = 2;
                lineToAim.StrokeThickness = 20;
                line.Stroke = new SolidColorBrush(Colors.Green);
                lineToAim.Stroke = new SolidColorBrush(Colors.Transparent);

                line.X1 = getX(troncon.Value.origine.x, minX, rX);
                line.Y1 = getY(troncon.Value.origine.y, minY, rY);
                line.X2 = getX(troncon.Value.destination.x, minX, rX);
                line.Y2 = getY(troncon.Value.destination.y, minY, rY);
                lineToAim.X1 = getX(troncon.Value.origine.x, minX, rX);
                lineToAim.Y1 = getY(troncon.Value.origine.y, minY, rY);
                lineToAim.X2 = getX(troncon.Value.destination.x, minX, rX);
                lineToAim.Y2 = getY(troncon.Value.destination.y, minY, rY);

                this.Children.Add(line);
                this.Children.Add(lineToAim);

                lineToAim.PointerEntered += Line_PointerEntered;
                lineToAim.PointerExited += Line_PointerExited;

                ToolTip tt = new ToolTip();
                tt.Content = "Nom rue: " + troncon.Value.rue;
                lineToAim.SetValue(ToolTipService.ToolTipProperty, tt);
                
            }
            foreach (var point in plan.points)
            {
                Ellipse circle = new Ellipse();
                Ellipse circleToAim = new Ellipse();
                circle.Height = TAILLE_POINTS;
                circleToAim.Height = TAILLE_POINTS_FAKE;
                circle.Width = TAILLE_POINTS;
                circleToAim.Width = TAILLE_POINTS_FAKE;
                Brush color = new SolidColorBrush(Colors.Black);
                Brush colorFake = new SolidColorBrush(Colors.Transparent);
                circle.Fill = color;
                circleToAim.Fill = colorFake;

                Canvas.SetTop(circle,getY(point.Value.y, minY, rY)-(TAILLE_POINTS/2));
                Canvas.SetLeft(circle, getX(point.Value.x, minY, rX) - (TAILLE_POINTS / 2));
                Canvas.SetTop(circleToAim, getY(point.Value.y, minY, rY) - (TAILLE_POINTS_FAKE / 2));
                Canvas.SetLeft(circleToAim, getX(point.Value.x, minY, rX) - (TAILLE_POINTS_FAKE / 2));

                this.Children.Add(circle);
                this.Children.Add(circleToAim);

                circleToAim.PointerEntered += CircleToAim_PointerEntered;
                circleToAim.PointerExited += CircleToAim_PointerExited;

                ToolTip tt = new ToolTip();
                tt.Content = "id : "+ point.Value.id + "\n x : " + point.Value.x + "\n y : " + point.Value.y;
                circleToAim.SetValue(ToolTipService.ToolTipProperty, tt);
            }
        }

        private void CircleToAim_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Ellipse circle = sender as Ellipse;
            ToolTip tt = circle.GetValue(ToolTipService.ToolTipProperty) as ToolTip;
            tt.IsOpen = false;
        }

        private void CircleToAim_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Ellipse circle = sender as Ellipse;
            ToolTip tt = circle.GetValue(ToolTipService.ToolTipProperty) as ToolTip;
            tt.Placement = Windows.UI.Xaml.Controls.Primitives.PlacementMode.Left;
            tt.IsOpen = true; //or false?!
        }

        private void Line_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Line line = sender as Line;
            ToolTip tt = line.GetValue(ToolTipService.ToolTipProperty) as ToolTip;
            tt.IsOpen = false;
        }

        private void Line_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Line line = sender as Line;
            ToolTip tt = line.GetValue(ToolTipService.ToolTipProperty) as ToolTip;
            tt.IsOpen = true; //or false?!
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
                double top = getY(point.Value.adresse.y, minY, rY) + recenterY;
                double left = getX(point.Value.adresse.x, minX, rX) + recenterX;
                Canvas.SetTop(intersection, top);
                Canvas.SetLeft(intersection, left);
                this.Children.Add(intersection);
            }
        }

        private void DisplayWay(List<Troncon> chemin)
        {
            foreach(var troncon in chemin)
            {
                Point first = troncon.origine;
                Point second = troncon.destination;

                Line line = new Line();

                line.Stroke = new SolidColorBrush(Colors.Blue);

                line.X1 = getX(first.x, minX, rX);
                line.Y1 = getY(first.y, minY, rY);
                line.X2 = getX(second.x, minX, rX);
                line.Y2 = getY(second.y, minY, rY);

                line.StrokeThickness = 3;
                this.Children.Add(line);
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
            this.Children.Add(intersection);
        }

        void MouseDownHandler(Object sender, RoutedEventArgs args)
        {
            ((Line)sender).Stroke = new SolidColorBrush(Colors.Red);
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
