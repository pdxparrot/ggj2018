using pdxpartyparrot.ggj2018.World;

using UnityEditor;
using UnityEngine;

namespace pdxpartyparrot.ggj2018.Editor
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
