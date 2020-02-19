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
    struct Modules
    {
        public int number;
        public string name;
        public bool Connected;
        public UduinoDevice device;

        public Modules(int number, string newName, bool newConnected, UduinoDevice newDevice)
        {
            this.number = number;
            this.name = newName;
            this.Connected = newConnected;
            this.device = newDevice;
        }
    }

    UduinoManager u;
    static float brightness = 50.0f,
        delay = 100.0f,
        brightnessTemp = 0,
        delayTemp = 0;
    int indexer = 0,
        pixelIndexer = 0;
    bool connected = false;
    Color colorChoice;
    Modules module1 = new Modules(1, "Marker#1", false, null);
    Modules module2 = new Modules(2, "Marker#2", false, null);
    Modules module3 = new Modules(3, "Marker#3", false, null);
    string[] moduleList = { "None", "#1", "#2", "#3" };
    string[] pixelList = { "None", "All", "Row 1", "Row 2" };
    bool patternGroup,
        chase,
        alternate,
        breathe,
        blink;

    /* 
 //Test Variables
 string[] moduleList = { "None","1", "2", "3" };

 string DeviceName = "Marker #1";
 string ServiceUUID = "1801";
 string SubscribeCharacteristic = "2A05";
 string WriteCharacteristic = "2222";


  * Bluetooth Connectivity
  * State machine
 enum States
 {
     None,
     Scan,
     ScanRSSI,
     Connect,
     Subscribe,
     Unsubscribe,
     Disconnect,
 }

 private bool _connected = false;
 private float _timeout = 0f;
 private States _state = States.None;
 private string _deviceAddress;
 private bool _foundSubscribeID = false;
 private bool _foundWriteID = false;
 private byte[] _dataBytes = null;
 private bool _rssiOnly = false;
 private int _rssi = 0;

 void Reset()
 {
     _connected = false;
     _timeout = 0f;
     _state = States.None;
     _deviceAddress = null;
     _foundSubscribeID = false;
     _foundWriteID = false;
     _dataBytes = null;
     _rssi = 0;
 }

 void SetState(States newState, float timeout)
 {
     _state = newState;
     _timeout = timeout;
 }

 void StartProcess()
 {
     Reset();
     BluetoothLEHardwareInterface.Initialize(true, false, () => {

         SetState(States.Scan, 0.1f);

     }, (error) => {

         BluetoothLEHardwareInterface.Log("Error during initialize: " + error);
     });
 }

     */

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

    void OnGUI()
    {
        UduinoManager.Instance.OnBoardConnected += OnBoardConnected;
        if (Selection.activeGameObject)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            indexer = EditorGUI.Popup(new Rect(0, 0, position.width, 20), "Module #: ", indexer, moduleList);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Connect"))
            {
                Debug.Log("Opening Ports");
                UduinoManager.Instance.DiscoverPorts();
                connected = true;
            }

            if (GUILayout.Button("Disconnect"))
            {
                UduinoManager.Instance.CloseAllDevices();
                
                Debug.Log("Closing Ports");
                connected = false;
                brightnessTemp = 0.0f;
                delayTemp = 0.0f;
            }

            EditorGUILayout.Space();

            if (indexer != 0 && connected)
            {
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
        if (connectedDevice.name == "Marker #1")
        {
            module1.Connected = true;
            module1.device = connectedDevice;
        }
        else if (connectedDevice.name == "Marker #2")
        {
            module2.Connected = true;
            module2.device = connectedDevice;
        }
        else if (connectedDevice.name == "Marker #3")
        {
            module3 = new Modules(3, "Marker #3", true, connectedDevice);
        }
    }

    /* Deprecated DO NOT USE
     * 
     * 
     * 
     * 
    //Bluetooth Update
    void Update()
    {
        if (_timeout > 0f)
        {
            _timeout -= Time.deltaTime;
            if (_timeout <= 0f)
            {
                _timeout = 0f;

                switch (_state)
                {
                    case States.None:
                        break;

                    case States.Scan:
                        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) => {

                            // if your device does not advertise the rssi and manufacturer specific data
                            // then you must use this callback because the next callback only gets called
                            // if you have manufacturer specific data

                            if (!_rssiOnly)
                            {
                                if (name.Contains(DeviceName))
                                {
                                    BluetoothLEHardwareInterface.StopScan();

                                    // found a device with the name we want
                                    // this example does not deal with finding more than one
                                    _deviceAddress = address;
                                    SetState(States.Connect, 0.5f);
                                }
                            }

                        }, (address, name, rssi, bytes) => {

                            // use this one if the device responses with manufacturer specific data and the rssi

                            if (name.Contains(DeviceName))
                            {
                                if (_rssiOnly)
                                {
                                    _rssi = rssi;
                                }
                                else
                                {
                                    BluetoothLEHardwareInterface.StopScan();

                                    // found a device with the name we want
                                    // this example does not deal with finding more than one
                                    _deviceAddress = address;
                                    SetState(States.Connect, 0.5f);
                                }
                            }

                        }, _rssiOnly); // this last setting allows RFduino to send RSSI without having manufacturer data

                        if (_rssiOnly)
                            SetState(States.ScanRSSI, 0.5f);
                        break;

                    case States.Connect:
                        // set these flags
                        _foundSubscribeID = false;
                        _foundWriteID = false;

                        // note that the first parameter is the address, not the name. I have not fixed this because
                        // of backwards compatiblity.
                        // also note that I am note using the first 2 callbacks. If you are not looking for specific characteristics you can use one of
                        // the first 2, but keep in mind that the device will enumerate everything and so you will want to have a timeout
                        // large enough that it will be finished enumerating before you try to subscribe or do any other operations.
                        BluetoothLEHardwareInterface.ConnectToPeripheral(_deviceAddress, null, null, (address, serviceUUID, characteristicUUID) => {

                            if (IsEqual(serviceUUID, ServiceUUID))
                            {
                                _foundSubscribeID = _foundSubscribeID || IsEqual(characteristicUUID, SubscribeCharacteristic);
                                _foundWriteID = _foundWriteID || IsEqual(characteristicUUID, WriteCharacteristic);

                                // if we have found both characteristics that we are waiting for
                                // set the state. make sure there is enough timeout that if the
                                // device is still enumerating other characteristics it finishes
                                // before we try to subscribe
                                if (_foundSubscribeID && _foundWriteID)
                                {
                                    _connected = true;
                                    SetState(States.Subscribe, 2f);
                                }
                            }
                        });
                        break;

                    case States.Subscribe:
                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, ServiceUUID, SubscribeCharacteristic, null, (address, characteristicUUID, bytes) => {

                            // we don't have a great way to set the state other than waiting until we actually got
                            // some data back. For this demo with the rfduino that means pressing the button
                            // on the rfduino at least once before the GUI will update.
                            _state = States.None;

                            // we received some data from the device
                            _dataBytes = bytes;
                        });
                        break;

                    case States.Unsubscribe:
                        BluetoothLEHardwareInterface.UnSubscribeCharacteristic(_deviceAddress, ServiceUUID, SubscribeCharacteristic, null);
                        SetState(States.Disconnect, 4f);
                        break;

                    case States.Disconnect:
                        if (_connected)
                        {
                            BluetoothLEHardwareInterface.DisconnectPeripheral(_deviceAddress, (address) => {
                                BluetoothLEHardwareInterface.DeInitialize(() => {

                                    _connected = false;
                                    _state = States.None;
                                });
                            });
                        }
                        else
                        {
                            BluetoothLEHardwareInterface.DeInitialize(() => {

                                _state = States.None;
                            });
                        }
                        break;
                }
            }
        }
    }

    void ImageParse()
    {

    }

    void SendByte(byte value)
    {
        byte[] data = new byte[] { value };
        BluetoothLEHardwareInterface.WriteCharacteristic(_deviceAddress, ServiceUUID, WriteCharacteristic, data, data.Length, true, (characteristicUUID) => {

            BluetoothLEHardwareInterface.Log("Write Succeeded");
        });
    }

    bool IsEqual(string uuid1, string uuid2)
    {
        return (uuid1.ToUpper().CompareTo(uuid2.ToUpper()) == 0);
    }

    */

    void OnApplicationQuit()
    {
        UduinoManager.Instance.CloseAllDevices();
        Debug.Log("Closing Ports");
    }
}
