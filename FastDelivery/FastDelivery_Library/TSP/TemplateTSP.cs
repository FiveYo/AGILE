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

        public DateTime heureDepart;
        public int[] meilleureSolution;
        private int coutMeilleureSolution = 0;
        private Boolean tempsLimiteAtteint;
        private DemandeDeLivraisons demande;
        public  List<DateTime> meilleurshoraires;
        public List<TimeSpan> meilleurtempsattente;
        

        public Boolean getTempsLimiteAtteint()
        {
            return tempsLimiteAtteint;
        }

        public void chercheSolution(TimeSpan tpsLimite, int nbSommets, int[,] cout, int[] duree, DemandeDeLivraisons demandeLiv)
        {

            tempsLimiteAtteint = false;
            demande = demandeLiv;
            heureDepart = DateTime.Parse(demande.entrepot.heureDepart);
            coutMeilleureSolution = int.MaxValue;
            meilleureSolution = new int [nbSommets];
            List<int> nonVus = new List<int>();
            for (int i = 1; i < nbSommets; i++) nonVus.Add(i);
            List<int> vus = new List<int>(nbSommets);
            List<DateTime> horaires = new List<DateTime>(nbSommets);
            List<TimeSpan> tempsattentes = new List<TimeSpan>(nbSommets);
            meilleurshoraires = new List<DateTime>(nbSommets);
            meilleurtempsattente = new List<TimeSpan>(nbSommets);
            vus.Add(0); // le premier sommet visite est 0
            horaires.Add(heureDepart);
            branchAndBound(0, nonVus, vus, 0, cout, duree, DateTime.Now, tpsLimite,horaires, tempsattentes,heureDepart);
        }

        public int? getMeilleureSolution(int i)
        {
            if ((meilleureSolution == null) || (i < 0) || (i >= meilleureSolution.Length))
                return null  ;
            return meilleureSolution[i];
        }
        public DateTime? getMeilleurHoraire(int i)
        {
            if ((meilleurshoraires == null) || (i < 0) || (i >= meilleurshoraires.Count)) return null;
            return meilleurshoraires[i];
        }
        public TimeSpan? getMeilleurTempsAttente(int i)
        {
            if ((meilleurtempsattente == null) || (i < 0) || (i >= meilleurtempsattente.Count)) return null;
            return meilleurtempsattente[i];
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
        protected abstract int bound(int sommetCourant, List<int> nonVus, int[,] cout, int[] duree, DemandeDeLivraisons demande,DateTime heuredepassage);

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
        void branchAndBound(int sommetCrt, List<int> nonVus, List<int> vus, int coutVus, int[,] cout, int[] duree, DateTime tpsDebut, TimeSpan tpsLimite,List<DateTime> horaires,List<TimeSpan> tempsattente,DateTime heure)
        {
            TimeSpan tempsdattente;
            DateTime lastheuredepart = heure;
           
            int idLivraison = sommetCrt;
            if (sommetCrt == 0)
            {
                var currentLivraison = demande.entrepot;
                lastheuredepart = heureDepart;
               
            }

            if (DateTime.Now - tpsDebut > tpsLimite)
            {
                tempsLimiteAtteint = true;
                return;
            }
            if (nonVus.Count == 0)
            { // tous les sommets ont ete visites
               
                if (sommetCrt!=0)
                {
                    
                    var currentLivraison = demande.livraisons[idLivraison];
                    if (currentLivraison.planifier)
                    {
                        DateTime DebutPlage = DateTime.Parse(currentLivraison.debutPlage);
                        if (lastheuredepart.CompareTo(DebutPlage) <= 0)
                        {
                            coutVus += cout[sommetCrt, 0]+(int)((TimeSpan)(DebutPlage.Subtract(lastheuredepart))).TotalSeconds+currentLivraison.duree;
                        }
                    }
                    else
                    {
                        coutVus += cout[sommetCrt, 0] + currentLivraison.duree;
                    }
                }             

                if (coutVus < coutMeilleureSolution) 
                { // on a trouve une solution meilleure que meilleureSolution
                    meilleureSolution = vus.ToArray<int>();
                    coutMeilleureSolution = coutVus;
                    meilleurshoraires = new List<DateTime>(horaires);
                    meilleurtempsattente = new List<TimeSpan>();
                    lastheuredepart = heureDepart; //AVEC OU SANS
                }
            }
            else if (coutVus + bound(sommetCrt, nonVus, cout, duree, demande, lastheuredepart) < coutMeilleureSolution)
            {
                
                IIterator<int> it = iterator(sommetCrt, nonVus, cout, duree);
                while (it.MoveNext())
                {
                    DateTime newheurepassage;
                    int prochainSommet=it.Current;
                    vus.Add(prochainSommet);
                    nonVus.Remove(prochainSommet);
                    if (sommetCrt != 0)
                    {
                        var currentLivraison = demande.livraisons[idLivraison];
                        var nextLivraison = demande.livraisons[prochainSommet];
                        
                        if (nextLivraison.planifier && currentLivraison.planifier)
                        {
                            DateTime nextFinPlage = DateTime.Parse(nextLivraison.finPlage);
                            DateTime nextDebutPlage = DateTime.Parse(nextLivraison.debutPlage);
                            DateTime currentDebutPlage = DateTime.Parse(currentLivraison.debutPlage);
                            DateTime heureprevu = lastheuredepart  +new TimeSpan(0, 0, cout[sommetCrt, prochainSommet] + duree[sommetCrt]);
                            //je crée mon temps d'attente!
                            if (lastheuredepart <= currentDebutPlage)
                            {
                                currentLivraison.tempsAttente = (TimeSpan)(currentDebutPlage.Subtract(lastheuredepart));
                            }
                            else
                            {
                                currentLivraison.tempsAttente = new TimeSpan(0, 0, 0);
                            }
                            //c'est fait!
                            if (((heureprevu + currentLivraison.tempsAttente)).CompareTo(nextFinPlage) <= 0)
                            {
                                tempsdattente = currentLivraison.tempsAttente;
                                newheurepassage = heureprevu + currentLivraison.tempsAttente;
                                horaires.Add(newheurepassage);
                                tempsattente.Add(tempsdattente);
                                branchAndBound(prochainSommet, nonVus, vus, coutVus + cout[sommetCrt, prochainSommet] + (int)((TimeSpan)(currentDebutPlage.Subtract(lastheuredepart))).TotalSeconds + currentLivraison.duree, cout, duree, tpsDebut, tpsLimite, horaires, tempsattente, newheurepassage);
                                horaires.Remove(newheurepassage);
                                tempsattente.Remove(tempsdattente);
                            }
                            else
                            {
                            }
                            
                        }
                        else if(nextLivraison.planifier && !currentLivraison.planifier)
                        {
                            DateTime heureprevu = lastheuredepart + new TimeSpan(0, 0, cout[sommetCrt, prochainSommet] + duree[sommetCrt]);
                            DateTime nextFinPlage = DateTime.Parse(nextLivraison.finPlage);
                            if ((heureprevu).CompareTo(nextFinPlage) <= 0)
                            {
                                tempsdattente = currentLivraison.tempsAttente;
                                newheurepassage = heureprevu;
                                horaires.Add(newheurepassage);
                                tempsattente.Add(tempsdattente);
                                branchAndBound(prochainSommet, nonVus, vus, coutVus + cout[sommetCrt, prochainSommet] + currentLivraison.duree, cout, duree, tpsDebut, tpsLimite, horaires, tempsattente,newheurepassage);
                                horaires.Remove(newheurepassage);
                                tempsattente.Remove(tempsdattente);
                            }
                        }
                        else if(!nextLivraison.planifier && currentLivraison.planifier)
                        {
                            DateTime heureprevu = lastheuredepart + new TimeSpan(0, 0, cout[sommetCrt, prochainSommet] + duree[sommetCrt]);
                            DateTime currentDebutPlage = DateTime.Parse(currentLivraison.debutPlage);
                            //je crée mon temps d'attente!
                            if (lastheuredepart <= currentDebutPlage)
                            {
                                currentLivraison.tempsAttente = (TimeSpan)(currentDebutPlage.Subtract(lastheuredepart));
                            }
                            else
                            {
                                currentLivraison.tempsAttente = new TimeSpan(0, 0, 0);
                            }
                            //c'est fait!
                            tempsdattente = currentLivraison.tempsAttente;
                            newheurepassage = heureprevu + currentLivraison.tempsAttente;
                            horaires.Add(newheurepassage);
                            tempsattente.Add(tempsdattente);
                            branchAndBound(prochainSommet, nonVus, vus, coutVus + cout[sommetCrt, prochainSommet] + (int)((TimeSpan)(currentDebutPlage.Subtract(lastheuredepart))).TotalSeconds + currentLivraison.duree, cout, duree, tpsDebut, tpsLimite, horaires, tempsattente,newheurepassage);
                            horaires.Remove(newheurepassage);
                            tempsattente.Remove(tempsdattente);
                        }
                        else
                        {
                            tempsdattente = currentLivraison.tempsAttente;
                            DateTime heureprevu = lastheuredepart + new TimeSpan(0, 0, cout[sommetCrt, prochainSommet] + duree[sommetCrt]);
                            newheurepassage = lastheuredepart.Add(new TimeSpan(0, 0, cout[sommetCrt, prochainSommet] + duree[sommetCrt]));
                            horaires.Add(newheurepassage);
                            tempsattente.Add(tempsdattente);
                            branchAndBound(prochainSommet, nonVus, vus, coutVus + cout[sommetCrt, prochainSommet] + currentLivraison.duree, cout, duree, tpsDebut, tpsLimite, horaires, tempsattente,newheurepassage);
                            horaires.Remove(newheurepassage);
                            tempsattente.Remove(tempsdattente);
                        }
                    }
                    else
                    {
                        newheurepassage = lastheuredepart.Add(new TimeSpan(0, 0, cout[sommetCrt, prochainSommet]));
                        horaires.Add(newheurepassage);
                        branchAndBound(prochainSommet, nonVus, vus, coutVus + cout[sommetCrt, prochainSommet], cout, duree, tpsDebut, tpsLimite, horaires, tempsattente,newheurepassage);
                        horaires.Remove(newheurepassage);
                    }
                    vus.Remove(prochainSommet);
                    nonVus.Add(prochainSommet);
                }
            }
        }

            int TSP.getMeilleureSolution(int i)
            {
                throw new NotImplementedException();
            }
     }


}
