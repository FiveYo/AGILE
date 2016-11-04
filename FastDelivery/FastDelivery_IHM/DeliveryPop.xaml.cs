using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Boîte de dialogue de contenu, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace FastDelivery_IHM
{
    public sealed partial class DeliveryPop : ContentDialog
    {
        public string idLiv
        {
            get
            {
                return id.Text;
            }
        }

        public string idPointLiv
        {
            get
            {
                return idPoint.Text;
            }
        }

        public string dureeLiv
        {
            get
            {
                return duree.Text;
            }
        }
        public DeliveryPop()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
        

        private void id_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tmp = sender as TextBox;
            if (!Regex.IsMatch(tmp.Text, "^\\d*$") && tmp.Text != "")
            {
                int pos = tmp.SelectionStart - 1;
                tmp.Text = tmp.Text.Remove(pos, 1);
                tmp.SelectionStart = pos;
            }
        }

        private void idPoint_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tmp = sender as TextBox;
            if (!Regex.IsMatch(tmp.Text, "^\\d*$") && tmp.Text != "")
            {
                int pos = tmp.SelectionStart - 1;
                tmp.Text = tmp.Text.Remove(pos, 1);
                tmp.SelectionStart = pos;
            }
        }

        private void duree_TextChanged(object sender, TextChangedEventArgs e)
        {
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
    }
}
