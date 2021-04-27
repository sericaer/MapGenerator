using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace MapGenerator
{
    internal class HeightMapGenerator2
    {
        MathNet.Numerics.Distributions.Normal normalDist = new Normal(0, 10);

        public HeightMapGenerator2()
        {
        }

        internal Dictionary<(int, int), int> Gen(int size, string seed)
        {
            this.size = size;

            activeCell.Clear();
            stableCell.Clear();

            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(seed);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            UnityEngine.Random.InitState(BitConverter.ToInt32(hashBytes, 0));

            for(int y=0; y<size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    stableCell.Add((x, y), 100);
                }
            }

            for(int i=0; i<100; i++)
            {
                Strike(1);
            }
            
            return stableCell;
        }

        private void Strike(int percent)
        {
            var selectList = new List<(int, int)>();
            var needCount = size * size * percent / 100;
            for(int i=0; i< needCount; i++)
            {
                selectList.Add((UnityEngine.Random.Range(0, size * size), UnityEngine.Random.Range(0, size * size)));
            }

            foreach(var select in selectList)
            {
                stableCell[select]--;
            }
        }

        private int CalcNextValue(IEnumerable<int> values)
        {
            if(values.Count() == 0)
            {
                return (int)(50 + normalDist.Sample());
            }

            int max = values.Max() - 50;
            int min = values.Min() - 50;
            if(max > 0)
            {
                if(min >= 0)
                {
                    return values.Max() + UnityEngine.Random.Range(0, 10);
                }
                else
                {
                    if(max > Math.Abs(min))
                    {
                        return values.Max() + UnityEngine.Random.Range(0, 10);
                    }
                    else
                    {
                        return values.Min() - UnityEngine.Random.Range(0, 10);
                    }
                }
            }
            else if(min < 0)
            {
                return values.Min() - UnityEngine.Random.Range(0, 10);
            }
            else
            {
                return (int)(values.Min() + normalDist.Sample());
            }

        }

        private IEnumerable<(int x, int y)> GetEmptyNearbys((int x, int y) key)
        {
            return GetNearbys(key).Where(e => !stableCell.ContainsKey(e) && !activeCell.ContainsKey(e)).ToArray();
        }

        private Dictionary<(int x, int y), int> GetCellNearbys((int x, int y) key)
        {
            var d1 = GetNearbys(key).Where(e => stableCell.ContainsKey(e)).ToDictionary(key => key, value => stableCell[value]);
            var d2 = GetNearbys(key).Where(e => activeCell.ContainsKey(e)).ToDictionary(key => key, value => activeCell[value]);

            return d1.Concat(d2).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());
        }

        private IEnumerable<(int x, int y)> GetNearbys((int x, int y) key)
        {
            var rslt = new (int x, int y)[] { (key.x + 1, key.y), (key.x - 1, key.y), (key.x, key.y + 1), (key.x, key.y - 1),
                                          (key.x + 1, key.y+1), (key.x + 1, key.y-1), (key.x - 1, key.y+1), (key.x - 1, key.y-1), };

            return rslt.Where(e => e.x < size && e.x >= 0 && e.y < size && e.y >= 0);
        }

        private int size;
        private Dictionary<(int x, int y), int> activeCell = new Dictionary<(int x, int y), int>();
        private Dictionary<(int x, int y), int> stableCell = new Dictionary<(int x, int y), int>();
    }
}