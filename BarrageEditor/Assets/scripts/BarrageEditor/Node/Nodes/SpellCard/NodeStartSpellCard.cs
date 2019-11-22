using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeStartSpellCard : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.StartSpellCard;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "startsc");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 符卡类型名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.CustomizedType);
            nodeAttr.Init(this, "SC Type Name", null);
            _attrs.Add(nodeAttr);
            // 符卡的boss
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.CustomizedType);
            nodeAttr.Init(this, "Boss", null);
            _attrs.Add(nodeAttr);
            // 是否等待符卡结束
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Wait", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "start spell card";
        }

        public override string ToDesc()
        {
            return string.Format("StartSepllCard \"{0}\"", GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaHead()
        {
            string ret = string.Format("StartSpellCard(SpellCard[\"{0}\"],{1})\n", 
                GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString());
            if (GetAttrByIndex(2).GetValueString()=="true")
            {
                ret += string.Format("if WaitForSpellCardFinish() == false then return end\n");
            }
            return ret;
        }
    }
}
