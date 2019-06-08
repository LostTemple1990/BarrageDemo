using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrBool : BaseNodeAttr
    {
        public override void BindItem(GameObject item)
        {
            base.BindItem(item);
            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
            optionList.Add(new Dropdown.OptionData("true"));
            optionList.Add(new Dropdown.OptionData("false"));
            _dropDown.options = optionList;
            _dropDown.onValueChanged.AddListener(OnDropdownValueChangedHandler);

            UIEventListener.Get(_editBtnGo).AddClick(OnEditBtnClickHandler);
        }

        private void OnDropdownValueChangedHandler(int value)
        {
            Dropdown.OptionData selectData = _dropDown.options[value];
            OnAttributeValueEdit(selectData.text);
            _valueText.text = selectData.text;
        }

        private void OnEditBtnClickHandler()
        {
            UIManager.GetInstance().OpenView(ViewID.AttrEditTextView, this);
        }

        public override void UnbindItem()
        {
            _dropDown.onValueChanged.RemoveAllListeners();
            base.UnbindItem();
        }
    }
}
