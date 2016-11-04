using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library.PatternUndoRedo
{
    public class ModifyTournee
    {
        public ActionType action;
        public Livraison livraison;
        public Modele.Carte carte;
        public int index;
    }
    public enum ActionType
    {
        modifier,
        supprimer,
        ajouter
    }
}
