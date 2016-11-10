﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using FastDelivery_Library.Modele;

namespace FastDelivery_Library

{
    public static class Outils
    {

        private static TSP1 tsp = new TSP1();
        /// <summary>
        /// Retourne une liste d'object, le premier étant la dictionnaire de Troncon,
        /// le second le dictionnaire de point
        /// </summary>
        /// <param name="streamFile">stream du fichier xml</param>
        /// <returns></returns>
        public static Carte ParserXml_Plan(System.IO.Stream streamFile)
        {
            //On initialise notre Xdocument avec le Path du fichier xml
            XDocument MyData = XDocument.Load(streamFile);

            //On récupète dans un dictionnaire la data avec le node qu'on veut 
            Dictionary<int, Point> PointHash = new Dictionary<int, Point>();
            Dictionary<int, Troncon> TronconHash = new Dictionary<int, Troncon>();

            //déclaration de la structure
            Carte carte;

            var nodes = MyData.Descendants("noeud");
            if (nodes.Count() == 0 || (MyData.Descendants("troncon").Count() == 0))
            {
                throw new Exception_XML("Fichier mal formaté");
            }


            //Initialisation variables 
            int xmax, xmin, ymax, ymin, Id, x, y, id_dest, id_origin,longueur,vitesse;
            string nomRue;
            //variable calcul xmax ymax xmin ymin
            xmin = int.MaxValue;
            ymin = int.MaxValue;
            xmax = int.MinValue;
            ymax = int.MinValue;
            //On génère les Points depuis le fichier XML en paramètre
            foreach (var node in nodes)
            {
                try
                {
                    Id = int.Parse(node.Attribute("id").Value);
                    x = int.Parse(node.Attribute("x").Value);
                    y = int.Parse(node.Attribute("y").Value);
                }
                catch (NullReferenceException ex)
                {
                    throw new Exception_XML("Fichier mal formaté", ex);
                }
                Point pt = new Point(Id, x, y);

                if (xmax < x)
                {
                    xmax = x;
                }

                if (ymax < y)
                {
                    ymax = y;
                }

                if (xmin > x)
                {
                    xmin = x;
                }

                if (ymin > y)
                {
                    ymin = y;
                }
                PointHash.Add(Id, pt);
            }
            int ID = 1;
            //On génère les Troncons depuis le fichier XML en paramètre

            foreach (var node in MyData.Descendants("troncon"))
            {
                //on récpuère les id des points d'origine

                try
                {
                    id_dest = int.Parse(node.Attribute("destination").Value);
                    id_origin = int.Parse(node.Attribute("origine").Value);
                }
                catch (NullReferenceException ex)
                {
                    throw new Exception_XML("Fichier mal formaté", ex);
                }

                // on crée les Points pour le constructeur
                Point Dest_Point;
                Point Origin_Point;
                
                //On cherche les objets Point dans la PointHash
                if ((PointHash.TryGetValue(id_dest, out Dest_Point)) && (PointHash.TryGetValue(id_origin, out Origin_Point)))
                {
                    // ON crée le nouvel objet Troncon
                    try
                    {
                        longueur = int.Parse(node.Attribute("longueur").Value);
                        vitesse = int.Parse(node.Attribute("vitesse").Value);
                        nomRue = node.Attribute("nomRue").Value;
                    }
                    catch (NullReferenceException ex)
                    {
                        throw new Exception_XML("Fichier mal formaté", ex);
                    }
                    Troncon Troncon_temp = new Troncon(
                        Dest_Point,
                        longueur,
                        Origin_Point,
                        vitesse,
                        nomRue,
                        ID
                        );

                    //On met a jour les voisins 
                    Origin_Point.AddVoisins(Troncon_temp);

                    //On met à jour la HashTable avec un nouvel id
                    TronconHash.Add(ID, Troncon_temp);
                }
                ID++;
            }

            carte = new Carte(PointHash, TronconHash, xmin, xmax, ymin, ymax);
            return carte;
        }

        public static void StopTsp()
        {
            tsp.stop = true;
        }

