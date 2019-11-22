using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeUnitSetResistEliminatedTypes : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.UnitSetResistEliminatedTypes;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "ignorecollisiongroup");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // 忽略的碰撞组
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.ResistEliminatedTypes);
            nodeAttr.Init(this, "Resist types(op and)", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "resist eliminated types";
        }

        public override string ToDesc()
        {
            return string.Format("set {0}'s resist eliminated types", GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:SetResistEliminatedTypes({1})\n", GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString());
        }
    }
}
