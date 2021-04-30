using HexMath;
using MapGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexMapControl : MonoBehaviour
{
    public Tilemap terrainMap;

    public HexTileSets tileSets;

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
        terrainMap.ClearAllTiles();



        var rslt = Enumerable.Range(0, size).SelectMany(x => Enumerable.Range(0, size).Select(y => (x, y))).ToDictionary(k => k, v => 0);
        foreach(var elem in rslt.Keys)
        {
            terrainMap.SetTile(new Vector3Int(elem.x, elem.y, 0), tileSets.GetTile(TerrainTileType.plain));
        }

        foreach (var elem in GetNearbys((6, 5)))
        {
            terrainMap.SetTile(new Vector3Int(elem.x, elem.y, 0), tileSets.GetTile(TerrainTileType.Mountain));
        }

        //Array values = Enum.GetValues(typeof(TerrainTileType));

        //var generator = new CellAutoGeneration4();


        //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        //stopwatch.Start();
        //dict = generator.Gen(size, seed);
        //stopwatch.Stop();
        //Debug.Log("Time taken: " + (stopwatch.Elapsed));
        //stopwatch.Reset();

        //list = dict.Keys.Select(e => (e.x, e.y, dict[e])).ToList();
        //list.OrderBy(x => x.value);

        //RenderTileMap();
    }

    private void RenderTileMap()
    {
        Array values = Enum.GetValues(typeof(TerrainTileType));

        HashSet<(int x, int y)> list = new HashSet<(int x, int y)>();
        List<(int x, int y)> listHill = new List<(int x, int y)>();
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
                listHill.Add((elem.Key.x, elem.Key.y));
            }
            else
            {
                terrainMap.SetTile(new Vector3Int(elem.Key.x - size / 2, elem.Key.y - size / 2, 0), tileSets.GetTile(TerrainTileType.plain));
            }
        }

        foreach (var wkey in list)
        {
            var nearbys = GetNearbys(wkey).Where(x => list.Contains(x));
            if (nearbys.Count() < 3)
            {
                terrainMap.SetTile(new Vector3Int(wkey.x - size / 2, wkey.y - size / 2, 0), tileSets.GetTile(TerrainTileType.plain));
                foreach (var near in nearbys)
                {
                    terrainMap.SetTile(new Vector3Int(near.x - size / 2, near.y - size / 2, 0), tileSets.GetTile(TerrainTileType.plain));
                }
            }
        }

        //int mountCount = 0;
        //foreach (var hill in listHill.OrderByDescending(x => dict[x]))
        //{
        //    if (GetNearbys(hill).Count(x => listHill.Contains(x)) >= 6)
        //    {
        //        terrainMap.SetTile(new Vector3Int(hill.x - size / 2, hill.y - size / 2, 0), tileSets.GetTile(TerrainTileType.Mountain));
        //        mountCount++;
        //    }
        //}

        ;
        foreach (var elem in GetNearbys((1, 2)))
        {
            terrainMap.SetTile(new Vector3Int(elem.x, elem.y, 0), tileSets.GetTile(TerrainTileType.Mountain));
        }

        list.Clear();
        listHill.Clear();
    }

    private IEnumerable<(int x, int y)> GetNearbys((int x, int y) key)
    {
        return new OffsetCoord(key.x, key.y).ToAxialCoord().GetNeighbors().Select(x => x.ToOffsetCoord()).Select(e => (e.row, e.col));
    }


    static Vector3Int
    LEFT = new Vector3Int(-1, 0, 0),
    RIGHT = new Vector3Int(1, 0, 0),
    DOWN = new Vector3Int(0, -1, 0),
    DOWNLEFT = new Vector3Int(-1, -1, 0),
    DOWNRIGHT = new Vector3Int(1, -1, 0),
    UP = new Vector3Int(0, 1, 0),
    UPLEFT = new Vector3Int(-1, 1, 0),
    UPRIGHT = new Vector3Int(1, 1, 0);

    static Vector3Int[] directions_when_y_is_even =
          { LEFT, RIGHT, DOWN, DOWNLEFT, UP, UPLEFT };
    static Vector3Int[] directions_when_y_is_odd =
          { LEFT, RIGHT, DOWN, DOWNRIGHT, UP, UPRIGHT };

    public IEnumerable<Vector3Int> Neighbors(Vector3Int node)
    {
        Vector3Int[] directions = (node.y % 2) == 0 ?
             directions_when_y_is_even :
             directions_when_y_is_odd;
        foreach (var direction in directions)
        {
            Vector3Int neighborPos = node + direction;
            yield return neighborPos;
        }
    }
}
