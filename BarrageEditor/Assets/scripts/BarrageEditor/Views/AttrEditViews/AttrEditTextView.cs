using System;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class AttrEditTextView : ViewBase
    {
        private Text _titleText;
        private Text _attrNameText;

        private InputField _inputField;
        private GameObject _okBtn;
        private GameObject _cancelBtn;

        private BaseNodeAttr _nodeAttr;

        protected override void Init()
        {
            _titleText = _viewTf.Find("Panel/Title").GetComponent<Text>();
            _attrNameText = _viewTf.Find("Panel/Window/AttrName").GetComponent<Text>();

            _inputField = _viewTf.Find("Panel/Window/ScrollView/Viewport/InputField").GetComponent<InputField>();
            _okBtn = _viewTf.Find("Panel/OKBtn").gameObject;
            _cancelBtn = _viewTf.Find("Panel/CancelBtn").gameObject;

            AddListeners();
        }

        private void AddListeners()
        {
            UIEventListener.Get(_okBtn).AddClick(OnOkBtnClickHandler);
            UIEventListener.Get(_cancelBtn).AddClick(OnCancelBtnClickHandler);
        }

        public override void OnShow(object data)
        {
            _nodeAttr = data as BaseNodeAttr;
            SetTitle("EditText");
            SetAttrName(_nodeAttr.GetAttrName());
            SetDefaultText(_nodeAttr.GetValueString());
            _inputField.ActivateInputField();
        }

        private void SetTitle(string title)
        {
            _titleText.text = title;
        }

        private void SetAttrName(string attrName)
        {
            _attrNameText.text = attrName;
        }

        private void SetDefaultText(string text)
        {
            _inputField.text = text;
        }

        private void OnOkBtnClickHandler()
        {
            string msg;
            string curValue = _inputField.text;
            if ( _nodeAttr.IsValueAvailable(curValue, out msg) )
            {
                _nodeAttr.SetValue(curValue);

                Close();
            }
            else
            {

            }
        }

        private void OnCancelBtnClickHandler()
        {
            Close();
        }
    }
}
