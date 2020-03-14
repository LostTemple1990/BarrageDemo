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
        public NodeAttrMoveMode()
            : base()
        {
            _optionStringList = new List<string>();
            _optionStringList.Add("IntModeLinear");
            _optionStringList.Add("IntModeEaseInQuad");
            _optionStringList.Add("IntModeEaseOutQuad");
            _optionStringList.Add("IntModeEaseInOutQuad");
            _optionStringList.Add("IntModeSin");
            _optionStringList.Add("IntModeCos");
        }

        public override void BindItem(RectTransform parentTf)
        {
            base.BindItem(parentTf);
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
