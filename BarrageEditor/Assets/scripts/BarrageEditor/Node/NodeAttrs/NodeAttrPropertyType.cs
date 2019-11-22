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
        public override void BindItem(RectTransform parentTf)
        {
            base.BindItem(parentTf);
            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
            optionList.Add(new Dropdown.OptionData("Prop_Velocity"));
            optionList.Add(new Dropdown.OptionData("Prop_Vx"));
            optionList.Add(new Dropdown.OptionData("Prop_Vy"));
            optionList.Add(new Dropdown.OptionData("Prop_VAngel"));
            optionList.Add(new Dropdown.OptionData("Prop_Acce"));
            optionList.Add(new Dropdown.OptionData("Prop_AccAngle"));
            optionList.Add(new Dropdown.OptionData("Prop_MaxVelocity"));
            optionList.Add(new Dropdown.OptionData("Prop_CurveRadius"));
            optionList.Add(new Dropdown.OptionData("Prop_CurveAngle"));
            optionList.Add(new Dropdown.OptionData("Prop_CurveDeltaR"));
            optionList.Add(new Dropdown.OptionData("Prop_CurveOmega"));
            optionList.Add(new Dropdown.OptionData("Prop_CurveCenterX"));
            optionList.Add(new Dropdown.OptionData("Prop_CurveCenterY"));
            optionList.Add(new Dropdown.OptionData("Prop_Alpha"));
            optionList.Add(new Dropdown.OptionData("Prop_ScaleX"));
            optionList.Add(new Dropdown.OptionData("Prop_ScaleY"));
            optionList.Add(new Dropdown.OptionData("Prop_LaserLength"));
            optionList.Add(new Dropdown.OptionData("Prop_LaserWidth"));
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
