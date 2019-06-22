using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCode : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.Code;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "code");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 代码块名称
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Code", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "code";
        }

        public override string ToDesc()
        {
            return attrs[0].GetValueString();
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}\n", GetAttrByIndex(0).GetValueString());
        }
    }
}
