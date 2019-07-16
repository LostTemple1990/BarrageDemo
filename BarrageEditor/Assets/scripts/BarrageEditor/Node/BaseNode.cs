using System;
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
        private static readonly Color ExpandImg_NodeHasChild_Color = new Color(1, 1, 1, 1);
        /// <summary>
        /// 可展开箭头，没有子节点的时候的颜色
        /// </summary>
        private static readonly Color ExpandImg_NodeHasNoChild_Color = new Color(1, 1, 1, 0.5f);

        private const string ExpandImg_NodeIsNotExpanded_Img = "ChildNodeNotExpand";
        private const string ExpandImg_NodeIsExpanded_Img = "ChildNodeExpand";

        private static readonly Color FocusOnColor = new Color(0, 0, 1, 1);
        private static readonly Color NotFocusOnColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        private static readonly Color NotSelectedColor = new Color(0, 0, 1, 0);

        protected enum ExpandImgState : byte
        {
            Undefined = 0,
            NoChild = 1,
            IsNotExpanded = 2,
            IsExpanded = 3,
        };

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
        protected bool _isExpand;
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

        /// <summary>
        /// 展开箭头的状态
        /// </summary>
        protected ExpandImgState _expandImgState;

        protected int _nodeShowIndex;
        protected int _nodeDepth;

        protected bool _isSelected;
        protected float _lastClickTime;
        protected float _clickCount;
        /// <summary>
        /// 进入该节点后lua代码的tab额外深度
        /// <para>默认为1</para>
        /// </summary>
        protected int _extraDepth;

        protected bool _isValid;
        /// <summary>
        /// 修改之前的参数值
        /// </summary>
        protected List<object> _preValues;

        public BaseNode()
        {
            _extraDepth = 1;
            _isValid = true;
            // 基本参数初始化
            childs = new List<BaseNode>();
            attrs = new List<BaseNodeAttr>();
        }

        public virtual void Init(RectTransform parentTf)
        {
            _nodeItemGo = ResourceManager.GetInstance().GetPrefab("Prefabs/Views", "MainView/NodeItem");
            _nodeItemTf = _nodeItemGo.GetComponent<RectTransform>();
            _nodeItemTf.SetParent(parentTf, false);
            _expandImg = _nodeItemTf.Find("ExpandImg").GetComponent<Image>();
            _expandImg.color = ExpandImg_NodeHasNoChild_Color;
            _functionImg = _nodeItemTf.Find("FunctionImg").GetComponent<Image>();
            _selectedImg = _nodeItemTf.Find("SelectImg").GetComponent<Image>();
            _selectedImg.color = NotSelectedColor;
            _descText = _nodeItemTf.Find("SelectImg/DescText").GetComponent<Text>();
            _clickGo = _nodeItemTf.Find("SelectImg/ClickImg").gameObject;
            // 事件监听
            UIEventListener.Get(_clickGo).AddClick(
                ()=> {
                    OnSelected(true);
                });
            UIEventListener.Get(_expandImg.gameObject).AddClick(OnExpandClickHander);
            _clickCount = 0;
            CreateDefaultAttrs();
            _preValues = new List<object>();
            for (int i=0;i<attrs.Count;i++)
            {
                _preValues.Add("");
            }
            IsExpand = true;
        }

        public virtual void CreateDefaultAttrs()
        {

        }

        public virtual void CreateDefualtChilds()
        {

        }

        /// <summary>
        /// 检测是否可以插入类型为nodeType的子节点
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        public bool CheckCanInsertChildNode(NodeType nodeType)
        {
            NodeType parentType = _nodeType;
            NodeType childType = nodeType;
            NodeConfig parentCfg = DatabaseManager.NodeDatabase.GetNodeCfgByNodeType(parentType);
            NodeConfig childCfg = DatabaseManager.NodeDatabase.GetNodeCfgByNodeType(childType);
            if (parentCfg.allowChilds != null)
            {
                if (parentCfg.allowChilds.IndexOf(childType) == -1)
                    return false;
            }
            if (childCfg.allowParents != null)
            {
                if (childCfg.allowParents.IndexOf(parentType) == -1)
                    return false;
            }
            if (childCfg.forbidParents != null)
            {
                if (childCfg.forbidParents.IndexOf(parentType) != -1)
                    return false;
            }
            if (childCfg.needAncestors != null)
            {
                BaseNode parent = this;
                bool hasAncestor = false;
                while (parent!=null)
                {
                    if (childCfg.needAncestors.IndexOf(parent.GetNodeType()) != -1)
                    {
                        hasAncestor = true;
                        break;
                    }
                    parent = parent.parentNode;
                }
                if (!hasAncestor)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 插入子节点
        /// <para>直接插入，不做检查</para>
        /// </summary>
        /// <param name="child"></param>
        /// <param name="index">子节点位置索引，-1为插入到最后</param>
        public virtual bool InsertChildNode(BaseNode child, int index)
        {
            if ( index < 0 ) { index = childs.Count; }
            if (index > childs.Count)
            {
                Logger.LogError(string.Format("invalid index {0} for insertChildNode", index));
                child.Destroy();
                return false;
            }
            childs.Add(null);
            for (int i = childs.Count - 1; i > index; i--)
            {
                childs[i] = childs[i - 1];
            }
            childs[index] = child;
            child.SetParent(this);
            UpdateExpandImg();
            return true;
        }

        public bool RemoveChildNode(BaseNode child)
        {
            NodeConfig childCfg = DatabaseManager.NodeDatabase.GetNodeCfgByNodeType(child.GetNodeType());
            if (!childCfg.isDeletable)
                return false;
            if (childs.Remove(child))
            {
                child.Destroy();
                UpdateExpandImg();
                Expand(_isExpand);
                return true;
            }
            return false;
        }

        public bool IsDeletable
        {
            get
            {
                NodeConfig cfg = DatabaseManager.NodeDatabase.GetNodeCfgByNodeType(_nodeType);
                return cfg.isDeletable;
            }
        }

        public int GetChildIndex(BaseNode child)
        {
            return childs.IndexOf(child);
        }

        public int GetChildCount()
        {
            return childs.Count;
        }

        /// <summary>
        /// 获取指定类型的直接点(只返回第一个
        /// <para></para>
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        public BaseNode GetChildByType(NodeType nodeType)
        {
            BaseNode child = null;
            for (int i=0;i<childs.Count;i++)
            {
                if (childs[i].GetNodeType() == nodeType)
                {
                    child = childs[i];
                    break;
                }
            }
            return child;
        }

        /// <summary>
        /// 根据下标返回对应的子节点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public BaseNode GetChildByIndex(int index)
        {
            if (index < 0 || index >= childs.Count)
                return null;
            return childs[index];
        }

        public void SetParent(BaseNode parent)
        {
            parentNode = parent;
            UpdateNodeDepth();
        }

        protected void UpdateNodeDepth()
        {
            _nodeDepth = parentNode == null ? 0 : parentNode.GetDepth() + 1;
            for (int i=0;i<childs.Count;i++)
            {
                childs[i].UpdateNodeDepth();
            }
        }

        public virtual void SetAttrsDefaultValues()
        {
            NodeConfig cfg = DatabaseManager.NodeDatabase.GetNodeCfgByNodeType(_nodeType);
            if (cfg.defaultAttrValues == null) return;
            SetAttrsValues(cfg.defaultAttrValues);
        }

        public void SetAttrsValues(List<object> values)
        {
            for (int i=0;i<attrs.Count;i++)
            {
                // 可能会因为增加节点属性导致保存的value不够，这个时候跳出循环即可
                if ( i >= values.Count )
                {
                    break;
                }
                attrs[i].SetValue(values[i],false);
            }
            OnAttributeValueChanged();
        }

        public virtual void OnAttributeValueChanged(BaseNodeAttr attr=null)
        {
            // 所有变量一起改变
            if (attr == null)
            {
                CachePreValue();
            }
            else
            {
                List<object> preValues = new List<object>();
                List<object> curValues = new List<object>();
                for (int i=0;i<attrs.Count;i++)
                {
                    preValues.Add(_preValues[i]);
                    curValues.Add(GetAttrByIndex(i).GetValueString());
                }
                OpNodeAttrValuesModificationHM hm = new OpNodeAttrValuesModificationHM
                {
                    nodeIndex = NodeManager.GetNodeIndex(this),
                    preValues = preValues,
                    curValues = curValues,
                };
                Undo.AddToUndoTask(hm);
                CachePreValue();
            }
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
            if ( _isExpand )
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
            int childCount = childs.Count;
            IsExpand = childCount == 0 ? false : value;
            for (int i = 0; i < childCount; i++)
            {
                childs[i].SetChildNodesVisible(IsExpand);
            }
            int newNodeIndex = _nodeShowIndex;
            RefreshPosition(ref newNodeIndex, this);
            EventManager.GetInstance().PostEvent(EditorEvents.NodeExpanded, newNodeIndex);
        }

        public void SetChildNodesVisible(bool value)
        {
            _nodeItemGo.SetActive(value);
            // 如果该节点可见，则更新一下扩展箭头
            if (value)
            {
                UpdateExpandImg();
            }
            bool childVisible = !value || !_isExpand ? false : value;
            for (int i = 0, len = childs.Count; i < len; i++)
            {
                childs[i].SetChildNodesVisible(childVisible);
            }
        }

        /// <summary>
        /// 显示可展开的图标
        /// </summary>
        private void UpdateExpandImg()
        {
            ExpandImgState newState;
            if ( childs.Count == 0 )
            {
                newState = ExpandImgState.NoChild;
            }
            else
            {
                newState = _isExpand ? ExpandImgState.IsExpanded : ExpandImgState.IsNotExpanded;
            }
            if (newState != _expandImgState)
            {
                _expandImgState = newState;
                if (newState == ExpandImgState.NoChild)
                {
                    _expandImg.color = ExpandImg_NodeHasNoChild_Color;
                    _expandImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", ExpandImg_NodeIsNotExpanded_Img);
                }
                else if (newState == ExpandImgState.IsNotExpanded)
                {
                    _expandImg.color = ExpandImg_NodeHasChild_Color;
                    _expandImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", ExpandImg_NodeIsNotExpanded_Img);
                }
                else
                {
                    _expandImg.color = ExpandImg_NodeHasChild_Color;
                    _expandImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", ExpandImg_NodeIsExpanded_Img);
                }
            }
        }

        /// <summary>
        /// 该节点是否展开
        /// </summary>
        public bool IsExpand
        {
            protected set
            {
                _isExpand = value;
                UpdateExpandImg();
            }
            get
            {
                return _isExpand;
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
            if (!_isValid)
                return;
            _isSelected = value;
            _selectedImg.color = _isSelected ? FocusOnColor : NotSelectedColor;
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
                        Expand(!_isExpand);
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

        public void FocusOn(bool value)
        {
            if (!_isValid)
                return;
            if (!_isSelected)
                return;
            _selectedImg.color = value ? FocusOnColor : NotFocusOnColor;
        }

        public void OnExpandClickHander()
        {
            Expand(!_isExpand);
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

        public BaseNodeAttr GetAttrByIndex(int index)
        {
            if ( index < 0 || index >= attrs.Count )
            {
                Logger.Log("Invalid attr index " + index + " for node " + _nodeType);
                return null;
            }
            return attrs[index];
        }

        public virtual void ToLua(int codeDepth, ref string luaStr)
        {
            if (BarrageProject.IsDebugStage)
            {
                if (this == BarrageProject.DebugFromNode)
                {
                    luaStr += "end ";
                }
            }
            string luaHead = ToLuaHead();
            if ( luaHead != "" )
            {
                NodeManager.ApplyDepthForLua(codeDepth, ref luaHead);
                luaStr += luaHead;
            }
            if (BarrageProject.IsDebugStage)
            {
                if (this==BarrageProject.DebugStageNode)
                {
                    luaStr += "if false then ";
                }
            }
            for (int i=0;i<childs.Count;i++)
            {
                childs[i].ToLua(codeDepth + _extraDepth, ref luaStr);
            }
            string luaFoot = ToLuaFoot();
            if ( luaFoot != "" )
            {
                NodeManager.ApplyDepthForLua(codeDepth, ref luaFoot);
                luaStr += luaFoot;
            }
        }

        public virtual string ToLuaHead()
        {
            return "";
        }

        public virtual string ToLuaFoot()
        {
            return "";
        }

        protected void CachePreValue()
        {
            for (int i=0;i<attrs.Count;i++)
            {
                _preValues[i] = attrs[i].GetValueString();
            }
        }

        public void InitWithNodeData(NodeData nd)
        {
            List<object> list = new List<object>();
            List<string> values = nd.attrValues;
            int valuesCount = values.Count;
            for (int i = 0; i < valuesCount; i++)
            {
                list.Add(values[i]);
            }
            SetAttrsValues(list);
            _isExpand = nd.isExpand;
        }

        public virtual void Destroy()
        {
            int i;
            for (i = 0; i < childs.Count; i++)
            {
                childs[i].Destroy();
            }
            childs.Clear();
            for (i = 0; i < attrs.Count; i++)
            {
                attrs[i].UnbindItem();
            }
            attrs.Clear();
            UIEventListener.Get(_clickGo).RemoveAllEvents();
            UIEventListener.Get(_expandImg.gameObject).RemoveAllEvents();
            parentNode = null;
            GameObject.Destroy(_nodeItemGo);    
            _nodeItemGo = null;
            _nodeItemTf = null;
            _expandImg = null;
            _functionImg = null;
            _selectedImg = null;
            _clickGo = null;
            _descText = null;
            _isValid = false;
        }
    }

    public enum NodeInsertMode : byte
    {
        InsertAfter = 0,
        InsertBefore = 1,
        InsertAsChild = 2,
    }

    [Serializable]
    public class NodeData
    {
        public int type;
        public List<NodeData> childs;
        public List<string> attrValues;
        public bool isExpand;
    }
}
