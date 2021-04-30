using HexMath;
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

        private Dictionary<AxialCoord, int> activeCell = new Dictionary<AxialCoord, int>();
        private Dictionary<AxialCoord, int> stableCell = new Dictionary<AxialCoord, int>();
        private LinkedList<int> randoms;
        private Dictionary<AxialCoord, int> rslt;

        internal Dictionary<AxialCoord, int> Gen(int size, string seed)
        {
            this.size = size;

            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(seed);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            UnityEngine.Random.InitState(BitConverter.ToInt32(hashBytes, 0));

            randoms = new LinkedList<int>(Enumerable.Range(0, 1000).Select(x=>UnityEngine.Random.Range(-size, size * size)));

            activeCell[new AxialCoord(0, 0)] = 0;

            rslt = Enumerable.Range(-size/2,size/2).SelectMany(x=> Enumerable.Range(-size/2, size/2).Select(y=> new OffsetCoord(x,y).ToAxialCoord())).ToDictionary(k=>k, v=>0);

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

            return Math.Abs(rslt);
        }

        private Dictionary<AxialCoord, int> GenMethod()
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

        private IEnumerable<AxialCoord> GetEmptyNearbys(AxialCoord key)
        {
            return GetNearbys(key).Where(e => !activeCell.ContainsKey(e) && !stableCell.ContainsKey(e)).ToArray();
        }

        private IEnumerable<AxialCoord> GetNearbys(AxialCoord key)
        {
            return key.GetNeighbors().Where(x => rslt.ContainsKey(x));
        }


        private Dictionary<AxialCoord, int> GetCellNearbys(AxialCoord key)
        {
            var nearbys = GetNearbys(key);

            return nearbys.Where(x=> stableCell.ContainsKey(x) || activeCell.ContainsKey(x))
                          .ToDictionary(key => key, value => stableCell.ContainsKey(value)? stableCell[value] : activeCell[value]);
        }
        
    }
}
