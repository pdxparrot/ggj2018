using UnityEngine;
using UnityEditor;

using System.IO;
 
namespace ggj2018.Util
{
    // http://wiki.unity3d.com/index.php?title=CreateScriptableObjectAsset
    public static class ScriptableObjectUtility
    {
        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static void CreateAsset<T> () where T: ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
 
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if(string.IsNullOrWhiteSpace(path))  {
                path = "Assets";
            }  else if(Path.HasExtension(path))  {
                path = path.Replace(Path.GetFileName(path), string.Empty);
            }
 
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/New {typeof(T).Name}.asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}
