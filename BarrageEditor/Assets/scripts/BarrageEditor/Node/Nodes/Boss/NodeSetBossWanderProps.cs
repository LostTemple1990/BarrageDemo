using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeSetBossWanderProps : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.BossSetWanderProps;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "setwanderprops");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // Boss
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // X Range
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "X Range", null);
            _attrs.Add(nodeAttr);
            // Y Range
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Y Range", null);
            _attrs.Add(nodeAttr);
            // X Amplitude
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "X Amplitude", null);
            _attrs.Add(nodeAttr);
            // Y Amplitude
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Y Amplitude", null);
            _attrs.Add(nodeAttr);
            // Movement mode
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.MoveMode);
            nodeAttr.Init(this, "Movement Mode", null);
            _attrs.Add(nodeAttr);
            // Direction Mode
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.DirectionMode);
            nodeAttr.Init(this, "Direction Mode", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "set boss wander properties";
        }

        public override string ToDesc()
        {
            return string.Format("{0} wander range of({1}),({2}),X Amplitude of {3},Y Amplitude of {4},{5},{6}", 
                GetAttrByIndex(0).GetValueString(), 
                GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString(),
                GetAttrByIndex(4).GetValueString(),
                GetAttrByIndex(5).GetValueString(),
                GetAttrByIndex(5).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:SetWanderRange({1},{2})\n{0}:SetWanderAmplitude({3},{4})\n{0}:SetWanderMode({5},{6})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString(), GetAttrByIndex(4).GetValueString(),
                GetAttrByIndex(5).GetValueString(), GetAttrByIndex(6).GetValueString());
        }
    }
}
