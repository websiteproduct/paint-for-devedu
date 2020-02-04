using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using PaintTool.Creators;

namespace PaintToolTests
{
    [TestFixture]
    public class Tests
    {

        // ¬¿ƒ–¿“
        [TestCase("point1", "point2", "pointList1")]
        [TestCase("point3", "point4", "pointList2")]
        [TestCase("pointStartSquer1", "pointFinishSquer1", "pointListSquer1")]
        public void CreateSquereTest(string pStartName, string pFinishName, string expectedPointsListName)
        {
            RectCreator rect = new RectCreator(true);
            Point pStart = GetPointByName(pStartName);
            Point pFinish = GetPointByName(pFinishName);
            List<Point> expected = GetPointsListByName(expectedPointsListName);

            List<Point> actual = rect.CreateShape(pStart, pFinish).FigurePoints;

            CollectionAssert.AreEqual(expected, actual);
        }

        //œ–ﬂÃŒ”√ŒÀ‹Õ» 
        [TestCase("point5", "point6", "pointList3")]
        [TestCase("point7", "point8", "pointList4")]
        [TestCase("point9", "point10", "pointList5")]
        [TestCase("point11", "point12", "pointList6")]
        public void CreateIsoscaleRectangleTest(string pStartName, string pFinishName, string expectedPointsListName)
        {
            RectCreator rect = new RectCreator(false);
            Point pStart = GetPointByName(pStartName);
            Point pFinish = GetPointByName(pFinishName);
            List<Point> expected = GetPointsListByName(expectedPointsListName);

            List<Point> actual = rect.CreateShape(pStart, pFinish).FigurePoints;

            CollectionAssert.AreEqual(expected, actual);
        }

        //–¿¬ÕŒ¡≈ƒ–≈ÕÕ€… “–≈”√ŒÀ‹Õ» 
        [TestCase("pointStartIsoscaleTriangle1", "pointFinishIsoscaleTriangle1", "pointListIsoscaleTriangle1")]
        [TestCase("pointStartIsoscaleTriangle2", "pointFinishIsoscaleTriangle2", "pointListIsoscaleTriangle2")]
        //[TestCase("point9", "point10", "pointList5")]
        //[TestCase("point11", "point12", "pointList6")]
        public void CreateIsoscaleTriangleTest(string pStartName, string pFinishName, string expectedPointsListName)
        {
            TriangleCreator tr = new TriangleCreator(false);
            Point pStart = GetPointByName(pStartName);
            Point pFinish = GetPointByName(pFinishName);
            List<Point> expected = GetPointsListByName(expectedPointsListName);

            List<Point> actual = tr.CreateShape(pStart, pFinish).FigurePoints;

            CollectionAssert.AreEqual(expected, actual);
        }

        //œ–ﬂÃŒ”√ŒÀ‹Õ€… “–≈”√ŒÀ‹Õ» 
        [TestCase("pointStartRectangularTriangle1", "pointFinishRectangularTriangle1", "pointListRectangularTriangle1")]
        [TestCase("pointStartRectangularTriangle3", "pointFinishRectangularTriangle4", "pointListRectangularTriangle2")]
        [TestCase("pointStartRectangularTriangle5", "pointFinishRectangularTriangle6", "pointListRectangularTriangle3")]
        [TestCase("pointStartRectangularTriangle7", "pointFinishRectangularTriangle8", "pointListRectangularTriangle4")]
        [TestCase("pointStartRectangularTriangle9", "pointFinishRectangularTriangle10", "pointListRectangularTriangle5")]
        [TestCase("pointStartRectangularTriangle11", "pointFinishRectangularTriangle12", "pointListRectangularTriangle6")]
        public void CreateRectangularTriangleTest(string pStartName, string pFinishName, string expectedPointsListName)
        {
            TriangleCreator tr = new TriangleCreator(true);
            Point pStart = GetPointByName(pStartName);
            Point pFinish = GetPointByName(pFinishName);
            List<Point> expected = GetPointsListByName(expectedPointsListName);

            List<Point> actual = tr.CreateShape(pStart, pFinish).FigurePoints;

            CollectionAssert.AreEqual(expected, actual);
        }


