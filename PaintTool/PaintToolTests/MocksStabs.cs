using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaintTool
{
    [TestFixture]
    public class TesMocksts
    {
        [Test]
        public void PickedColorTest()
        {
            var mock = new MockTest();
            var currentColor = new PaintColor();

            PaintColor.ColorData = new byte[] { 0, 0, 255, 255 };
            mock.PickedColor();

            Assert.That(mock.ColorData, Is.EqualTo(PaintColor.ColorData));
        }
    }

    public class MockTest
        {
        static byte[] colorData;

        public byte[] ColorData
        {
            get
            {
                return colorData;
            }
            set
            {
                colorData = value;
            }
        }

        public void PickedColor()
        {
            System.Windows.Media.Color clr = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("Red");
            colorData = new byte[] { clr.B, clr.G, clr.R, 255 };
        }

    }
}
