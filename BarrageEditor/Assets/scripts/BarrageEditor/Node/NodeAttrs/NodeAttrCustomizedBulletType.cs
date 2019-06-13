using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrCustomizedBulletType : BaseNodeAttr
    {
        public override void BindItem(GameObject item)
        {
            base.BindItem(item);
            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
            CustomDefineType type = CustomDefine.GetTypeByNodeType(_node.GetNodeType());
            List<string> list = CustomDefine.GetCustomDefineListByType(type);
            for (int i=0;i<list.Count;i++)
            {
                optionList.Add(new Dropdown.OptionData(list[i]));
            }
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
            OpenEditView();
        }

        public override void OpenEditView()
        {
            UIManager.GetInstance().OpenView(ViewID.AttrSelectCustomizedTypeView, this);
        }

        public override void UnbindItem()
        {
            _dropDown.onValueChanged.RemoveAllListeners();
            base.UnbindItem();
        }
    }
}
