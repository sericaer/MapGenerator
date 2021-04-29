using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MapGenerator
{
    class CellAutoGeneration2
    {
        private int size;

        private Dictionary<(int x, int y), int> activeCell = new Dictionary<(int x, int y), int>();
        private Dictionary<(int x, int y), int> stableCell = new Dictionary<(int x, int y), int>();


        internal Dictionary<(int, int), int> Gen(int size, string seed)
        {
            this.size = size;

            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(seed);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            UnityEngine.Random.InitState(BitConverter.ToInt32(hashBytes, 0));

            activeCell[(size / 2, size / 2)] = 0;

            while(activeCell.Count() != 0)
            {
                int random = UnityEngine.Random.Range(0, size * size);

                var currKey = activeCell.Keys.ElementAt(random% activeCell.Count());

                var emptyNearbys = GetEmptyNearbys(currKey);
                if (emptyNearbys.Count() == 0)
                {
                    stableCell[currKey] = activeCell[currKey];
                    activeCell.Remove(currKey);
                    continue;
                }

                var nearby = emptyNearbys.ElementAt(random % emptyNearbys.Count());
                var cellNearbys = GetCellNearbys(nearby);

                var sum = cellNearbys.Values.Sum();
                if (sum != 0 && UnityEngine.Random.Range(0, 100) < 50)
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

            var rslt = stableCell.ToDictionary(key=>key.Key, value=>0);
            var keys = stableCell.Keys.Where(x => stableCell[x] != 0);

            foreach (var key in keys)
            {
                int distance = 1;
                while (true)
                {
                    if (GetNearbys(key, distance).Any(x => stableCell[x] == 0))
                    {
                        break;
                    }
                    distance++;
                }

                var nearbys = GetNearbys(key);

                var count = nearbys.Where(x => stableCell[x] != 0).Count();
                rslt[key] = Enumerable.Range(0, count * distance).Sum(_ => UnityEngine.Random.Range(count * distance / 2, count * distance));
            }

            //foreach(var key in keys)
            //{
            //    int distance = 1;
            //    while(true)
            //    {
            //        var nearbys = GetNearbys(key, distance);
            //        if(nearbys.Any(x => stableCell[x] == 0))
            //        {
            //            break;
            //        }
            //        distance++;
            //    }

            //    rslt[key] = UnityEngine.Random.Range((int)Math.Pow((distance + 1), 3), (int)Math.Pow((distance + 1), 3)*2);
            //}

            return rslt;
        }

        private IEnumerable<(int x, int y)> GetEmptyNearbys((int x, int y) key)
        {
            return GetNearbys(key).Where(e => !activeCell.ContainsKey(e) && !stableCell.ContainsKey(e)).ToArray();
        }

        private IEnumerable<(int x, int y)> GetNearbys((int x, int y) key)
        {
            var rslt = new (int x, int y)[] { (key.x + 1, key.y), (key.x - 1, key.y), (key.x, key.y + 1), (key.x, key.y - 1),
                                          (key.x + 1, key.y+1), (key.x + 1, key.y-1), (key.x - 1, key.y+1), (key.x - 1, key.y-1), };

            return rslt.Where(e => e.x < size && e.x >= 0 && e.y < size && e.y >= 0);
        }

        private IEnumerable<(int x, int y)> GetNearbys((int x, int y) key, int distance)
        {
            return Enumerable.Range(-1 * distance, distance*2+1)
                .SelectMany(x => Enumerable.Range(-1 * distance, distance*2+1).Select(y => (key.x + x, key.y + y)))
                .Where(e => e.Item1 < size && e.Item1 >= 0 && e.Item2 < size && e.Item2 >= 0);
        }

        private Dictionary<(int x, int y), int> GetCellNearbys((int x, int y) key)
        {
            var nearbys = GetNearbys(key);

            return nearbys.Where(x=> stableCell.ContainsKey(x) || activeCell.ContainsKey(x)).ToDictionary(key => key, value => stableCell.ContainsKey(value)? stableCell[value] : activeCell[value]);
        }

        private int[] GetRandomArray(int min, int max, int count)
        {

            if(max -min < count)
            {
                throw new Exception();
            }

            List<int> rslt = new List<int>();
            while(rslt.Count() < count)
            {
                int value = UnityEngine.Random.Range(min, max);
                if(rslt.Contains(value))
                {
                    continue;
                }

                rslt.Add(value);
            }

            return rslt.ToArray();
        }

        
    }
}
