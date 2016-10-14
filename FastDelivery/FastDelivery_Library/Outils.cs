using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FastDelivery_Library

{
    public class Outils
    {   
        public Outils()
        {

        }

        /// <summary>
        /// Retourne une liste d'object, le premier étant la dictionnaire de Troncon,
        /// le second le dictionnaire de point
        /// </summary>
        /// <param name="streamFile">stream du fichier xml</param>
        /// <returns></returns>
        public static StructPlan ParserXml_Plan(System.IO.Stream streamFile)
        { 
            //On initialise notre Xdocument avec le Path du fichier xml
            XDocument MyData = XDocument.Load(streamFile);

            //On récupète dans un dictionnaire la data avec le node qu'on veut 
            Dictionary<int, Point> PointHash = new Dictionary<int, Point>();
            Dictionary<int, Troncon> TronconHash = new Dictionary<int, Troncon>();

            //déclaration de la structure
            StructPlan Hashstruct = new StructPlan();
            
            var nodes = MyData.Descendants("noeud");

            //variable calcul xmax ymax xmin ymin
            int xmax = int.Parse(nodes.First().Attribute("x").Value);
            int xmin = xmax;
            int ymax = int.Parse(nodes.First().Attribute("y").Value);
            int ymin = ymax;

            //On génère les Points depuis le fichier XML en paramètre
            foreach (var node  in nodes)
            {
                int Id = int.Parse(node.Attribute("id").Value);
                int x = int.Parse(node.Attribute("x").Value);
                int y = int.Parse(node.Attribute("y").Value);
                Point pt = new Point(Id,x,y);

                if (xmax<x)
                {
                    xmax = x;
                }

                if (ymax<y)
                {
                    ymax = y;
                }

                if(xmin>x)
                {
                    xmin = x;
                }

                if(ymin>y)
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
                int id_dest = int.Parse(node.Attribute("destination").Value);
                int id_origin = int.Parse(node.Attribute("origine").Value);


                // on crée les Points pour le constructeur
                Point Dest_Point;
                Point Origin_Point;
                
                //On cherche les objets Point dans la PointHash
                if ( (PointHash.TryGetValue(id_dest, out Dest_Point)) && (PointHash.TryGetValue(id_origin, out Origin_Point)) )
                {
                    // ON crée le nouvel objet Troncon
                    Troncon Troncon_temp = new Troncon(
                        Dest_Point,
                        int.Parse(node.Attribute("longueur").Value),
                        Origin_Point,
                        int.Parse(node.Attribute("vitesse").Value),
                        node.Attribute("nomRue").Value,
                        ID
                        );

                    //On met a jour les voisins 
                    Origin_Point.SetVoisins(Troncon_temp);

                    //On met à jour la HashTable avec un nouvel id
                    TronconHash.Add(ID, Troncon_temp);
                    }
                ID++;
            }

            Hashstruct.HashPoint = PointHash;
            Hashstruct.HashTroncon = TronconHash;
            Hashstruct.Xmax = xmax;
            Hashstruct.Xmin = xmin;
            Hashstruct.Ymax = ymax;
            Hashstruct.Ymin = ymin;

            return Hashstruct;
        }
        public static Dictionary<int,Livraison> ParserXml_Livraison(string File_PATH, Dictionary<int,Point> HashPoint)
        {
            //On initialise notre Xdocument avec le Path du fichier xml
            XDocument MyData = XDocument.Load(File_PATH);

            //On récupète dans un dictionnaire la data avec le node qu'on veut 
            Dictionary<int, Livraison> LivHash = new Dictionary<int, Livraison>();
            
            // On fait une liste d'entrepot car on sait jamais poto
            List<Entrepot> ListEntrepot = new List<Entrepot>();

            //On génère les Livraisons depuis le fichier XML en paramètre 
            int ID = 1;

            var EntrepotXML = MyData.Descendants("entrepot").First();
            Entrepot entrepot = new Entrepot(
            1,
            int.Parse(EntrepotXML.Attribute("adresse").Value),
            EntrepotXML.Attribute("heureDepart").Value
            );

            Point AdressePoint;
            foreach (var node in MyData.Descendants("livraison"))
            {
                int id = int.Parse(node.Attribute("adresse").Value);
                if (HashPoint.TryGetValue(id, out AdressePoint))
                { 
                    Livraison liv = new Livraison(
                        AdressePoint,
                        int.Parse(node.Attribute("duree").Value),
                        entrepot
                        );

                    string PlageDebut = node.Attribute("debutPlage") != null ? node.Attribute("debutPlage").Value : "False";
                    string PlageFin = node.Attribute("finPlage") != null ? node.Attribute("finPlage").Value : "False";

                    if (PlageDebut != "False")
                    {
                        liv.SetPlage(PlageDebut, PlageFin);
                    }

                    LivHash.Add(ID, liv);

                    ID++;
                }   
            }
            return LivHash;
        }
        
    }
    public struct StructPlan
    {
        public Dictionary<int, Point> HashPoint;
        public Dictionary<int, Troncon> HashTroncon;
        public int Xmin, Xmax, Ymin, Ymax;
    }
}
