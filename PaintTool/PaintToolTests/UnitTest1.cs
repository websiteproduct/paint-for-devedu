using NUnit.Framework;
using System;

namespace PaintToolTests
{
    [TestFixture]
    public class Tests
    {

        [TestCase(0,1,0,5,0,4)]
        [TestCase(0,0,0,0,0,0)]
        public void LineTestTrue(int Ax, int Ay, int Bx, int By,int Cx, int Cy)
        {

            // (Cy - Ay)  * (Bx - Ax) = (By - Ay) * (Cx - Ax)

            Assert.IsTrue(PointOnLineSegment(Ax,Ay,Bx,By,Cx,Cy));

        }
        
        [TestCase(1, 1, 2, 2, 3, 3)]
        [TestCase(1, 0, 2, 6, 8, 3)]
        public void LineTestFalse(int Ax, int Ay, int Bx, int By, int Cx, int Cy)
        {

            // (Cy - Ay)  * (Bx - Ax) = (By - Ay) * (Cx - Ax)

            Assert.IsFalse(PointOnLineSegment(Ax, Ay, Bx, By, Cx, Cy));

        }

        public static bool PointOnLineSegment(int Ax, int Ay, int Bx, int By, int Cx, int Cy, double epsilon = 0.001)
        {
            if (Cx - Math.Max(Ax, Bx) > epsilon ||
                Math.Min(Ax, Bx) - Cx > epsilon ||
                Cy - Math.Max(Ay, By) > epsilon ||
                Math.Min(Ay, By) - Cy > epsilon)
                return false;

            if (Math.Abs(Bx - Ax) < epsilon)
                return Math.Abs(Ax - Cx) < epsilon || Math.Abs(Bx - Cx) < epsilon;
            if (Math.Abs(By - Ay) < epsilon)
                return Math.Abs(Ay - Cy) < epsilon || Math.Abs(By - Cy) < epsilon;

            double x = Ax + (Cy - Ay) * (Bx - Ax) / (By - Ay);
            double y = Ay + (Cx - Ax) * (By - Ay) / (Bx - Ax);

            return Math.Abs(Cx - x) < epsilon || Math.Abs(Cy - y) < epsilon;

        }
    }
}