        // ÃÕŒ√Œ”√ŒÀ‹Õ» 
        [TestCase("pointStartPolugon1", "pointFinishPolygon1", 3, "pointListPolygon1")]
        [TestCase("pointStartPolugon2", "pointFinishPolygon2", 5, "pointListPolygon2")]
        [TestCase("pointStartPolugon3", "pointFinishPolygon3", 8, "pointListPolygon3")]
        public void CreatePolygonTriangleTest(string pStartName, string pFinishName, int sides, string expectedPointsListName)
        {
            PolygonCreator polygon = new PolygonCreator(sides);
            Point pStart = GetPointByName(pStartName);
            Point pFinish = GetPointByName(pFinishName);
            List<Point> expected = GetPointsListByName(expectedPointsListName);

            List<Point> actual = polygon.CreateShape(pStart, pFinish).FigurePoints;

            CollectionAssert.AreEqual(expected, actual);
        }

        // –”√
        [TestCase("pointStartCircle1", "pointFinishCircle1", "pointListCircle1")]
        public void CreateCircleTest(string pStartName, string pFinishName, string expectedPointsListName)
        {
            CircleCreator rect = new CircleCreator(true);
            Point pStart = GetPointByName(pStartName);
            Point pFinish = GetPointByName(pFinishName);
            List<Point> expected = GetPointsListByName(expectedPointsListName);

            List<Point> actual = rect.CreateShape(pStart, pFinish).FigurePoints;

            CollectionAssert.AreEqual(expected, actual);
        }

        //›ÀÀ»œ—
        [TestCase("pointStartEllipce1", "pointFinishEllipce1", "pointListEllipce1")]
        public void CreateEllipseTest(string pStartName, string pFinishName, string expectedPointsListName)
        {
            CircleCreator rect = new CircleCreator(false);
            Point pStart = GetPointByName(pStartName);
            Point pFinish = GetPointByName(pFinishName);
            List<Point> expected = GetPointsListByName(expectedPointsListName);

            List<Point> actual = rect.CreateShape(pStart, pFinish).FigurePoints;

            CollectionAssert.AreEqual(expected, actual);
        }
        public Point GetPointByName(string name)
        {
            switch (name)
            {
                case "point1":
                    return new Point(1, 4);
                case "point2":
                    return new Point(4, 50);
                case "point3":
                    return new Point(-450, -100);
                case "point4":
                    return new Point(-300, 0);
                case "point5":
                    return new Point(5, 10);
                case "point6":
                    return new Point(10, 5);
                case "point7":
                    return new Point(0, 0);
                case "point8":
                    return new Point(0, 0);
                case "point9":
                    return new Point(0, -5);
                case "point10":
                    return new Point(-5, 0);
                case "point11":
                    return new Point(800, 0);
                case "point12":
                    return new Point(0, 800);
                case "pointStartSquer1":
                    return new Point(20, 40);
                case "pointFinishSquer1":
                    return new Point(30, 100);
                case "pointStartIsoscaleTriangle1":
                    return new Point(10, 20);
                case "pointFinishIsoscaleTriangle1":
                    return new Point(30, 10);
                case "pointStartIsoscaleTriangle2":
                    return new Point(129, 170);
                case "pointFinishIsoscaleTriangle2":
                    return new Point(331, 40);

                case "pointStartRectangularTriangle1":
                    return new Point(2, 3);
                case "pointFinishRectangularTriangle1":
                    return new Point(5, 1);
                case "pointStartRectangularTriangle3":
                    return new Point(0, 0);
                case "pointFinishRectangularTriangle4":
                    return new Point(-100, 100);
                case "pointStartRectangularTriangle5":
                    return new Point(0, 0);
                case "pointFinishRectangularTriangle6":
                    return new Point(0, 0);
                case "pointStartRectangularTriangle7":
                    return new Point(0, 900);
                case "pointFinishRectangularTriangle8":
                    return new Point(-600, 0);
                case "pointStartRectangularTriangle9":
                    return new Point(-230, -800);
                case "pointFinishRectangularTriangle10":
                    return new Point(-900, -700);
                case "pointStartRectangularTriangle11":
                    return new Point(-200, -200);
                case "pointFinishRectangularTriangle12":
                    return new Point(-200, -200);

                case "pointStartPolugon1":
                    return new Point(500, 200);
                case "pointFinishPolygon1":
                    return new Point(300, 100);
                case "pointStartPolugon2":
                    return new Point(50, 500);
                case "pointFinishPolygon2":
                    return new Point(400, 400);
                case "pointStartPolugon3":
                    return new Point(300, 300);
                case "pointFinishPolygon3":
                    return new Point(400, 400);


                case "pointStartCircle1":
                    return new Point(0, 0);
                case "pointFinishCircle1":
                    return new Point(3, 4);

                case "pointStartEllipce1":
                    return new Point(8, 3);
                case "pointFinishEllipce1":
                    return new Point(10, 6);

                default:
                    return new Point();
            }

        }

