using UnityEngine;
using UnityEngine.UI;
using YKEngine;
using System.Collections.Generic;

namespace BarrageEditor
{
    public class NodeDefineBoss : BaseNode, IEventReciver
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.DefineBoss;
            _extraDepth = 0;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bossdefine");
            EventManager.GetInstance().Register(EditorEvents.DefineNodeDestroy, this);
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 定义的Boss类型
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Type name", null);
            attrs.Add(nodeAttr);
        }

        public override void CreateDefualtChilds()
        {
            // 添加OnCreate子节点
            BaseNode onCreateNode = NodeManager.CreateNode(NodeType.OnBossCreate);
            onCreateNode.SetAttrsDefaultValues();
            InsertChildNode(onCreateNode, -1);
        }

        public override string GetNodeName()
        {
            return "define boss";
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
                        _isWatchingData = CustomDefine.AddData(CustomDefineType.Boss, newTypeName, "");
                    }
                }
                else
                {
                    string fromName = attr.GetPreValue().ToString();
                    if (fromName != "")
                    {
                        if (_isWatchingData)
                        {
                            _isWatchingData = CustomDefine.ModifyDefineName(CustomDefineType.Boss, fromName, attr.GetValueString());
                        }
                        else
                        {
                            _isWatchingData = CustomDefine.AddData(CustomDefineType.Boss, attr.GetValueString(), "");
                        }
                    }
                    else
                    {
                        string newTypeName = attr.GetValueString();
                        if (newTypeName != "")
                        {
                            _isWatchingData = CustomDefine.AddData(CustomDefineType.Boss, newTypeName, "");
                        }
                    }
                }
            }
            else // 载入节点数据or设置节点默认值时
            {
                string typeName = GetAttrByIndex(0).GetValueString();
                if (typeName != "")
                {
                    _isWatchingData = CustomDefine.AddData(CustomDefineType.Boss, typeName, "");
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
                CustomDefine.RemoveData(CustomDefineType.Boss, typeName);
                EventManager.GetInstance().PostEvent(EditorEvents.DefineNodeDestroy, new List<object> { CustomDefineType.Boss, typeName });
            }
            base.Destroy();
        }

        public override string ToDesc()
        {
            return string.Format("define boss \"{0}\"", attrs[0].GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("BossTable[\"{0}\"] = {{}}\n", attrs[0].GetValueString());
        }

        public void Execute(int eventId, object data)
        {
            if (eventId == EditorEvents.DefineNodeDestroy)
            {
                if (!_isWatchingData)
                {
                    List<object> datas = data as List<object>;
                    string typeName = GetAttrByIndex(0).GetValueString();
                    if ((CustomDefineType)datas[0] == CustomDefineType.Boss && (string)datas[1] == typeName)
                    {
                        _isWatchingData = CustomDefine.AddData(CustomDefineType.Boss, typeName, "");
                    }
                }
            }
        }
    }
}
