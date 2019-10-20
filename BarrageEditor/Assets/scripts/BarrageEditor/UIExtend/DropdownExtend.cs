using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BarrageEditor
{
    public class DropdownExtend : Dropdown
    {
        new public void Show()
        {
            base.Show();
            var toggleRoot = transform.Find("Dropdown List/Viewport/Content");
            var toggleList = toggleRoot.GetComponentsInChildren<Toggle>(false);
            foreach (var toggle in toggleList)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.isOn = false;
                var tmp = toggle;
                toggle.onValueChanged.AddListener(isOn => { OnSelectItemExtend(tmp); });
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            Show();
        }

        protected void OnSelectItemExtend(Toggle toggle)
        {
            if (!toggle.isOn)
            {
                toggle.isOn = true;
            }
            var selectedIndex = -1;
            var tr = toggle.transform;
            var parent = tr.parent;
            for (var i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i) != tr) continue;
                selectedIndex = i - 1;
                break;
            }
            if (selectedIndex < 0)
                return;
            if (value == selectedIndex)
                onValueChanged.Invoke(value);
            else
                value = selectedIndex;
            Hide();
        }
    }
}