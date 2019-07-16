using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeDefineEnemy : BaseNode, IEventReciver
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.DefineEnemy;
            _extraDepth = 0;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "enemydefine");
            EventManager.GetInstance().Register(EditorEvents.DefineNodeDestroy, this);
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 定义的敌机类型
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Type name", null);
            attrs.Add(nodeAttr);
        }

        public override void CreateDefualtChilds()
        {
            // 添加OnCreate子节点
            BaseNode onCreateNode = NodeManager.CreateNode(NodeType.OnEnemyCreate);
            onCreateNode.SetAttrsDefaultValues();
            InsertChildNode(onCreateNode, -1);
        }

        public override string GetNodeName()
        {
            return "define enemy";
        }

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
                        BaseNode onCreteNode = GetChildByType(NodeType.OnEnemyCreate);
                        _isWatchingData = CustomDefine.AddData(CustomDefineType.Enemy, newTypeName, onCreteNode.GetAttrByIndex(1).GetValueString());
                    }
                }
                else
                {
                    string fromName = attr.GetPreValue().ToString();
                    if (fromName != "")
                    {
                        if (_isWatchingData)
                        {
                            CustomDefine.ModifyDefineName(CustomDefineType.Enemy, fromName, attr.GetValueString());
                        }
                        else
                        {
                            BaseNode onCreteNode = GetChildByType(NodeType.OnEnemyCreate);
                            _isWatchingData = CustomDefine.AddData(CustomDefineType.Enemy, attr.GetValueString(), onCreteNode.attrs[1].GetValueString());
                        }
                    }
                    else
                    {
                        string newTypeName = attr.GetValueString();
                        if (newTypeName != "")
                        {
                            BaseNode onCreteNode = GetChildByType(NodeType.OnEnemyCreate);
                            _isWatchingData = CustomDefine.AddData(CustomDefineType.Enemy, newTypeName, onCreteNode.GetAttrByIndex(1).GetValueString());
                        }
                    }
                }
            }
            else // 载入节点数据or设置节点默认值时
            {
                string typeName = GetAttrByIndex(0).GetValueString();
                if (typeName != "")
                {
                    BaseNode onCreteNode = GetChildByType(NodeType.OnEnemyCreate);
                    string paraList = onCreteNode.GetAttrByIndex(1).GetValueString();
                    _isWatchingData = CustomDefine.AddData(CustomDefineType.Enemy, typeName, paraList);
                }
            }
            base.OnAttributeValueChanged(attr);
        }

        public override string ToDesc()
        {
            return string.Format("define enemy type \"{0}\"", attrs[0].GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("CustomizedEnemyTable[\"{0}\"] = {{}}\n", attrs[0].GetValueString());
        }

        public override void Destroy()
        {
            EventManager.GetInstance().Remove(EditorEvents.DefineNodeDestroy, this);
            if (_isWatchingData)
            {
                string typeName = GetAttrByIndex(0).GetValueString();
                CustomDefine.RemoveData(CustomDefineType.Enemy, typeName);
                EventManager.GetInstance().PostEvent(EditorEvents.DefineNodeDestroy, new List<object> { CustomDefineType.Enemy, typeName });
            }
            base.Destroy();
        }

        public void Execute(int eventId, object data)
        {
            if (eventId == EditorEvents.DefineNodeDestroy)
            {
                if (!_isWatchingData)
                {
                    List<object> datas = data as List<object>;
                    string typeName = GetAttrByIndex(0).GetValueString();
                    if ((CustomDefineType)datas[0] == CustomDefineType.Enemy && (string)datas[1] == typeName)
                    {
                        BaseNode onCreteNode = GetChildByType(NodeType.OnEnemyCreate);
                        string paraList = onCreteNode.GetAttrByIndex(1).GetValueString();
                        _isWatchingData = CustomDefine.AddData(CustomDefineType.Enemy, typeName, paraList);
                    }
                }
            }
        }
    }
}
