﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PaintTool
{
    public class PaintColor
    {
        static byte[] colorData;
        
        public PaintColor()
        {
            colorData = new byte[] { 0, 0, 0, 255};
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

        //public void SetColor(byte blue, byte green, byte red, byte alpha = 255)
        //{
        //    colorData = new byte[] { blue, green, red, alpha };
        //}        
    }
}
