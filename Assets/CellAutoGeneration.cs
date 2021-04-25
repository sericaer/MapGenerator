using MathNet.Numerics.Distributions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapGenerator
{
    public class CellAutoGeneration 
    {
        MathNet.Numerics.Distributions.Normal normalDist = new Normal(0, 5);

        public Dictionary<(int x, int y), int> Gen(int size)
        {
            this.size = size;
            //var datas = Enumerable.Range(0, size * size).ToDictionary(key => (key % size, key / size), value => (int?)null);

            var datas = new Dictionary<(int x, int y), int>();
            datas[CenterIndex] = 50;
            activeCell.Add(CenterIndex, 50);
            totalCell.Add(CenterIndex, 50);
            while (activeCell.Count() != 0)
            {
                var currCell = activeCell.First();
                var emptyNearbys = GetEmptyNearbys(currCell.Key);
                if(emptyNearbys.Count() == 0)
                {
                    stableCell.Add(currCell.Key, currCell.Value);
                    activeCell.Remove(currCell.Key);
                    continue;
                }

                var nearby = emptyNearbys.First();
                var cellNearbys = GetCellNearbys(nearby);

                
                double randomGaussianValue = normalDist.Sample();

                var value = (int)(cellNearbys.Values.Sum()/cellNearbys.Count() + randomGaussianValue);
                value = Mathf.Min(value, 100);
                value = Mathf.Max(value, 0);
                activeCell.Add(nearby, value);
                totalCell.Add(nearby, value);
            }

            return stableCell;
        }


        private IEnumerable<(int x, int y)> GetEmptyNearbys((int x, int y) key)
        {
            return GetNearbys(key).Where(e => !totalCell.ContainsKey(e)).ToArray();
        }

        private Dictionary<(int x, int y), int> GetCellNearbys((int x, int y) key)
        {
            return GetNearbys(key).Where(e => totalCell.ContainsKey(e)).ToDictionary(key=>key, value=>totalCell[value]);
        }

        private IEnumerable<(int x, int y)> GetNearbys((int x, int y) key)
        {
            var rslt = new (int x, int y)[] { (key.x + 1, key.y), (key.x - 1, key.y), (key.x, key.y + 1), (key.x, key.y - 1),
                                          (key.x + 1, key.y+1), (key.x + 1, key.y-1), (key.x - 1, key.y+1), (key.x - 1, key.y-1), };

            return rslt.Where(e => e.x < size && e.x >= 0 && e.y < size && e.y >= 0);
        }

        private Dictionary<(int x, int y), int> activeCell = new Dictionary<(int x, int y), int>();
        private Dictionary<(int x, int y), int> stableCell = new Dictionary<(int x, int y), int>();
        private Dictionary<(int x, int y), int> totalCell = new Dictionary<(int x, int y), int>();

        Dictionary<(int x, int y), (int x, int y)[]> GetActiveCellWithNearbys(IEnumerable<(int x, int y)> keys)
        {
            var rslt = new Dictionary<(int x, int y), (int x, int y)[]>();
            foreach (var key in keys)
            {
                var nearbys = GetNearBy(key).Where(x => !keys.Contains(x)).ToArray();
                if(nearbys.Length == 0)
                {
                    continue;
                }

                rslt.Add(key, nearbys);
            }

            return rslt;
        }

        (int x, int y)[] GetNearBy((int x, int y) key)
        {
            var rslt = new (int x, int y)[] { (key.x + 1, key.y), (key.x - 1, key.y), (key.x, key.y + 1), (key.x, key.y - 1),
                                          (key.x + 1, key.y+1), (key.x + 1, key.y-1), (key.x - 1, key.y+1), (key.x - 1, key.y-1), };

            return rslt.Where(e => e.x < size && e.x >= 0 && e.y < size && e.y >= 0).ToArray();
        }

        int size;
        (int x, int y) CenterIndex => (size/2, size/2);
    }
}