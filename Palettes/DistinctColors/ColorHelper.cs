using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace TransportLinesManager.Palettes.DistinctColors
{
    public static class ColorHelper
    {
        public static readonly Vector3d Ref = new Vector3d(96.422, 100.0, 82.521);

        public static Color32 Hex2Rgb(this string hex)
        {
            if (hex.Length != 6) throw new ArgumentException(hex);
            var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }

        public static string Rgb2Hex(this Color32 rgb)
        {
            return $"{rgb.r:X2}{rgb.g:X2}{rgb.b:X2}";
        }

        public static Vector3d Rgb2Hsv(this Color32 rgb)
        {
            var r = rgb.r / 255;
            var g = rgb.g / 255;
            var b = rgb.b / 255;
            int h, min;

            var max = min = r;
            if (g > max) max = g; if (g < min) min = g;
            if (b > max) max = b; if (b < min) min = b;
            var d = max - min;
            var v = max;
            var s = (max > 0) ? d / max : 0;

            if (s == 0) h = 0;
            else
            {
                h = 60 * ((r == max) ? (g - b) / d : ((g == max) ? 2 + (b - r) / d : 4 + (r - g) / d));
                if (h < 0) h += 360;
            }

            return new Vector3d(h, s, v);
        }

        public static Color32 Hsv2Rgb(this Vector3d hsv)
        {
            var h = hsv.X;
            var s = hsv.Y;
            var v = hsv.Z;
            double r = 0, g = 0, b = 0, i, f, p, q, t;
            while (h < 0) h += 360;
            h %= 360;
            s = s > 1 ? 1 : s < 0 ? 0 : s;
            v = v > 1 ? 1 : v < 0 ? 0 : v;

            if (s == 0) r = g = b = v;
            else
            {
                h /= 60;
                f = h - (i = Math.Floor(h));
                p = v * (1 - s);
                q = v * (1 - s * f);
                t = v * (1 - s * (1 - f));
                switch (i)
                {
                    case 0: r = v; g = t; b = p; break;
                    case 1: r = q; g = v; b = p; break;
                    case 2: r = p; g = v; b = t; break;
                    case 3: r = p; g = q; b = v; break;
                    case 4: r = t; g = p; b = v; break;
                    case 5: r = v; g = p; b = q; break;
                }
            }

            return new Color32((byte)(r * 255.0), (byte)(g * 255.0), (byte)(b * 255.0), 255);
        }

        public static Vector3d Rgb2Xyz(this Color32 Color32)
        {
            var r = Color32.r / 255.0;
            var g = Color32.g / 255.0;
            var b = Color32.b / 255.0;
            r = 100 * ((r > 0.04045) ? Math.Pow((r + 0.055) / 1.055, 2.4) : r / 12.92);
            g = 100 * ((g > 0.04045) ? Math.Pow((g + 0.055) / 1.055, 2.4) : g / 12.92);
            b = 100 * ((b > 0.04045) ? Math.Pow((b + 0.055) / 1.055, 2.4) : b / 12.92);
            return new Vector3d(r * 0.4124 + g * 0.3576 + b * 0.1805, r * 0.2126 + g * 0.7152 + b * 0.0722, r * 0.0193 + g * 0.1192 + b * 0.9505);
        }

        public static Vector3d Xyz2Rgb(this Vector3d xyz)
        {
            var x = xyz.X / 100;        //X from 0 to  95.047      (Observer = 2°, Illuminant = D65)
            var y = xyz.Y / 100;        //Y from 0 to 100.000
            var z = xyz.Z / 100;        //Z from 0 to 108.883

            var r = x * 3.2406 + y * -1.5372 + z * -0.4986;
            var g = x * -0.9689 + y * 1.8758 + z * 0.0415;
            var b = x * 0.0557 + y * -0.2040 + z * 1.0570;

            return new Vector3d (
                x: 255 * (r > 0.0031308 ? (1.055 * Math.Pow(r, 1 / 2.4) - 0.055) : 12.92 * r),
                y: 255 * (g > 0.0031308 ? (1.055 * Math.Pow(g, 1 / 2.4) - 0.055) : 12.92 * g),
                z: 255 * (b > 0.0031308 ? (1.055 * Math.Pow(b, 1 / 2.4) - 0.055) : 12.92 * b)
            );
        }

        public static Vector3d Xyz2Lab(this Vector3d xyz)
        {
            xyz /= Ref;
            var x = xyz.X;
            var y = xyz.Y;
            var z = xyz.Z;

            x = (x > 0.008856) ? Math.Pow(x, 1 / 3.0) : 7.787 * x + 16 / 116.0;
            y = (y > 0.008856) ? Math.Pow(y, 1 / 3.0) : 7.787 * y + 16 / 116.0;
            z = (z > 0.008856) ? Math.Pow(z, 1 / 3.0) : 7.787 * z + 16 / 116.0;

            return new Vector3d(
                x: 116 * y - 16,
                y: 500 * (x - y),
                z: 200 * (y - z)
            );
        }

        public static Vector3d Lab2Xyz(this Vector3d lab)
        {
            var y = (lab.X + 16) / 116;
            var x = lab.Y / 500 + y;
            var z = y - lab.Z / 200;

            y = Math.Pow(y, 3) > 0.008856 ? Math.Pow(y, 3) : (y - 16 / 116.0) / 7.787;
            x = Math.Pow(x, 3) > 0.008856 ? Math.Pow(x, 3) : (x - 16 / 116.0) / 7.787;
            z = Math.Pow(z, 3) > 0.008856 ? Math.Pow(z, 3) : (z - 16 / 116.0) / 7.787;

            return new Vector3d(x, y, z) * Ref;
        }

        public static Vector3d Rgb2Lab(this Color32 rgb) => Xyz2Lab(Rgb2Xyz(rgb));
        public static Color32 Lab2Rgb(this Vector3d lab) => Xyz2Rgb(Lab2Xyz(lab));

        public static Vector3d Hsv2Lab(this Vector3d hsv) => Rgb2Lab(Hsv2Rgb(hsv));

        public static double LabDistance(Vector3d lab1, Vector3d lab2, double lightness = 2, double chroma = 1)
        {
            var (deltaL, deltaA, deltaB) = lab1 - lab2;
            var C1 = Math.Sqrt(lab1.Y * lab1.Y + lab1.Z * lab1.Z);
            var C2 = Math.Sqrt(lab2.Y * lab2.Y + lab2.Z * lab2.Z);
            var deltaC = C1 - C2;
            var deltaH = Math.Sqrt(deltaA * deltaA + deltaB * deltaB - deltaC * deltaC);

            var H1 = (180 * Math.Atan2(lab1.Z, lab1.Y) / Math.PI + 360) % 360;

            var C1_4 = C1 * C1 * C1 * C1;
            var F = Math.Sqrt(C1_4 / (C1_4 + 1900));
            var T = (H1 > 345 || H1 < 164) ? (0.36 + Math.Abs(0.4 * Math.Cos(Math.PI * (H1 + 35) / 180))) : (0.56 + Math.Abs(0.2 * Math.Cos(Math.PI * (H1 + 168) / 180)));
            var SL = lab1.X < 16 ? 0.511 : (0.040975 * lab1.X) / (1 + 0.01765 * lab1.X);
            var SC = (0.0638 * C1) / (1 + 0.0131 * C1) + 0.638;
            var SH = SC * (F * T + 1 - F);
            return Math.Sqrt(Math.Pow(deltaL / (lightness * SL), 2) + Math.Pow(deltaC / (chroma * SC), 2) + Math.Pow(deltaH / SH, 2));
        }
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var rnd = new Random();
            return source.OrderBy((item) => rnd.Next());
        }

        public static IEnumerable<Vector3d> Interleave(IList<Vector3d> items, int parts = 1)
        {
            if (parts < 1) throw new ArgumentException();

            var stride = Math.Min(items.Count / parts, 1);
            var len = items.Count;
            for (var i = 0; i < stride; ++i)
            {
                for (var j = i; j < len; j += stride)
                {
                    yield return items[j];
                }
            }
        }
    }
}