        public static DemandeDeLivraisons ParserXml_Livraison(System.IO.Stream streamFile, Dictionary<int, Point> HashPoint)
        {
            //On initialise notre Xdocument avec le Path du fichier xml
            XDocument MyData = XDocument.Load(streamFile);
            //On récupète dans un dictionnaire la data avec le node qu'on veut 
            Dictionary<int, Livraison> LivHash = new Dictionary<int, Livraison>();

            // On fait une liste d'entrepot
            List<Entrepot> ListEntrepot = new List<Entrepot>();

            // On initialise la structure
            DemandeDeLivraisons demandeLivaisons;
            Entrepot entrepot = null;
            //On génère les Livraisons depuis le fichier XML en paramètre 
            int ID = 1;

            // On initialise les adresse
            Point AdressePointLivraison;
            Point AdressePointEntrepot;

            //On initialise des élements testés dans le Try Entrepot 
            int idAdresseEntrepot = 0;
            XElement EntrepotXML = null;
            DateTime PlageDebut;
            DateTime PlageFin;
            int duree;
            int id;

            try
            {
                EntrepotXML = MyData.Descendants("entrepot").First();
                //On récupère les infos sur l'entrepot, son adresse sera un objet Point
                idAdresseEntrepot = int.Parse(EntrepotXML.Attribute("adresse").Value);
            }
            catch (System.NullReferenceException ex)
            {
                throw new Exception_XML("Fichier mal formaté", ex);

            } 
            if (HashPoint.TryGetValue(idAdresseEntrepot, out AdressePointEntrepot))
            {
                var nodeHeure = EntrepotXML.Attribute("heureDepart");
                DateTime heure;
                if(nodeHeure != null && DateTime.TryParse(nodeHeure.Value, out heure))
                {
                    entrepot = new Entrepot(
                    1,
                    AdressePointEntrepot,
                    heure
                    );
                }
                else
                {
                    throw new Exception_XML("Heure de l'entrepôt mal formaté");
                }
            }
            

            //On récupère les infos sur les livraisons, leurs adresses seront des objets Point
            foreach (var node in MyData.Descendants("livraison"))
            {
                try
                {
                    id = int.Parse(node.Attribute("adresse").Value);
                    duree = int.Parse(node.Attribute("duree").Value);
                }
                catch (NullReferenceException ex)
                {
                    throw new Exception_XML("Dossier XML incorrect", ex);
                }
                if (HashPoint.TryGetValue(id, out AdressePointLivraison))
                {
                    Livraison liv = new Livraison(
                        AdressePointLivraison,
                        duree
                        );

                    try
                    {
                        var debut = node.Attribute("debutPlage");
                        var fin = node.Attribute("finPlage");
                        if (debut != null && fin != null)
                        {
                            PlageDebut = DateTime.Parse(debut.Value);
                            PlageFin = DateTime.Parse(fin.Value);
                            liv.SetPlage(PlageDebut, PlageFin);
                        }
                    }
                    
                    catch (ArgumentNullException)
                    {}
                    catch (FormatException)
                    {}

                    LivHash.Add(ID, liv);

                    ID++;
                }
                else
                {
                    throw new Exception_XML("Le point avec l'id " + id.ToString() + " n'existe pas");
                }
            }
            demandeLivaisons = new DemandeDeLivraisons(LivHash, entrepot);
            return demandeLivaisons;
        }

        /// <summary>
        /// Créé la matrice carré de cout.
        /// </summary>
        /// <param name="LivStruct"></param>
        /// <param name="PointStruct"></param>
        /// <returns></returns>
        public static int[,] CreateCostMatrice(DemandeDeLivraisons LivStruct, Modele.Carte carte)
        {
            DijkstraAlgorithm d = new DijkstraAlgorithm(carte);
            LinkedList<Point> linked;
            int longueur = LivStruct.livraisons.Count + 1;
            int[,] matrice = new int[longueur, longueur];
            int i = 1, j = 1;
            foreach (var points in LivStruct.livraisons)
            {

                Point adresse = points.Value.adresse;
                d.execute(adresse);
                foreach (var autrespoints in LivStruct.livraisons)
                {
                    if (autrespoints.Value.adresse.id != adresse.id)
                    {
                        linked = d.getPath(autrespoints.Value.adresse);
                        matrice[i, j] = (int)calculcout(linked);
                        
                    }
                    j++;
                }
                j = 1;
                i++;
            }
            Point entrepot = LivStruct.entrepot.adresse;
            int cout;
            i = 1;
            d.execute(entrepot);
            // rempli la ligne 0
            foreach (var points in LivStruct.livraisons)
            {
                if (points.Value.adresse.id != entrepot.id)
                {
                    linked = d.getPath(points.Value.adresse);
                    cout = (int)calculcout(linked);
                    matrice[0, i] = cout;
                }
                i++;
            }
            i = 1;
            // rempli la colonne 0
            foreach (var points in LivStruct.livraisons)
            {
                if (points.Value.adresse.id != entrepot.id)
                {
                    d.execute(points.Value.adresse);
                    linked = d.getPath(entrepot);
                    cout = (int)calculcout(linked);
                    matrice[i, 0] = cout;
                }
                i++;
            }

            return matrice;
        }
        public static double calculcout(LinkedList<Point> linkedPoint)
        {
            int i = 0;
            double cout = 0;
            Point depart = linkedPoint.First();
            while (depart != linkedPoint.Last())
            {
                foreach (var voisins in depart.voisins)
                {
                    if (voisins.destination == linkedPoint.ElementAt(i + 1))

                    {
                        cout += (double)voisins.longueur / voisins.vitesse;
                        depart = linkedPoint.ElementAt(i + 1);
                        i++;
                        break;

                    }
                }

            }
            return cout;
        }

