﻿using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

using System.Runtime.InteropServices;

public class CircleCollisionChecker
{
    [DllImport("MyUnityPlugin")]
    public static extern int InitFoo(int f_new = 0);

    [DllImport("MyUnityPlugin")]
    public static extern int DoFoo(int bar = 0);

    [DllImport("MyUnityPlugin")]
    public static extern int TermFoo();

    
}