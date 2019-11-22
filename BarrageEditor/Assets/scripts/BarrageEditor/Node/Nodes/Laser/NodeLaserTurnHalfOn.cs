using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeLaserTurnHalfOn : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.LaserTurnHalfOn;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "laserturnhalfon");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 子弹
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // 半开激光的宽度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "To Width", null);
            _attrs.Add(nodeAttr);
            // 展开时间
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Duration", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "laser turn half on";
        }

        public override string ToDesc()
        {
            return string.Format("turn half on to width of {0} in {1} frame(s)", GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:TurnHalfOn({1},{2})\n", GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString());
        }
    }
}