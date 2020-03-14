using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrUnitEventType : NodeAttrUneditableDropdown
    {
        public NodeAttrUnitEventType()
            : base()
        {
            _optionStringList = new List<string>();
            _optionStringList.Add("OnKill");
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
            string[] values = new string[] { "OnKill" };
            List<object> datas = new List<object> { this, "EditUnitEventTriggerType", values };
            UIManager.GetInstance().OpenView(ViewID.AttrEditRadioView, datas);
        }

        public override void UnbindItem()
        {
            base.UnbindItem();
        }
    }
}
