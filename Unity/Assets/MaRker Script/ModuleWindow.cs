using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.IO.Ports;
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


    /* Asset Load
     * texWidth should always be 1
     * texHeight should be the length of the NeoPixel Arrays;
     */
    Texture2D pattern;
    int texWidth = 1;
    int texHeight;

    /* Port Communication
     * 
     */
    SerialPort sp = new SerialPort("COM7");
    

    //Test Variables
    string[] test = { "None","1", "2", "3" };
    string testS;


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

        pattern = (Texture2D)EditorGUILayout.ObjectField("Pattern: ",
                                                         pattern,
                                                         typeof(Texture2D),
                                                         false
                                                         );
        if (GUILayout.Button("Load Pattern"))
        {
            if (!pattern)
            {
                ImageParse();
            }
        }

        if (GUILayout.Button("Connect Module"))
        {
            if (!pattern)
            {   
                Connect();
            }
        }
    }

    void Update()
    {
        testS = sp.ReadLine();
        if(testS != "0")
        {
            Debug.Log(testS);
        }
        else
        {
            Debug.Log("0");
        }
    }
    void OnInspectorUpdate()
    {
        Repaint();
    }

    void ImageParse()
    {
        int x = 0;
        int y = 0;
        Color[] pixelData = pattern.GetPixels(x, y, texWidth, texHeight);

    }

    void Connect()
    {
        if(sp.IsOpen)
        {
            sp.Close();
            Debug.Log("Closing Port");
        }else
        {
            if(sp != null)
            {
                sp.Open();
                sp.ReadTimeout = 50;
                Debug.Log("Opening Port");
            }
            else
            {
                Debug.Log("No Active Module");
            }
        }
    }

    void OnApplicationQuit()
    {
        sp.Close();
        Debug.Log("Closing Port");
    }
}
