using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGenerator
{
    class RandomDataGenerator
    {
        public int[] Gen(int size)
        {
            return Enumerable.Range(0, size * size).Select(x => UnityEngine.Random.Range(0, 100)).ToArray();
        }
    }
}
