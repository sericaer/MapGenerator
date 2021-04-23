using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapGenerator
{
    public class TileSets : MonoBehaviour
    {
        [Serializable]
        public class TileType
        {
            public TerrainTileType terrainType;
            public Color Color;
        }

        [SerializeField]
        public TileType[] TileTypes;

        private Dictionary<TerrainTileType, Tile> dict;


        public Tile GetTile(TerrainTileType terrainTileType)
        {
            if (dict == null)
            {
                dict = CreateTileDict();
            }

            return dict[terrainTileType];
        }

        private Dictionary<TerrainTileType, Tile> CreateTileDict()
        {
            var rslt = new Dictionary<TerrainTileType, Tile>();

            var tileSprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);

            foreach (var tiletype in TileTypes)
            {
                var tile = ScriptableObject.CreateInstance<Tile>();
                tiletype.Color.a = 1;
                tile.color = tiletype.Color;
                tile.sprite = tileSprite;
                rslt.Add(tiletype.terrainType, tile);
            }

            return rslt;
        }

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}