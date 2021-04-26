
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MapGenerator
{

#if UNITY_EDITOR
    [CustomEditor(typeof(MapControl))]
    public class MapControlEditor : Editor
    {
        private bool m_EnableToogle;
        public override void OnInspectorGUI()
        {
            ////获取脚本对象
            MapControl script = target as MapControl;
            script.tileSets = EditorGUILayout.ObjectField(script.tileSets, typeof(TileSets), true) as TileSets;
            script.terrainMap = EditorGUILayout.ObjectField(script.terrainMap, typeof(Tilemap), true) as Tilemap;

            script.size = EditorGUILayout.IntField("size", script.size);
            script.seed = EditorGUILayout.TextField("seed", script.seed);
            if(GUILayout.Button("generate"))
            {
                script.GenerateMap();
            }

            script.hillLevel = EditorGUILayout.IntSlider("HillLevel", script.hillLevel, 0, 100);
            script.waterLevel = EditorGUILayout.IntSlider("WaterLevel", script.waterLevel, 0, 100);
        }
    }
#endif
}
