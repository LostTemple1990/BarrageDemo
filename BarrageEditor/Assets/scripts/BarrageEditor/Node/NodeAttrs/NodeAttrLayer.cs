using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrLayer : NodeAttrUneditableDropdown
    {
        public override void BindItem(RectTransform parentTf)
        {
            base.BindItem(parentTf);
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
        }

        private void OnEditBtnClickHandler()
        {
            OpenEditView();
        }

        public override void OpenEditView()
        {
            string[] values = new string[] { "LayerEnemy", "LayerEnemyBullet", "LayerPlayer", "LayerPlayerBullet", "LayerItem",
                "LayerEffectBottom", "LayerEffectNormal", "LayerEffectTop", "LayerUIBottom", "LayerUINormal", "LayerUITop" };
            List<object> datas = new List<object> { this, "EditLayer", values };
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
