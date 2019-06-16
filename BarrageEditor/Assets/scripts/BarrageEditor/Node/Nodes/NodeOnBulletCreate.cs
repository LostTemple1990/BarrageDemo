using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeOnBulletCreate : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.OnBulletCreate;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "bulletinit");
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            // 参数列表
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Parameter list", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "on bullet create";
        }

        public override string ToDesc()
        {
            return string.Format("on create:({0})", attrs[0].GetValueString());
        }
    }
}
