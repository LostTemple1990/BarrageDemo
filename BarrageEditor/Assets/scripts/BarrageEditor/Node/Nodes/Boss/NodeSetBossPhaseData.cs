using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeSetBossPhaseData : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.SetBossPhaseData;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bossphasedata");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // Boss
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Boss", null);
            _attrs.Add(nodeAttr);
            // 阶段显示权重
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Phase weight", null);
            _attrs.Add(nodeAttr);
            // 是否多阶段
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Is multi phase", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "set boss phase data";
        }

        public override string ToDesc()
        {
            return string.Format("set {0}'s phase data with weights \"{1}\"",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:SetPhaseData({1},{2})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString());
        }
    }
}
