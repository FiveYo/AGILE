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
         /*
	     * Methode devant etre redefinie par les sous-classes de TemplateTSP
	     * @param sommetCrt
	     * @param nonVus : tableau des sommets restant a visiter
	     * @param cout : cout[i][j] = duree pour aller de i a j, avec 0 <= i < nbSommets et 0 <= j < nbSommets
	     * @param duree : duree[i] = duree pour visiter le sommet i, avec 0 <= i < nbSommets
	     * @return un iterateur permettant d'iterer sur tous les sommets de nonVus
	     */
        override protected IIterator<int> iterator(int sommetCrt, List<int> nonVus, int[,] cout, int[] duree)
        {
            return new IteratorSeq(nonVus, sommetCrt, cout, duree);
        }

       /*
       * Methode devant etre redefinie par les sous-classes de TemplateTSP
       * @param sommetCourant
       * @param nonVus : tableau des sommets restant a visiter
       * @param cout : cout[i][j] = duree pour aller de i a j, avec 0 <= i < nbSommets et 0 <= j < nbSommets
       * @param duree : duree[i] = duree pour visiter le sommet i, avec 0 <= i < nbSommets
       * @return une borne inferieure du cout des permutations commencant par sommetCourant, 
       * contenant chaque sommet de nonVus exactement une fois et terminant par le sommet 0
       */
        override protected int bound(int sommetCourant, List<int> nonVus, int[,] cout, int[] duree)
        {
            List<int> listeCout = new List<int>();
            for(int i=0; i<nonVus.Count; i++)
            {
                listeCout.Add(cout[sommetCourant, i] + duree[i]);
            }
            if(listeCout.Count != 0)
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
