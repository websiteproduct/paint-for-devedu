using System;
using System.Collections.Generic;
using System.Text;

namespace PaintTool
{
    public class FillColor
    {
        static byte[] colorData;

        public FillColor()
        {
            colorData = new byte[] { 0, 0, 255, 255 };
        }

        public static byte[] ColorData
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
    }
}
