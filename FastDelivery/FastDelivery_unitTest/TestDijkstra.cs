using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FastDelivery_Library.Modele;
using FastDelivery_Library;
using System.Collections.Generic;
using System.Linq;

namespace FastDelivery_unitTest
{
    [TestClass]
    public class TestDijkstra
    {
        Carte c;
        public void Fixtures()
        {
            int id = 0;
            Point p1 = new Point(id++, 10, 10);
            Point p2 = new Point(id++, 100, 100);
            Point p3 = new Point(id++, 0, 10);
            Point p4 = new Point(id++, 0, 100);
            Point p5 = new Point(id++, 50, 10);

            id = 0;

            Troncon t1 = new Troncon(p5, 100, p1, 10, "rue1", id++);
            Troncon t2 = new Troncon(p4, 1000, p5, 10, "rue1", id++);
            Troncon t3 = new Troncon(p3, 100, p4, 10, "rue1", id++);
            Troncon t4 = new Troncon(p2, 100, p3, 10, "rue1", id++);
            Troncon t5 = new Troncon(p1, 100, p2, 10, "rue1", id++);
            Troncon t6 = new Troncon(p3, 100, p1, 10, "rue1", id++);
            Troncon t7 = new Troncon(p4, 100, p2, 10, "rue1", id++);
            Troncon t8 = new Troncon(p3, 100, p5, 10, "rue1", id++);
            Troncon t9 = new Troncon(p2, 100, p4, 10, "rue1", id++);
            Troncon t10 = new Troncon(p1, 100, p5, 10, "rue1", id++);
            Troncon t11 = new Troncon(p3, 100, p2, 10, "rue1", id++);

            p1.AddVoisins(t1);
            p1.AddVoisins(t6);

            p2.AddVoisins(t5);
            p2.AddVoisins(t7);
            p2.AddVoisins(t11);

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

            c = new Carte(points, troncons, 0, 100, 10, 100);
        }

        [TestMethod]
        public void TestGetNeighboors()
        {
            Fixtures();
            DijkstraAlgorithm dij = new DijkstraAlgorithm(c);
            Point p1 = c.points.First().Value;
            Point p2 = c.points.ElementAt(1).Value;

            dij.execute(p1);
            LinkedList<Point> result = dij.getPath(p2);

            Assert.AreEqual(p1.id, result.ElementAt(0).id);
            Assert.AreEqual(c.points.ElementAt(2).Value.id, result.ElementAt(1).id);
            Assert.AreEqual(c.points.ElementAt(1).Value.id, result.Last.Value.id);
        }
    }
}
