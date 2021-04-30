using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MapGenerator
{
    class CellAutoGeneration4
    {
        private int size;

        private Dictionary<(int x, int y), int> activeCell = new Dictionary<(int x, int y), int>();
        private Dictionary<(int x, int y), int> stableCell = new Dictionary<(int x, int y), int>();
        private LinkedList<int> randoms;

        internal Dictionary<(int, int), int> Gen(int size, string seed)
        {
            this.size = size;

            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(seed);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            UnityEngine.Random.InitState(BitConverter.ToInt32(hashBytes, 0));

            randoms = new LinkedList<int>(Enumerable.Range(0, 1000).Select(x=>UnityEngine.Random.Range(0, size * size)));

            activeCell[(0, 0)] = 0;

            var rslt = Enumerable.Range(0,size).SelectMany(x=> Enumerable.Range(0, size).Select(y=>(x,y))).ToDictionary(k=>k, v=>0);

            for(int i=0; i<10; i++)
            {
                GenMethod();

                activeCell = stableCell.Keys.Where(x => stableCell[x] == 1).ToDictionary(k => k, v => 1);
                foreach(var key in activeCell.Keys)
                {
                    rslt[key] += 10;
                }
                stableCell.Clear();
            }

            foreach (var key in rslt.Keys.ToArray())
            {
                rslt[key] = UnityEngine.Random.Range(rslt[key] - 10, rslt[key]);
                rslt[key] = rslt[key] < 10 ? UnityEngine.Random.Range(0, 10) : rslt[key];
            }

            return rslt;
        }

        private int GetNextRandom()
        {
            var rslt = randoms.First();
            randoms.RemoveFirst();
            randoms.AddLast(rslt);

            return rslt;
        }

        private Dictionary<(int, int), int> GenMethod()
        {
            while (activeCell.Count() != 0)
            {

                var currKey = activeCell.Keys.ElementAt(GetNextRandom() % activeCell.Count());

                var emptyNearbys = GetEmptyNearbys(currKey);
                if (emptyNearbys.Count() == 0)
                {
                    stableCell[currKey] = activeCell[currKey];
                    activeCell.Remove(currKey);
                    continue;
                }

                var nearby = emptyNearbys.ElementAt(GetNextRandom() % emptyNearbys.Count());
                var cellNearbys = GetCellNearbys(nearby);

                var sum = cellNearbys.Values.Sum();
                if (sum != 0 && GetNextRandom()%100 < 30)
                {
                    activeCell[nearby] = 1;
                }
                else
                {
                    if (UnityEngine.Random.Range(0,200) < 1)
                    {
                        activeCell[nearby] = 1;
                    }
                    else
                    {
                        activeCell[nearby] = 0;
                    }

                }
            }

            return stableCell;
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
