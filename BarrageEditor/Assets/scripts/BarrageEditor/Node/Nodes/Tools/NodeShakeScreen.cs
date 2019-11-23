using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeShakeScreen : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.ShakeScreen;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "shakescreen");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Name", null);
            _attrs.Add(nodeAttr);
            // 延迟时间
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Delay", null);
            _attrs.Add(nodeAttr);
            // 持续时间
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Duration", null);
            _attrs.Add(nodeAttr);
            // 抖动间隔时间
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Interval", null);
            _attrs.Add(nodeAttr);
            // 震幅
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "ShakeLevel", null);
            _attrs.Add(nodeAttr);
            // 最小震幅
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "MinShakeLevel", null);
            _attrs.Add(nodeAttr);
            // 最大抖动距离
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "MaxRange", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "shake screen";
        }

        public override string ToDesc()
        {
            return string.Format("shake screen in {0} frame(s) delay {1} frame(s)",
                _attrs[2].GetValueString(), _attrs[1].GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("ShakeScreen(\"{0}\",{1},{2},{3},{4},{5},{6})\n", 
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString(), GetAttrByIndex(3).GetValueString(),
                GetAttrByIndex(4).GetValueString(), GetAttrByIndex(5).GetValueString(), GetAttrByIndex(6).GetValueString());
        }
    }
}
