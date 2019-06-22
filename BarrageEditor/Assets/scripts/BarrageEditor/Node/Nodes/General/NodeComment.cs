using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeComment : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.Comment;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "comment");
            _functionImg.color = new Color(0.4f, 0.4f, 0.4f);
            _descText.color = new Color(0.4f, 0.4f, 0.4f);
        }

        public override void CreateDefaultAttrs()
        {
            BaseNodeAttr nodeAttr;
            nodeAttr = NodeManager.CreateNodeAttr(NodeAttrType.Any);
            nodeAttr.Init(this, "Comment", null);
            attrs.Add(nodeAttr);
        }

        public override string GetNodeName()
        {
            return "comment";
        }

        public override string ToDesc()
        {
            return string.Format("[comment] {0}", attrs[0].GetValueString());
        }

        public override string ToLuaHead()
        {
            return string.Format("--{0}\n", GetAttrByIndex(0).GetValueString());
        }
    }
}
