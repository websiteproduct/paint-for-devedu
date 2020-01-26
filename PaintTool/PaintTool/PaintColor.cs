using System;
using System.Collections.Generic;
using System.Text;

namespace PaintTool
{
    public class PaintColor
    {
        byte[] colorData;
        
        public PaintColor()
        {
            colorData = new byte[] { 255, 255, 255, 255};
        }
        public void SetColor(byte blue, byte green, byte red, byte alpha = 255)
        {
            colorData = new byte[] { blue, green, red, alpha };
        }
        public byte[] GetColor()        
        {            
            return colorData;
        }
    }
}
