using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapGenerator
{
    public class Map : MonoBehaviour
    {
        public Tilemap terrainMap;
        public TileSets tileSets;

        public int size;

        [SerializeField]
        private AlgorithmBase[] _algorithms;

        // Start is called before the first frame update
        void Start()
        {
            Array values = Enum.GetValues(typeof(TerrainTileType));

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var generator = new CellAutoGeneration();
            stopwatch.Stop();
            Debug.Log("Time taken: " + (stopwatch.Elapsed));
            stopwatch.Reset();

            
            //RenderTileMap(generator.Gen(size).Select(x=>
            //{
            //    int index = (int)(x *0.01 * (values.Length-1));
            //    return (TerrainTileType)values.GetValue(index);
            //}).ToArray());

            RenderTileMap(generator.Gen(size));
        }

        private void RenderTileMap(Dictionary<(int x, int y), int> dictionary)
        {
            Array values = Enum.GetValues(typeof(TerrainTileType));
            foreach (var elem in dictionary)
            {
                
                int index = (int)(elem.Value * 0.01 * (values.Length - 1));
                terrainMap.SetTile(new Vector3Int(elem.Key.x - size / 2, elem.Key.y - size / 2, 0), tileSets.GetTile((TerrainTileType)values.GetValue(index)));
            }
        }

        private void RenderTileMap(TerrainTileType[] terrains)
        {
            for(int i=0; i<terrains.Length; i++)
            {
                terrainMap.SetTile(new Vector3Int(i / size - size/2, i % size  - size/2, 0), tileSets.GetTile(terrains[i]));
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        
    }

}
