#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FishSwimPointMaster))]
public class FishSwimPointMasterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();  // Draws the default inspector

        FishSwimPointMaster script = (FishSwimPointMaster)target;

        if (GUILayout.Button("Save Swim Points"))
        {
            script.SavePoints();  // Calls the SavePoints method on your FishSwimPointMaster script
        }
    }
}
#endif
