using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeShowBossAura : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.ShowBossAura;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bossshowaura");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // Boss
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // 显示BOSS光环
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Show Aura", null);
            _attrs.Add(nodeAttr);
            // 是否播放动画
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Play Ani", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "show boss aura";
        }

        public override string ToDesc()
        {
            string showStr = GetAttrByIndex(1).GetValueString();
            string statusStr;
            if (showStr == "true")
                statusStr = "show";
            else if (showStr == "false")
                statusStr = "hide";
            else
                statusStr = "set";
            return string.Format("{0} aura of {1},play ani = {2}",
                statusStr,
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(2).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:ShowAura({1},{2})\n",
                GetAttrByIndex(0).GetValueString(),
                GetAttrByIndex(1).GetValueString(), GetAttrByIndex(2).GetValueString());
        }
    }
}
