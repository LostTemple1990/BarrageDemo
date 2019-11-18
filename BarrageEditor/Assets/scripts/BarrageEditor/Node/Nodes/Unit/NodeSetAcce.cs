using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeSetAcce : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.UnitSetAcce;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "setaccel");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            attrs.Add(nodeAttr);
            // 加速度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Acceleration", null);
            attrs.Add(nodeAttr);
            // 角度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Angle", null);
            attrs.Add(nodeAttr);
            // 是否朝向于玩家
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Aim to player", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "set acceleration";
        }

        public override string ToDesc()
        {

            BaseNodeAttr aimToPlayerAttr = GetAttrByIndex(3);
            return string.Format("set {0}'s acce,a = {1},angle = {2} {3}",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString() == "true" ? "aim to player" : "");
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:SetAcce({1},{2},{3})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString());
        }
    }
}
