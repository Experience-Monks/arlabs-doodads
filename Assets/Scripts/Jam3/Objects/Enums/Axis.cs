using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jam3
{
    [Flags]
    public enum Axis
    {
        None = 0,
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2,
        XY = X | Y,
        XZ = X | Z,
        YZ = Y | Z,
        XYZ = X | Y | Z,
    }
}
