using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace MapGenerator
{
    internal class HeightMapGenerator3
    {
        

        public HeightMapGenerator3()
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

            GenerateHardCells();

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

            Subside(stableCell.Keys.ToArray());

            return stableCell;
        }

        private void GenerateHardCells()
        {
            activeCell[(size / 2, size / 2)] = 0;

            while (activeCell.Count() != 0)
            {
                int random = UnityEngine.Random.Range(0, size * size);

                var currKey = activeCell.Keys.ElementAt(random % activeCell.Count());

                var emptyNearbys = GetEmptyNearbys(currKey);
                if (emptyNearbys.Count() == 0)
                {
                    hardCell[currKey] = activeCell[currKey];
                    activeCell.Remove(currKey);
                    continue;
                }

                var nearby = emptyNearbys.ElementAt(random % emptyNearbys.Count());
                var cellNearbys = GetCellNearbys(nearby);

                var sum = cellNearbys.Values.Sum();
                if (sum != 0 && UnityEngine.Random.Range(0, 100) < 90)
                {
                    activeCell[nearby] = 1;
                }
                else
                {
                    if (random % 100 < 5)
                    {
                        activeCell[nearby] = 1;
                    }
                    else
                    {
                        activeCell[nearby] = 0;
                    }

                }
            }
        }

        private void Subside(IEnumerable<(int, int)> cores)
        {
            var temp = stableCell.Keys.ToDictionary(key=>key, value=> stableCell[value]);

            foreach (var b in cores.SelectMany(x => GetNearbys(x)))
            {
                int value = stableCell[b];

                var nearbys = GetNearbys(b);
                foreach (var c in nearbys)
                {
                    value += temp[c];
                }

                if (hardCell[b] == 1)
                {
                    value = value / (nearbys.Count() + 1);
                }
                else
                {
                    value = value / (nearbys.Count() + 1);
                }
                

                stableCell[b] = value < 0 ? 0 : value;
            }
        }

        private IEnumerable<(int, int)> Strike(double percent)
        {
            var selectList = new List<(int, int)>();
            var needCount = size * size * percent / 100;
            for(int i=0; i< needCount; i++)
            {
                selectList.Add((UnityEngine.Random.Range(0, size ), UnityEngine.Random.Range(0, size )));
            }

            var rslt = new List<(int, int)>();
            foreach(var select in selectList)
            {
                if(hardCell[select] == 1)
                {
                    if(UnityEngine.Random.Range(0, 100) < 80)
                    {
                        var value = stableCell[select] - 1;

                        stableCell[select] = value < 0 ? 0 : value;
                        rslt.Add(select);
                    }
                }
                else
                {
                    var value = stableCell[select] - UnityEngine.Random.Range(2, 5);

                    stableCell[select] = value < 0 ? 0 : value;
                    rslt.Add(select);
                }

            }

            return rslt;
        }

        private IEnumerable<(int x, int y)> GetEmptyNearbys((int x, int y) key)
        {
            return GetNearbys(key).Where(e => !activeCell.ContainsKey(e) && !hardCell.ContainsKey(e)).ToArray();
        }

        private IEnumerable<(int x, int y)> GetNearbys((int x, int y) key)
        {
            var rslt = new (int x, int y)[] { (key.x + 1, key.y), (key.x - 1, key.y), (key.x, key.y + 1), (key.x, key.y - 1),
                                          (key.x + 1, key.y+1), (key.x + 1, key.y-1), (key.x - 1, key.y+1), (key.x - 1, key.y-1), };

            return rslt.Where(e => e.x < size && e.x >= 0 && e.y < size && e.y >= 0);
        }

        private Dictionary<(int x, int y), int> GetCellNearbys((int x, int y) key)
        {
            var nearbys = GetNearbys(key);

            return nearbys.Where(x => hardCell.ContainsKey(x) || activeCell.ContainsKey(x)).ToDictionary(key => key, value => hardCell.ContainsKey(value) ? hardCell[value] : activeCell[value]);
        }

        private int size;
        private Dictionary<(int x, int y), int> activeCell = new Dictionary<(int x, int y), int>();
        private Dictionary<(int x, int y), int> stableCell = new Dictionary<(int x, int y), int>();
        private Dictionary<(int x, int y), int> hardCell = new Dictionary<(int x, int y), int>();
    }
}