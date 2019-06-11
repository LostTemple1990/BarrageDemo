using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class BaseNodeAttr
    {
        protected BaseNode _node;
        protected NodeAttrType _type;
        protected string _attrName;
        public string attrDesc;
        private object _value;
        public delegate bool CheckValueAvailable(object value,out string msg);

        private CheckValueAvailable _checkValueFunc;

        protected GameObject _itemGo;
        protected Text _attrNameText;
        protected InputField _valueText;
        protected Dropdown _dropDown;
        protected Image _arrowImg;
        protected GameObject _editBtnGo;

        /// <summary>
        /// value是否被改变过
        /// </summary>
        protected bool _isValueEdit;
        /// <summary>
        /// 临时缓存被改变过的value
        /// </summary>
        protected string _cacheEditValue;

        public virtual void Init(BaseNode node,string name,CheckValueAvailable checkFunc)
        {
            _node = node;
            _attrName = name;
            _checkValueFunc = checkFunc;
        }

        public virtual bool IsValueAvailable(object value,out string msg)
        {
            if ( _checkValueFunc == null )
            {
                msg = "";
                return true;
            }
            else
            {
                return _checkValueFunc(value, out msg);
            }
        }

        /// <summary>
        /// 设置节点属性的值
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetValue(object value)
        {
            _value = value;
            _isValueEdit = false;
            if ( _itemGo != null )
            {
                _valueText.text = GetValueString();
            }
            _node.UpdateDesc();
        }

        /// <summary>
        /// 获取value的值
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            return _value;
        }

        /// <summary>
        /// 获取节点属性的字符串
        /// </summary>
        /// <returns></returns>
        public virtual string GetValueString()
        {
            return _value.ToString();
        }

        public virtual void BindItem(GameObject item)
        {
            _itemGo = item;
            RectTransform tf = item.GetComponent<RectTransform>();
            _dropDown = tf.Find("Dropdown").GetComponent<Dropdown>();
            _arrowImg = tf.Find("Dropdown/Arrow").GetComponent<Image>();
            _editBtnGo = tf.Find("EditBtn").gameObject;
            _attrNameText = tf.Find("AttrNameText").GetComponent<Text>();
            _valueText = tf.Find("Dropdown/Label").GetComponent<InputField>();
            // 名称
            _attrNameText.text = GetAttrName();
            // 值
            _valueText.text = GetValueString();

            _valueText.onEndEdit.AddListener(OnAttributeValueEdit);
        }

        /// <summary>
        /// 当节点属性的值被编辑完成的时候调用
        /// </summary>
        /// <param name="value"></param>
        protected void OnAttributeValueEdit(string value)
        {
            _isValueEdit = true;
            _cacheEditValue = value;
        }

        public virtual void UnbindItem()
        {
            _valueText.onEndEdit.RemoveAllListeners();
            _dropDown.options.Clear();
            UIEventListener.Get(_editBtnGo).RemoveAllEvents();
            _dropDown = null;
            _arrowImg = null;
            _editBtnGo = null;
            _attrNameText = null;
            _valueText = null;
            _itemGo = null;
            if ( _isValueEdit )
            {
                SetValue(_cacheEditValue);
                _cacheEditValue = null;
                _isValueEdit = false;
            }
        }

        public string GetAttrName()
        {
            return _attrName;
        }

        /// <summary>
        /// 打开该属性的编辑界面
        /// </summary>
        public virtual void OpenEditView()
        {

        }

        public virtual string ToDesc()
        {
            return "undefined node attr desc";
        }

    }

    public enum NodeAttrType : byte
    {
        Any = 0,
        Bool = 1,
        Int = 2,
        String = 3, 
    }

    public class NodeAttrData
    {
        public NodeAttrType type;
        public string attrName;
        public object attrValue;
    }
}
