using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollisionObject
{
    int GetCollisionParams(out float arg1,out float arg2,out float arg3,out float arg4);
}