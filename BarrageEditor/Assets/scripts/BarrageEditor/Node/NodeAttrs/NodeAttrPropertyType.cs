using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrPropertyType : NodeAttrUneditableDropdown
    {
        public NodeAttrPropertyType()
            : base()
        {
            _optionStringList = new List<string>();
            _optionStringList.Add("Prop_Velocity");
            _optionStringList.Add("Prop_Vx");
            _optionStringList.Add("Prop_Vy");
            _optionStringList.Add("Prop_VAngel");
            _optionStringList.Add("Prop_Acce");
            _optionStringList.Add("Prop_AccAngle");
            _optionStringList.Add("Prop_MaxVelocity");
            _optionStringList.Add("Prop_CurveRadius");
            _optionStringList.Add("Prop_CurveAngle");
            _optionStringList.Add("Prop_CurveDeltaR");
            _optionStringList.Add("Prop_CurveOmega");
            _optionStringList.Add("Prop_CurveCenterX");
            _optionStringList.Add("Prop_CurveCenterY");
            _optionStringList.Add("Prop_Alpha");
            _optionStringList.Add("Prop_ScaleX");
            _optionStringList.Add("Prop_ScaleY");
            _optionStringList.Add("Prop_LaserLength");
            _optionStringList.Add("Prop_LaserWidth");
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
            string[] values = new string[] { "Prop_Velocity", "Prop_Vx", "Prop_Vy", "Prop_VAngel",
                "Prop_Acce", "Prop_AccAngle", "Prop_MaxVelocity",
                "Prop_CurveRadius", "Prop_CurveAngle", "Prop_CurveDeltaR", "Prop_CurveOmega", "Prop_CurveCenterX", "Prop_CurveCenterY",
                "Prop_Alpha", "Prop_ScaleX", "Prop_ScaleY", "Prop_LaserLength", "Prop_LaserWidth" };
            List<object> datas = new List<object> { this, "EditPropertyType", values };
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
