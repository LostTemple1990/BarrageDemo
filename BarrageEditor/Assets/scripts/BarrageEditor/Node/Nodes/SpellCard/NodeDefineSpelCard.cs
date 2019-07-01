﻿using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeDefineSpellCard : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.DefineSpellCard;
            _extraDepth = 0;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "scdefine");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 符卡类型标识
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Type name", null);
            attrs.Add(nodeAttr);
            // 符卡名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Spellcard name", null);
            attrs.Add(nodeAttr);
            // 符卡包含boss数量
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Boss count", null);
            attrs.Add(nodeAttr);
            // 符卡持续时间
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Duration(in sec)", null);
            attrs.Add(nodeAttr);
            // 符卡击破条件
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.SCCondition);
            nodeAttr.Init(this, "Condition", null);
            attrs.Add(nodeAttr);
            // 非符、符卡
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Is Spellcard", null);
            attrs.Add(nodeAttr);
        }

        public override void CreateDefualtChilds()
        {
            // 添加OnCreate子节点
            BaseNode onCreateNode = NodeManager.CreateNode(NodeType.SpellCardInit);
            onCreateNode.SetAttrsDefaultValues();
            InsertChildNode(onCreateNode, -1);
            // 添加OnFinish子节点
            BaseNode onFinishNode = NodeManager.CreateNode(NodeType.SpellCardFinish);
            onFinishNode.SetAttrsDefaultValues();
            InsertChildNode(onFinishNode, -1);
        }

        public override string GetNodeName()
        {
            return "define spell card";
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
                        CustomDefine.AddData(CustomDefineType.SpellCard, newTypeName, "");
                    }
                }
                else
                {
                    string fromName = attr.GetPreValue().ToString();
                    if (fromName != "")
                    {
                        CustomDefine.ModifyDefineName(CustomDefineType.SpellCard, fromName, attr.GetValueString());
                    }
                    else
                    {
                        string newTypeName = attr.GetValueString();
                        if (newTypeName != "")
                        {
                            CustomDefine.AddData(CustomDefineType.SpellCard, newTypeName, "");
                        }
                    }
                }
            }
            else // 载入节点数据or设置节点默认值时
            {
                string typeName = GetAttrByIndex(0).GetValueString();
                if (typeName != "")
                {
                    CustomDefine.AddData(CustomDefineType.SpellCard, typeName, "");
                }
            }
            base.OnAttributeValueChanged(attr);
        }

        public override string ToDesc()
        {
            bool isSpellCard = GetAttrByIndex(5).GetValueString() == "true" ? true : false;
            return string.Format("define {0}spell card \"{1}\" with name \"{2}\"",
                isSpellCard ? "non-" : "",
                GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString());
        }

        public override string ToLuaHead()
        {
            string scTypeName = GetAttrByIndex(0).GetValueString();
            return string.Format("SpellCard[\"{0}\"] = {{}}\n", scTypeName);
        }
    }
}
