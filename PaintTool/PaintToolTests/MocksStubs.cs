using NUnit.Framework;
using PaintTool.figures;
using PaintTool.Surface;
using PaintTool.Thickness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PaintTool
{
    [TestFixture]
    public class TestPickedColor
    {
        [Test]
        public void PickedColorTest()
        {
            var mock = new PickedColorMock();
            mock.PickedColor();

            Assert.That(mock.ColorData, Is.EqualTo(PaintColor.ColorData));
        }
    }

    public class PickedColorMock
    {
        byte[] colorData;
        public byte[] ColorData { 
            get
            {
                return colorData;
            }
        }

        public void PickedColor()
        {
            System.Windows.Media.Color clr = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("Red");
            colorData = new byte[] { clr.B, clr.G, clr.R, 255 };
            PaintColor.ColorData = colorData;
        }
    }

    public class ThickenssTest
    {
        public List<Point> dotsList;
        

    }

}
