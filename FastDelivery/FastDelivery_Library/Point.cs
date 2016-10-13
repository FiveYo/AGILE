
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDelivery_Library
{
    public class Point
    {
        public int id, x, y;

        public Point(int ID , int X, int Y)
        {
            this.id = ID;
            this.x = X;
            this.y = Y;

        }
        public static int AfficherCoordx(Point pt)
        {
            return pt.x;
        }

    }
}
