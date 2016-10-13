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
        public static Dictionary<int,Point> ParserXml_Plan(string File_PATH)
        { 
            //On initialise notre Xdocument avec le Path du fichier xml
            XDocument MyData = XDocument.Load(File_PATH);

            //On récupète dans un dictionnaire la data avec le node qu'on veut 
            Dictionary<int, Point> PointHash = new Dictionary<int, Point>();
            Dictionary<int, Troncon> TronconHash = new Dictionary<int, Troncon>();

            //On génère les Points depuis le fichier XML en paramètre 
            foreach (var node  in MyData.Descendants("noeud"))
            {
                int ID = int.Parse(node.Attribute("id").Value);

                Point pt = new Point(
                    ID,
                    int.Parse(node.Attribute("x").Value),
                    int.Parse(node.Attribute("y").Value)
                    );

                PointHash.Add(ID, pt);
            }

            //On génère les Troncons depuis le fichier XML en paramètre 
            foreach (var node in MyData.Descendants("troncon"))
            {   
                //on récpuère les id des points d'origine
                int id_dest = int.Parse(node.Attribute("destination").Value);
                int id_origin = int.Parse(node.Attribute("origine").Value);

                int ID = int.Parse(node.Attribute("id").Value);

                // on crée les Points pour le constructeur
                Point Dest_Point;
                Point Origin_Point;

                //On cherche les objets Point dans la PointHash
                if (PointHash.TryGetValue(id_dest, out Dest_Point))
                {
                    if (PointHash.TryGetValue(id_origin, out Origin_Point))
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
                        if ((Dest_Point is Point) && (Origin_Point is Point))
                        {
                            PointHash[id_origin].SetVoisins(Troncon_temp);
                        }

                        //On met à jour la HashTable avec un nouvel id
                        TronconHash.Add(ID, Troncon_temp);
                    }
                }
            }
            
            return PointHash;


        }
    }
}
