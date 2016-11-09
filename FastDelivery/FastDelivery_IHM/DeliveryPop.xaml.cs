using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using FastDelivery_Library.Modele;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;

// Pour plus d'informations sur le modèle d'élément Boîte de dialogue de contenu, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace FastDelivery_IHM
{
    public sealed partial class DeliveryPop : ContentDialog
    {
        /// <summary>
        /// Contient le lieu qui précède la livraison que l'on va créé
        /// </summary>
        internal Lieu lieu
        {
            get;
            set;
        }

        private Point _point;
        internal bool planifier
        {
            get
            {
                return planifierUI.IsChecked == true;
            }
        }

        /// <summary>
        /// Correspond à l'adresse de la nouvelle livraison
        /// </summary>
        public Point adresse
        {
            get
            {
                return _point;
            }
            set
            {
                _point = value;
                infoPoint.Text = "(" + _point.x + ", " + _point.y + ")";
            }
        }

        /// <summary>
        /// Informe si l'utilisateur a annulé l'ajout de la livraison ou non
        /// </summary>
        public bool continu
        {
            get; set;
        }

        public int duree
        {
            get
            {
                return int.Parse(dureeLiv.Text);
            }
        }

        public Brush ColorTimePickerEnable;
        public Brush ColorTimePickerDisable;

        public DeliveryPop()
        {
            this.InitializeComponent();
            ColorTimePickerEnable = start.Background;
            ColorTimePickerDisable = new SolidColorBrush(Colors.Gray);

        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            continu = false;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if(dureeLiv.Text == "")
            {
                args.Cancel = true;
                errorDuree.Visibility = Visibility.Visible;
                return;
            }
            else if (planifier && end.Time < start.Time)
            {
                args.Cancel = true;
                end.Focus(FocusState.Pointer);
                return;
            }
            continu = true;
        }
        private void duree_TextChanged(object sender, TextChangedEventArgs e)
        {
            errorDuree.Visibility = Visibility.Collapsed;
            TextBox tmp = sender as TextBox;
            if (!Regex.IsMatch(tmp.Text, "^\\d*$") && tmp.Text != "")
            {
                int pos = tmp.SelectionStart - 1;
                tmp.Text = tmp.Text.Remove(pos, 1);
                tmp.SelectionStart = pos;
            }
        }

        private void TimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            TimePicker d = sender as TimePicker;
            if (d.Time < start.Time)
            {
                error.Visibility = Visibility.Visible;
            }
            else
            {
                error.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            continu = true;
            Hide();
        }

        private void planifierUI_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked != true)
            {
                start.IsHitTestVisible = false;
                start.Background = ColorTimePickerDisable;
                end.IsHitTestVisible = false;
                end.Background = ColorTimePickerDisable;
                error.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (end.Time < start.Time)
                {
                    error.Visibility = Visibility.Visible;
                }
                start.IsHitTestVisible = true;
                start.Background = ColorTimePickerEnable;
                end.IsHitTestVisible = true;
                end.Background = ColorTimePickerEnable;
            }
        }
    }
}
