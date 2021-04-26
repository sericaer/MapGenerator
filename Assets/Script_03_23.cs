using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Script_03_23 : MonoBehaviour
{
    public Vector3 scrollPos;
    public int myId;
    public string myName;
    public GameObject prefab;
    public MyEnum myEnum = MyEnum.One;
    public bool toogle1;
    public bool toogle2;
    public float slider1 
    {
        get
        {
            return _slider1;

        }
        set
        {
            if(_slider1 == value)
            {
                return;
            }
            Debug.Log(_slider1);
            _slider1 = value;
        }
    }

    private float _slider1 = 0;
    public enum MyEnum
    {
        One = 1,
        Two,
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Script_03_23))]
    public class ScriptEditor_03_23 : Editor
    {
        private bool m_EnableToogle;
        public override void OnInspectorGUI()
        {
            ////获取脚本对象
            Script_03_23 script = target as Script_03_23;

            ////绘制滚动条
            //script.scrollPos =
            //    EditorGUILayout.BeginScrollView(script.scrollPos, false, true);
            //script.myName = EditorGUILayout.TextField("text", script.myName);
            //script.myId = EditorGUILayout.IntField("int", script.myId);

            //script.prefab = EditorGUILayout.ObjectField("GameObject", script.prefab,
            //    typeof(GameObject), true) as GameObject;

            ////绘制按钮
            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Button("1");
            //GUILayout.Button("2");
            //script.myEnum = (Script_03_23.MyEnum)EditorGUILayout.EnumPopup("MyEnum:",
            //    script.myEnum);

            //EditorGUILayout.EndHorizontal();

            ////Toogle 组件
            //m_EnableToogle = EditorGUILayout.BeginToggleGroup("EnableToogle",
            //    m_EnableToogle);
            //script.toogle1 = EditorGUILayout.Toggle("toogle1", script.toogle1);
            //script.toogle2 = EditorGUILayout.Toggle("toogle2", script.toogle2);
    

            //EditorGUILayout.EndToggleGroup();

            script.slider1 = EditorGUILayout.Slider("slider1", script.slider1, 0f, 1.0f);

            //EditorGUILayout.EndScrollView();
        }
    }
}
#endif
