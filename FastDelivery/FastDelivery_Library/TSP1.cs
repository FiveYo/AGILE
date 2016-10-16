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
        override protected IIterator<int> iterator(int sommetCrt, List<int> nonVus, int[][] cout, int[] duree)
    {
        return new IteratorSeq(nonVus, sommetCrt);
    }

        override protected int bound(int sommetCourant, List<int> nonVus, int[][] cout, int[] duree)
    {
        return 0;
    }
}

}
