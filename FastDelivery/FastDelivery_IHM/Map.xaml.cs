﻿using FastDelivery_Library;
using FastDelivery_Library.Modele;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public SolidColorBrush colorAim { get; set; }

        public SolidColorBrush colorPoint { get; set; }

        const int TAILLE_POINTS = 8;
        const int TAILLE_POINTS_FAKE = 16;
        const int TAILLE_LIGNE_MAP = 2;
        const int TAILLE_LIGNE_MAP_FAKE = 20;
        const int TAILLE_LIGNE_CHEMIN = 3;

        public Map()
        {
            this.InitializeComponent();
            SizeChanged += MapView_SizeChanged;
            colorMap = new SolidColorBrush(Colors.Green);
            colorWay = new SolidColorBrush(Colors.Blue);
            colorAim = new SolidColorBrush(Colors.Transparent);
            colorPoint = new SolidColorBrush(Colors.Black);
        }

        public void LoadMap(Carte plan)
        {
            carteUI.Children.Clear();
            DisplayMap(plan);
        }

        public void LoadDeliveries(DemandeDeLivraisons demandeLivraisons)
        {
            livraisonUI.Children.Clear();
            DisplayEntrepot(demandeLivraisons.entrepot);
            DisplayDeliveries(demandeLivraisons.livraisons);
        }

        public async void LoadWay(Tournee t)
        {
            cheminUI.Children.Clear();
            Chemin chemin;
            foreach (var livraison in t.livraisons)
            {
                t.Hashchemin.TryGetValue(livraison, out chemin);
                DisplayWay(chemin.getTronconList());
                await Task.Delay(1000);
            }
            DisplayWay(t.Hashchemin[t.entrepot].getTronconList());
        }

        private void MapView_SizeChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //this.Display();
        }

        private void DisplayMap(Carte plan)
        {
            double X1, Y1, X2, Y2;
            minX = plan.minX;
            minY = plan.minY;
            rX = map.ActualWidth / (plan.maxX - plan.minX);
            rY = map.ActualHeight / (plan.maxY - plan.minY);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (var troncon in plan.troncons)
            {
                Line line = new Line();
                Line lineToAim = new Line();

                lineToAim.PointerEntered += Line_PointerEntered;
                lineToAim.PointerExited += Line_PointerExited;

                X1 = getX(troncon.Value.origine.x, minX, rX);
                Y1 = getY(troncon.Value.origine.y, minY, rY);
                X2 = getX(troncon.Value.destination.x, minX, rX);
                Y2 = getY(troncon.Value.destination.y, minY, rY);

                line.X1 = X1;
                line.Y1 = Y1;
                line.X2 = X2;
                line.Y2 = Y2;
                lineToAim.X1 = X1;
                lineToAim.Y1 = Y1;
                lineToAim.X2 = X2;
                lineToAim.Y2 = Y2;

                ToolTip tt = new ToolTip();
                tt.Content = "Nom rue: " + troncon.Value.rue;
                lineToAim.SetValue(ToolTipService.ToolTipProperty, tt);

                line.Stroke = colorMap;
                lineToAim.Stroke = colorAim;
                line.StrokeThickness = 2;
                lineToAim.StrokeThickness = 20;
                
                carteUI.Children.Add(line);
                carteUI.Children.Add(lineToAim);
            }

            foreach (var point in plan.points)
            {
                Ellipse circle = new Ellipse();
                Ellipse circleToAim = new Ellipse();

                circleToAim.PointerEntered += CircleToAim_PointerEntered;
                circleToAim.PointerExited += CircleToAim_PointerExited;

                ToolTip tt = new ToolTip();
                tt.Content = "id : " + point.Value.id + "\n x : " + point.Value.x + "\n y : " + point.Value.y;
                circleToAim.SetValue(ToolTipService.ToolTipProperty, tt);

                circle.Height = TAILLE_POINTS;
                circleToAim.Height = TAILLE_POINTS_FAKE;
                circle.Width = TAILLE_POINTS;
                circleToAim.Width = TAILLE_POINTS_FAKE;

                circle.Fill = colorPoint;
                circleToAim.Fill = colorAim;

                X1 = getX(point.Value.x, minX, rX);
                Y1 = getY(point.Value.y, minY, rY);

                Canvas.SetTop(circle, Y1 - TAILLE_POINTS / 2);
                Canvas.SetLeft(circle, X1 - TAILLE_POINTS / 2);

                Canvas.SetTop(circleToAim, Y1 - TAILLE_POINTS_FAKE / 2);
                Canvas.SetLeft(circleToAim, X1 - TAILLE_POINTS_FAKE / 2);

                carteUI.Children.Add(circle);
                carteUI.Children.Add(circleToAim);
            }

            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            string elapsedTime = "RunTime " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Debug.WriteLine(elapsedTime);
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

                line.StrokeThickness = TAILLE_LIGNE_CHEMIN;

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

        private void Line_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Line line = sender as Line;
            ToolTip tt = line.GetValue(ToolTipService.ToolTipProperty) as ToolTip;
            tt.IsOpen = true;
        }

        private void Line_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Line line = sender as Line;
            ToolTip tt = line.GetValue(ToolTipService.ToolTipProperty) as ToolTip;
            tt.IsOpen = false;
        }

        private void CircleToAim_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Ellipse circle = sender as Ellipse;
            ToolTip tt = circle.GetValue(ToolTipService.ToolTipProperty) as ToolTip;
            tt.Placement = Windows.UI.Xaml.Controls.Primitives.PlacementMode.Left;
            tt.IsOpen = true; //or false?!
        }

        private void CircleToAim_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Ellipse circle = sender as Ellipse;
            ToolTip tt = circle.GetValue(ToolTipService.ToolTipProperty) as ToolTip;
            tt.IsOpen = false;
        }




        private static double getX(double X, double minX, double rX)
        {
            return (X - minX) * rX;
        }

        private static double getY(double Y, double minY, double rY)
        {
            return (Y - minY) * rY;
        }
        public async void DisplayDelivery(Livraison point)
        {
            var source = new BitmapImage();
            var rass = RandomAccessStreamReference.CreateFromUri(new Uri(this.BaseUri, "/Assets/pointeur_entrepot.png"));

            IRandomAccessStream stream = await rass.OpenReadAsync();
            IRandomAccessStream stream2 = stream.CloneStream();

            source.SetSource(stream2);

            var decoder = await BitmapDecoder.CreateAsync(stream);
            var size = new BitmapSize { Width = decoder.PixelWidth, Height = decoder.PixelHeight };

            var recenterY = -size.Height;
            var recenterX = -size.Width / 2;

            Image intersection = new Image();
            intersection.Source = source;
            double top = getY(point.adresse.y, minY, rY) + recenterY;
            double left = getX(point.adresse.x, minX, rX) + recenterX;
            Canvas.SetTop(intersection, top);
            Canvas.SetLeft(intersection, left);
            livraisonUI.Children.Add(intersection);
        }
    }
}
