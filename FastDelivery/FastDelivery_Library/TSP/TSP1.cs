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
        override protected int bound(int sommetCourant, List<int> nonVus, int[,] cout, int[] duree,DemandeDeLivraisons truc, DateTime truc2)
        {
            List<int> listeCout = new List<int>();
            for (int i = 0; i < nonVus.Count; i++)
            {
                listeCout.Add(cout[sommetCourant, i] + duree[i]);
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
        //override protected int bound(int sommetCourant, List<int> nonVus, int[,] cout, int[] duree, DemandeDeLivraisons demande,DateTime heurepassage)
        //{
        //    List<int> listeCout = new List<int>();
        //    TimeSpan t = new TimeSpan(0, 0, cout[0, sommetCourant]);
        //    int i = 0;
        //    foreach (int s in nonVus)
        //    {
        //        if (i != sommetCourant)
        //        {
        //            if (demande.livraisons[s].planifier && s != 0)
        //            {
        //                DateTime finHoraire = DateTime.Parse(demande.livraisons[s].finPlage);
        //                DateTime debutHoraire = DateTime.Parse(demande.livraisons[s].debutPlage);
        //                DateTime test1 = heurepassage + new TimeSpan(0, 0, cout[sommetCourant, i] + duree[i]);
        //                if ((test1 <= finHoraire) && (test1 >= debutHoraire))
        //                {
        //                    listeCout.Add(cout[sommetCourant, i] + duree[i]);
        //                }
        //                i++;
        //            }
        //            else
        //            {
        //                listeCout.Add(cout[sommetCourant, i] + duree[i]);
        //                i++;
        //            }
        //        }
        //    } 
        //    if (listeCout.Count != 0)
        //    {
        //        return listeCout.Min();
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}
    }

}
