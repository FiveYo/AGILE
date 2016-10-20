﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    /// <summary>
    /// Contient la durée, l'adresse d'une livraison. Si celle-ci est planifié contient la plage horaire
    /// </summary>
    public class Livraison
    {
        public int duree;
        public string debutPlage, finPlage;
        public bool planifier = false;
        public Point adresse;

        public Livraison(Point adresse , int duree)
        {
            this.adresse = adresse;
            this.duree = duree;
        }
        
        public void SetPlage(string debut, string fin)
        {
            debutPlage = debut;
            finPlage = fin;
            planifier = true;
        }
    }
}