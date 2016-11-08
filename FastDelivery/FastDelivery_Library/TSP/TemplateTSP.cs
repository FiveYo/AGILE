using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using System.Diagnostics;

namespace FastDelivery_Library
{
    
    public abstract class TemplateTSP : TSP
    {

        public DateTime heurePassage;
        public DateTime heureDepart;
        public int[] meilleureSolution;
        private int coutMeilleureSolution = 0;
        private Boolean tempsLimiteAtteint;
        private DemandeDeLivraisons demande;

        public Boolean getTempsLimiteAtteint()
        {
            return tempsLimiteAtteint;
        }

        public void chercheSolution(TimeSpan tpsLimite, int nbSommets, int[,] cout, int[] duree, DemandeDeLivraisons demandeLiv)
        {

            tempsLimiteAtteint = false;
            demande = demandeLiv;
            heureDepart = DateTime.Parse(demande.entrepot.heureDepart);
            heurePassage = heureDepart;
            coutMeilleureSolution = int.MaxValue;
            meilleureSolution = new int [nbSommets];
            List<int> nonVus = new List<int>();
            for (int i = 1; i < nbSommets; i++) nonVus.Add(i);
            List<int> vus = new List<int>(nbSommets);
            vus.Add(0); // le premier sommet visite est 0
            branchAndBound(0, nonVus, vus, 0, cout, duree, DateTime.Now, tpsLimite);
        }

        public int? getMeilleureSolution(int i)
        {
            if ((meilleureSolution == null) || (i < 0) || (i >= meilleureSolution.Length))
                return null  ;
            return meilleureSolution[i];
        }

        public int getCoutMeilleureSolution()
        {
            return coutMeilleureSolution;
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
        protected abstract int bound(int sommetCourant, List<int> nonVus, int[,] cout, int[] duree, DemandeDeLivraisons demande);

        /*
	     * Methode devant etre redefinie par les sous-classes de TemplateTSP
	     * @param sommetCrt
	     * @param nonVus : tableau des sommets restant a visiter
	     * @param cout : cout[i][j] = duree pour aller de i a j, avec 0 <= i < nbSommets et 0 <= j < nbSommets
	     * @param duree : duree[i] = duree pour visiter le sommet i, avec 0 <= i < nbSommets
	     * @return un iterateur permettant d'iterer sur tous les sommets de nonVus
	     */
        protected abstract IIterator<int> iterator(int sommetCrt, List<int> nonVus, int[,] cout, int[] duree);

        /*
	     * Methode definissant le patron (template) d'une resolution par separation et evaluation (branch and bound) du TSP
	     * @param sommetCrt le dernier sommet visite
	     * @param nonVus la liste des sommets qui n'ont pas encore ete visites
	     * @param vus la liste des sommets visites (y compris sommetCrt)
	     * @param coutVus la somme des couts des arcs du chemin passant par tous les sommets de vus + la somme des duree des sommets de vus
	     * @param cout : cout[i][j] = duree pour aller de i a j, avec 0 <= i < nbSommets et 0 <= j < nbSommets
	     * @param duree : duree[i] = duree pour visiter le sommet i, avec 0 <= i < nbSommets
	     * @param tpsDebut : moment ou la resolution a commence
	     * @param tpsLimite : limite de temps pour la resolution
	     */
        void branchAndBound(int sommetCrt, List<int> nonVus, List<int> vus, int coutVus, int[,] cout, int[] duree, DateTime tpsDebut, TimeSpan tpsLimite)
        {
            if (sommetCrt == 0)
            {
                var currentLivraison = demande.entrepot;
            }
            else
            {
                var currentLivraison = demande.livraisons[sommetCrt];
            }
            
            if (DateTime.Now - tpsDebut > tpsLimite)
            {
                tempsLimiteAtteint = true;
                return;
            }
            if (nonVus.Count == 0)
            { // tous les sommets ont ete visites
                coutVus += cout[sommetCrt,0];

                //there test si on arrive avant la plage de debut, ajouter la diff entre temsp arrivee et plage debut
                DateTime heuretemp = heurePassage + new TimeSpan(0, 0, coutVus);
                if (sommetCrt!=0)
                {
                    var currentLivraison = demande.livraisons[sommetCrt];
                    if (currentLivraison.planifier)
                    {
                        DateTime DebutPlage = DateTime.Parse(currentLivraison.debutPlage);
                        if (heurePassage.CompareTo(DebutPlage) < 0)
                        {
                            coutVus += ((TimeSpan)DebutPlage.Subtract(heurePassage)).Seconds;
                        }
                    }
                }

                if (coutVus < coutMeilleureSolution) 
                { // on a trouve une solution meilleure que meilleureSolution
                    meilleureSolution = vus.ToArray<int>();
                    coutMeilleureSolution = coutVus;
                    

                }
            }
            else if (coutVus + bound(sommetCrt, nonVus, cout, duree, demande) < coutMeilleureSolution)
            {
                
                IIterator<int> it = iterator(sommetCrt, nonVus, cout, duree);
                while (it.MoveNext())
                {
                    int prochainSommet=it.Current; 
                    vus.Add(prochainSommet);
                    nonVus.Remove(prochainSommet);

                    if (prochainSommet != 0)
                    {
                        var currentLivraison = demande.livraisons[prochainSommet];
                        if (currentLivraison.planifier)
                        {
                            DateTime FinPlage = DateTime.Parse(currentLivraison.finPlage);
                            DateTime heureprevu = heurePassage + new TimeSpan(0, 0, cout[0, prochainSommet] + duree[prochainSommet]);
                            if (heureprevu.CompareTo(FinPlage) < 0)
                            {
                                heurePassage += new TimeSpan(0, 0, coutVus) + new TimeSpan(0, 0, currentLivraison.duree);
                                branchAndBound(prochainSommet, nonVus, vus, coutVus + cout[sommetCrt, prochainSommet] + duree[prochainSommet], cout, duree, tpsDebut, tpsLimite);
                                vus.Remove(prochainSommet);
                                nonVus.Add(prochainSommet);
                            }
                        }
                        else
                        {
                            heurePassage += new TimeSpan(0, 0, coutVus) + new TimeSpan(0, 0, currentLivraison.duree);
                            branchAndBound(prochainSommet, nonVus, vus, coutVus + cout[sommetCrt, prochainSommet] + duree[prochainSommet], cout, duree, tpsDebut, tpsLimite);
                            vus.Remove(prochainSommet);
                            nonVus.Add(prochainSommet);
                        }
                    }
                    else
                    {
                        branchAndBound(prochainSommet, nonVus, vus, coutVus + cout[sommetCrt, prochainSommet] + duree[prochainSommet], cout, duree, tpsDebut, tpsLimite);
                        vus.Remove(prochainSommet);
                        nonVus.Add(prochainSommet);
                    }
                }
            }
        }

            int TSP.getMeilleureSolution(int i)
            {
                throw new NotImplementedException();
            }
     }


}
