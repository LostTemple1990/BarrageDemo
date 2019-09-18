using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeChangeLengthTo : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.LaserChangeLengthTo;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "lasergrow");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 子弹
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            attrs.Add(nodeAttr);
            // 延长激光的长度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "To Length", null);
            attrs.Add(nodeAttr);
            // 展开时间
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Duration", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "laser change length";
        }

        public override string ToDesc()
        {
            return string.Format("change length to {0} in {1} frame(s)", GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:ChangeLengthTo({1},{2})\n", GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString());
        }
    }
}