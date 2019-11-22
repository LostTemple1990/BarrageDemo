using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeLaserTurnOn : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.LaserTurnOn;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "laserturnon");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 子弹
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // 展开时间
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Duration", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "laser turn on";
        }

        public override string ToDesc()
        {
            return string.Format("turn on in {0} frame(s)", GetAttrByIndex(1).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:TurnOn({1})\n", GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString());
        }
    }
}