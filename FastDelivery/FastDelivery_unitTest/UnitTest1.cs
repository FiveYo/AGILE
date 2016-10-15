using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FastDelivery_Library;
using System.Collections.Generic;

namespace FastDelivery_unitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestParserXml_Plan()
        {
            FileStream file1 = new FileStream(@"..\..\..\..\XMLExample\plan5x5.xml", FileMode.Open, FileAccess.Read);
            StructPlan structRetournee = Outils.ParserXml_Plan(file1);

            StructPlan structAttendue = new StructPlan();
            Dictionary<int, Point> pointsAttendus = new Dictionary<int, Point>();
            Dictionary<int, Troncon> tronconsAttendus = new Dictionary<int, Troncon>();

            pointsAttendus.Add(0, new Point(0, 134, 193));
            pointsAttendus.Add(1, new Point(1, 195, 291));
            pointsAttendus.Add(2, new Point(2, 140, 420));
            pointsAttendus.Add(3, new Point(3, 132, 470));
            pointsAttendus.Add(4, new Point(4, 128, 660));
            pointsAttendus.Add(5, new Point(5, 263, 81));
            pointsAttendus.Add(6, new Point(6, 244, 277));
            pointsAttendus.Add(7, new Point(7, 244, 345));
            pointsAttendus.Add(8, new Point(8, 300, 506));
            pointsAttendus.Add(9, new Point(9, 205, 629));
            pointsAttendus.Add(10, new Point(10, 382, 165));
            pointsAttendus.Add(11, new Point(11, 378, 209));
            pointsAttendus.Add(12, new Point(12, 433, 455));
            pointsAttendus.Add(13, new Point(13, 376, 591));
            pointsAttendus.Add(14, new Point(14, 424, 655));
            pointsAttendus.Add(15, new Point(15, 522, 136));
            pointsAttendus.Add(16, new Point(16, 488, 320));
            pointsAttendus.Add(17, new Point(17, 470, 435));
            pointsAttendus.Add(18, new Point(18, 546, 583));
            pointsAttendus.Add(19, new Point(19, 572, 604));
            pointsAttendus.Add(20, new Point(20, 636, 184));
            pointsAttendus.Add(21, new Point(21, 651, 255));
            pointsAttendus.Add(22, new Point(22, 619, 355));
            pointsAttendus.Add(23, new Point(23, 705, 536));
            pointsAttendus.Add(24, new Point(24, 652, 684));
            tronconsAttendus.Add(1, new Troncon(new Point(1, 195, 291),
                                                 9234,
                                                 new Point(0, 134, 193),
                                                 41, "v0", 1));

            tronconsAttendus.Add(2, new Troncon(new Point(5, 263, 81),
                                                 13666,
                                                 new Point(0, 134, 193),
                                                 47, "h0", 2));

            tronconsAttendus.Add(3, new Troncon(new Point(0, 134, 193),
                                                 9234,
                                                 new Point(1, 195, 291),
                                                 46, "v0", 3));

            tronconsAttendus.Add(4, new Troncon(new Point(2, 140, 420),
                                                 11218,
                                                 new Point(1, 195, 291),
                                                 39, "v0", 4));

            tronconsAttendus.Add(5, new Troncon(new Point(6, 244, 277),
                                                 4076,
                                                 new Point(1, 195, 291),
                                                 40, "h1", 5));

            tronconsAttendus.Add(6, new Troncon(new Point(1, 195, 291),
                                                 11218,
                                                 new Point(2, 140, 420),
                                                 38, "v0", 6));

            tronconsAttendus.Add(7, new Troncon(new Point(3, 132, 470),
                                                 4050,
                                                 new Point(2, 140, 420),
                                                 40, "v0", 7));

            tronconsAttendus.Add(8, new Troncon(new Point(7, 244, 345),
                                                 10257,
                                                 new Point(2, 140, 420),
                                                 38, "h2", 8));

            tronconsAttendus.Add(9, new Troncon(new Point(2, 140, 420),
                                                 4050,
                                                 new Point(3, 132, 470),
                                                 40, "v0", 9));

            tronconsAttendus.Add(10, new Troncon(new Point(4, 128, 660),
                                                 15203,
                                                 new Point(3, 132, 470),
                                                 46, "v0", 10));

            tronconsAttendus.Add(11, new Troncon(new Point(3, 132, 470),
                                                 15203,
                                                 new Point(4, 128, 660),
                                                 44, "v0", 11));

            tronconsAttendus.Add(12, new Troncon(new Point(9, 205, 629),
                                                 6640,
                                                 new Point(4, 128, 660),
                                                 41, "h4", 12));

            tronconsAttendus.Add(13, new Troncon(new Point(0, 134, 193),
                                                 13666,
                                                 new Point(5, 263, 81),
                                                 43, "h0", 13));

            tronconsAttendus.Add(14, new Troncon(new Point(10, 382, 165),
                                                 11652,
                                                 new Point(5, 263, 81),
                                                 46, "h0", 14));

            tronconsAttendus.Add(15, new Troncon(new Point(1, 195, 291),
                                                 4076,
                                                 new Point(6, 244, 277),
                                                 44, "h1", 15));

            tronconsAttendus.Add(16, new Troncon(new Point(7, 244, 345),
                                                 5440,
                                                 new Point(6, 244, 277),
                                                 43, "v1", 16));

            tronconsAttendus.Add(17, new Troncon(new Point(6, 244, 277),
                                                 5440,
                                                 new Point(7, 244, 345),
                                                 46, "v1", 17));

            tronconsAttendus.Add(18, new Troncon(new Point(2, 140, 420),
                                                 10257,
                                                 new Point(7, 244, 345),
                                                 38, "h2", 18));

            tronconsAttendus.Add(19, new Troncon(new Point(8, 300, 506),
                                                 13636,
                                                 new Point(7, 244, 345),
                                                 39, "v1", 19));

            tronconsAttendus.Add(20, new Troncon(new Point(12, 433, 455),
                                                 17494,
                                                 new Point(7, 244, 345),
                                                 43, "h2", 20));

            tronconsAttendus.Add(21, new Troncon(new Point(7, 244, 345),
                                                 13636,
                                                 new Point(8, 300, 506),
                                                 44, "v1", 21));

            tronconsAttendus.Add(22, new Troncon(new Point(13, 376, 591),
                                                 9121,
                                                 new Point(8, 300, 506),
                                                 43, "h3", 22));

            tronconsAttendus.Add(23, new Troncon(new Point(4, 128, 660),
                                                 6640,
                                                 new Point(9, 205, 629),
                                                 45, "h4", 23));

            tronconsAttendus.Add(24, new Troncon(new Point(5, 263, 81),
                                                 11652,
                                                 new Point(10, 382, 165),
                                                 38, "h0", 24));

            tronconsAttendus.Add(25, new Troncon(new Point(11, 378, 209),
                                                 3534,
                                                 new Point(10, 382, 165),
                                                 46, "v2", 25));

            tronconsAttendus.Add(26, new Troncon(new Point(15, 522, 136),
                                                 11437,
                                                 new Point(10, 382, 165),
                                                 39, "h0", 26));

            tronconsAttendus.Add(27, new Troncon(new Point(10, 382, 165),
                                                 3534,
                                                 new Point(11, 378, 209),
                                                 44, "v2", 27));

            tronconsAttendus.Add(28, new Troncon(new Point(12, 433, 455),
                                                 20165,
                                                 new Point(11, 378, 209),
                                                 38, "v2", 28));

            tronconsAttendus.Add(29, new Troncon(new Point(16, 488, 320),
                                                 12501,
                                                 new Point(11, 378, 209),
                                                 47, "h1", 29));

            tronconsAttendus.Add(30, new Troncon(new Point(11, 378, 209),
                                                 20165,
                                                 new Point(12, 433, 455),
                                                 45, "v2", 30));

            tronconsAttendus.Add(31, new Troncon(new Point(7, 244, 345),
                                                 17494,
                                                 new Point(12, 433, 455),
                                                 42, "h2", 31));

            tronconsAttendus.Add(32, new Troncon(new Point(13, 376, 591),
                                                 11796,
                                                 new Point(12, 433, 455),
                                                 39, "v2", 32));

            tronconsAttendus.Add(33, new Troncon(new Point(12, 433, 455),
                                                 11796,
                                                 new Point(13, 376, 591),
                                                 47, "v2", 33));

            tronconsAttendus.Add(34, new Troncon(new Point(8, 300, 506),
                                                 9121,
                                                 new Point(13, 376, 591),
                                                 45, "h3", 34));

            tronconsAttendus.Add(35, new Troncon(new Point(14, 424, 655),
                                                 6400,
                                                 new Point(13, 376, 591),
                                                 46, "v2", 35));

            tronconsAttendus.Add(36, new Troncon(new Point(13, 376, 591),
                                                 6400,
                                                 new Point(14, 424, 655),
                                                 47, "v2", 36));

            tronconsAttendus.Add(37, new Troncon(new Point(10, 382, 165),
                                                 11437,
                                                 new Point(15, 522, 136),
                                                 40, "h0", 37));

            tronconsAttendus.Add(38, new Troncon(new Point(20, 636, 184),
                                                 9895,
                                                 new Point(15, 522, 136),
                                                 43, "h0", 38));

            tronconsAttendus.Add(39, new Troncon(new Point(11, 378, 209),
                                                 12501,
                                                 new Point(16, 488, 320),
                                                 45, "h1", 39));

            tronconsAttendus.Add(40, new Troncon(new Point(17, 470, 435),
                                                 9312,
                                                 new Point(16, 488, 320),
                                                 38, "v3", 40));

            tronconsAttendus.Add(41, new Troncon(new Point(21, 651, 255),
                                                 14038,
                                                 new Point(16, 488, 320),
                                                 41, "h1", 41));

            tronconsAttendus.Add(42, new Troncon(new Point(16, 488, 320),
                                                 9312,
                                                 new Point(17, 470, 435),
                                                 45, "v3", 42));

            tronconsAttendus.Add(43, new Troncon(new Point(18, 546, 583),
                                                 13309,
                                                 new Point(17, 470, 435),
                                                 40, "v3", 43));

            tronconsAttendus.Add(44, new Troncon(new Point(17, 470, 435),
                                                 13309,
                                                 new Point(18, 546, 583),
                                                 47, "v3", 44));

            tronconsAttendus.Add(45, new Troncon(new Point(19, 572, 604),
                                                 2673,
                                                 new Point(18, 546, 583),
                                                 39, "v3", 45));

            tronconsAttendus.Add(46, new Troncon(new Point(18, 546, 583),
                                                 2673,
                                                 new Point(19, 572, 604),
                                                 41, "v3", 46));

            tronconsAttendus.Add(47, new Troncon(new Point(24, 652, 684),
                                                 9050,
                                                 new Point(19, 572, 604),
                                                 42, "h4", 47));

            tronconsAttendus.Add(48, new Troncon(new Point(15, 522, 136),
                                                 9895,
                                                 new Point(20, 636, 184),
                                                 42, "h0", 48));

            tronconsAttendus.Add(49, new Troncon(new Point(16, 488, 320),
                                                 14038,
                                                 new Point(21, 651, 255),
                                                 42, "h1", 49));

            tronconsAttendus.Add(50, new Troncon(new Point(22, 619, 355),
                                                 8399,
                                                 new Point(21, 651, 255),
                                                 41, "v4", 50));

            tronconsAttendus.Add(51, new Troncon(new Point(21, 651, 255),
                                                 8399,
                                                 new Point(22, 619, 355),
                                                 38, "v4", 51));

            tronconsAttendus.Add(52, new Troncon(new Point(23, 705, 536),
                                                 16031,
                                                 new Point(22, 619, 355),
                                                 39, "v4", 52));

            tronconsAttendus.Add(53, new Troncon(new Point(22, 619, 355),
                                                 16031,
                                                 new Point(23, 705, 536),
                                                 44, "v4", 53));

            tronconsAttendus.Add(54, new Troncon(new Point(24, 652, 684),
                                                 12576,
                                                 new Point(23, 705, 536),
                                                 43, "v4", 54));

            tronconsAttendus.Add(55, new Troncon(new Point(23, 705, 536),
                                                 12576,
                                                 new Point(24, 652, 684),
                                                 45, "v4", 55));

            tronconsAttendus.Add(56, new Troncon(new Point(19, 572, 604),
                                                 9050,
                                                 new Point(24, 652, 684),
                                                 45, "h4", 56));

            structAttendue.Xmax = 705;
            structAttendue.Xmin = 128;
            structAttendue.Ymax = 684;
            structAttendue.Ymin = 81;
            structAttendue.HashTroncon = tronconsAttendus;
            structAttendue.HashPoint = pointsAttendus;

            // Tests d'égalité
            Assert.AreEqual(structAttendue.Xmax, structRetournee.Xmax);
            Assert.AreEqual(structAttendue.Xmin, structRetournee.Xmin);
            Assert.AreEqual(structAttendue.Ymax, structRetournee.Ymax);
            Assert.AreEqual(structAttendue.Ymin, structRetournee.Ymin);
            foreach (var key in structAttendue.HashPoint.Keys)
            {
                Assert.AreEqual(structAttendue.HashPoint[key].id, structRetournee.HashPoint[key].id);
                Assert.AreEqual(structAttendue.HashPoint[key].x, structRetournee.HashPoint[key].x);
                Assert.AreEqual(structAttendue.HashPoint[key].y, structRetournee.HashPoint[key].y);
            }
            foreach(var key in structAttendue.HashTroncon.Keys)
            {
                Assert.AreEqual(structAttendue.HashTroncon[key].Id, structRetournee.HashTroncon[key].Id);
                Assert.AreEqual(structAttendue.HashTroncon[key].Length, structRetournee.HashTroncon[key].Length);
                Assert.AreEqual(structAttendue.HashTroncon[key].Speed, structRetournee.HashTroncon[key].Speed);
                Assert.AreEqual(structAttendue.HashTroncon[key].StreetName, structRetournee.HashTroncon[key].StreetName);
                Assert.AreEqual(structAttendue.HashTroncon[key].Origin.id, structRetournee.HashTroncon[key].Origin.id);
                Assert.AreEqual(structAttendue.HashTroncon[key].Origin.x, structRetournee.HashTroncon[key].Origin.x);
                Assert.AreEqual(structAttendue.HashTroncon[key].Origin.y, structRetournee.HashTroncon[key].Origin.y);
                Assert.AreEqual(structAttendue.HashTroncon[key].Destination.id, structRetournee.HashTroncon[key].Destination.id);
                Assert.AreEqual(structAttendue.HashTroncon[key].Destination.x, structRetournee.HashTroncon[key].Destination.x);
                Assert.AreEqual(structAttendue.HashTroncon[key].Destination.y, structRetournee.HashTroncon[key].Destination.y);
            }
        }

        public void TestParserXml_Livraison()
        {

        }
    }
}
