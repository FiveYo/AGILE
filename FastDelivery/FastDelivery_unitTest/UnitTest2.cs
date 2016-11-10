using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using FastDelivery_Library;
using FastDelivery_Library.Modele;
using System.Linq;

namespace FastDelivery_unitTest
{
    [TestClass]
    public class TestOutils
    {
        Carte c;
        DemandeDeLivraisons demand;
        Dictionary<int, Livraison> HashLivraison = new Dictionary<int, Livraison>();
        Entrepot entrepotTest;
        public void CarteGenerator()
        {
            int id = 1;
            Point p1 = new Point(id++, 10, 10);
            Point p2 = new Point(id++, 100, 100);
            Point p3 = new Point(id++, 0, 10);
            Point p4 = new Point(id++, 0, 100);
            Point p5 = new Point(id++, 50, 10);

            id = 0;

            Troncon t1 = new Troncon(p5, 100, p1, 10, "rue1", id++);
            Troncon t2 = new Troncon(p4, 100, p5, 10, "rue1", id++);
            Troncon t3 = new Troncon(p3, 100, p4, 10, "rue1", id++);
            Troncon t4 = new Troncon(p2, 100, p3, 10, "rue1", id++);
            Troncon t5 = new Troncon(p1, 100, p2, 10, "rue1", id++);
            Troncon t6 = new Troncon(p3, 100, p1, 10, "rue1", id++);
            Troncon t7 = new Troncon(p4, 100, p2, 10, "rue1", id++);
            Troncon t8 = new Troncon(p3, 100, p5, 10, "rue1", id++);
            Troncon t9 = new Troncon(p2, 100, p4, 10, "rue1", id++);
            Troncon t10 = new Troncon(p1, 100, p5, 10, "rue1", id++);
            Troncon t11 = new Troncon(p2, 100, p1, 10, "rue1", id++);
            Troncon t12 = new Troncon(p3, 100, p2, 10, "rue1", id++);

            p1.AddVoisins(t1);
            p1.AddVoisins(t6);
            p1.AddVoisins(t11);

            p2.AddVoisins(t5);
            p2.AddVoisins(t7);
            p2.AddVoisins(t12);

            p3.AddVoisins(t4);

            p4.AddVoisins(t3);
            p4.AddVoisins(t9);

            p5.AddVoisins(t2);
            p5.AddVoisins(t8);
            p5.AddVoisins(t10);

            Dictionary<int, Point> points = new Dictionary<int, Point>();
            points.Add(p1.id, p1);
            points.Add(p2.id, p2);
            points.Add(p3.id, p3);
            points.Add(p4.id, p4);
            points.Add(p5.id, p5);

            Dictionary<int, Troncon> troncons = new Dictionary<int, Troncon>();
            troncons.Add(t1.id, t1);
            troncons.Add(t2.id, t2);
            troncons.Add(t3.id, t3);
            troncons.Add(t4.id, t4);
            troncons.Add(t5.id, t5);
            troncons.Add(t6.id, t6);
            troncons.Add(t7.id, t7);
            troncons.Add(t8.id, t8);
            troncons.Add(t9.id, t9);
            troncons.Add(t10.id, t10);
            troncons.Add(t11.id, t11);
            troncons.Add(t12.id, t12);

            c = new Carte(points, troncons, 0, 100, 10, 100);
        }

        
        public void LivGenerator()
        {
            int id = 0;
            int duree = 100;
            foreach (Point pt in c.points.Values.Skip(3))
            {
                Livraison tmp = new Livraison(pt, duree++);
                HashLivraison.Add(id, tmp);
                id++;
            }
            entrepotTest = new Entrepot(1, c.points[1], DateTime.Parse("10:00:00"));
            demand = new DemandeDeLivraisons(HashLivraison, entrepotTest);
            
        }

        [TestMethod]
        public void Test_CreateCostMatrice()
        {
            CarteGenerator();
            LivGenerator();
            int[,] MatriceReturn = Outils.CreateCostMatrice(demand, c);
            int[,] MatriceAttendu = new int[3, 3];
            MatriceAttendu[0, 0] = 0;
            MatriceAttendu[0, 1] = 20;
            MatriceAttendu[0, 2] = 10;
            MatriceAttendu[1, 0] = 20;
            MatriceAttendu[1, 1] = 0;
            MatriceAttendu[1, 2] = 30;
            MatriceAttendu[2, 0] = 10;
            MatriceAttendu[2, 1] = 10;
            MatriceAttendu[2, 2] = 0;
            for (int i = 0; i<3;i++)
            {
                for (int j = 0; j<3;j++)
                {
                    Assert.AreEqual(MatriceAttendu[i,j], MatriceReturn[i,j]);
                }
            }
        }

        [TestMethod]
        public void Test_calculcout()
        {
            CarteGenerator();
            int id= 0;
            double coutAttendu=10;
            double coutRetourne;
            LinkedList<Point> PointOrd = new LinkedList<FastDelivery_Library.Modele.Point>();
            PointOrd.AddLast(c.points[2]);
            PointOrd.AddLast(c.points[3]);
            coutRetourne = Outils.calculcout(PointOrd);
            Assert.AreEqual(coutRetourne, coutAttendu);

        }
    }
}
