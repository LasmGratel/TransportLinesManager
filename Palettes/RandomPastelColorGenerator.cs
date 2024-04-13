using System.Collections.Generic;
using System.Linq;
using Commons.Utils;
using TransportLinesManager.Palettes.DistinctColors;
using UnityEngine;

namespace TransportLinesManager.Palettes
{
    public class RandomPastelColorGenerator
    {
        private Queue<Color32> colorQueue;

        public RandomPastelColorGenerator()
        {
            colorQueue = new Queue<Color32>(GenNewQueue());
        }


        public IEnumerable<Color32> GenNewQueue()
        {
            var c = new DistinctColors.DistinctColors();
            c.Possibles.AddRange(DistinctColors.DistinctColors.GeneratePossibleColor(true,
                DistinctColors.DistinctColors.GetAxes(0, 360, 8).ToList(),
                DistinctColors.DistinctColors.GetAxes(100, 25, 25).ToList(),
                DistinctColors.DistinctColors.GetAxes(100, 20, 5).ToList()
            ));
            return ColorHelper.Interleave(c.FindResults(15), 3).Select(x => x.Lab2Rgb());
        }

        /// <summary>
        /// Returns a random pastel color
        /// </summary>
        /// <returns></returns>
        public Color32 GetNext()
        {
            if (colorQueue.Count == 0)
            {
                foreach (var newColor in GenNewQueue())
                {
                    colorQueue.Enqueue(newColor);
                }
                
            }
            var color = colorQueue.Dequeue();
            LogUtils.DoLog(color.ToString());

            return color;
        }
    }

}

