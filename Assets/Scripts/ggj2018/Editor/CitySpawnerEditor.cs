using UnityEditor;
using UnityEngine;

namespace ggj2018.ggj2018.Editor
{
    [CustomEditor(typeof(CitySpawner))]
    public class CitySpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CitySpawner spawner = (CitySpawner)target;
            if(GUILayout.Button("Generate")) {
                spawner.Generate(true);
            }
        }
    }
}
