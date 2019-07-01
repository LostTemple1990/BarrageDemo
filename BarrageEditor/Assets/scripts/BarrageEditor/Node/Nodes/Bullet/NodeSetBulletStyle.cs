using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeSetBulletStyle : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.SetBulletStyle;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bulletchangestyle");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 子弹
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            attrs.Add(nodeAttr);
            // 样式的id
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Style id", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "set bullet style";
        }

        public override string ToDesc()
        {
            return string.Format("set {0} style to {1}", GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}:SetStyleById({1})\n", GetAttrByIndex(0).GetValueString(), GetAttrByIndex(1).GetValueString());
        }
    }
}
