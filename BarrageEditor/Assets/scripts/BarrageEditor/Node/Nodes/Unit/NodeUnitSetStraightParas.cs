using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeUnitSetStraightParas : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.UnitSetStraightParas;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "setstraightparas");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // 速度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Velocity", null);
            _attrs.Add(nodeAttr);
            // 角度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Angle", null);
            _attrs.Add(nodeAttr);
            // 是否朝向于玩家
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Aim to player", null);
            _attrs.Add(nodeAttr);
            // 加速度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Acce", null);
            _attrs.Add(nodeAttr);
            // 加速度角度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "AccAngle", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "set straight paras";
        }

        public override string ToDesc()
        {

            BaseNodeAttr aimToPlayerAttr = GetAttrByIndex(3);
            return string.Format("set {0}'s velocity={1} angle={2} {3} acc={4} accAngle={5}",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString() == "true" ? "aim to player" : "",
                GetAttrByIndex(4).GetValueString(),
                GetAttrByIndex(5).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:SetStraightParas({1},{2},{3},{4},{5})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString(),
                GetAttrByIndex(4).GetValueString(),
                GetAttrByIndex(5).GetValueString());
        }
    }
}
