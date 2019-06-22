using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BarrageEditor
{
    public class DropdownEx : Dropdown
    {
        new public int value
        {
            get
            {
                return base.value;
            }
            set
            {
                if (Application.isPlaying && options.Count == 0)
                    return;
                if (this.value == value)
                {
                    RefreshShownValue();
                    UISystemProfilerApi.AddMarker("Dropdown.value", this);
                    onValueChanged.Invoke(value);
                }
                else
                {
                    base.value = value;
                }
            }
        }

        //void Set(int value, bool sendCallback = true)
        //{
        //    if (Application.isPlaying && options.Count == 0)
        //        return;
        //    if ( this.value == value )
        //    {
        //        RefreshShownValue();
        //        if ( sendCallback )
        //        {
        //            UISystemProfilerApi.AddMarker("Dropdown.value", this);
        //            onValueChanged.Invoke(value);
        //        }
        //    }
        //    else
        //    {
        //        base.value = value;
        //    }
        //}
    }
}
