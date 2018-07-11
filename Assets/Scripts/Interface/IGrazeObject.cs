using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrazeObject
{
    int GetGrazeParams(out float arg1, out float arg2, out float arg3, out float arg4);
}