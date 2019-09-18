using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeCreateBoss : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.CreateBoss;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bosscreate");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 敌机类型
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.CustomizedType);
            nodeAttr.Init(this, "Type name", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "create boss";
        }

        public override string ToDesc()
        {
            return string.Format("create boss of type \"{0}\"",
                attrs[0].GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("last = CreateBoss(\"{0}\")\n",
                attrs[0].GetValueString());
        }
    }
}
