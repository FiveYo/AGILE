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
    public class Tournee
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

        public void CalculHeurePassage()
        {
            Dictionary<Livraison, DateTime> HashLivraison = new Dictionary<Livraison, DateTime>();
            Chemin chemin;
            // On initialise le temps du départ avec l'heure départ de l'entrepot
            DateTime Livtime = entrepot.heureDepart;
            foreach (var livraison in livraisons)
            {
                if (Hashchemin.TryGetValue(livraison, out chemin))
                {

                    foreach (var troncon in chemin.getTronconList())
                    {
                        Livtime = Livtime.Add(TimeSpan.FromSeconds(troncon.cout));
                    }

                    HashLivraison.Add(livraison, Livtime);
                    livraison.HeureDePassage = Livtime;
                    Livtime.Add(TimeSpan.FromSeconds(livraison.duree));
                }

            }
            this.HeuredePassage = HashLivraison;
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

            foreach (Livraison livraison in result.Keys)
            {
                if (result[livraison][0] == 1)
                {
                    ErrorLivraison.Add(livraison, Error.After);
                }
                else if (result[livraison][0] == -1)
                {

                    ErrorLivraison.Add(livraison, Error.Before);
                }
            }

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

            DateTime DebutPlage;
            DateTime FinPlage;
            DateTime Heurepassage;

            List<double> CostList = new List<double>();
            double costgap;
            double danslintervalle;

            foreach (var livraison in livraisons)
            {
                if (HeuredePassage.TryGetValue(livraison, out Heurepassage))
                {
                    if (livraison.planifier)
                    {
                        DebutPlage = livraison.debutPlage;
                        FinPlage = livraison.finPlage;

                        if (DateTime.Compare(DebutPlage, Heurepassage) > 0)
                        {
                            danslintervalle = 1;
                            costgap = ((TimeSpan)DebutPlage.Subtract(Heurepassage)).TotalSeconds;
                        }
                        else if (DateTime.Compare(FinPlage, Heurepassage) < 0)
                        {
                            danslintervalle = -1;
                            costgap = -((TimeSpan)Heurepassage.Subtract(FinPlage)).TotalSeconds;
                        }
                        else
                        {
                            danslintervalle = 0;
                            costgap = 0;
                        }

                        CostList.Add(danslintervalle);
                        CostList.Add(costgap);
                        listlivraisonout.Add(livraison, CostList);

                        //on vide la liste
                        CostList.Clear();
                    }
                }
            }
            return listlivraisonout;
        }
    }
}