using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Collections;

namespace FastDelivery_Library
{
    /// <summary>
    /// Classe de l'itérateur utilisé dans le TSP
    /// </summary>
    public class IteratorSeq : IIterator<int>
    {
        /// <summary>
        /// Définit le tableau de sommets
        /// </summary>
        private int[] candidats;

        /// <summary>
        /// Le nombre de sommet restant
        /// </summary>
        private int nbCandidats;
        /// <summary>
        /// renvoie le sommet actuel
        /// </summary>
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

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="sommetCrt"></param>
        /// <param name="nonVus" >"nonVus : tableau des sommets restant a visiter"></param>
        /// <param name="cout" >"cout[i][j] = duree pour aller de i a j, avec 0 <= i < nbSommets et 0 <= j < nbSommets"></param>
        /// <param name="duree" >"duree[i] = duree pour visiter le sommet i, avec 0 <= i < nbSommets"></param>
        /// <returns>Renvoie un iterateur permettant d'iterer sur tous les sommets de nonVus</returns>
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

        /// <summary>
        /// Fait avancer l'itérateur
        /// </summary>
        /// <returns>REnvoie la valeur du prochain sommer en décrémentant ceux vus</returns>
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