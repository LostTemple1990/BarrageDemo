using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class BaseNode
    {
        /// <summary>
        /// 可展开箭头，有子节点的时候的颜色
        /// </summary>
        private static Color ExpandImg_NodeHasChild_Color = new Color(1, 1, 1, 1);
        /// <summary>
        /// 可展开箭头，没有子节点的时候的颜色
        /// </summary>
        private static Color ExpandImg_NodeHasNoChild_Color = new Color(1, 1, 1, 0.5f);

        private const float RootNodeOffset = 12f;
        private const float DepthInterval = 24;
        public const float NodeHeight = 30;


        /// <summary>
        /// 节点类型
        /// </summary>
        protected NodeType _nodeType;
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


        public virtual void Init(RectTransform parentTf)
        {
            _nodeItemGo = ResourceManager.GetInstance().GetPrefab("Prefabs/Views", "MainView/NodeItem");
            _nodeItemTf = _nodeItemGo.GetComponent<RectTransform>();
            _nodeItemTf.SetParent(parentTf, false);
            _expandImg = _nodeItemTf.Find("ExpandImg").GetComponent<Image>();
            _expandImg.color = ExpandImg_NodeHasNoChild_Color;
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
            CreateDefaultAttrs();
        }

        public virtual void CreateDefaultAttrs()
        {

        }

        /// <summary>
        /// 插入子节点
        /// </summary>
        /// <param name="child"></param>
        /// <param name="index">子节点位置索引，-1为插入到最后</param>
        public virtual void InsertChildNode(BaseNode child, int index)
        {
            if ( index < 0 ) { index = childs.Count; }
            if (index > childs.Count)
            {
                Logger.LogError("invalid index for insertChildNode");
            }
            childs.Add(null);
            for (int i = childs.Count - 1; i > index; i--)
            {
                childs[i] = childs[i - 1];
            }
            childs[index] = child;
            child.SetParent(this);
            // 显示可展开的图标
            _expandImg.color = ExpandImg_NodeHasChild_Color;
        }

        public int GetChildIndex(BaseNode child)
        {
            return childs.IndexOf(child);
        }

        public void SetParent(BaseNode parent)
        {
            parentNode = parent;
            OnParentDepthChanged();
            //_nodeDepth = parent == null ? 0 : parent.GetDepth() + 1;
        }

        protected void OnParentDepthChanged()
        {
            _nodeDepth = parentNode == null ? 0 : parentNode.GetDepth() + 1;
            for (int i=0;i<childs.Count;i++)
            {
                childs[i].OnParentDepthChanged();
            }
        }

        public void SetAttrsDefaultValues()
        {
            NodeConfig cfg = DatabaseManager.NodeDatabase.GetNodeCfgByNodeType(_nodeType);
            if (cfg.defaultAttrValues == null) return;
            SetAttrsValues(cfg.defaultAttrValues);
        }

        public void SetAttrsValues(List<object> values)
        {
            for (int i=0;i<attrs.Count;i++)
            {
                attrs[i].SetValue(values[i],false);
            }
            OnAttributeValueChanged();
        }

        public virtual void OnAttributeValueChanged(BaseNodeAttr attr=null)
        {
            UpdateDesc();
        }

        public void RefreshPosition(ref int showIndex,BaseNode beginNode,BaseNode fromChild = null)
        {
            if (beginNode != this && fromChild == null)
            {
                showIndex++;
                _nodeShowIndex = showIndex;
            }
            float posX = _nodeDepth == 0 ? RootNodeOffset : (_nodeDepth - 1) * DepthInterval + RootNodeOffset;
            float posY = _nodeShowIndex * -NodeHeight;
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
            EventManager.GetInstance().PostEvent(EditorEvents.NodeExpanded, newNodeIndex);
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

        public int GetShowIndex()
        {
            return _nodeShowIndex;
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

        public NodeType GetNodeType()
        {
            return _nodeType;
        }

        public List<BaseNodeAttr> GetAttrs()
        {
            return attrs;
        }
    }

    public enum NodeInsertMode : byte
    {
        InsertAfter = 0,
        InsertBefore = 1,
        InsertAsChild = 2,
    }

    public class NodeData
    {
        public NodeType type;
        public NodeData parent;
        public List<NodeData> childs;
        public List<object> attrValues;
    }
}
