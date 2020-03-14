using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrLayer : NodeAttrUneditableDropdown
    {
        public NodeAttrLayer()
            : base()
        {
            _optionStringList = new List<string>();
            _optionStringList.Add("LayerEnemy");
            _optionStringList.Add("LayerEnemyBullet");
            _optionStringList.Add("LayerPlayer");
            _optionStringList.Add("LayerPlayerBullet");
            _optionStringList.Add("LayerItem");
            _optionStringList.Add("LayerEffectBottom");
            _optionStringList.Add("LayerEffectNormal");
            _optionStringList.Add("LayerEffectTop");
            _optionStringList.Add("LayerUIBottom");
            _optionStringList.Add("LayerUINormal");
            _optionStringList.Add("LayerUITop");
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
