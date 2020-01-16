﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrShapeType : NodeAttrUneditableDropdown
    {
        public override void BindItem(RectTransform parentTf)
        {
            base.BindItem(parentTf);
            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
            optionList.Add(new Dropdown.OptionData("TypeCircle"));
            optionList.Add(new Dropdown.OptionData("TypeRect"));
            optionList.Add(new Dropdown.OptionData("TypeItalicRect"));
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
            string[] values = new string[] { "TypeCircle", "TypeRect", "TypeItalicRect" };
            List<object> datas = new List<object> { this, "EditShapeType", values };
            UIManager.GetInstance().OpenView(ViewID.AttrEditRadioView, datas);
        }

        public override void UnbindItem()
        {
            base.UnbindItem();
        }
    }
}
