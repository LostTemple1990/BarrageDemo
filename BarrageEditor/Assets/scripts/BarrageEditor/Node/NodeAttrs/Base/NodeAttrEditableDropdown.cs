using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrEditableDropdown : BaseNodeAttr
    {
        protected Dropdown _dropDown;
        protected Image _arrowImg;

        protected InputField _valueText;

        public NodeAttrEditableDropdown()
            : base()
        {
            _prefabName = "NodeAttributeItemEditableDropdown";
        }

        /// <summary>
        /// 设置节点属性的值
        /// <para>value</para>
        /// <para>updateNode 是否更新Node的显示</para>
        /// </summary>
        /// <param name="value"></param>
        public override void SetValue(object value, bool notifyNode = true)
        {
            _preValue = _value;
            _value = value;
            _isValueEdit = false;
            if (_itemGo != null)
            {
                _valueText.text = GetValueString();
            }
            if (notifyNode)
            {
                _node.OnAttributeValueChanged(this);
            }
        }

        public override void BindItem(RectTransform parentTf)
        {
            base.BindItem(parentTf);
            _dropDown = _itemTf.Find("Dropdown").GetComponent<Dropdown>();
            _arrowImg = _itemTf.Find("Dropdown/Arrow").GetComponent<Image>();

            _valueText = _itemTf.Find("Dropdown/Label").GetComponent<InputField>();
            // 值
            _valueText.text = GetValueString();
            _valueText.onEndEdit.AddListener(OnAttributeValueEdit);
            UIEventListener.Get(_valueText.gameObject).AddPointerEnter(OnPointerEnter);
            UIEventListener.Get(_valueText.gameObject).AddPointerExit(OnPointerExit);
        }

        private void OnPointerEnter()
        {
            string value = GetValueString();
            if (value != null && value != "")
            {
                UIManager.GetInstance().OpenView(ViewID.TooltipView, value);
            }
        }

        private void OnPointerExit()
        {
            UIManager.GetInstance().CloseView(ViewID.TooltipView);
        }

        public override void UnbindItem()
        {
            if (_itemGo == null)
                return;
            // dropdown
            _dropDown.options.Clear();
            _dropDown.onValueChanged.RemoveAllListeners();
            _dropDown = null;
            if (!_arrowImg.gameObject.activeSelf)
            {
                _arrowImg.gameObject.SetActive(true);
            }
            _arrowImg = null;
            // text
            UIEventListener.Get(_valueText.gameObject).RemoveAllEvents();
            _valueText.onEndEdit.RemoveAllListeners();
            _valueText = null;
            base.UnbindItem();
        }

    }
}
