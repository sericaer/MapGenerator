
using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapGenerator
{
    partial class MapControl : MonoBehaviour
    {
        public Tilemap terrainMap;

        public TileSets tileSets;

        public int size
        {
            get
            {
                return _size;

            }
            set
            {
                if (_size == value)
                {
                    return;
                }
                _size = value;
            }
        }

        public string seed
        {
            get
            {
                return _seed;

            }
            set
            {
                if (_seed == value)
                {
                    return;
                }
                _seed = value;
            }
        }

        public int hillLevel
        {
            get
            {
                return _hillLevel;

            }
            set
            {
                if (_hillLevel == value)
                {
                    return;
                }
                _hillLevel = value;

                RenderTileMap();
            }
        }

        public int waterLevel
        {
            get
            {
                return _waterLevel;

            }
            set
            {
                if (_waterLevel == value)
                {
                    return;
                }
                _waterLevel = value;

                RenderTileMap();
            }
        }


        private int _hillLevel;
        private int _waterLevel;
        private int _size;
        private string _seed;
        private Dictionary<(int x, int y), int> dict;

        public void GenerateMap()
        {
            Array values = Enum.GetValues(typeof(TerrainTileType));

            var generator = new HeightMapGenerator();


            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            dict = generator.Gen(size, seed);
            stopwatch.Stop();
            Debug.Log("Time taken: " + (stopwatch.Elapsed));
            stopwatch.Reset();
           


            RenderTileMap();
        }

        private void RenderTileMap()
        {
            Array values = Enum.GetValues(typeof(TerrainTileType));
            foreach (var elem in dict)
            {
                if (elem.Value < waterLevel)
                {
                    terrainMap.SetTile(new Vector3Int(elem.Key.x - size / 2, elem.Key.y - size / 2, 0), tileSets.GetTile(TerrainTileType.water));
                }
                else if (elem.Value < hillLevel)
                {
                    terrainMap.SetTile(new Vector3Int(elem.Key.x - size / 2, elem.Key.y - size / 2, 0), tileSets.GetTile(TerrainTileType.hill));
                }
                else
                {
                    terrainMap.SetTile(new Vector3Int(elem.Key.x - size / 2, elem.Key.y - size / 2, 0), tileSets.GetTile(TerrainTileType.plain));
                }
            }
        }
    }
}
