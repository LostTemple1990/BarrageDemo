using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrLayer : BaseNodeAttr
    {
        public override void BindItem(GameObject item)
        {
            base.BindItem(item);
            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
            optionList.Add(new Dropdown.OptionData("LayerEnemy"));
            optionList.Add(new Dropdown.OptionData("LayerEnemyBullet"));
            optionList.Add(new Dropdown.OptionData("LayerPlayer"));
            optionList.Add(new Dropdown.OptionData("LayerPlayerBullet"));
            optionList.Add(new Dropdown.OptionData("LayerItem"));
            optionList.Add(new Dropdown.OptionData("LayerEffectBottom"));
            optionList.Add(new Dropdown.OptionData("LayerEffectNormal"));
            optionList.Add(new Dropdown.OptionData("LayerEffectTop"));
            optionList.Add(new Dropdown.OptionData("LayerUIBottom"));
            optionList.Add(new Dropdown.OptionData("LayerUINormal"));
            optionList.Add(new Dropdown.OptionData("LayerUITop"));
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
            UIManager.GetInstance().OpenView(ViewID.AttrEditTextView, this);
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
