using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ModuleWindow : EditorWindow
{
    /* Common variables
     * 
     */
    static float brightness = 50.0f;
    Color pixelColor;
    int indexer = 0;
    object image;

    //Test Variables
    string[] test = { "None","1", "2", "3" };


    [MenuItem("Window/Module")]
    static void Init()
    {
        ModuleWindow window = EditorWindow.GetWindow<ModuleWindow>("Module");
        window.position = new Rect(0, 0, 180, 80);
        window.Show();
    }

    void OnGUI()
    {
        if (Selection.activeGameObject)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            indexer = EditorGUI.Popup(new Rect(0, 0, position.width, 20), "Module #: ", indexer, test);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            /* Activation Control
                * Sends Toggle Control from GUI Elements to Physical Module using GPIO pins.
                */
            if (GUILayout.Button("Activate Module"))
            {
                Debug.Log("Module Activated");
            }

            EditorGUILayout.Space();
            /* Brightness control
                * for overall brightness of the lighting strip.
                */
            EditorGUILayout.LabelField("Brightness:");
            brightness = EditorGUILayout.Slider(brightness, 0, 100);

            EditorGUILayout.Space();
            /* Pixel Color
                * For monocolor pixels only.
                */
            EditorGUILayout.LabelField("Pixel Color (Monotone Pattern Only): ");
            pixelColor = EditorGUILayout.ColorField(pixelColor);
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void ImageParse(object img)
    {

    }
}
