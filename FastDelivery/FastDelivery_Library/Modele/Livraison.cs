using FastDelivery_Library.Modele;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library.Modele
{
    /// <summary>
    /// Contient la durée, l'adresse d'une livraison. Si celle-ci est planifié contient la plage horaire
    /// </summary>
    public class Livraison : Lieu, INotifyPropertyChanged
    {
        public int duree;
        private DateTime _debutPlage;
        private DateTime _finPlage;

        public DateTime debutPlage {
            get
            {
                return _debutPlage;
            }
            set
            {
                if (value != this.debutPlage)
                {
                    _debutPlage = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public DateTime finPlage
        {
            get
            {
                return _finPlage;
            }
            set
            {
                if (value != this.finPlage)
                {
                    _finPlage = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool planifier { get; set; }
        public Point adresse { get; set; }

        public DateTime HeureDePassage;

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Livraison(Point adresse , int duree)
        {
            planifier = false;
            this.adresse = adresse;
            this.duree = duree;
        }
        
        public void SetPlage(DateTime debut, DateTime fin)
        {
            debutPlage = debut;
            finPlage = fin;
            planifier = true;
        }

        public void SetHeureDePassage(DateTime heurePassage)
        {
            HeureDePassage = heurePassage;
        }
    }
}
