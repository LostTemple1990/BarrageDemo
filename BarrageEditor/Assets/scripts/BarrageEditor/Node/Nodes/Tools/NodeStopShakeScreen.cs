using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeStopShakeScreen : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.StopShakeScreen;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "stopshakescreen");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Name", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "stop shake screen";
        }

        public override string ToDesc()
        {
            return string.Format("stop shake screen of name {0}", GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("StopShakeScreen(\"{0}\")\n", GetAttrByIndex(0).GetValueString());
        }
    }
}
