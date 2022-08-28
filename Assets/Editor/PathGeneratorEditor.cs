using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathGenerator))]
public class PathGeneratorEditor : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        PathGenerator myItem = (PathGenerator)target;

        if(GUILayout.Button("Generate Platforms")){
            myItem.GeneratePlatforms();
            if(PathGenerator.instance.Regenerate) {myItem.GeneratePlatforms(); Debug.Log("regenerated");}
        }

        if(GUILayout.Button("Clear Platforms"))
        {
            myItem.ClearPlatforms();
        }
        
    }
}
