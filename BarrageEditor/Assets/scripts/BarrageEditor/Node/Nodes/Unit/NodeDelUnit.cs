using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeDelUnit : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.DelUnit;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "unitdel");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 单位
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Unit", null);
            _attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "delete unit";
        }

        public override string ToDesc()
        {
            return string.Format("delete {0}", GetAttrByIndex(0).GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("DelUnit({0})\n",
                GetAttrByIndex(0).GetValueString());
        }
    }
}
