using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Module))]
public class ModuleInspector : Editor
{
    float brightness;
    static void Init()
    {
        brightness = getBrightness();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Module module = (Module)target;

        GUILayout.BeginHorizontal();
            brightness = EditorGUILayout.Slider(brightness, 0, 100);

        GUILayout.EndHorizontal();
    }

    public void OnInspectorUpdate()
    {
        SetBrightness(brightness);
    }
}