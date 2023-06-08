using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commands
{
    public static Dictionary<KeyCode[],Func<Vector3>> commands = new Dictionary<KeyCode[],Func<Vector3>>()
    {
        {new KeyCode[3]{ KeyCode.UpArrow, KeyCode.I, KeyCode.DownArrow }, () => {return new Vector3(0f, -20f, 0f);}}
    };
}
