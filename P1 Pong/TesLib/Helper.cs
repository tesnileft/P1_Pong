using System;
using System.Numerics;
using Microsoft.Xna.Framework;
using Vector3 = System.Numerics.Vector3;

namespace TestLib.Helper
{
    public static class Helper
    {
        public static Color HsvToCol(float h, float s, float v)
        {
            float c = v * s;
            float hi = h / 60;
            float x = c * (1 - Math.Abs((float)hi % 2 - 1) );
            var rgb1 = new Vector3(0f, 0f, 0f);
            switch (hi)
            {
                case <1 : rgb1 = new Vector3(c, x, 0); break;
                case <2 : rgb1 = new Vector3(x, c, 0); break;
                case <3 : rgb1 = new Vector3(0, c, x); break;
                case <4 : rgb1 = new Vector3(0, x, c); break;
                case <5 : rgb1 = new Vector3(x, 0, c); break;
                case <6 : rgb1 = new Vector3(c, 0, x); break;
            }
            var m = v - c;
            return new Color(rgb1.X + m, rgb1.Y + m, rgb1.Z + m);
        }
        public static Point VecToPoint(this System.Numerics.Vector2 vec, float scale = 1)
        {
            return new Point((int)(vec.X * scale), (int)(vec.Y * scale));
        }
    }
    
}