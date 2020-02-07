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
        [TestCase("pointStartSquere1", "pointFinishSquere1", "pointListSquere1")]
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
        public void CreateRectangleTest(string pStartName, string pFinishName, string expectedPointsListName)
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
        public void CreateIsoscelesTriangleTest(string pStartName, string pFinishName, string expectedPointsListName)
        {
            TriangleCreator tr = new TriangleCreator(false);
            Point pStart = GetPointByName(pStartName);
            Point pFinish = GetPointByName(pFinishName);
            List<Point> expected = GetPointsListByName(expectedPointsListName);

            List<Point> actual = tr.CreateShape(pStart, pFinish).FigurePoints;

            CollectionAssert.AreEqual(expected, actual);
        }

        //–¿¬ÕŒ—“Œ–ŒÕÕ»… “–≈”√ŒÀ‹Õ» 
        [TestCase("pointStartEquilateralTriangle1", "pointFinishEquilateralTriangle1", "pointListEquilateralTriangle1")]
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
        [TestCase("pointStartPolygon1", "pointFinishPolygon1", 3, "pointListPolygon1")]
        [TestCase("pointStartPolygon2", "pointFinishPolygon2", 5, "pointListPolygon2")]
        [TestCase("pointStartPolygon3", "pointFinishPolygon3", 8, "pointListPolygon3")]
        public void CreatePolygonTest(string pStartName, string pFinishName, int sides, string expectedPointsListName)
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
        [TestCase("pointStartEllipse1", "pointFinishEllipse1", "pointListEllipse1")]
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
                    return new Point(1, 1);
                case "point2":
                    return new Point(50, 4);

                case "point3":
                    return new Point(450, 100);
                case "point4":
                    return new Point(300, 0);

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

                case "pointStartSquere1":
                    return new Point(20, 40);
                case "pointFinishSquere1":
                    return new Point(30, 100);

                case "pointStartIsoscelesTriangle1":
                    return new Point(10, 20);
                case "pointFinishIsoscelesTriangle1":
                    return new Point(30, 10);

                case "pointStartIsoscelesTriangle2":
                    return new Point(129, 170);
                case "pointFinishIsoscelesTriangle2":
                    return new Point(331, 40);

                case "pointStartEquilateralTriangle1":
                    return new Point(2, 3);
                case "pointFinishEquilateralTriangle1":
                    return new Point(5, 1);
               

                case "pointStartPolygon1":
                    return new Point(500, 200);
                case "pointFinishPolygon1":
                    return new Point(300, 100);
                case "pointStartPolygon2":
                    return new Point(50, 500);
                case "pointFinishPolygon2":
                    return new Point(400, 400);
                case "pointStartPolygon3":
                    return new Point(300, 300);
                case "pointFinishPolygon3":
                    return new Point(400, 400);


                case "pointStartCircle1":
                    return new Point(0, 0);
                case "pointFinishCircle1":
                    return new Point(3, 4);

                case "pointStartEllipse1":
                    return new Point(8, 3);
                case "pointFinishEllipse1":
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
                        new Point(1,1),
                        new Point(50,1),
                        new Point(50,50),
                        new Point(1,50),
                    };
                case "pointList2":
                    return new List<Point>() {
                        new Point(450,100),
                        new Point(300,100),
                        new Point(300,-50),
                        new Point(450,-50),
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
                case "pointListSquere1":
                    return new List<Point>() {
                        new Point(20,40),
                        new Point(30,40),
                        new Point(30,50),
                        new Point(20,50),
                    };
                case "pointListIsoscelesTriangle1":
                    return new List<Point>() {
                        new Point(10,20),
                        new Point(30,10),
                        new Point(10,20),
                        new Point(10,10),
                        new Point(10,10),
                        new Point(30,10),
                    };
                case "pointListIsoscelesTriangle2":
                    return new List<Point>() {
                        new Point(129,170),
                        new Point(331,40),
                        new Point(129,170),
                        new Point(129,40),
                        new Point(129,40),
                        new Point(331,40),
                    };


                case "pointListEquilateralTriangle1":
                    return new List<Point>() {
                        new Point(2,3),
                        new Point(3,1),
                        new Point(5,3),
                    
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

                case "pointListEllipse1":
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
