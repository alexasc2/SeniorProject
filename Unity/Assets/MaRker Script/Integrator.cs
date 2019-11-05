/* Main Script for MaRker
 * 
 * Reminder: Set the API Compatability Level in Unity (Under Opimization) to .NET 2.0, NOT .NET 2.0 Subset
 * 
 */

using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class Integrator : MonoBehaviour
{
    //SerialPort takes two arguments: Path, baud rate
    public static SerialPort sp = new SerialPort("test", 9600);

    void Start()
    {
        //Port assert
        Debug.Assert(sp != null);

        if (!sp.IsOpen)
        {
            print("New Port");
            sp.Open();
            sp.ReadTimeout = 500;
        }
        else
        {
            print("Port is already open!");
        }
    }

    void Send(int AssetID)
    {

    }

    void Quit()
    {
        sp.Close();
    }
}