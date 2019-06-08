using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class BaseNode
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        public NodeType nodeType;
        /// <summary>
        /// 参数
        /// </summary>
        public List<BaseNodeAttr> attrs;
        /// <summary>
        /// 当前是否展开子节点
        /// </summary>
        public bool isExpand;
        /// <summary>
        /// 子节点
        /// </summary>
        public List<BaseNode> childs;
        public BaseNode parentNode;

        protected GameObject _nodeItemGo;
        protected RectTransform _nodeItemTf;
        protected Image _expandImg;
        protected Image _functionImg;
        protected Image _selectedImg;
        protected GameObject _clickGo;
        protected Text _descText;

        protected int _nodeShowIndex;
        protected int _nodeDepth;

        protected bool _isSelected;
        protected float _lastClickTime;
        protected float _clickCount;


        public virtual void Init(BaseNode parent,RectTransform parentTf)
        {
            _nodeItemGo = ResourceManager.GetInstance().GetPrefab("Prefabs/Views", "MainView/NodeItem");
            _nodeItemTf = _nodeItemGo.GetComponent<RectTransform>();
            _nodeItemTf.SetParent(parentTf, false);
            _expandImg = _nodeItemTf.Find("ExpandImg").GetComponent<Image>();
            _functionImg = _nodeItemTf.Find("FunctionImg").GetComponent<Image>();
            _selectedImg = _nodeItemTf.Find("SelectImg").GetComponent<Image>();
            _selectedImg.color = new Color(0, 0, 1, 0);
            _descText = _nodeItemTf.Find("SelectImg/DescText").GetComponent<Text>();
            _clickGo = _nodeItemTf.Find("SelectImg/ClickImg").gameObject;
            // 事件监听
            UIEventListener.Get(_clickGo).AddClick(
                ()=> {
                    OnSelected(true);
                });
            UIEventListener.Get(_expandImg.gameObject).AddClick(OnExpandClickHander);
            _clickCount = 0;
            // 基本参数初始化
            childs = new List<BaseNode>();
            attrs = new List<BaseNodeAttr>();
            parentNode = parent;
            _nodeDepth = parent == null ? 0 : parent.GetDepth() + 1;
            if ( parent != null )
            {
                parent.OnChildAdded(this);
            }
            CreateDefailtAttrs();
        }

        public virtual void CreateDefailtAttrs()
        {

        }

        public void RefreshPosition(ref int showIndex,BaseNode beginNode,BaseNode fromChild = null)
        {
            if (beginNode != this && fromChild == null)
            {
                showIndex++;
                _nodeShowIndex = showIndex;
            }
            float posX = _nodeDepth == 0 ? 12 : (_nodeDepth - 1) * 24 + 12;
            float posY = _nodeShowIndex * -30;
            _nodeItemTf.anchoredPosition = new Vector2(posX, posY);
            if ( isExpand )
            {
                int childCount = childs.Count;
                int i = 0;
                // 找到当前遍历序列的下一个子节点
                if (fromChild != null)
                {
                    for (; i < childCount; i++)
                    {
                        if (childs[i] == fromChild)
                        {
                            i++;
                            break;
                        }
                    }
                }
                for (; i < childCount; i++)
                {
                    childs[i].RefreshPosition(ref showIndex,beginNode);
                }
            }
            if ( parentNode != null )
            {
                // 初始的搜索节点or是往上搜索的父节点
                if ( beginNode == this || fromChild != null )
                {
                    parentNode.RefreshPosition(ref showIndex, beginNode, this);
                }
            }
        }

        /// <summary>
        /// 展开、收起节点
        /// </summary>
        /// <param name="value"></param>
        public void Expand(bool value)
        {
            if ( childs.Count == 0 )
            {
                return;
            }
            if ( isExpand != value )
            {
                isExpand = value;
            }
            for (int i = 0, len = childs.Count; i < len; i++)
            {
                childs[i].SetChildNodesVisible(value);
            }
            int newNodeIndex = _nodeShowIndex;
            RefreshPosition(ref newNodeIndex, this);
        }

        public void SetChildNodesVisible(bool value)
        {
            _nodeItemGo.SetActive(value);
            if ( isExpand )
            {
                for (int i = 0, len = childs.Count; i < len; i++)
                {
                    childs[i].SetChildNodesVisible(value);
                }
            }
        }

        public int GetDepth()
        {
            return _nodeDepth;
        }

        public float GetItemOffset()
        {
            return 0;
        }

        public virtual string GetNodeName()
        {
            return "undefined nodeName";
        }

        public virtual string ToDesc()
        {
            return base.ToString();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public void OnChildAdded(BaseNode child)
        {
            childs.Add(child);
            if (parentNode == null) return;
            if ( !_expandImg.gameObject.activeSelf )
            {
                _expandImg.gameObject.SetActive(true);
            }
        }

        public void OnSelected(bool value)
        {
            _isSelected = value;
            _selectedImg.color = _isSelected ? new Color(0, 0, 1, 1) : new Color(0, 0, 1, 0);
            if ( value )
            {
                if (_clickCount == 0)
                {
                    _lastClickTime = Time.realtimeSinceStartup;
                    _clickCount = 1;
                }
                else
                {
                    // 双击检测
                    float nowTime = Time.realtimeSinceStartup;
                    if (nowTime - _lastClickTime <= 0.5f)
                    {
                        _clickCount = 0;
                        Expand(!isExpand);
                    }
                    else
                    {
                        _lastClickTime = Time.realtimeSinceStartup;
                        _clickCount = 1;
                    }
                }
                EventManager.GetInstance().PostEvent(EditorEvents.NodeSelected, this, true);
            }
            else
            {
                _clickCount = 0;
                _lastClickTime = 0;
            }
        }

        public void OnExpandClickHander()
        {
            Expand(!isExpand);
        }

        public void UpdateDesc()
        {
            _descText.text = ToDesc();
        }

        public List<BaseNodeAttr> GetAttrs()
        {
            return attrs;
        }
    }

    public enum NodeType : int
    {
        Root = 0,
    }

    public enum NodeInsertMode : byte
    {
        InsertAfter = 0,
        InsertBefore = 1,
        InsertAsChild = 2,
    }
}
