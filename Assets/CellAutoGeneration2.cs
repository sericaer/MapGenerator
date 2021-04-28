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

            while(stableCell.Count() < size*size)
            {
                int random = UnityEngine.Random.Range(0, size * size);

                var currKey = activeCell.ElementAt(random% activeCell.Count()).Key;

                var emptyNearbys = GetEmptyNearbys(currKey);
                if (emptyNearbys.Count() == 0)
                {
                    stableCell[currKey] = activeCell[currKey];
                    activeCell.Remove(currKey);
                    continue;
                }

                var nearby = emptyNearbys.ElementAt(random%emptyNearbys.Count());
                var cellNearbys = GetCellNearbys(nearby);
                if(random%100<10 || cellNearbys.Values.Where(x => x).Count() * 100 / cellNearbys.Count() > 50)
                {
                    activeCell[nearby] = 1;
                }
                else
                {
                    activeCell[nearby] = 0;
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

        private Dictionary<(int x, int y), bool> GetCellNearbys((int x, int y) key)
        {
            return GetNearbys(key).ToDictionary(key => key, value => stableCell.ContainsKey(key) || activeCell.ContainsKey(key));
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
