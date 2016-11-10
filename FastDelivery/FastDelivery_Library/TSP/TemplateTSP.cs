using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Collections;
using FastDelivery_Library.Modele;

namespace FastDelivery_Library
{
    /// <summary>
    /// Classe abstraite dérivant de l'interface TSP
    /// </summary>
    public abstract class TemplateTSP : TSP
    {
        /// <summary>
        /// flag determinant si le calcul doit être stoppé ou pas
        /// </summary>
        public bool stop = false;

        /// <summary>
        /// Heure de Depart de la tournée ( initialisée à celle de l'entrepot)
        /// </summary>
        public DateTime heureDepart;

        /// <summary>
        /// tableaux avec la meilleur solution (tableau d'entier)
        /// </summary>
        public int[] meilleureSolution;

        /// <summary>
        /// Cout de la meilleur solution 
        /// </summary>
        private int coutMeilleureSolution = 0;

        /// <summary>
        /// flag determinant si le temps de calcul maximal prévu a été atteint
        /// </summary>
        private Boolean tempsLimiteAtteint;

        /// <summary>
        /// Liste des livraisons à classer
        /// </summary>
        private DemandeDeLivraisons demande;

        /// <summary>
        /// list des meilleurs horaires ( même odre que la meilleur solution)
        /// </summary>
        public List<DateTime> meilleurshoraires;

        /// <summary>
        /// list des meilleurs tempsd'attente (même odre que le temps d'attente)
        /// </summary>
        public List<TimeSpan> meilleurtempsattente;

        /// <summary>
        /// méthode pour stopper le calcul si le temps de calcul devient trop grand
        /// </summary>
        /// <returns>Renvoie le booleen qui servira a voir si on a dépasser la limite de temps</returns>
        public Boolean getTempsLimiteAtteint()
        {
            return tempsLimiteAtteint;
        }

        /// <summary>
        /// Méthode principale de la classe qui cherchera automatiquement le meilleur coût via l'appel de la fonction brancAndBound
        /// </summary>
        /// <param name="tpsLimite" >"limite de temps pour la resolution"></param>
        /// <param name="nbSommets" >"nombre de sommet présent dans le graphe"></param>
        /// <param name="cout" >"cout[i][j] = duree pour aller de i a j, avec 0 <= i<nbSommets et 0 <= j<nbSommets "></param>
        /// <param name="duree" >" duree[i] = duree pour visiter le sommet i, avec 0 <= i<nbSommets"></param>
        /// <param name="demandeLiv" >"Objet regroupant les livraisons souhaitées"></param>
        public void chercheSolution(TimeSpan tpsLimite, int nbSommets, int[,] cout, int[] duree, DemandeDeLivraisons demandeLiv)
        {

            tempsLimiteAtteint = false;
            demande = demandeLiv;
            heureDepart = demande.entrepot.heureDepart;
            coutMeilleureSolution = int.MaxValue;
            meilleureSolution = new int[nbSommets];
            List<int> nonVus = new List<int>();
            for (int i = 1; i < nbSommets; i++) nonVus.Add(i);
            List<int> vus = new List<int>(nbSommets);
            List<DateTime> horaires = new List<DateTime>(nbSommets);
            List<TimeSpan> tempsattentes = new List<TimeSpan>(nbSommets);
            meilleurshoraires = new List<DateTime>(nbSommets);
            meilleurtempsattente = new List<TimeSpan>(nbSommets);
            vus.Add(0); // le premier sommet visite est 0
            horaires.Add(heureDepart);
            tempsattentes.Add(new TimeSpan(0, 0, 0));
            branchAndBound(0, nonVus, vus, 0, cout, duree, DateTime.Now, tpsLimite, horaires, tempsattentes, heureDepart);
        }

