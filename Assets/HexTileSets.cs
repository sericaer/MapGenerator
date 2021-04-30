using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapGenerator
{
    public class HexTileSets : MonoBehaviour
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
            var rs = Resources.Load<Texture2D>("HexTileSet");

            var rslt = new Dictionary<TerrainTileType, Tile>();

            var tileSprite = Sprite.Create(rs, new Rect(0.0f, 0.0f, rs.width, rs.height), new Vector2(0.5f, 0.5f), 100.0f);

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