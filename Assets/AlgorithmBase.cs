using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapGenerator
{
    public abstract class AlgorithmBase : ScriptableObject
    {
        public abstract void Apply(Tilemap tilemap);
    }
}
