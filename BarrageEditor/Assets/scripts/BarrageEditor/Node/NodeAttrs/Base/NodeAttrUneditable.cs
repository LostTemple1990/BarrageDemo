using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeAttrUneditable : BaseNodeAttr
    {
        protected Text _valueText;

        public NodeAttrUneditable()
            : base()
        {
            _prefabName = "NodeAttributeItemUneditable";
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
            _valueText = _itemTf.Find("ValueContainer/Label").GetComponent<Text>();
            // 值
            _valueText.text = GetValueString();
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
            UIEventListener.Get(_valueText.gameObject).RemoveAllEvents();
            _valueText = null;
            base.UnbindItem();
        }
    }
}
