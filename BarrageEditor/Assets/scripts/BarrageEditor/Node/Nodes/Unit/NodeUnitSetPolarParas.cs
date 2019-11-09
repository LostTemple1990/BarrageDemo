using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeUnitSetPolarParas : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.UnitSetPolarParas;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "setpolarparas");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            attrs.Add(nodeAttr);
            // 初始半径
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Radius", null);
            attrs.Add(nodeAttr);
            // 角度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Angle", null);
            attrs.Add(nodeAttr);
            // 半径增量
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "DeltaR", null);
            attrs.Add(nodeAttr);
            // 角速度
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Omega", null);
            attrs.Add(nodeAttr);
            // 中心位置
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "CenterPos", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "set polar paras";
        }

        public override string ToDesc()
        {
            string centerPosStr = "";
            if (GetAttrByIndex(5).GetValueString() != "")
            {
                centerPosStr = string.Format(" from ({0})", GetAttrByIndex(5).GetValueString());
            }
            return string.Format("set {0}'s polar paras(radius={1} angle={2} deltaR={3} omega={4}){5}",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString(),
                GetAttrByIndex(4).GetValueString(),
                centerPosStr);
        }

        public override string ToLuaHead()
        {
            string centerPosStr = "";
            if (GetAttrByIndex(5).GetValueString() != "")
            {
                centerPosStr = string.Format(",{0}", GetAttrByIndex(5).GetValueString());
            }
            return string.Format("{0}:SetPolarParas({1},{2},{3},{4}{5})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(),
                GetAttrByIndex(2).GetValueString(),
                GetAttrByIndex(3).GetValueString(),
                GetAttrByIndex(4).GetValueString(),
                centerPosStr);
        }
    }
}
