using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TransportLinesManager.Palettes.DistinctColors
{
    public class DistinctColors
    {
        public List<Vector3d> Possibles { get; } = new List<Vector3d>();

        public List<Vector3d> FindResults(double threshold, bool shuffle = true)
        {
            var results = new List<Vector3d>();
            var all = Possibles.ToList();
            if (shuffle) all = all.Shuffle().ToList();
            foreach (var possible in all)
            {
                var good = true;
                for (var j = results.Count - 1; j >= 0; j--)
                {
                    if (ColorHelper.LabDistance(results[j], possible) < threshold)
                    {
                        good = false;
                        break;
                    }
                }

                if (good)
                {
                    results.Add(possible);
                }
            }
            return results;
        }

        public static IEnumerable<Vector3d> GeneratePossibleColor(bool isHsv, IList<int> x, IList<int> y, IList<int> z)
        {
            if (isHsv)
            {
                foreach (var i1 in x)
                {
                    foreach (var i2 in y)
                    {
                        foreach (var i3 in z)
                        {
                            var lab = new Vector3d(i1, i2 / 100.0, i3 / 100.0).Hsv2Lab();
                            yield return lab;
                        }
                    }
                }
            }
            else
            {
                foreach (var i1 in x)
                {
                    foreach (var i2 in y)
                    {
                        foreach (var i3 in z)
                        {
                            var lab = new Vector3d(i1, i2, i3);
                            var rgb = lab.Lab2Rgb();
                            var lab2 = new Color32(
                                Math.Min((byte)255, Math.Max((byte)0, rgb.r)),
                                Math.Min((byte)255, Math.Max((byte)0, rgb.g)),
                                Math.Min((byte)255, Math.Max((byte)0, rgb.b)),
                                255
                            ).Rgb2Lab();
                            yield return lab2;
                        }
                    }
                }
            }
        }



        public static IEnumerable<int> GetAxes(int min, int max, int inc)
        {
            if (min > max)
            {
                for (var i = min; i >= max; i -= inc)
                {
                    yield return i;
                }
            }
            else
            {

                for (var i = min; i <= max; i += inc)
                {
                    yield return i;
                }
            }
        }
    }
}
