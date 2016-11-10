using FastDelivery_Library.Modele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    /// <summary>
    /// Contient la liste des livraisons ordonnées et des tronçons ainsi que l'entrepot
    /// </summary>
    public class Tournee : IComparable
    {
        public Entrepot entrepot;

        // Les livraisons sont ordonnées par ordre de passage
        public List<Livraison> livraisons;

        // Si on veut connaitre le chemin pour aller à une livraisons on peut y accéder par sa clef
        public Dictionary<Lieu, Chemin> Hashchemin;

        public enum Error { Before, After };

        public Dictionary<Livraison, DateTime> HeuredePassage = new Dictionary<Livraison, DateTime>();

        public Tournee(Entrepot entrepot, List<Livraison> livraisons, Dictionary<Lieu, Chemin> minche)
        {
            this.entrepot = entrepot;
            this.livraisons = livraisons;
            this.Hashchemin = minche;
        }

        public void UpdateHeurePassage()
        {
            DateTime tA=entrepot.heureDepart;
            DateTime tD = entrepot.heureDepart;
            Chemin chemintemp;
            Livraison livtemp;
            for(int i=0; i<livraisons.Count;i++)
            {
                livtemp = livraisons[i];
                chemintemp = Hashchemin[livtemp];
                tA = tD + new TimeSpan(0, 0, (int)chemintemp.cout);
                if ( i<livraisons.Count-1 && livraisons[i+1].planifier )
                {
                    tD = tA + new TimeSpan(0, 0, livtemp.duree)+livraisons[i+1].tempsAttente;
                }
                else
                {
                    tD = tA + new TimeSpan(0, 0, livtemp.duree);
                }
                livtemp.heureArrivee = tA;
                livtemp.heureArrivee = tD;
            }
            var result=Check();
            if (result.Count!=0)
            {
                throw new Exception();
            }



        }
        public Dictionary<Livraison, Error> AddLivraison(Carte carte, Livraison newlivraison, int index)
        {

            Point positionelementprecedent;
            Point positionelementsuivant;

            Lieu lieuprecedent;
            Lieu lieusuivant;


            DijkstraAlgorithm dijkstra = new DijkstraAlgorithm(carte);

            List<Troncon> tronconprecedent = new List<Troncon>();
            List<Troncon> tronconsuivant = new List<Troncon>();



            if (index == -1)
            {
                positionelementprecedent = entrepot.adresse;
                lieuprecedent = entrepot;
                lieusuivant = livraisons[0];
                positionelementsuivant = livraisons[0].adresse;
            }
            else if (index == livraisons.Count - 1)
            {
                positionelementsuivant = entrepot.adresse;
                positionelementprecedent = livraisons[index].adresse;
                lieusuivant = entrepot;
                lieuprecedent = livraisons[index];

            }

            else
            {
                lieuprecedent = livraisons[index];
                positionelementsuivant = livraisons[index + 1].adresse;
                lieusuivant = livraisons[index + 1];
                positionelementprecedent = livraisons[index].adresse;
            }
            //calcul du plus court chemin menant à la nouvelle livraison depuis la livraison précédente
            dijkstra.execute(positionelementprecedent);
            Chemin cheminprecedent = new Chemin(Outils.PathToTroncon(dijkstra.getPath(newlivraison.adresse)));

            //calcul du plus court chemin menant à la livraison suivant depuis la nouvelle livraison 
            dijkstra.execute(newlivraison.adresse);
            Chemin cheminsuivant = new Chemin(Outils.PathToTroncon(dijkstra.getPath(positionelementsuivant)));

            //on met a jour la liste des livraisons
            livraisons.Insert(index + 1, newlivraison);


            //on met a jour la liste des tronçons

            Hashchemin.Add(newlivraison, cheminprecedent);
            Hashchemin[lieusuivant] = cheminsuivant;
            HeuredePassage.Add(newlivraison, new DateTime());

            //on check si tout va bien
            //Dictionary<Livraison, List<double>> result = Check();

            Dictionary<Livraison, Error> ErrorLivraison = new Dictionary<Livraison, Error>();

            //foreach (Livraison livraison in result.Keys)
            //{
            //    if (result[livraison][0] == 1)
            //    {
            //        ErrorLivraison.Add(livraison, Error.After);
            //    }
            //    else if (result[livraison][0] == -1)
            //    {

            //        ErrorLivraison.Add(livraison, Error.Before);
            //    }
            //}
            UpdateHeurePassage();
            return ErrorLivraison;

        }

        public Dictionary<Livraison, Error> DelLivraison(Carte carte, Livraison badlivraison)
        {
            Point positionelementprecedent;
            Point positionelementsuivant;
            int index;
            index = livraisons.IndexOf(badlivraison);

            Lieu lieuprecedent;
            Lieu lieusuivant;


            DijkstraAlgorithm dijkstra = new DijkstraAlgorithm(carte);
            List<Troncon> tronconsuivant = new List<Troncon>();


            if (index == 0)
            {
                positionelementprecedent = entrepot.adresse;
                positionelementsuivant = livraisons[index + 1].adresse;
                lieuprecedent = entrepot;
                lieusuivant = livraisons[index + 1];


            }
            else if (index == livraisons.Count - 1)
            {
                positionelementsuivant = entrepot.adresse;
                positionelementprecedent = livraisons[index - 1].adresse;
                lieusuivant = entrepot;
                lieuprecedent = livraisons[index - 1];
            }
            else
            {
                positionelementprecedent = livraisons[index - 1].adresse;
                positionelementsuivant = livraisons[index + 1].adresse;
                lieuprecedent = livraisons[index - 1];
                lieusuivant = livraisons[index + 1];
            }




            dijkstra.execute(positionelementprecedent);
            Chemin cheminsuivant = new Chemin(Outils.PathToTroncon(dijkstra.getPath(positionelementsuivant)));

            //on met a jour la liste des livraisons
            livraisons.RemoveAt(index);


            //on met a jour la liste des tronçons

            Hashchemin.Remove(badlivraison);
            Hashchemin[lieusuivant] = cheminsuivant;

            //on check si tout va bien
            Dictionary<Livraison, List<double>> result = Check();

            Dictionary<Livraison, Error> ErrorLivraison = new Dictionary<Livraison, Error>();

            //foreach (Livraison livraison in result.Keys)
            //{
            //    if (result[livraison][0] == 1)
            //    {
            //        ErrorLivraison.Add(livraison, Error.After);
            //    }
            //    else if (result[livraison][0] == -1)
            //    {

            //        ErrorLivraison.Add(livraison, Error.Before);
            //    }
            //}
            UpdateHeurePassage();
            return ErrorLivraison;
        }



        //Cette méthode renvoie un dictionnaire avec les livraisons qui demandent  des plages horaires
        //elle prend en argument une tournee ( avec les plages horaires initialisée) et la livraison (avec une plage horaire sinon on s'en fout)
        //un dictionnaire(Livraison,list<Double>) la liste des livraisons en clé et une liste de double
        //la liste de double sera composée de deux éléments (toujours)
        //---le premier élément sera le flag qui donnera des informations concernant la plage horaire  :
        //si il vaut -1, c'est que l'heure de passage est après la fin de la plage horaire ( il faut donc baisser le cout de cette livraison afin d'avancer l'heure de passage)
        //si il faut 1, c'est que l'heure de passage est avant le début de la plage horaire ( il faut donc augmenter le cout de cette livraison afin de retarder l'heure de passage)
        //il vaudra 0 si tout va bien
        //---le second élément sera le temps en seconde qui sépare l'intervalle (borne supérieure( FinPlage si -1 ) ou inférieur(DebutPlage si 1) voulu des plages : 
        //il peut etre négatif ou positif ( respectivement avec les -1 / 1 du premier élément de la liste)
        //il servira a bidouiller le cout afin de respecter les conditions de plages horaires

        //CEPANDANT, le  problème se situe dans l'achitecture même du projet, car si on change le cout a chaque fois on peut tomber sur une boucle infinie qui anéantira toute vie sur terre, meme ton chien va prendre tarif poto.
        public Dictionary<Livraison, List<double>> Check()
        {
            // pour exécuter cette méthode il faut que tournee ait son attribut ' HeureDePassage ' initialisé
            // et que la livraison soit munie d'une plage horaire sinon on s'en fout
            if (HeuredePassage == null)
            {
                throw new Exception("HeureDePassage non intialisé");
            }

            Dictionary<Livraison, List<double>> listlivraisonout = new Dictionary<Livraison, List<double>>();

           
            DateTime FinPlage;
            DateTime HeureArrive;

            List<double> CostList = new List<double>();
            double costgap;
            double danslintervalle;

            foreach (var livraison in livraisons)
            {
                if (HeuredePassage.TryGetValue(livraison, out HeureArrive))
                {
                    if (livraison.planifier)
                    {
                        
                        FinPlage = livraison.finPlage;
                        if (DateTime.Compare(FinPlage, HeureArrive) < 0)
                        {
                            danslintervalle = -1;
                            costgap = -((TimeSpan)HeureArrive.Subtract(FinPlage)).TotalSeconds;
                            CostList.Add(danslintervalle);
                            CostList.Add(costgap);
                            listlivraisonout.Add(livraison, CostList);
                        }
                        //on vide la liste
                        CostList.Clear();
                    }
                }
            }
            return listlivraisonout;
        }

        // Cette méthode implémente la modification d'une plage horaire
        /*
         ** T0D0 : check si la plage horaire n'existe pas
         */
        public void ModifPlage(Livraison livraisonNewPlage, DemandeDeLivraisons LivStruct, Carte carte)
        {
            foreach (var livraison in livraisons) { }

            // Récupérer l'heure de livraison de la livraison à modif
            DateTime DebutNewPlage = livraisonNewPlage.debutPlage;
            DateTime FinNewPlage = livraisonNewPlage.finPlage;
            DateTime heurePassage;

            //Copie la liste de livraisons dans une nouvelle liste
            List<Livraison> livraisonsInversee = new List<Livraison>(livraisons);
            livraisonsInversee.Reverse();
            int indexLivraisonInverseeNewPlage = livraisonsInversee.IndexOf(livraisonNewPlage);
            int indexLivraisonNewPlage = livraisons.IndexOf(livraisonNewPlage);

            if (HeuredePassage.TryGetValue(livraisonNewPlage, out heurePassage))
            {
                // Si la nouvelle plage horaire respecte l'heure de passage
                if (DebutNewPlage < heurePassage && heurePassage < FinNewPlage)
                {

                }

                /* Dans cette partie nous prenons en compte 3 variables livraison
                 * livraison1, et livraison2, qui est la livraison suivante à la 1
                 * livraisonNewPlage qui est la livraison dont nous avons changer le plage horaire
                 * Calcul de l'heure d'arrivee pour la livraison dont on a modif la plage horaire
                 */
                else
                {
                    // Le livreur doit venir en avance
                    // livraison2 est initialisée à livraisonNewPlage
                    if (FinNewPlage.CompareTo(heurePassage) < 0)
                    {
                        if (livraisons.Count() != indexLivraisonInverseeNewPlage+1)
                        {
                            // Calcul de l'heure d'arrivee pour la livraison dont on a modif la plage horaire
                            Livraison livraison1 = livraisonsInversee[indexLivraisonInverseeNewPlage + 1];
                            Livraison livraison2 = livraisonNewPlage;
                            Lieu pointArrivee = livraison2;

                            //Plages de la nouvelle livraison
                            DateTime finPlageLivraison2 = livraison2.finPlage;

                            //Calcule l'heure d'arrivée minimale à la prochaine livraison
                            TimeSpan trajetALivraison2 = TimeSpan.FromSeconds((double)livraison1.duree + Hashchemin[pointArrivee].cout);
                            DateTime heureArrivMinLivr1 = HeuredePassage[livraison2].Subtract(trajetALivraison2); // heure minimum pour arriver à la livraison


                            if (heureArrivMinLivr1.CompareTo(FinNewPlage) > 0)
                            {
                                HeuredePassage[livraisonNewPlage] = FinNewPlage;
                                livraisonNewPlage.SetHeureDePassage(FinNewPlage);

                            }
                            else if (heureArrivMinLivr1.CompareTo(DebutNewPlage) < 0)
                            {
                                HeuredePassage[livraisonNewPlage] = DebutNewPlage;
                                livraisonNewPlage.SetHeureDePassage(DebutNewPlage);
                            }
                            else
                            {
                                HeuredePassage[livraisonNewPlage] = heureArrivMinLivr1;
                                livraisonNewPlage.SetHeureDePassage(heureArrivMinLivr1);
                            }
                            livraisonNewPlage.heureDepart = livraisonNewPlage.heureArrivee.Add(TimeSpan.FromSeconds((double)livraisonNewPlage.duree));


                            foreach (var livraison in livraisons.Skip(livraisons.IndexOf(livraisonNewPlage)))
                            {
                                pointArrivee = livraison;

                                trajetALivraison2 = TimeSpan.FromSeconds(Hashchemin[pointArrivee].cout);

                                // Teste si l'heure de passage actuel de livraison2 est humainement faisable
                                if (HeuredePassage[livraison1].Add(trajetALivraison2).CompareTo(HeuredePassage[livraison]) > 0)
                                {
                                    DateTime nouvelleHeurePassage = HeuredePassage[livraison1].Add(trajetALivraison2);
                                    HeuredePassage[livraison] = HeuredePassage[livraison1].Add(trajetALivraison2);
                                    livraison.SetHeureDePassage(HeuredePassage[livraison]);
                                    livraison.heureDepart = livraison.heureArrivee.Add(TimeSpan.FromSeconds(livraison.duree));

                                   /* //Teste si plage horaire respectee
                                    if (HeuredePassage[livraison].CompareTo(livraison.debutPlage) < 0)
                                    {
                                        livraison.heureDepart = livraison.debutPlage.Add(TimeSpan.FromSeconds(livraison.duree));
                                    }
                                    else if (HeuredePassage[livraison].CompareTo(livraison.finPlage) > 0)
                                    {
                                        livraison.heureDepart = livraison1.heureDepart.Add(TimeSpan.FromSeconds(livraison.duree) + TimeSpan.FromSeconds(Hashchemin[pointArrivee].cout));
                                    }*/

                                }

                                else
                                {
                                    break;
                                }
                                livraison1 = livraison;
                            }
                        }
                        else
                        {

                        }
                    }

                    // le livreur arrive plus tard que prévu
                    // livraison1 est initialisée à livraisonNewPlage
                    else if (DebutNewPlage.CompareTo(heurePassage) > 0)
                    {
                        if ((livraisons.Count()) != indexLivraisonNewPlage+1)
                        {
                            // Calcul de l'heure d'arrivee pour la livraison dont on a modif la plage horaire
                            Livraison livraison2 = livraisons[indexLivraisonNewPlage + 1];
                            Livraison livraison1 = livraisonNewPlage;
                            Lieu pointArrivee = livraison2;

                            //Plages de la nouvelle livraison
                            DateTime finPlageLivraison2 = livraison2.finPlage;

                            //Calcule l'heure d'arrivée minimale à la prochaine livraison
                            TimeSpan duree = TimeSpan.FromSeconds((double)livraison1.duree);
                            TimeSpan trajet = TimeSpan.FromSeconds(Hashchemin[pointArrivee].cout);
                            foreach (var cout in Hashchemin)
                            { }
                            TimeSpan trajetALivraison2 = TimeSpan.FromSeconds((double)livraison1.duree + Hashchemin[pointArrivee].cout);
                            DateTime heureArrivMinLivr1 = HeuredePassage[livraison2].Subtract(trajetALivraison2); // heure minimum pour arriver à la livraison


                            if (heureArrivMinLivr1.CompareTo(FinNewPlage) > 0)
                            {
                                HeuredePassage[livraisonNewPlage] = FinNewPlage;
                                livraisonNewPlage.SetHeureDePassage(FinNewPlage);
                            }
                            else if (heureArrivMinLivr1.CompareTo(DebutNewPlage) < 0)
                            {
                                HeuredePassage[livraisonNewPlage] = DebutNewPlage;
                                livraisonNewPlage.SetHeureDePassage(DebutNewPlage);
                            }
                            else
                            {
                                HeuredePassage[livraisonNewPlage] = heureArrivMinLivr1;
                                livraisonNewPlage.SetHeureDePassage(heureArrivMinLivr1);
                            }
                            livraisonNewPlage.heureDepart = livraisonNewPlage.heureArrivee.Add(TimeSpan.FromSeconds((double)livraisonNewPlage.duree));


                            foreach (var livraison in livraisons.Skip(livraisons.IndexOf(livraisonNewPlage)))
                            {
                                pointArrivee = livraison;

                                trajetALivraison2 = TimeSpan.FromSeconds(Hashchemin[pointArrivee].cout);

                                // Teste si l'heure de passage actuel de livraison2 est humainement faisable
                                if (HeuredePassage[livraison1].Add(trajetALivraison2).CompareTo(HeuredePassage[livraison]) > 0)
                                {
                                    DateTime nouvelleHeurePassage = HeuredePassage[livraison1].Add(trajetALivraison2);
                                    HeuredePassage[livraison] = HeuredePassage[livraison1].Add(trajetALivraison2);
                                    livraison.SetHeureDePassage(HeuredePassage[livraison]);
                                    livraison.heureDepart = livraison.heureArrivee.Add(TimeSpan.FromSeconds(livraison.duree));

                                    //Teste si plage horaire respectee
                                    /*if (HeuredePassage[livraison].CompareTo(livraison.debutPlage) < 0)
                                    {
                                        livraison.heureDepart = livraison.debutPlage.Add(TimeSpan.FromSeconds(livraison.duree));
                                    }
                                    else if(HeuredePassage[livraison].CompareTo(livraison.finPlage) > 0)
                                    {
                                        livraison.heureDepart = livraison1.heureDepart.Add(TimeSpan.FromSeconds(livraison.duree)+TimeSpan.FromSeconds(Hashchemin[pointArrivee].cout));
                                    }*/

                                }
                                else
                                {
                                    break;
                                }
                                livraison1 = livraison;
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }
            else
            {

            }
            //on check si tout va bien
            /*Dictionary<Livraison, List<double>> result = Check();

            Dictionary<Livraison, Error> ErrorLivraison = new Dictionary<Livraison, Error>();

            foreach (Livraison livraisonCheck in result.Keys)
            {
                if (result[livraisonCheck][0] == 1)
                {
                    ErrorLivraison.Add(livraisonCheck, Error.After);
                }
                else if (result[livraisonCheck][0] == -1)
                {

                    ErrorLivraison.Add(livraisonCheck, Error.Before);
                }
            }
            return ErrorLivraison;*/
            //return;

            foreach (var livraison in livraisons) { }
        }

        public int CompareTo(object objet)
        {
            Tournee obj = objet as Tournee;
            if(obj == null)
            {
                return 1;
            }
            bool equals = true;
            for(int i=0; i<livraisons.Count; i++)
            { 
                if(livraisons[i].adresse.id != obj.livraisons[i].adresse.id)
                {
                    equals = false;
                }
            }
            if(!equals)
            {
                return 0;
            }
            else
            {
                return (int)((TimeSpan)(livraisons[livraisons.Count-1].heureDepart.Subtract(obj.livraisons[obj.livraisons.Count-1].heureDepart))).TotalSeconds;
            }
        }
    }
}