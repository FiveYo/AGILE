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
    public sealed partial class ChangePlagePop : ContentDialog
    {
        public bool continu
        {
            get; set;
        }

        public TimeSpan debutPlage
        {
            get
            {
                return startPlage.Time;
            }

        }

        public TimeSpan finPlage
        {
            get
            {
                return endPlage.Time;

            }
        }

        public ChangePlagePop()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            continu = false;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if(errorPlage.Visibility == Visibility.Visible)
            {

            }
            else
            {
                continu = true;
            }
            
        }




        private void TimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            TimePicker d = sender as TimePicker;
            if (endPlage.Time < startPlage.Time)
            {
                errorPlage.Visibility = Visibility.Visible;
                IsSecondaryButtonEnabled = false;
            }
            else
            {
                errorPlage.Visibility = Visibility.Collapsed;
                IsSecondaryButtonEnabled = true;
            }
        }
    }

}
