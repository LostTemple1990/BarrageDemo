using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeShowBossBloodBar : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.ShowBossBloodBar;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bossshowbloodbar");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // Boss
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            attrs.Add(nodeAttr);
            // 显示BOSS血条
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Show Blood Bar", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "show boss blood bar";
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
            return string.Format("{0} {1}'s blood bar",
                statusStr,
                GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:ShowBloodBar({1})\n", GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString());
        }
    }
}
