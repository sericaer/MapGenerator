
using System;
using System.Collections.Generic;
using System.Linq;

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
        private List<(int x, int y, int value)> list;
        public void GenerateMap()
        {
            Array values = Enum.GetValues(typeof(TerrainTileType));

            var generator = new CellAutoGeneration3();


            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            dict = generator.Gen(size, seed);
            stopwatch.Stop();
            Debug.Log("Time taken: " + (stopwatch.Elapsed));
            stopwatch.Reset();

            list = dict.Keys.Select(e=>(e.x, e.y, dict[e])).ToList();
            list.OrderBy(x => x.value);

            RenderTileMap();
        }

        private void RenderTileMap()
        {
            Array values = Enum.GetValues(typeof(TerrainTileType));

            HashSet<(int x, int y)> list = new HashSet<(int x, int y)>();

            foreach (var elem in dict)
            {
                if (elem.Value < waterLevel)
                {
                    terrainMap.SetTile(new Vector3Int(elem.Key.x - size / 2, elem.Key.y - size / 2, 0), tileSets.GetTile(TerrainTileType.water));
                    list.Add(elem.Key);
                }
                else if (elem.Value > hillLevel)
                {
                    terrainMap.SetTile(new Vector3Int(elem.Key.x - size / 2, elem.Key.y - size / 2, 0), tileSets.GetTile(TerrainTileType.hill));
                }
                else
                {
                    terrainMap.SetTile(new Vector3Int(elem.Key.x - size / 2, elem.Key.y - size / 2, 0), tileSets.GetTile(TerrainTileType.plain));
                }
            }

            foreach(var wkey in list)
            {
                var nearbys = GetNearbys(wkey).Where(x => list.Contains(x));
                if (nearbys.Count() < 3)
                {
                    terrainMap.SetTile(new Vector3Int(wkey.x - size / 2, wkey.y - size / 2, 0), tileSets.GetTile(TerrainTileType.plain));
                    foreach(var near in nearbys)
                    {
                        terrainMap.SetTile(new Vector3Int(near.x - size / 2, near.y - size / 2, 0), tileSets.GetTile(TerrainTileType.plain));
                    }
                }
            }
            
        }

        private IEnumerable<(int x, int y)> GetNearbys((int x, int y) key)
        {
            var rslt = new (int x, int y)[] { (key.x + 1, key.y), (key.x - 1, key.y), (key.x, key.y + 1), (key.x, key.y - 1),
                                          (key.x + 1, key.y+1), (key.x + 1, key.y-1), (key.x - 1, key.y+1), (key.x - 1, key.y-1), };

            return rslt.Where(e => e.x < size && e.x >= 0 && e.y < size && e.y >= 0);
        }
    }
}
