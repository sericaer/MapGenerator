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


        // Start is called before the first frame update
        void Start()
        {
            Array values = Enum.GetValues(typeof(TerrainTileType));
            var generator = new RandomDataGenerator();
            RenderTileMap(generator.Gen(size).Select(x=>
            {
                int index = (int)(x *0.01 * values.Length);
                return (TerrainTileType)values.GetValue(index);
            }).ToArray());
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