        public int? getMeilleureSolution(int i)
        {
            if ((meilleureSolution == null) || (i < 0) || (i >= meilleureSolution.Length))
                return null;
            return meilleureSolution[i];
        }
        public DateTime? getMeilleurHoraire(int i)
        {
            if ((meilleurshoraires == null) || (i < 0) || (i >= meilleurshoraires.Count)) return null;
            return meilleurshoraires[i];
        }
        public TimeSpan? getmeilleurtempsattente(int i)
        {
            if ((meilleurtempsattente == null) || (i < 0) || (i >= meilleurtempsattente.Count)) return null;
            return meilleurtempsattente[i];
        }
        public int getCoutMeilleureSolution()
        {
            return coutMeilleureSolution;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sommetCourant" >"Sommet courant"></param>
        /// <param name="nonVus" >"tableau des sommets restant a visiter"></param>
        /// <param name="cout" >"cout[i][j] = duree pour aller de i a j, avec 0 <= i < nbSommets et 0 <= j < nbSommets"></param>
        /// <param name="duree" >"duree[i] = duree pour visiter le sommet i, avec 0 <= i < nbSommets"></param>
        /// <param name="demande"></param>
        /// <param name="heuredepassage"></param>
        /// <returns>Renvoies une borne inferieure du cout des permutations commencant par sommetCourant, 
        /// contenant chaque sommet de nonVus exactement une fois et terminant par le sommet 0</returns>
        protected abstract int bound(int sommetCourant, List<int> nonVus, int[,] cout, int[] duree, DemandeDeLivraisons demande, DateTime heuredepassage);


        /// <summary>
        /// Creer un itérateur pour naviguer entre les sommets.
        /// </summary>
        /// <param name="sommetCrt"></param>
        /// <param name="nonVus" >"nonVus : tableau des sommets restant a visiter"></param>
        /// <param name="cout" >"cout[i][j] = duree pour aller de i a j, avec 0 <= i < nbSommets et 0 <= j < nbSommets"></param>
        /// <param name="duree" >"duree[i] = duree pour visiter le sommet i, avec 0 <= i < nbSommets"></param>
        /// <returns>Renvoie un iterateur permettant d'iterer sur tous les sommets de nonVus</returns>
        protected abstract IIterator<int> iterator(int sommetCrt, List<int> nonVus, int[,] cout, int[] duree);


        /// <summary>
        /// méthode principale qui cherchera la meilleur solution en essayant chaque sommet.
        /// </summary>
        /// <param name="sommetCrt" >"Sommet Courant"></param>
        /// <param name="nonVus" >"la liste des sommets qui n'ont pas encore ete visites"></param>
        /// <param name="vus" >"vus la liste des sommets visites (y compris sommetCrt)"></param>
        /// <param name="coutVus" >"la somme des couts des arcs du chemin passant par tous les sommets de vus + la somme des duree des sommets de vus"></param>
        /// <param name="cout" >"cout[i][j] = duree pour aller de i a j, avec 0 <= i < nbSommets et 0 <= j < nbSommets"></param>
        /// <param name="duree" >"duree[i] = duree pour visiter le sommet i, avec 0 <= i < nbSommets"></param>
        /// <param name="tpsDebut" >"moment ou la resolution a commence"></param>
        /// <param name="tpsLimite" >"limite de temps pour la resolution"></param>
        /// <param name="horaires" >"liste de datetimes représentant les différentes heures de passages"></param>
        /// <param name="tempsattente" >"liste de TimeSpan(intervalles d'heures)"></param>
        /// <param name="heure " >"heure"></param>
        void branchAndBound(int sommetCrt, List<int> nonVus, List<int> vus, int coutVus, int[,] cout, int[] duree, DateTime tpsDebut, TimeSpan tpsLimite, List<DateTime> horaires, List<TimeSpan> tempsattente, DateTime heure)
        {

            if (stop)
            {
                throw new TimeoutException();
            }
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

                if (sommetCrt != 0)
                {

                    var currentLivraison = demande.livraisons[idLivraison];
                    if (currentLivraison.planifier)
                    {
                        DateTime DebutPlage = currentLivraison.debutPlage;
                        if (lastheuredepart.CompareTo(DebutPlage) <= 0)
                        {
                            coutVus += cout[sommetCrt, 0] + (int)((TimeSpan)(DebutPlage.Subtract(lastheuredepart))).TotalSeconds + currentLivraison.duree;
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
                    meilleurtempsattente = new List<TimeSpan>(tempsattente);
                    lastheuredepart = heureDepart; //AVEC OU SANS
                }
            }
            else if (coutVus + bound(sommetCrt, nonVus, cout, duree, demande, lastheuredepart) < coutMeilleureSolution)
            {

                IIterator<int> it = iterator(sommetCrt, nonVus, cout, duree);
                while (it.MoveNext())
                {
                    DateTime newheurepassage;
                    int prochainSommet = it.Current;
                    vus.Add(prochainSommet);
                    nonVus.Remove(prochainSommet);
                    if (sommetCrt != 0)
                    {
                        var currentLivraison = demande.livraisons[idLivraison];
                        var nextLivraison = demande.livraisons[prochainSommet];

                        if (nextLivraison.planifier && currentLivraison.planifier)
                        {
                            DateTime nextFinPlage = nextLivraison.finPlage;
                            DateTime nextDebutPlage = nextLivraison.debutPlage;
                            DateTime currentDebutPlage = currentLivraison.debutPlage;
                            DateTime heureprevu = lastheuredepart + new TimeSpan(0, 0, cout[sommetCrt, prochainSommet] + duree[sommetCrt]);
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
                        else if (nextLivraison.planifier && !currentLivraison.planifier)
                        {
                            DateTime heureprevu = lastheuredepart + new TimeSpan(0, 0, cout[sommetCrt, prochainSommet] + duree[sommetCrt]);
                            DateTime nextFinPlage = nextLivraison.finPlage;
                            if ((heureprevu).CompareTo(nextFinPlage) <= 0)
                            {
                                tempsdattente = currentLivraison.tempsAttente;
                                newheurepassage = heureprevu;
                                horaires.Add(newheurepassage);
                                tempsattente.Add(tempsdattente);
                                branchAndBound(prochainSommet, nonVus, vus, coutVus + cout[sommetCrt, prochainSommet] + currentLivraison.duree, cout, duree, tpsDebut, tpsLimite, horaires, tempsattente, newheurepassage);
                                horaires.Remove(newheurepassage);
                                tempsattente.Remove(tempsdattente);
                            }
                        }
                        else if (!nextLivraison.planifier && currentLivraison.planifier)
                        {
                            DateTime heureprevu = lastheuredepart + new TimeSpan(0, 0, cout[sommetCrt, prochainSommet] + duree[sommetCrt]);
                            DateTime currentDebutPlage = currentLivraison.debutPlage;
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
                            branchAndBound(prochainSommet, nonVus, vus, coutVus + cout[sommetCrt, prochainSommet] + (int)((TimeSpan)(currentDebutPlage.Subtract(lastheuredepart))).TotalSeconds + currentLivraison.duree, cout, duree, tpsDebut, tpsLimite, horaires, tempsattente, newheurepassage);
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
                            branchAndBound(prochainSommet, nonVus, vus, coutVus + cout[sommetCrt, prochainSommet] + currentLivraison.duree, cout, duree, tpsDebut, tpsLimite, horaires, tempsattente, newheurepassage);
                            horaires.Remove(newheurepassage);
                            tempsattente.Remove(tempsdattente);
                        }
                    }
                    else
                    {
                        tempsdattente = new TimeSpan(0, 0, 0);
                        newheurepassage = lastheuredepart.Add(new TimeSpan(0, 0, cout[sommetCrt, prochainSommet]));
                        horaires.Add(newheurepassage);
                        tempsattente.Add(tempsdattente);
                        branchAndBound(prochainSommet, nonVus, vus, coutVus + cout[sommetCrt, prochainSommet], cout, duree, tpsDebut, tpsLimite, horaires, tempsattente, newheurepassage);
                        horaires.Remove(newheurepassage);
                        tempsattente.Remove(tempsdattente);
                    }
                    vus.Remove(prochainSommet);
                    nonVus.Add(prochainSommet);
                }
            }
        }




    }
}