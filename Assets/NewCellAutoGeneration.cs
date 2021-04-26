using MathNet.Numerics.Distributions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapGenerator
{
    public class NewCellAutoGeneration 
    {
        int hillfactor;
        int warerfactor;
        int mountfactor;

        public Dictionary<(int x, int y), TerrainTileType> Gen(int size, int hillfator, int waterfacor, int mountfactor)
        {
            this.hillfactor = hillfator;
            this.warerfactor = waterfacor;
            this.mountfactor = mountfactor;

            this.size = size;
            //var datas = Enumerable.Range(0, size * size).ToDictionary(key => (key % size, key / size), value => (int?)null);

            var datas = new Dictionary<(int x, int y), TerrainTileType>();
            datas[CenterIndex] = TerrainTileType.plain;
            activeCell.Add(CenterIndex, TerrainTileType.plain);
            totalCell.Add(CenterIndex, TerrainTileType.plain);

            while (activeCell.Count() != 0)
            {
                var currCell = activeCell.First();
                var emptyNearbys = GetEmptyNearbys(currCell.Key);
                if (emptyNearbys.Count() == 0)
                {
                    stableCell.Add(currCell.Key, currCell.Value);
                    activeCell.Remove(currCell.Key);
                    continue;
                }

                var nearby = emptyNearbys.ElementAt(UnityEngine.Random.Range(0, emptyNearbys.Count()));
                var cellNearbys = GetCellNearbys(nearby);

                var value = CalcTerrainType(cellNearbys.Values);
                activeCell.Add(nearby, value);
                totalCell.Add(nearby, value);
            }

            var hillIndexs = totalCell.Keys.Where(x => totalCell[x] == TerrainTileType.hill).ToArray();
            foreach (var hillIndex in hillIndexs)
            {
                var cellNearbys = GetCellNearbys(hillIndex);
                if(cellNearbys.All(x=>x.Value == TerrainTileType.hill || x.Value == TerrainTileType.Mountain))
                {
                    var percent = mountfactor;
                    int value = UnityEngine.Random.Range(0, 101);
                    if(value < percent)
                    {
                        totalCell[hillIndex] = TerrainTileType.Mountain;
                        stableCell[hillIndex] = TerrainTileType.Mountain;
                    }
                }

            }
            return stableCell;
        }

        private TerrainTileType CalcTerrainType(IEnumerable<TerrainTileType> terrains)
        {
            var baseType =  CalcBaseTerrainType(terrains);

            int value = UnityEngine.Random.Range(0, 1000);

            if (baseType == TerrainTileType.plain)
            {
                if(value < warerfactor / 5)
                {
                    return TerrainTileType.water;
                }
                else if(value < warerfactor /5 + hillfactor / 2)
                {
                    return TerrainTileType.hill;
                }
                else
                {
                    return TerrainTileType.plain;
                }
            }
            else if (baseType == TerrainTileType.hill)
            {
                if (value < 700)
                {
                    return TerrainTileType.hill;
                }
                else
                {
                    return TerrainTileType.plain;
                }
            }
            else if (baseType == TerrainTileType.water)
            {
                if (value < 700)
                {
                    return TerrainTileType.water;
                }
                else
                {
                    return TerrainTileType.plain;
                }
            }
            else
            {
                throw new System.Exception();
            }

        }

        private TerrainTileType CalcBaseTerrainType(IEnumerable<TerrainTileType> terrains)
        {
            int waterCount = terrains.Count(x => x == TerrainTileType.water);
            int hillCount = terrains.Count(x => x == TerrainTileType.hill) + 2 * terrains.Count(x => x == TerrainTileType.Mountain);

            if (waterCount == 0 && hillCount == 0)
            {
                return TerrainTileType.plain;
            }

            if (waterCount * warerfactor > hillCount * hillfactor)
            {
                return TerrainTileType.water;
            }
            else
            {
                return TerrainTileType.hill;
            }
        }

        private IEnumerable<(int x, int y)> GetEmptyNearbys((int x, int y) key)
        {
            return GetNearbys(key).Where(e => !totalCell.ContainsKey(e)).ToArray();
        }

        private Dictionary<(int x, int y), TerrainTileType> GetCellNearbys((int x, int y) key)
        {
            return GetNearbys(key).Where(e => totalCell.ContainsKey(e)).ToDictionary(key=>key, value=>totalCell[value]);
        }

        private IEnumerable<(int x, int y)> GetNearbys((int x, int y) key)
        {
            var rslt = new (int x, int y)[] { (key.x + 1, key.y), (key.x - 1, key.y), (key.x, key.y + 1), (key.x, key.y - 1),
                                          (key.x + 1, key.y+1), (key.x + 1, key.y-1), (key.x - 1, key.y+1), (key.x - 1, key.y-1), };

            return rslt.Where(e => e.x < size && e.x >= 0 && e.y < size && e.y >= 0);
        }

        private Dictionary<(int x, int y), TerrainTileType> activeCell = new Dictionary<(int x, int y), TerrainTileType>();
        private Dictionary<(int x, int y), TerrainTileType> stableCell = new Dictionary<(int x, int y), TerrainTileType>();
        private Dictionary<(int x, int y), TerrainTileType> totalCell = new Dictionary<(int x, int y), TerrainTileType>();

        Dictionary<(int x, int y), (int x, int y)[]> GetActiveCellWithNearbys(IEnumerable<(int x, int y)> keys)
        {
            var rslt = new Dictionary<(int x, int y), (int x, int y)[]>();
            foreach (var key in keys)
            {
                var nearbys = GetNearBy(key).Where(x => !keys.Contains(x)).ToArray();
                if(nearbys.Length == 0)
                {
                    continue;
                }

                rslt.Add(key, nearbys);
            }

            return rslt;
        }

        (int x, int y)[] GetNearBy((int x, int y) key)
        {
            var rslt = new (int x, int y)[] { (key.x + 1, key.y), (key.x - 1, key.y), (key.x, key.y + 1), (key.x, key.y - 1),
                                          (key.x + 1, key.y+1), (key.x + 1, key.y-1), (key.x - 1, key.y+1), (key.x - 1, key.y-1), };

            return rslt.Where(e => e.x < size && e.x >= 0 && e.y < size && e.y >= 0).ToArray();
        }

        int size;
        (int x, int y) CenterIndex => (size/2, size/2);
    }
}