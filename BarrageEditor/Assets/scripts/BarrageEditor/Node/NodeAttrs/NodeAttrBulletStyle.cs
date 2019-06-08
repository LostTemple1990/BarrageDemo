using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrBulletStyle : BaseNodeAttr
    {
        public override void BindItem(GameObject item)
        {
            base.BindItem(item);
            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
            List<BulletStyleCfg> bulletCfgs = DatabaseManager.BulletDatabase.GetBulletStyleCfgs();
            for (int i = 0; i < bulletCfgs.Count; i++)
            {
                optionList.Add(new Dropdown.OptionData(bulletCfgs[i].name));
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
            UIManager.GetInstance().OpenView(ViewID.AttrSelectBulletStyleView, this);
        }

        public override void UnbindItem()
        {
            _dropDown.onValueChanged.RemoveAllListeners();
            base.UnbindItem();
        }
    }
}
