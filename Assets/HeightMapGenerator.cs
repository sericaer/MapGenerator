using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MapGenerator
{
    internal class HeightMapGenerator
    {
        public HeightMapGenerator()
        {
        }

        internal Dictionary<(int, int), int> Gen(int size, string seed)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(seed);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            UnityEngine.Random.InitState(BitConverter.ToInt32(hashBytes, 0));

            var rslt = new Dictionary<(int, int), int>();
            for (int y=0; y< size; y++)
            {
                for(int x=0; x<size; x++)
                {
                    rslt.Add((x, y), UnityEngine.Random.Range(0, 100));
                }
            }
            return rslt;
        }
    }
}