using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeUnitSetRelativePos : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.UnitSetRelativePos;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "setrelpos");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // X轴偏移量
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Offset X", null);
            _attrs.Add(nodeAttr);
            // Y轴偏移量
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Offset Y", null);
            _attrs.Add(nodeAttr);
            // 旋转角度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Rotation", null);
            _attrs.Add(nodeAttr);
            // 是否跟随Master的角度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Follow rotation of master", null);
            _attrs.Add(nodeAttr);
            // 是否持续跟随Master
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Follow master continuously", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "set relative position";
        }

        public override string ToDesc()
        {
            string ret = string.Format("set position to ({0},{1}) relatived to master with rot ({2}){3}{4}",
                 GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString(),
                 GetAttrByIndex(3).GetValueString(),
                 GetAttrByIndex(3).GetValueString() == "true" ? ",follow rot of master" : "",
                 GetAttrByIndex(4).GetValueString() == "true" ? ",follow master continuously" : "");
            return ret;
        }

        public override string ToLuaHead()
        {

            return string.Format("{0}:SetRelativePos({1},{2},{3},{4},{5})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString(),
                GetAttrByIndex(4).GetValueString(),
                GetAttrByIndex(5).GetValueString());
        }
    }
}
