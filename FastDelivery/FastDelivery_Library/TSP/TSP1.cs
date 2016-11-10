using FastDelivery_Library.Modele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace FastDelivery_Library
{
    /// <summary>
    /// Classe principale de TSP
    /// </summary>
    public class TSP1 : TemplateTSP
    {


        override protected IIterator<int> iterator(int sommetCrt, List<int> nonVus, int[,] cout, int[] duree)
        {
            return new IteratorSeq(nonVus, sommetCrt, cout, duree);
        }
        override protected int bound(int sommetCourant, List<int> nonVus, int[,] cout, int[] duree, DemandeDeLivraisons truc, DateTime truc2)
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
    }
}