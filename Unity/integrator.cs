using UnityEngine;
using System;
using System.IO.Ports;


public class Integrator : MonoBehaviour
{
    //Define Port variables
    SerialPort send = new SerialPort(/*Insert port here*/"", 9600);  //generic bound rate, should be checked

    void Initialize()
    {
        
    }

    void Send()
    {

    }
}