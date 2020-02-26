using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Management;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Uduino;

public class ModuleWindow : EditorWindow
{
    UduinoDevice[] devices;
    static float brightness = 50.0f,
        delay = 100.0f,
        brightnessTemp = 0,
        delayTemp = 0;
    int indexer = 0,
        pixelIndexer = 0;
    bool connected = false;
    Color colorChoice;
    Dictionary<int,Modules> mList = new Dictionary<int,Modules>()
    {
        {1,new Modules("Marker#1", false, null)},
        {2,new Modules("Marker#2", false, null)},
        {3,new Modules("Marker#3", false, null)}
    };

    string[] moduleList = { "None", "Marker#1", "Marker#2", "Marker#3" };
    string[] pixelList = { "None", "All", "Row 1", "Row 2" };
    bool patternGroup,
        chase,
        alternate,
        breathe,
        blink;

    /*
     * MaRker Window Creation
     */
    [MenuItem("Window/MaRker Module")]
    static void Init()
    {
        ModuleWindow window = EditorWindow.GetWindow<ModuleWindow>("MaRker");
        window.position = new Rect(60, 60, 280, 180);
        window.Show();
    }

    public void Awake()
    {
        UduinoManager.Instance.OnBoardConnected += OnBoardConnected;
    }

    void OnGUI()
    {
        UduinoManager.Instance.OnBoardConnected += OnBoardConnected;
        List<string> tempList = new List<string>();
        tempList.Add("None");
        foreach (KeyValuePair<int, Modules> i in mList)
        {
            if (i.Value.Connected) { tempList.Add(i.Value.name); };
        }
        string[] indexList = tempList.ToArray();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        indexer = EditorGUI.Popup(new Rect(0, 0, position.width, 20), "Module #: ", indexer, indexList);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (Selection.activeGameObject)
        {
            if (UduinoManager.Instance.hasBoardConnected())
            {
                foreach (KeyValuePair<int, Modules> i in mList)
                {
                    if (i.Value.Connected)
                    {
                        GUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
                        GUILayout.Label(i.Value.name);
                        GUILayout.EndVertical();
                    }
                }
            }
            else
            {
                GUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
                GUILayout.Label("No Arduino connected");
                GUILayout.EndVertical();
            }
            if (GUILayout.Button("Open Ports"))
            {
                Debug.Log("Opening Ports");
                UduinoManager.Instance.DiscoverPorts();
                connected = true;
            }

            if (GUILayout.Button("Close Ports"))
            {
                UduinoManager.Instance.CloseAllDevices();

                foreach(KeyValuePair<int, Modules> i in mList)
                {
                    i.Value.Connected = false;
                }

                Debug.Log("Closing Ports");
                connected = false;
            }

            if(mList.TryGetValue(indexer,out Modules temp))
            {
                if (temp.Connected)
                {
                    connected = true;
                }
                else
                {
                    connected = false;
                }
            }

            if (indexer != 0 && connected == true)
            {

                EditorGUILayout.Space();
                /* Activation Control
                    * Sends Toggle Control from GUI Elements to Physical Module using GPIO pins.
                    */
                if (GUILayout.Button("Activate Module"))
                {
                    UduinoManager.Instance.sendCommand("activate","1",brightness,delay);
                    Debug.Log("Module Activated");
                }
                if (GUILayout.Button("Deactivate Module"))
                {
                    UduinoManager.Instance.sendCommand("activate", "0",brightness,delay);
                    Debug.Log("Module Deactivated");
                }

                EditorGUILayout.Space();
                /* Brightness control
                    * for overall brightness of the lighting strip.
                    */
                EditorGUILayout.LabelField("Brightness:");
                brightnessTemp = EditorGUILayout.Slider(brightness, 0, 100);

                /* Delay Control
                 * for Special Patterns
                 */
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Delay(ms):");
                delayTemp = EditorGUILayout.Slider(delay, 0, 1000);
                if (brightness != brightnessTemp || delay != delayTemp)
                {
                    brightness = brightnessTemp;
                    delay = delayTemp;
                    UduinoManager.Instance.sendCommand("brightness", brightness);
                    UduinoManager.Instance.sendCommand("delayControl", delay);
                }

                EditorGUILayout.Space();
                /* Pixel Color
                    * For monocolor pixels only.
                    */
                //EditorGUILayout.LabelField("Pixel Color (Monotone Pattern Only): ");
                EditorGUILayout.Space();
                pixelIndexer = EditorGUI.Popup(new Rect(0, 210, position.width, 20), "Set Pixel Colors", pixelIndexer, pixelList);
                EditorGUILayout.Space();
                if (pixelIndexer != 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    colorChoice = EditorGUI.ColorField(new Rect(0, 230, position.width, 15), "New Color:", colorChoice);

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Set Color"))
                    {
                        
                    }
                }

                EditorGUILayout.Space();
                patternGroup = EditorGUILayout.BeginToggleGroup("Special Patterns", patternGroup);
                if (patternGroup)
                {
                    chase = EditorGUILayout.Toggle("Chase", chase);
                    if (chase)
                    {
                        alternate = false;
                        breathe = false;
                        blink = false;
                    }
                    alternate = EditorGUILayout.Toggle("Alternate", alternate);
                    if (alternate)
                    {
                        chase = false;
                        breathe = false;
                        blink = false;
                    }
                    breathe = EditorGUILayout.Toggle("Breathe", breathe);
                    if (breathe)
                    {
                        chase = false;
                        alternate = false;
                        blink = false;
                    }
                    blink = EditorGUILayout.Toggle("Blink", blink);
                    if (blink)
                    {
                        chase = false;
                        alternate = false;
                        breathe = false;
                    }
                }
                EditorGUILayout.EndToggleGroup();
            }
            
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnBoardConnected(UduinoDevice connectedDevice)
    {
        foreach(KeyValuePair<int, Modules> i in mList)
        {
            if (connectedDevice.name == i.Value.name)
            {
                i.Value.Connected = true;
                i.Value.device = connectedDevice;
            }
        }
    }

    void OnApplicationQuit()
    {
        UduinoManager.Instance.CloseAllDevices();
        Debug.Log("Closing Ports");
    }
}

public class Modules
{
    public string name { get; set; }
    public bool Connected { get; set; }
    public UduinoDevice device { get; set; }

    public Modules(string newName, bool newConnected, UduinoDevice newDevice)
    {
        this.name = newName;
        this.Connected = newConnected;
        this.device = newDevice;
    }
}