        public List<Point> GetPointsListByName(string name)
        {
            switch (name)
            {
                case "pointList1":
                    return new List<Point>() {
                        new Point(1,4),
                        new Point(4,4),
                        new Point(4,7),
                        new Point(1,7),
                    };
                case "pointList2":
                    return new List<Point>() {
                        new Point(-450,-100),
                        new Point(-300,-100),
                        new Point(-300,50),
                        new Point(-450,50),
                    };
                case "pointList3":
                    return new List<Point>() {
                        new Point(5,10),
                        new Point(5,5),
                        new Point(10,5),
                        new Point(10,10),
                    };
                case "pointList4":
                    return new List<Point>() {
                        new Point(0,0),
                        new Point(0,0),
                        new Point(0,0),
                        new Point(0,0),
                    };
                case "pointList5":
                    return new List<Point>() {
                        new Point(0, -5),
                        new Point(0,0),
                        new Point(-5,0),
                        new Point(-5,-5),
                    };
                case "pointList6":
                    return new List<Point>() {
                        new Point(800,0),
                        new Point(800,800),
                        new Point(0,800),
                        new Point(0,0),
                    };
                case "pointListSquer1":
                    return new List<Point>() {
                        new Point(20,40),
                        new Point(30,40),
                        new Point(30,50),
                        new Point(20,50),
                    };
                case "pointListIsoscaleTriangle1":
                    return new List<Point>() {
                        new Point(10,20),
                        new Point(30,10),
                        new Point(10,20),
                        new Point(10,10),
                        new Point(10,10),
                        new Point(30,10),
                    };
                case "pointListIsoscaleTriangle2":
                    return new List<Point>() {
                        new Point(129,170),
                        new Point(331,40),
                        new Point(129,170),
                        new Point(129,40),
                        new Point(129,40),
                        new Point(331,40),
                    };


                case "pointListRectangularTriangle1":
                    return new List<Point>() {
                        new Point(2,3),
                        new Point(5,1),
                        new Point(2,3),
                        new Point(-1,1),
                        new Point(-1,1),
                        new Point(5,1),
                    };
                case "pointListRectangularTriangle2":
                    return new List<Point>() {
                        new Point(0,0),
                        new Point(-100,100),
                        new Point(0,0),
                        new Point(100,100),
                        new Point(100,100),
                        new Point(-100,100),
                    };
                case "pointListRectangularTriangle3":
                    return new List<Point>() {
                        new Point(0,0),
                        new Point(0,0),
                        new Point(0,0),
                        new Point(0,0),
                        new Point(0,0),
                        new Point(0,0),
                    };
                case "pointListRectangularTriangle4":
                    return new List<Point>() {
                        new Point(0,900),
                        new Point(-600,0),
                        new Point(0,900),
                        new Point(600,0),
                        new Point(600,0),
                        new Point(-600,0),
                    };
                case "pointListRectangularTriangle5":
                    return new List<Point>() {
                        new Point(-230,-800),
                        new Point(-900,-700),
                        new Point(-230,-800),
                        new Point(440,-700),
                        new Point(440,-700),
                        new Point(-900,-700),
                    };
                case "pointListRectangularTriangle6":
                    return new List<Point>() {
                        new Point(-200,-200),
                        new Point(-200,-200),
                        new Point(-200,-200),
                        new Point(-200,-200),
                        new Point(-200,-200),
                        new Point(-200,-200),
                    };
                case "pointListPolygon1":
                    return new List<Point>() {
                        new Point(300, 100),
                        new Point(513, 423),
                        new Point(687, 77)
                    };
                case "pointListPolygon2":
                    return new List<Point>() {
                        new Point(400, 400),
                        new Point(63, 136),
                        new Point(-292, 375),
                        new Point(-174, 787),
                        new Point(253, 802)
                    };
                case "pointListPolygon3":
                    return new List<Point>() {
                        new Point(400, 400),
                        new Point(441, 300),
                        new Point(400, 200),
                        new Point(300, 159),
                        new Point(200, 200),
                        new Point(159, 300),
                        new Point(200, 400),
                        new Point(300, 441)
                    };
                case "pointListCircle1":
                    return new List<Point>() {
                        //ÔÂ‚˚È ÙÓ
                        new Point(-0,-5),
                        new Point(1,(int)-4.8989794855663561963945681494118),
                        new Point(2,(int)-4.582575694955840006588047193728),
                        new Point(3,-4),
                        //‚ÚÓÓÈ ÙÓ
                        new Point(4,-3),
                        new Point((int)4.582575694955840006588047193728,-2),
                        new Point((int)4.8989794855663561963945681494118,-1),
                        //ÚÂÚËÈ ÙÓ
                        new Point(5,0),
                        new Point((int)4.8989794855663561963945681494118,1),
                        new Point((int)4.582575694955840006588047193728,2),
                        new Point(4,3),
                        //˜ÂÚ‚ÂÚ˚È ÙÓ
                        new Point(3,4),
                        new Point(2,(int)4.582575694955840006588047193728),
                        new Point(1,(int)4.8989794855663561963945681494118),
                        //ÔˇÚ˚È ÙÓ
                        new Point(0,5),
                        new Point(-1,(int)4.8989794855663561963945681494118),
                        new Point(-2,(int)4.582575694955840006588047193728),
                        new Point(-3,4),
                        //¯ÂÒÚÓÈ ÙÓ
                        new Point(-4,3),
                        new Point((int)-4.582575694955840006588047193728,2),
                        new Point((int)-4.8989794855663561963945681494118,1),
                        //ÒÂ‰¸ÏÓÈ ÙÓ
                        new Point(-5,0),
                        new Point((int)-4.8989794855663561963945681494118,-1),
                        new Point((int)-4.582575694955840006588047193728,-2),
                        new Point(-4,-3),
                        //‚ÓÒ¸ÏÓÈ ÙÓ
                        new Point(-3,-4),
                        new Point(-2,(int)-4.582575694955840006588047193728),
                        new Point(-1,(int)-4.8989794855663561963945681494118),

                    };

                case "pointListEllipce1":
                    return new List<Point>() {
                        new Point(8,0),
                        new Point(9,0),
                        new Point(9,0),
                        new Point(9,1),
                        new Point(9,2),
                        new Point(10,3),
                        new Point(9,4),
                        new Point(9,5),
                        new Point(9,5),
                        new Point(8,5),
                        new Point(8,6),
                        new Point(7,5),
                        new Point(6,5),
                        new Point(6,4),
                        new Point(6,3),
                        new Point(6,3),
                        new Point(6,2),
                        new Point(6,1),
                        new Point(6,0),
                        new Point(7,0),
                    };



                default:
                    return new List<Point>();
            }
        }
    }
}
