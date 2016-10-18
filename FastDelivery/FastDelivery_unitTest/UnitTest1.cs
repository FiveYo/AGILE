using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FastDelivery_Library;
using System.Collections.Generic;

namespace FastDelivery_unitTest
{
    [TestClass]
    public class UniTest1
    {
        [TestMethod]
        public void TestParserXml_Livraison()
        {
            FileStream planFile = new FileStream(@"..\..\..\..\XMLExample\plan5x5.xml", FileMode.Open, FileAccess.Read);
            FileStream livraisonFile = new FileStream(@"..\..\..\..\XMLExample\livraison5x5-4.xml", FileMode.Open, FileAccess.Read);
            StructPlan structPlan = Outils.ParserXml_Plan(planFile);
            StructLivraison livRetournee = Outils.ParserXml_Livraison(livraisonFile, structPlan.HashPoint);
            StructLivraison livAttendu = new StructLivraison();
        }
    }
}