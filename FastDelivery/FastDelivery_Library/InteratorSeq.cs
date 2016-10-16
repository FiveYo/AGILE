using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;

namespace FastDelivery_Library
{
    
    public class IteratorSeq : IIterator<int> {


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
        public IteratorSeq(ICollection<int> nonVus, int sommetCrt)
        {
            this.candidats = new int[nonVus.Count()];
            nbCandidats = 0;
            foreach (int s in nonVus)
            {
                candidats[nbCandidats++] = s;
            }
        }
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
            if (nbCandidats < candidats.Length)
            {
                nbCandidats++;
                return true;
            }
            return false;
        }
    }



}
