using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeDefineLaser : BaseNode, IEventReciver
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.DefineLaser;
            _extraDepth = 0;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "laserdefine");
            EventManager.GetInstance().Register(EditorEvents.DefineNodeDestroy, this);
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 定义的子弹类型
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Type name", null);
            _attrs.Add(nodeAttr);
        }

        public override void CreateDefualtChilds()
        {
            // 添加OnCreate子节点
            BaseNode onCreateNode = NodeManager.CreateNode(NodeType.OnLaserCreate);
            onCreateNode.SetAttrsDefaultValues();
            InsertChildNode(onCreateNode, -1);
        }

        public override string GetNodeName()
        {
            return "define laser";
        }

        /// <summary>
        /// 是否正在监视数据的改变
        /// </summary>
        private bool _isWatchingData = false;

        public bool IsWatchingData
        {
            get { return _isWatchingData; }
        }

        public override void OnAttributeValueChanged(BaseNodeAttr attr = null)
        {
            if (attr != null)
            {
                if (attr.GetPreValue() == null)
                {
                    string newTypeName = attr.GetValueString();
                    if (newTypeName != "")
                    {
                        BaseNode onCreteNode = GetChildByType(NodeType.OnLaserCreate);
                        _isWatchingData = CustomDefine.AddData(CustomDefineType.Laser, newTypeName, onCreteNode.GetAttrByIndex(0).GetValueString());
                    }
                }
                else
                {
                    string fromName = attr.GetPreValue().ToString();
                    if (fromName != "")
                    {
                        if (_isWatchingData)
                        {
                            _isWatchingData = CustomDefine.ModifyDefineName(CustomDefineType.Laser, fromName, attr.GetValueString());
                        }
                        else
                        {
                            BaseNode onCreteNode = GetChildByType(NodeType.OnLaserCreate);
                            _isWatchingData = CustomDefine.AddData(CustomDefineType.Laser, attr.GetValueString(), onCreteNode.GetAttrByIndex(0).GetValueString());
                        }
                    }
                    else
                    {
                        string newTypeName = attr.GetValueString();
                        if (newTypeName != "")
                        {
                            BaseNode onCreteNode = GetChildByType(NodeType.OnLaserCreate);
                            _isWatchingData = CustomDefine.AddData(CustomDefineType.Laser, newTypeName, onCreteNode.GetAttrByIndex(0).GetValueString());
                        }
                    }
                }
            }
            else // 载入节点数据or设置节点默认值时
            {
                string typeName = GetAttrByIndex(0).GetValueString();
                if (typeName != "")
                {
                    BaseNode onCreteNode = GetChildByType(NodeType.OnLaserCreate);
                    string paraList = onCreteNode.GetAttrByIndex(0).GetValueString();
                    _isWatchingData = CustomDefine.AddData(CustomDefineType.Laser, typeName, paraList);
                }
            }
            base.OnAttributeValueChanged(attr);
        }

        public override void Destroy()
        {
            EventManager.GetInstance().Remove(EditorEvents.DefineNodeDestroy, this);
            if (_isWatchingData)
            {
                string typeName = GetAttrByIndex(0).GetValueString();
                CustomDefine.RemoveData(CustomDefineType.Laser, typeName);
                EventManager.GetInstance().PostEvent(EditorEvents.DefineNodeDestroy, new List<object> { CustomDefineType.Laser, typeName });
            }
            base.Destroy();
        }

        public override string ToDesc()
        {
            return string.Format("define laser type \"{0}\"", _attrs[0].GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("CustomizedTable[\"{0}\"] = {{}}\n", _attrs[0].GetValueString());
        }

        public void Execute(int eventId, object data)
        {
            if (eventId == EditorEvents.DefineNodeDestroy)
            {
                if (!_isWatchingData)
                {
                    List<object> datas = data as List<object>;
                    string typeName = GetAttrByIndex(0).GetValueString();
                    if ((CustomDefineType)datas[0] == CustomDefineType.Laser && (string)datas[1] == typeName)
                    {
                        BaseNode onCreteNode = GetChildByType(NodeType.OnLaserCreate);
                        string paraList = onCreteNode.GetAttrByIndex(0).GetValueString();
                        _isWatchingData = CustomDefine.AddData(CustomDefineType.Laser, typeName, paraList);
                    }
                }
            }
        }
    }
}
