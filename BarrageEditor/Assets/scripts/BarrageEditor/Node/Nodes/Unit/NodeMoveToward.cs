using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeMoveTowards : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.UnitMoveTowards;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "moveto");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            attrs.Add(nodeAttr);
            // 速度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Velocity", null);
            attrs.Add(nodeAttr);
            // 角度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Angle", null);
            attrs.Add(nodeAttr);
            // 是否朝向于玩家
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Aim to player", null);
            attrs.Add(nodeAttr);
            // 持续时间
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Duration", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "move towards";
        }

        public override string ToDesc()
        {
            BaseNodeAttr aimToPlayerAttr = GetAttrByIndex(3);
            return string.Format("{0} move towards with velocity of {1},angle of {2}{3} in {4} frames",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString() == "true" ? " aim to player" : "",
                GetAttrByIndex(4).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:MoveTowards({1},{2},{3},{4})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString(),
                GetAttrByIndex(4).GetValueString());
        }
    }
}
