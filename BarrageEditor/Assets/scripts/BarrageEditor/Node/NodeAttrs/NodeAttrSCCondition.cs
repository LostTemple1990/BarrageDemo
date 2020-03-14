using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrSCCondition : NodeAttrUneditableDropdown
    {
        public NodeAttrSCCondition()
            : base()
        {
            _optionStringList = new List<string>();
            _optionStringList.Add("ConditionEliminateAll");
            _optionStringList.Add("ConditionEliminateOne");
            _optionStringList.Add("ConditionEliminateSpecificOne");
            _optionStringList.Add("ConditionTimeOver");
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
            string[] values = new string[] { "ConditionEliminateAll", "ConditionEliminateOne", "ConditionEliminateSpecificOne", "ConditionTimeOver" };
            List<object> datas = new List<object> { this, "EditSpellCardCondition", values };
            UIManager.GetInstance().OpenView(ViewID.AttrEditRadioView, datas);
        }

        public override void UnbindItem()
        {
            base.UnbindItem();
        }
    }
}
