using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeMoveTo : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.UnitMoveTo;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "moveto");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // 终点坐标x
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "To posX", null);
            _attrs.Add(nodeAttr);
            // 终点坐标y
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "To posY", null);
            _attrs.Add(nodeAttr);
            // 时间
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Duration", null);
            _attrs.Add(nodeAttr);
            // 移动方式
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.MoveMode);
            nodeAttr.Init(this, "Move mode", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "move to";
        }

        public override string ToDesc()
        {
            return string.Format("{0} move to ({1},{2}) in {3} frames,mode of {4}",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString(),
                GetAttrByIndex(4).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:MoveTo({1},{2},{3},{4})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString(),
                GetAttrByIndex(4).GetValueString());
        }
    }
}
