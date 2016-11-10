using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library.Modele
{
    /// <summary>
    /// Interface permettant de lier l'adresse d'entrepot et de livraison
    /// </summary>
    public interface Lieu
    {
        Point adresse { get; set; }
    }
}
