using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;

namespace FastDelivery_Library
{

    public class IteratorSeq : IIterator<int>
    {

        TimeSpan runtime;
        private int[] candidats;
        private int nbCandidats;

        public int Current
        {
            get
            {
                return candidats[nbCandidats];
            }
        }
        public bool HasCurrent
        {
            get
            {
                return nbCandidats > 0;
            }
        }

        int IIterator<int>.Current
        {
            get
            {
                return candidats[nbCandidats];
            }
        }

        bool IIterator<int>.HasCurrent
        {
            get
            {
                return nbCandidats > 0;
            }
        }

        /**
            * Cree un iterateur pour iterer sur l'ensemble des sommets de nonVus
            * @param nonVus
            * @param sommetCrt
            */
        public IteratorSeq(ICollection<int> nonVus, int sommetCrt, int[,] cout, int[] duree)
        {
            List<int> listeCouts = new List<int>();
            foreach (int s in nonVus)
            {
                listeCouts.Add(cout[sommetCrt, s]);
            }
            List<int> listeCoutsTri = new List<int>(listeCouts);
            listeCoutsTri.Sort();
            this.candidats = new int[nonVus.Count()];
            nbCandidats = 0;
            int test = nonVus.Count - 1;
            foreach (int s in nonVus)
            {
                candidats[test--] = nonVus.ElementAt(listeCouts.IndexOf(listeCoutsTri[nbCandidats++]));
            }
        }

        //public IteratorSeq(ICollection<int> nonVus, int sommetCrt, int[,] cout, int[] duree)
        //{
        //    this.candidats = new int[nonVus.Count()];
        //    nbCandidats = 0;
        //    foreach (int s in nonVus)
        //    {
        //        candidats[nbCandidats++] = s;
        //    }
        //}

        public int MoveNext()
        {
            return candidats[--nbCandidats];

        }



        uint IIterator<int>.GetMany(int[] items)
        {
            throw new NotImplementedException();
        }

        bool IIterator<int>.MoveNext()
        {
            if (nbCandidats > 0)
            {
                nbCandidats--;
                return true;
            }
            return false;
        }
    }



}