        public static Task startTsp(DemandeDeLivraisons LivStruct, Carte carte)
        {
            Livraison tmp;
            int[,] cost = CreateCostMatrice(LivStruct, carte);
            tsp = new TSP1();
            int[] duree = new int[LivStruct.livraisons.Count + 1];
            for (int i = 0; i < duree.Length; i++)
            {
                if (LivStruct.livraisons.TryGetValue(i, out tmp))
                {
                    duree[i] = tmp.duree;
                }
            }

            Task t = new Task(() =>
            {
                tsp.chercheSolution(new TimeSpan(0, 1, 0, 0), LivStruct.livraisons.Count + 1, cost,
                duree, LivStruct);
            });
            t.Start();
            return t;
        }

        public async static Task<Tournee> getResultActual(DemandeDeLivraisons demande, Carte carte)
        {
            Tournee t2 = null;
            await Task.Delay(100);
            List<Livraison> resultat = new List<Livraison>();
            List<DateTime> resultatDate = new List<DateTime>();
            List<TimeSpan> resultatTemp = new List<TimeSpan>();

            // On vérifie qu'il existe déjà des solutions
            if (tsp.meilleureSolution.Where((i) => { return i == 0; }).Count() == 1)
            {

                foreach (var index in tsp.meilleureSolution.Skip(1))
                {
                    resultat.Add(demande.livraisons.ElementAt(index - 1).Value);
                }
                foreach (var horaire in tsp.meilleurshoraires.Skip(1))
                {
                    resultatDate.Add(horaire);
                }
                foreach(var temp in tsp.meilleurtempsattente.Skip(1))
                {
                    resultatTemp.Add(temp);
                }

                DijkstraAlgorithm dijkstra = new DijkstraAlgorithm(carte);

                Dictionary<Lieu, Chemin> chemins = new Dictionary<Lieu, Chemin>();

                Point start = demande.entrepot.adresse;

                foreach (var livraison in resultat)
                {
                    dijkstra.execute(start);
                    chemins.Add(livraison, new Chemin(PathToTroncon(dijkstra.getPath(livraison.adresse))));
                    start = livraison.adresse;
                }


                dijkstra.execute(resultat.Last().adresse);
                chemins.Add(
                    demande.entrepot,
                    new Chemin(PathToTroncon(dijkstra.getPath(demande.entrepot.adresse))));

                t2 = new Tournee(demande.entrepot, resultat, chemins);
                for (int i = 0; i < resultat.Count; i++)
                {
                    t2.HeuredePassage.Add(resultat[i], resultatDate[i]);
                    resultat[i].heureArrivee = resultatDate[i];
                    resultat[i].heureDepart = resultatDate[i] + resultatTemp[i] + new TimeSpan(0, 0, resultat[i].duree);
                }
            }
            return t2;
        }

        public static List<Troncon> PathToTroncon(LinkedList<Point> points)
        {
            List<Troncon> troncons = new List<Troncon>();

            Point start = points.First();

            foreach(var point in points.Skip(1))
            {
                foreach (var voisin in start.voisins)
                {
                    if(voisin.destination == point)
                    {
                        troncons.Add(voisin);
                        break;
                    }
                }
                start = point;
            }
            return troncons;
        }
    }
}
