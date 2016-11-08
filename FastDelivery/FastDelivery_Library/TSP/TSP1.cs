using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace FastDelivery_Library
{

    public class TSP1 : TemplateTSP
    {
   
    
        override protected IIterator<int> iterator(int sommetCrt, List<int> nonVus, int[,] cout, int[] duree)
        {
            return new IteratorSeq(nonVus, sommetCrt,cout,duree);
        }

        override protected int bound(int sommetCourant, List<int> nonVus, int[,] cout, int[] duree, DemandeDeLivraisons demande)
        {
            List<int> listeCout = new List<int>();
            DateTime HeureEntrepot = DateTime.Parse(demande.entrepot.heureDepart);
            TimeSpan t = new TimeSpan(0, 0, cout[0, sommetCourant]);
            DateTime heureDepart = HeureEntrepot + t;
            int i = 0;
            foreach (int s in nonVus)
            {
                
                if (demande.livraisons[s].planifier && s!=0)
                {
                    DateTime finHoraire = DateTime.Parse(demande.livraisons[s].finPlage);
                    DateTime debutHoraire = DateTime.Parse(demande.livraisons[s].debutPlage);
                    if ((heureDepart + new TimeSpan(0, 0, cout[sommetCourant, i] + duree[i]) <= finHoraire) && (heureDepart + new TimeSpan(cout[sommetCourant, i] + duree[i]) >= debutHoraire) && (demande.livraisons[s].planifier))
                    {
                        listeCout.Add(cout[sommetCourant, i] + duree[i]);
                    }
                    i++;
                }
                else
                {
                    listeCout.Add(cout[sommetCourant, i] + duree[i]);
                    i++;
                }
            } 
            if (listeCout.Count != 0)
            {
                return listeCout.Min();
            }
            else
            {
                return 0;
            }
        }
    }

}
