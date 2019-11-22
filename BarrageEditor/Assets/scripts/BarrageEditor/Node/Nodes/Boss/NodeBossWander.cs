using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeBossWander : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.BossWander;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bosswander");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // Boss
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // 走动时间
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Duration(in frame)", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "boss wander";
        }

        public override string ToDesc()
        {
            return string.Format("{0} wander in {1} frame(s)", GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:Wander({1})\n", GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString());
        }
    }
}
