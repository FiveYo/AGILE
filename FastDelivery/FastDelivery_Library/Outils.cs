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
        public static List<Troncon> ParserXml_Plan(string File_PATH)
        { 
            //On initialise notre Xdocument avec le Path du fichier xml
            XDocument MyData = XDocument.Load(File_PATH);
           
            //On récupète dans une liste la data avec le node qu'on veut 
            List<Point> PointList = new List<Point>();
            foreach (var node in MyData.Descendants("noeud"))
            {
                PointList.Add(new Point(
                    int.Parse(node.Attribute("id").Value),
                    int.Parse(node.Attribute("x").Value),
                    int.Parse(node.Attribute("y").Value)
                    ));
            }
            List<Troncon> TronconList = new List<Troncon>();
            foreach (var node in MyData.Descendants("troncon"))
            {
                TronconList.Add(new Troncon(
                    int.Parse(node.Attribute("destination").Value),
                    int.Parse(node.Attribute("longueur").Value),
                    int.Parse(node.Attribute("origine").Value),
                    int.Parse(node.Attribute("vitesse").Value),
                    node.Attribute("nomRue").Value
                    ));
            }
           
            return TronconList;
        }
    }
}
