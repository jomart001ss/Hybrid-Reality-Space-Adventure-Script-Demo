using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Linq;

public class ArduinoManager : Singleton<ArduinoManager>
{
    string portName = "COM3";
    private SerialPort sp;
    private Transform newTransform;
    public int buttonCount;

    void Start()
    {
        InitializeArrays();
        sp = new SerialPort(portName, 9600);
        bool portExists = SerialPort.GetPortNames().Any(x => x == portName);
        if (portExists)
        {
            sp.Open();
            sp.ReadTimeout = 1;
        }
    }

    private bool[] buttonStates;
    private bool[] buttonDownStates;

    void InitializeArrays ()
    {
        buttonStates = new bool[buttonCount];
        buttonDownStates = new bool[buttonCount];
    }

    void Update()
    {
        TurnOffAllDownStates();
        if (sp.IsOpen)
        {
            try
            {
                int byteRead = sp.ReadByte();
                SetButtonState(byteRead);
            }
            catch (System.Exception)
            {
            }
        }
    }

    void TurnOffAllDownStates()
    {
        for (int i = 0; i < buttonCount; i++)
        {
            buttonDownStates[i] = false;
        }
    }

    void SetButtonState (int newByte)
    {
        bool pressed = false;
        if (newByte % 10 == 0)
        {
            pressed = false;
            newByte /= 10;
        }
        else
        {
            pressed = true;
        }
        int index = newByte - 1;
        bool wasPressed = buttonStates[index];
        buttonStates[index] = pressed;
        buttonDownStates[index] = wasPressed != pressed && pressed;
    }

    public bool GetButtonState (int index)
    {
        if (index < buttonCount)
        {
            return buttonStates[index];
        }
        else
        {
            return false;
        }
    }

    public bool GetButtonDownState (int index)
    {
        if (index < buttonCount)
        {
            return buttonDownStates[index];
        }
        else
        {
            return false;
        }  
    }
}
