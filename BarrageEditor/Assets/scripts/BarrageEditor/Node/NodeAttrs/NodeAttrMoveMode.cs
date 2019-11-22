using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrMoveMode : NodeAttrUneditableDropdown
    {
        public override void BindItem(RectTransform parentTf)
        {
            base.BindItem(parentTf);
            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
            optionList.Add(new Dropdown.OptionData("IntModeLinear"));
            optionList.Add(new Dropdown.OptionData("IntModeEaseInQuad"));
            optionList.Add(new Dropdown.OptionData("IntModeEaseOutQuad"));
            optionList.Add(new Dropdown.OptionData("IntModeEaseInOutQuad"));
            optionList.Add(new Dropdown.OptionData("IntModeSin"));
            optionList.Add(new Dropdown.OptionData("IntModeCos"));
            _dropDown.options = optionList;
            _dropDown.onValueChanged.AddListener(OnDropdownValueChangedHandler);

            UIEventListener.Get(_editBtnGo).AddClick(OnEditBtnClickHandler);
        }

        private void OnDropdownValueChangedHandler(int value)
        {
            Dropdown.OptionData selectData = _dropDown.options[value];
            OnAttributeValueEdit(selectData.text);
        }

        private void OnEditBtnClickHandler()
        {
            OpenEditView();
        }

        public override void OpenEditView()
        {
            string[] values = new string[] { "IntModeLinear", "IntModeEaseInQuad", "IntModeEaseOutQuad", "IntModeEaseInOutQuad", "IntModeSin", "IntModeCos" };
            List<object> datas = new List<object> { this, "EditInterpolationMode", values };
            UIManager.GetInstance().OpenView(ViewID.AttrEditRadioView, datas);
        }

        public override void UnbindItem()
        {
            if (_itemGo == null)
                return;
            _dropDown.onValueChanged.RemoveAllListeners();
            base.UnbindItem();
        }
    }
}
