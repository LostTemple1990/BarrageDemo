using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeShowBossPosHint : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.ShowBossPosHint;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bossshowposhint");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // Boss
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
            // 显示BOSS位置提示
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Bool);
            nodeAttr.Init(this, "Is show", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "show boss position hint";
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
            return string.Format("{0} position hint of {1}",
                statusStr,
                GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:ShowPosHint({1})\n", 
                GetAttrByIndex(0).GetValueString(), 
                GetAttrByIndex(1).GetValueString());
        }
    }
}
