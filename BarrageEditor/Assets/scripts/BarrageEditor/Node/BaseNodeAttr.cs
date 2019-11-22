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
        protected object _value;
        protected object _preValue;
        public delegate bool CheckValueAvailable(object value,out string msg);

        protected CheckValueAvailable _checkValueFunc;

        protected GameObject _itemGo;
        protected RectTransform _itemTf;
        protected Text _attrNameText;
        protected GameObject _editBtnGo;

        /// <summary>
        /// value是否被改变过
        /// </summary>
        protected bool _isValueEdit;
        /// <summary>
        /// 临时缓存被改变过的value
        /// </summary>
        protected string _cacheEditValue;

        protected string _prefabName;

        public virtual void Init(BaseNode node,string name,CheckValueAvailable checkFunc)
        {
            _node = node;
            _attrName = name;
            _checkValueFunc = checkFunc;
            _value = "";
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
        /// <para>value</para>
        /// <para>updateNode 是否更新Node的显示</para>
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetValue(object value, bool notifyNode = true)
        {
            _preValue = _value;
            _value = value;
            _isValueEdit = false;
        }

        /// <summary>
        /// 获取value的值
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            return _value;
        }

        public object GetPreValue()
        {
            return _preValue;
        }

        /// <summary>
        /// 属性所属于的节点
        /// </summary>
        public BaseNode Node
        {
            get
            {
                return _node;
            }
        }

        /// <summary>
        /// 获取节点属性的字符串
        /// </summary>
        /// <returns></returns>
        public virtual string GetValueString()
        {
            return _value.ToString();
        }

        public virtual void BindItem(RectTransform parentTf)
        {
            _itemGo = ResourceManager.GetInstance().GetPrefab("Prefabs/Views/MainView", _prefabName, parentTf, false, true);
            _itemTf = _itemGo.GetComponent<RectTransform>();
            _editBtnGo = _itemTf.Find("EditBtn").gameObject;
            _attrNameText = _itemTf.Find("AttrNameText").GetComponent<Text>();
            // 名称
            _attrNameText.text = GetAttrName();
        }

        public virtual void UnbindItem()
        {
            if (_itemGo == null)
                return;
            UIEventListener.Get(_editBtnGo).RemoveAllEvents();
            if (!_editBtnGo.gameObject.activeSelf)
            {
                _editBtnGo.gameObject.SetActive(true);
            }
            _editBtnGo = null;
            _attrNameText = null;
            ResourceManager.GetInstance().RestorePrefabToPool("Prefabs/Views/MainView/" + _prefabName, _itemGo);
            _itemGo = null;
            _itemTf = null;
            if ( _isValueEdit )
            {
                SetValue(_cacheEditValue);
                _cacheEditValue = null;
                _isValueEdit = false;
            }
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
        BulletId = 4,
        CustomizedType = 5,
        ParaList = 6,
        MoveMode = 7,
        SCCondition = 8,
        ItemType = 9,
        CollisionGroups = 10,
        ResistEliminatedTypes = 11,
        PropertyChangeMode = 12,
        PropertyType = 13,
        BlendMode = 14,
        Layer = 15,
        ShapeType = 16,
        DirectionMode = 17,
        UnitEventType = 18,
        LaserId = 19,
        EnemyStyle = 20,
        ReboundBorder = 21,
        DropItems = 22,
    }

    public class NodeAttrData
    {
        public NodeAttrType type;
        public string attrName;
        public object attrValue;
    }
}
